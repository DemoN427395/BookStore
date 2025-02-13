using UserService.DTOs;

namespace UserService.Services;

public class AuthServiceClient
{
    private readonly HttpClient _httpClient;

    public AuthServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApplicationUserDto?> GetUserByIdAsync()
    {
        var response = await _httpClient.GetAsync($"/api/users/me");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ApplicationUserDto>();
        }
        return null;
    }
}
