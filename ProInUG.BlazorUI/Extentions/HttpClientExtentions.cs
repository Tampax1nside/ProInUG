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
        
        public static Task<HttpResponseMessage> SendAsJson(this HttpClient client, HttpMethod method, 
            object? content, string uri, string jwt = "", string requestId = "")
        {
            var httpRequest = new HttpRequestMessage(method, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(content), Encoding.UTF8, "application/json"),
            };

            if (!string.IsNullOrEmpty(jwt))
            {
                httpRequest.Headers.Add("Authorization", "Bearer " + jwt);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                httpRequest.Headers.Add("Request-id", requestId);
            }

            return client.SendAsync(httpRequest);
        }
        
        public static async Task<T?> ReadAs<T>(this HttpContent responseContent) where T : class
        {
            var jsonStream = await responseContent.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(
                jsonStream, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
