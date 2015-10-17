using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json.Linq;

namespace SoracomApiCrawler
{
    public class SoracomCrawler
    {
        const string TARGET_URI = "https://dev.soracom.io/jp/docs/swagger/soracom-api.ja.json";
        const string BLOB_TITLE_FORMAT = "yyyy-MM-dd-HH-mm-ss";
        const string RAW_DOC_CONTAINER = "raw-doc";
        const string EXTRACTED_LIST_CONTAINER = "extracted-list";

        public async Task PerformCrawl(string connectionString)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(TARGET_URI);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Requesting {TARGET_URI} failed.");
                return;
            }
            var title = DateTime.UtcNow.ToString(BLOB_TITLE_FORMAT);

            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            // store data as-is (w/o further string conversion)
            await UploadRawDoc(await response.Content.ReadAsByteArrayAsync(), $"{title}.json", blobClient);
            var responseBody = await response.Content.ReadAsStringAsync();
            // store summary
            var apiMap = ExtractApis(responseBody);
            await UploadApiSummary(apiMap, $"{title}.txt", blobClient);
        }

        private Dictionary<string, List<string>> ExtractApis(string responseBody)
        {
            var result = new Dictionary<string, List<string>>();
            var jsonObj = JObject.Parse(responseBody);
            foreach (JProperty apiPath in jsonObj["paths"])
            {
                result[apiPath.Name] = apiPath.Values().Select(o => (o as JProperty).Name).ToList();
            }
            return result;
        }

        private static async Task UploadRawDoc(byte[] buf, string title, CloudBlobClient client)
        {
            var blockBlob = await CreateBlockBlob(client, RAW_DOC_CONTAINER, title);
            await blockBlob.UploadFromByteArrayAsync(buf, 0, buf.Length);
        }

        private static async Task UploadApiSummary(Dictionary<string, List<string>> apiMap, string title, CloudBlobClient client)
        {
            var blockBlob = await CreateBlockBlob(client, EXTRACTED_LIST_CONTAINER, title);
            var s = new StringBuilder();
            foreach (var kv in apiMap)
            {
                s.Append($"{kv.Key}\t{String.Join(",", kv.Value)}\n");
            }
            var buf = s.ToString();
            await blockBlob.UploadTextAsync(buf);
        }

        private static async Task<CloudBlockBlob> CreateBlockBlob(CloudBlobClient client, string containerName, string title)
        {
            var container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            return container.GetBlockBlobReference(title);
        }
    }
}
