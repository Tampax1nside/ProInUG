using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Extentions
{
    public static class HttpClientExtentions
    {

        // TODO: тут если честно тоже повторяемость кода которая пока мне не очень, может как-то убрать?
        // добавить параметр HttpMethod а остальное примерно одинаково. И назвать SendAsJson
        public static Task<HttpResponseMessage> PostAsJson(this HttpClient client, object? content, string uri, string jwt = "", string requestId = "")
        {
            var httpreq = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"),
            };

            if (!string.IsNullOrEmpty(jwt))
            {
                httpreq.Headers.Add("Authorization", "Bearer " + jwt);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                httpreq.Headers.Add("Request-id", requestId);
            }

            return client.SendAsync(httpreq);
        }

        public static Task<HttpResponseMessage> GetAsJson(this HttpClient client, string uri, string jwt = "", string requestId = "")
        {
            var httpreq = new HttpRequestMessage(HttpMethod.Get, uri);

            if (!string.IsNullOrEmpty(jwt))
            {
                httpreq.Headers.Add("Authorization", "Bearer " + jwt);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                httpreq.Headers.Add("Request-id", requestId);
            }

            return client.SendAsync(httpreq);
        }

        public static Task<HttpResponseMessage> DeleteAsJson(this HttpClient client, string uri, object? content = default, string jwt = "", string requestId = "")
        {
            var httpreq = new HttpRequestMessage(HttpMethod.Delete, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"),
            };

            if (!string.IsNullOrEmpty(jwt))
            {
                httpreq.Headers.Add("Authorization", "Bearer " + jwt);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                httpreq.Headers.Add("Request-id", requestId);
            }

            return client.SendAsync(httpreq);
        }

        public static async Task<T?> ReadAs<T>(this HttpContent responseContent) where T : class
        {
            var jsonStream = await responseContent.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(jsonStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
