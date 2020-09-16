using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Search;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lesson1a
{
    class Program
    {
        private const string qnaEndpoint = @"https://ar-qaknowledgemaker-app.azurewebsites.net";
        private const string qnaEndpointKey = "06edf498-f452-4b3b-ad0c-55da172a8d54";
        private const string kbIdVar = "51e1727a-34b9-46a6-a3c6-2ad636027453";

        //search service
        private const string searchEndpoint = @"https://arqaknowledgemakerapp-aspqgp2j5jekw4o.search.windows.net";
        private const string searchKey = "B56B79DDDBAE2F6B317EEA3672763F62";
        private const string searchAPIVersion = "2020-06-30";

        //storage
        private const string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=aiknast;AccountKey=s7CaUtBdvCLAKvMneN3HCc3FiLmIDmat+VK+Xnc22RGAte6BRVWuBnCjARDbi3yLjRIOgLo2wk8jjMy9MEiyMw==;EndpointSuffix=core.windows.net";

        // Your QnA Maker resource endpoint.
        // From Publish Page: HOST
        // Example: https://YOUR-RESOURCE-NAME.azurewebsites.net/
        private static readonly string endpoint = qnaEndpoint;
        // Authorization endpoint key
        // From Publish Page
        // Note this is not the same as your QnA Maker subscription key.
        private static readonly string endpointKey = qnaEndpointKey;
        private static readonly string kbId = kbIdVar;

        static async Task Main(string[] args)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            string searchServiceName = configuration["SearchServiceName"];
            string adminApiKey = configuration["SearchServiceAdminApiKey"];

            Uri serviceEndpoint = new Uri($"https://{searchServiceName}.search.windows.net/");
            AzureKeyCredential credential = new AzureKeyCredential(adminApiKey);
            SearchIndexClient searchIndexClient = new SearchIndexClient(serviceEndpoint, credential);
            SearchClient searchServiceClient = new SearchClient(serviceEndpoint,"newknadocsindex", credential);

            if (configuration["SearchServiceName"] == "Put your search service name here")
            {
                Console.Error.WriteLine("Specify SearchServiceName in appsettings.json");
                Environment.Exit(-1);
            }

            if (configuration["SearchServiceAdminApiKey"] == "Put your primary or secondary API key here")
            {
                Console.Error.WriteLine("Specify SearchServiceAdminApiKey in appsettings.json");
                Environment.Exit(-1);
            }

            if (configuration["AzureStoreageConnectionString"] == "Put your Storage ConnnectionString here")
            {
                Console.Error.WriteLine("Specify SearchServiceAdminApiKey in appsettings.json");
                Environment.Exit(-1);
            }

            Console.WriteLine("Press c to create datasource. Press ci to create an index. Press l to list the datasources. Press S to seary the SearchService. Press q to query QnA.");
            var res = Console.ReadLine().ToString();

            if(res == "c")
            {
                Console.WriteLine("Creating Data Source!");
                var dsresponse = await CreateDataSource(searchServiceClient, configuration);
            }
            else if( res == "ci")
            {
                Console.WriteLine("Create Index! What name should we use?");
                var indexName = Console.ReadLine();
                var index = CreateIndex(indexName);
                searchIndexClient.CreateIndex(index);
            }
            else if( res == "l")
            {
                Console.WriteLine("Get Data Source List!");
                var dsresponse = await ListDataSource(searchServiceClient, configuration);
            }
            else if( res == "s")
            {
                Console.WriteLine("Search SearchService!");
                RunQueries(searchServiceClient);
            }
            else if(res == "q")
            {
                Console.WriteLine("Query QnA!");
                var resp = await QueryQnA();
            }
        }
    
        static async Task<string> QueryQnA()
        {
            if (null == endpointKey)
            {
                throw new Exception("Please set/export the environment variable: " + qnaEndpointKey);
            }
            if (null == endpoint)
            {
                throw new Exception("Please set/export the environment variable: " + qnaEndpoint);
            }
            if (null == kbId)
            {
                throw new Exception("Please set/export the environment variable: " + kbIdVar);
            }
            
            var uri = endpoint + "/qnamaker/knowledgebases/" + kbId + "/generateAnswer";

            Console.WriteLine("uri: " + uri);

            // JSON format for passing question to service
            string question = @"{'question': 'What services do you provide?','top': 1}";

            Console.WriteLine("Q: " + question);

            // Create http client
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // POST method
                request.Method = HttpMethod.Post;

                // Add host + service to get full URI
                request.RequestUri = new Uri(uri);

                // set question
                request.Content = new StringContent(question, Encoding.UTF8, "application/json");

                // set authorization
                request.Headers.Add("Authorization", "EndpointKey " + endpointKey);

                // Send request to Azure service, get response
                var response = await client.SendAsync(request);
                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                // Output JSON response
                Console.WriteLine("A: " + jsonResponse);
            }
            return "";
        }

        static SearchIndex CreateIndex(string indexName)
        {
            SearchIndex index = new SearchIndex(indexName)
            {
                Fields =
                {
                    new SimpleField("id",SearchFieldDataType.String){IsKey = true, IsFilterable = true, IsSortable = true},
                    new SearchableField("url"){ IsFilterable = true, IsSortable = true},
                    new SearchableField("file_name"){ IsFilterable = true, IsSortable = true},
                    new SearchableField("content"){ IsFilterable = true, IsSortable = true},
                    new SimpleField("size", SearchFieldDataType.Int64){ IsFilterable = true, IsSortable = true},
                    new SimpleField("last_modified", SearchFieldDataType.DateTimeOffset){ IsFilterable = true, IsSortable = true},
                }
            };

            return index;
        }
        //ID Data source DefaultEndpointsProtocol=https;AccountName=aiknast;AccountKey=s7CaUtBdvCLAKvMneN3HCc3FiLmIDmat+VK+Xnc22RGAte6BRVWuBnCjARDbi3yLjRIOgLo2wk8jjMy9MEiyMw==;EndpointSuffix=core.windows.net
        static async Task<string> CreateDataSource(SearchClient serviceClient, IConfigurationRoot configuration)
        {
            
            
            return "ok";
        }

        

        
        static async Task<string> ListDataSource( SearchClient serviceClient, IConfigurationRoot configuration)
        {
            
            //Microsoft.Azure.Search.Models.DataSourceListResult dsList = await serviceClient.DataSources.ListAsync();
            return "";
            //serviceClient.
        }
        //Create an Azure Cognitive Search index whose schema is compatible with your data source.
        //Create an Azure Cognitive Search data source as described in Create Data Source (Azure Cognitive Search REST API).
        //https://docs.microsoft.com/en-us/rest/api/searchservice/create-data-source

        //Create an Azure Cognitive Search indexer as described in Create Indexer (Azure Cognitive Search REST API).
        //https://docs.microsoft.com/en-us/rest/api/searchservice/create-indexer
        private static void RunQueries(SearchClient srchclient)
        {
            SearchOptions options;
            SearchResults<DocumentSample> response;


            Console.WriteLine("Query #1: Search all documents that include 'New York' (there should be 18)...\n");

            options = new SearchOptions()
            {
                Filter = "",
                OrderBy = { "" }
            };

            response = srchclient.Search<DocumentSample>("New York", options);
            WriteDocuments(response);
            return;

            Console.WriteLine("Query #2: Find all documents that include 'London' and 'Buckingham Palace' (there should be 2)...\n");

            options = new SearchOptions()
            {
                Filter = "hotelCategory eq 'hotel'",
            };

            response = srchclient.Search<DocumentSample>("*", options);
            WriteDocuments(response);

            Console.WriteLine("Query #3: Filter  all documents that contain the term 'Las Vegas' that have 'reviews' in their URL (there should be 13)...\n");

            options = new SearchOptions()
            {
                Filter = "baseRate lt 200",
                OrderBy = { "lastRenovationDate desc" }
            };

            response = srchclient.Search<DocumentSample>("*", options);
            WriteDocuments(response);
        }

        // Write search results to console
        private static void WriteDocuments(SearchResults<DocumentSample> searchResults)
        {
            foreach (SearchResult<DocumentSample> response in searchResults.GetResults())
            {
                DocumentSample doc = response.Document;
                var score = response.Score;
                Console.WriteLine($"Id: {doc.Id}, Url: {doc.Url}, FileName: {doc.FileName}, Content: {doc.Content}, Size: {doc.Size}, LastModified: {doc.LastModified}, Score: {score}");
            }

            Console.WriteLine();
        }

    }

    public class DocumentSample
    {
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("file_name")]
        public string FileName { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("size")]
        public Int64 Size { get; set; }

        [JsonPropertyName("last_modified")]
        public DateTime LastModified { get; set; }
    }
}
