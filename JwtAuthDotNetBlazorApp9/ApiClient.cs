using Newtonsoft.Json;
using System.Net.Http;

namespace JwtAuthDotNetBlazorApp9
{
    public class ApiClient(HttpClient httpClient)
    {
        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            // SetAuthorizeHeader();
            return await httpClient.GetFromJsonAsync<T>(path);
        }

        public async Task<T1> SimplePostAsync<T1, T2>(string path, T2 postModel)
        {
            //await SetAuthorizeHeader();

            var res = await httpClient.PostAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }

        public async Task<T1> PostAsync<T1, T2>(string path, T2 postModel)
        {
            //await SetAuthorizeHeader();

            var res = await httpClient.PostAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }
        public async Task<T1> PutAsync<T1, T2>(string path, T2 postModel)
        {
            //await SetAuthorizeHeader();
            var res = await httpClient.PutAsJsonAsync(path, postModel);
            if (res != null && res.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T1>(await res.Content.ReadAsStringAsync());
            }
            return default;
        }
        public async Task<T> DeleteAsync<T>(string path)
        {
            //await SetAuthorizeHeader();
            return await httpClient.DeleteFromJsonAsync<T>(path);
        }
    }
}
