using Azure.AI.OpenAI;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Azure.AI.Open.Bot.Builder.Community.ConnectOpenAI
{
    public class OpenAI
    {
        private readonly OpenAIClient client;
        public OpenAI(string endPoint,string apiKey,string modelName)
        {
            if (string.IsNullOrEmpty(endPoint))
                throw new ArgumentException("EndPoint should not be null");

            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("ApiKey should not be null");

            if (string.IsNullOrEmpty(modelName))
                throw new ArgumentException("Model Name should not be empty");

            ModelName = modelName;

            client = new OpenAIClient(new Uri(endPoint), new AzureKeyCredential(apiKey));

            if (client == null)
                throw new NullReferenceException("OpenAIClient object is null");
        }

        public string ModelName { get;}

        public async  Task<JObject> InvokeAsync(string invokeName, dynamic options)
        {
            JObject result =null;

            if (string.IsNullOrEmpty(invokeName))
                throw new ArgumentException("Invoke Name should be empty");

            if (string.Compare(invokeName, "GetCompletionsAsync", StringComparison.Ordinal) == 0)
            {
                CompletionsOptions completionsOptions = (CompletionsOptions)options;

                if (options == null)
                    throw new ArgumentException("GetCompletionsAsync Invoke CompletionsOptions should not be null");

                var response = await client.GetCompletionsAsync(ModelName, completionsOptions);

                if (response.HasValue)
                {
                    var serialize = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        WriteIndented = true 
                    });

                    //var object1 = JsonConvert.SerializeObject(serialize);
                    result = JObject.Parse(serialize);
                }
            }

            return result;
        }

    }
}
