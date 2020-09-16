using System;
using System.Net.Http;
using System.Text;

namespace Lesson1a
{
    class Program1
    {
        private const string endpointVar = @"https://ar-qaknowledgemaker-app.azurewebsites.net";
        private const string endpointKeyVar = "06edf498-f452-4b3b-ad0c-55da172a8d54";
        private const string kbIdVar = "51e1727a-34b9-46a6-a3c6-2ad636027453";
        // Your QnA Maker resource endpoint.
        // From Publish Page: HOST
        // Example: https://YOUR-RESOURCE-NAME.azurewebsites.net/
        private static readonly string endpoint = endpointVar;
        // Authorization endpoint key
        // From Publish Page
        // Note this is not the same as your QnA Maker subscription key.
        private static readonly string endpointKey = endpointKeyVar;
        private static readonly string kbId = kbIdVar;

        static void Main1(string[] args)
        {
            
            if (null == endpointKey)
            {
                throw new Exception("Please set/export the environment variable: " + endpointKeyVar);
            }
            if (null == endpoint)
            {
                throw new Exception("Please set/export the environment variable: " + endpointVar);
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
                var response = client.SendAsync(request).Result;
                var jsonResponse = response.Content.ReadAsStringAsync().Result;

                // Output JSON response
                Console.WriteLine("A: " + jsonResponse);

                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }

            Console.WriteLine("Hello World!");
        }
        

    }
}
