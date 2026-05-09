using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManagementSystem.Web.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApiClient(HttpClient http) => _http = http;

    public async Task<T?> GetAsync<T>(string url)
    {
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task PostAsync<T>(string url, object body)
    {
        var response = await _http.PostAsJsonAsync(url, body, JsonOptions);
        response.EnsureSuccessStatusCode();
        await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task PutAsync<T>(string url, object body)
    {
        var response = await _http.PutAsJsonAsync(url, body, JsonOptions);
        response.EnsureSuccessStatusCode();
        await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task DeleteAsync(string url)
    {
        var response = await _http.DeleteAsync(url);
        response.EnsureSuccessStatusCode();
    }
}
