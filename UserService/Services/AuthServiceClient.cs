// Services/AuthServiceClient.cs
using BookStoreLib.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace UserService.Services
{
    public class AuthServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApplicationUser?> GetCurrentUserAsync(string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                return await _httpClient.GetFromJsonAsync<ApplicationUser>("/api/users/me");
            }
            catch
            {
                return null;
            }
        }
    }
}