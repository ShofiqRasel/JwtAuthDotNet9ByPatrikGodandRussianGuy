using Blazored.LocalStorage;
using JwtAuthDotNet9WASM.Model;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace JwtAuthDotNet9WASM.Service
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;
        private readonly ISyncLocalStorageService localStorage;
        private string UserName { get; set; } = string.Empty;
        private string UserId { get; set; } = string.Empty;

        public CustomAuthenticationStateProvider(HttpClient httpClient, ISyncLocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;

            var accessToken = localStorage.GetItem<string>("accessToken");
            if (!string.IsNullOrEmpty(accessToken))
            {
                UserName = JwtHelper.GetUserName(accessToken!);
                UserId = JwtHelper.GetNameIdentifier(accessToken!);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            try
            {
                string? email = UserName; //localStorage.GetItemAsString("email");
                string? userId = UserId; //localStorage.GetItemAsString("userId");

                if (!string.IsNullOrEmpty(email))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, email),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.NameIdentifier, userId!), //// 🔥 Preserve NameIdentifier
                    };
                    Console.WriteLine($"User ID: {userId}");
                    var identity = new ClaimsIdentity(claims, "Token");
                    user = new ClaimsPrincipal(identity);
                    return new AuthenticationState(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new AuthenticationState(user);
        }
        public async Task<FromResult> LoginAsync(UserDto userDto)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("/api/Auth/login", userDto);
                if (response.IsSuccessStatusCode)
                {
                    var strResponse = await response.Content.ReadAsStringAsync();
                    var jsonNode = JsonNode.Parse(strResponse);
                    var strAccessToken = jsonNode?["accessToken"]?.ToString();
                    var strRefreshToken = jsonNode?["refreshToken"]?.ToString();

                    localStorage.SetItem("accessToken", strAccessToken);
                    localStorage.SetItem("refreshToken", strRefreshToken);
                    //localStorage.SetItem("email", userDto.Username);

                    // 🔥 Extract UserId (NameIdentifier) from AccessToken
                    //var userId = JwtHelper.GetNameIdentifier(strAccessToken!.ToString());
                    //if (!string.IsNullOrEmpty(userId))
                    //{
                    //    localStorage.SetItem("userId", userId);
                    //}

                    UserName = JwtHelper.GetUserName(strAccessToken!);
                    UserId = JwtHelper.GetNameIdentifier(strAccessToken!);

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strAccessToken);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return new FromResult { Success = true };
                }
                else
                {
                    return new FromResult
                    {
                        Success = false,
                        Errors = ["Bad Email or Password"]
                    };
                }
            }
            catch
            {

            }

            return new FromResult
            {
                Success = false,
                Errors = ["Connection Error"]
            };
        }

        //private string? GetClaimFromJwt(string? jwtToken, string claimType)
        //{
        //    if (string.IsNullOrEmpty(jwtToken))
        //        return null;

        //    var parts = jwtToken.Split('.');
        //    if (parts.Length != 3)
        //        return null;

        //    var payload = parts[1];
        //    var jsonBytes = ParseBase64WithoutPadding(payload);
        //    var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        //    if (keyValuePairs != null && keyValuePairs.TryGetValue(claimType, out var claimValue))
        //    {
        //        return claimValue?.ToString();
        //    }

        //    return null;
        //}

        //private byte[] ParseBase64WithoutPadding(string base64)
        //{
        //    switch (base64.Length % 4)
        //    {
        //        case 2: base64 += "=="; break;
        //        case 3: base64 += "="; break;
        //    }
        //    return Convert.FromBase64String(base64);
        //}

    }

    public class FromResult
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}
