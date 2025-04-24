using JwtAuthDotNet9WASM.Model;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Nodes;

namespace JwtAuthDotNet9WASM.Service
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;

        public CustomAuthenticationStateProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            try
            {
                //var response = await httpClient.GetAsync("/api/Auth/usernameandid");
                //if (response.IsSuccessStatusCode)
                //{
                //var strResponse = await response.Content.ReadAsStringAsync();
                //var jsonNode = JsonNode.Parse(strResponse);
                //var email = jsonNode?["Username"]?.ToString();
                var email = "shofiq.rasel@gmail.com";
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Email, email),
                };

                var identity = new ClaimsIdentity(claims, "Token");
                user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new AuthenticationState(user);

            //var claims = new List<Claim>
            //   {
            //       new Claim(ClaimTypes.Name, "Rasel"),
            //   };
            //var identity = new ClaimsIdentity(claims, "ANY");
            //var user = new ClaimsPrincipal(identity);

            //return Task.FromResult(new AuthenticationState(user));
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
    }

    public class FromResult
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }
}
