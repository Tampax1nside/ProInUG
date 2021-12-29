using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProInUG.BlazorUI.Extentions
{
    public static class HttpClientExtentions
    {
        private static readonly Random Rnd = new Random();

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

        public static Task<HttpResponseMessage> DeleteAsJson(this HttpClient client, string uri,
            object? content = default, string jwt = "", string requestId = "")
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"),
            };

            if (!string.IsNullOrEmpty(jwt))
            {
                httpRequestMessage.Headers.Add("Authorization", "Bearer " + jwt);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                httpRequestMessage.Headers.Add("Request-id", requestId);
            }

            return client.SendAsync(httpRequestMessage);
        }

        public static Task<HttpResponseMessage> PatchAsJson(this HttpClient client, string uri,
            object? content = default, string jwt = "", string requestId = "")
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"),
            };

            if (!string.IsNullOrEmpty(jwt))
            {
                httpRequestMessage.Headers.Add("Authorization", "Bearer " + jwt);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                httpRequestMessage.Headers.Add("Request-id", requestId);
            }

            return client.SendAsync(httpRequestMessage);
        }

        public static async Task<T?> ReadAs<T>(this HttpContent responseContent) where T : class
        {
            var jsonStream = await responseContent.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(jsonStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        /// <summary>
        /// Сгенерировать Id запроса
        /// </summary>
        /// <returns></returns>
        public static string GenerateRequestId(this HttpClient client)
        {
            var rndBuff = new byte[8];
            Rnd.NextBytes(rndBuff);
            return Convert.ToBase64String(rndBuff);
        }
    }
}
