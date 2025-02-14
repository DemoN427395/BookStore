// Services/AuthServiceClient.cs
using System.Net.Http.Json;
using BookStoreLib.Models;

namespace UserService.Services
{
    public class AuthServiceClient
    {
        private readonly HttpClient _httpClient;

        public AuthServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ApplicationUser>(
                    $"/api/users/{userId}");
            }
            catch
            {
                return null;
            }
        }
    }
}