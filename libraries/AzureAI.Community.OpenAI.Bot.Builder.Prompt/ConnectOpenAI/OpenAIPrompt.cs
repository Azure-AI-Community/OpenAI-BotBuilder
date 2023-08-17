using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AzureAI.Community.OpenAI.Bot.Builder.Prompt.ConnectOpenAI
{
    /// <summary>
    /// OpenAI Prompt class to invoke OpenAI API calls based on the invoke name
    /// using the Azure.AI.OpenAI SDK
    /// </summary>
    public class OpenAIPrompt
    {
        private readonly OpenAIClient client;

        /// <summary>
        /// Constructor to initialize the OpenAI client
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="apiKey"></param>
        /// <param name="modelName"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public OpenAIPrompt(string endPoint, string apiKey, string modelName)
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

        /// <summary>
        /// Model Name
        /// </summary>
        private string ModelName { get; }

        /// <summary>
        /// Invoke the OpenAI API based on the invoke name
        /// </summary>
        /// <param name="invokeName"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<JObject> InvokeAsync(string invokeName, dynamic options)
        {
            JObject result = null;

            if (string.IsNullOrEmpty(invokeName))
                throw new ArgumentException("Invoke Name should be empty");

            if (string.Compare(invokeName, "GetCompletionsAsync", StringComparison.Ordinal) == 0)
            {
                var objCompletions = await GetCompletions(options);
                result = ConvertJObject<Completions>(objCompletions);
            }
            else if (string.Compare(invokeName, "GetChatCompletionsAsync", StringComparison.Ordinal) == 0)
            {
                var objChatCompletions = await GetChatCompletions(options);
                result = ConvertJObject<ChatCompletions>(objChatCompletions);
            }
            
            return result;
        }

        /// <summary>
        /// Invoke the GetCompletionsAsync API call and return the completions
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<Response<Completions>> GetCompletions(dynamic options)
        {
            if (options == null)
                throw new ArgumentException("GetCompletionsAsync Invoke CompletionsOptions should not be null");

            var completionsOptions = (CompletionsOptions)options;

            return await client.GetCompletionsAsync(ModelName, completionsOptions);
        }

        /// <summary>
        /// Invoke the GetChatCompletionsAsync API call and return the chat completions
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task<Response<ChatCompletions>> GetChatCompletions(dynamic options)
        {
            if (options == null)
                throw new ArgumentException("GetCompletionsAsync Invoke CompletionsOptions should not be null");
            try
            {
                ChatCompletionsOptions chatCompletions = (ChatCompletionsOptions)options;
                return await client.GetChatCompletionsAsync(ModelName, chatCompletions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        
        /// <summary>
        /// Convert the response to JObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private JObject ConvertJObject<T>(Response<T> response)
        {
            JObject result = null;
            if (response.HasValue)
            {
                var serialize = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                result = JObject.Parse(serialize);
            }
            return result;
        }
    }
}