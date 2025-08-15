using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace MovieApp.Helpers
{
    public class SupabaseUploader
    {
        private readonly string _url;
        private readonly string _key;
        private readonly string _bucket;

        public SupabaseUploader(IConfiguration config)
        {
            _url = config["Supabase:Url"];
            _key = config["Supabase:Key"];
            _bucket = config["Supabase:Bucket"];
        }

        public async Task<string> UploadPosterAsync(Stream fileStream, string fileName, string contentType)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _key);

            var requestUrl = $"{_url}/storage/v1/object/{_bucket}/{fileName}";

            var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            var response = await httpClient.PutAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return $"{_url}/storage/v1/object/public/{_bucket}/{fileName}";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase upload failed: {response.StatusCode}, {error}");
            }
        }
    }
}
