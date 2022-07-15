using System.Net.Http.Json;

namespace CentralStation.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<List<T>> GetList<T>(this HttpClient client, string url) => await client.GetFromJsonAsync<List<T>>(url) ?? new List<T>();
    }
}
