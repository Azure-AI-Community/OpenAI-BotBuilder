using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.AI.OpenAI;

namespace AzureAI.Community.OpenAI.Bot.Builder.Prompt.PromptConfig
{
    /// <summary>
    /// Prompt Generator class to generate prompt options
    /// </summary>
    public class PromptGenerator
    {
        /// <summary>
        ///  Check if required parameters are present in the json string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public (string IsError, JsonElement? root) CheckRequiredParameterIsPresent(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            List<string> missingParameters = CheckMissingParameters(root, "EndPoint", "Apikey",
                "DeploymentorModelName", "Invoke", "Prompts");

            if (missingParameters.Count > 0)
            {
                string errorMsg = $"Missing parameters: {string.Join(", ", missingParameters)}";
                return (errorMsg, null);
            }

            return (string.Empty, root);
        }

        /// <summary>
        ///  Generate CompletionsOptions object from json string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public CompletionsOptions GenerateCompletionsOptions(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;


            List<string> missingParameters = CheckMissingParameters(root, "Prompts");

            if (missingParameters.Count > 0)
            {
                Console.WriteLine($"Missing parameters: {string.Join(", ", missingParameters)}");
                return null;
            }

            CompletionsOptions completionsOptions = GetCompletionsOptions(root);

            return completionsOptions;
        }

        /// <summary>
        /// Generate ChatCompletionsOptions object from json string
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public ChatCompletionsOptions GenerateChatCompletionsOptions(string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;

            var missingParameters = CheckMissingParameters(root, "ChatMessages");

            if (missingParameters.Count > 0)
            {
                Console.WriteLine($"Missing parameters: {string.Join(", ", missingParameters)}");
                return null;
            }

            ChatCompletionsOptions chatCompletionsOptions = GetChatCompletionsOptions(root);
            return chatCompletionsOptions;
        }

        /// <summary>
        /// Check if required parameters are present in the json string
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<string> CheckMissingParameters(JsonElement root, params string[] parameters)
        {
            List<string> missingParameters = new List<string>();
            foreach (string parameter in parameters)
            {
                if (!root.TryGetProperty(parameter, out _))
                {
                    missingParameters.Add(parameter);
                }
            }
            return missingParameters;
        }

        /// <summary>
        /// Prepare ChatCompletionsOptions object from json string
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static ChatCompletionsOptions GetChatCompletionsOptions(JsonElement root)
        {
            var chatMessage = GenerateChatMessage(root, "ChatMessages");

            if (chatMessage.Count <= 0)
                return null;

            var chatCompletionsOptions = new ChatCompletionsOptions();

            foreach (var chatMsg in chatMessage)
            {
                chatCompletionsOptions.Messages.Add(chatMsg);
            }

            if (root.TryGetProperty("ChoiceCount", out var generationSampleCount))
                chatCompletionsOptions.ChoiceCount = generationSampleCount.GetInt32();

            if (root.TryGetProperty("Temperature", out var temperature))
                chatCompletionsOptions.Temperature = temperature.GetSingle();

            if (root.TryGetProperty("User", out var user))
                chatCompletionsOptions.User = user.GetString();

            if (root.TryGetProperty("MaxTokens", out var maxTokens))
                chatCompletionsOptions.MaxTokens = maxTokens.GetInt32();

            if (root.TryGetProperty("TokenSelectionBiases", out var tokenSelectionBiases))
            {
                var tokenBiases = GetDictionaryPropertyOrDefault(tokenSelectionBiases);

                if (tokenBiases?.Count > 0)
                {
                    foreach (var tokenBiase in tokenBiases)
                    {
                        chatCompletionsOptions.TokenSelectionBiases.Add(tokenBiase.Key, tokenBiase.Value);
                    }
                }
            }

            if (root.TryGetProperty("StopSequences", out var stopSequences))
            {
                var stopDictionary = GetListPropertyOrDefault(stopSequences);
                if (stopDictionary?.Count > 0)
                {
                    foreach (var stop in stopDictionary)
                    {
                        chatCompletionsOptions.StopSequences.Add(stop);
                    }
                }
            }

            if (root.TryGetProperty("FrequencyPenalty", out var frequencyPenalty))
                chatCompletionsOptions.FrequencyPenalty = frequencyPenalty.GetSingle();

            if (root.TryGetProperty("NucleusSamplingFactor", out var nucleusSamplingFactor))
                chatCompletionsOptions.NucleusSamplingFactor = nucleusSamplingFactor.GetSingle();

            if (root.TryGetProperty("PresencePenalty", out var presencePenalty))
                chatCompletionsOptions.PresencePenalty = presencePenalty.GetSingle();

            return chatCompletionsOptions;
        }

        /// <summary>
        /// Prepare CompletionsOptions object from json string
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static CompletionsOptions GetCompletionsOptions(JsonElement root)
        {
            var prompts = GetListPropertyOrDefault(root, "Prompts", new List<string>());

            if (prompts.Count <= 0)
                return null;

            var requestOptions = new CompletionsOptions();

            foreach (var prompt in prompts)
            {
                requestOptions.Prompts.Add(prompt);
            }

            if (root.TryGetProperty("GenerationSampleCount", out var generationSampleCount))
                requestOptions.GenerationSampleCount = generationSampleCount.GetInt32();

            if (root.TryGetProperty("Temperature", out var temperature))
                requestOptions.Temperature = temperature.GetSingle();

            if (root.TryGetProperty("User", out var user))
                requestOptions.User = user.GetString();

            if (root.TryGetProperty("Echo", out var echo))
                requestOptions.Echo = echo.GetBoolean();

            if (root.TryGetProperty("LogProbabilityCount", out var logProbabilityCount))
                requestOptions.LogProbabilityCount = logProbabilityCount.GetInt32();

            if (root.TryGetProperty("MaxTokens", out var maxTokens))
                requestOptions.MaxTokens = maxTokens.GetInt32();

            if (root.TryGetProperty("TokenSelectionBiases", out var tokenSelectionBiases))
            {
                var tokenBiases = GetDictionaryPropertyOrDefault(tokenSelectionBiases);

                if (tokenBiases?.Count > 0)
                {
                    foreach (var tokenBiase in tokenBiases)
                    {
                        requestOptions.TokenSelectionBiases.Add(tokenBiase.Key, tokenBiase.Value);
                    }
                }
            }

            if (root.TryGetProperty("StopSequences", out var stopSequences))
            {
                var stopDictionary = GetListPropertyOrDefault(stopSequences);
                if (stopDictionary?.Count > 0)
                {
                    foreach (var stop in stopDictionary)
                    {
                        requestOptions.StopSequences.Add(stop);
                    }
                }
            }

            if (root.TryGetProperty("ChoicesPerPrompt", out var choicesPerPrompt))
                requestOptions.ChoicesPerPrompt = choicesPerPrompt.GetInt32();

            if (root.TryGetProperty("FrequencyPenalty", out var frequencyPenalty))
                requestOptions.FrequencyPenalty = frequencyPenalty.GetSingle();

            if (root.TryGetProperty("NucleusSamplingFactor", out var nucleusSamplingFactor))
                requestOptions.NucleusSamplingFactor = nucleusSamplingFactor.GetSingle();

            if (root.TryGetProperty("PresencePenalty", out var presencePenalty))
                requestOptions.PresencePenalty = presencePenalty.GetSingle();

            return requestOptions;
        }

        /// <summary>
        /// Get the InvokeType from the json string
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private static string GetInvokeType(JsonElement root)
        {
            string invoke = null;
            if (root.TryGetProperty("Invoke", out var invokeFunction))
            {
                invoke = invokeFunction.GetString();
            }

            return string.IsNullOrEmpty(invoke) ? null : invoke;
        }

        /// <summary>
        /// Get the list of strings from the json string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static List<string> GetListPropertyOrDefault(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                var result = new List<string>();
                foreach (var item in element.EnumerateArray())
                {
                    var addItem = item.GetString();
                    if (!string.IsNullOrEmpty(addItem))
                        result.Add(addItem);
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Get the dictionary of int, int from the json string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static Dictionary<int, int> GetDictionaryPropertyOrDefault(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                var result = new Dictionary<int, int>();
                foreach (var item in element.EnumerateObject())
                {
                    int key = int.Parse(item.Name);
                    int value = item.Value.GetInt32();
                    result.Add(key, value);
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// Get the list of strings from the json string
        /// </summary>
        /// <param name="element"></param>
        /// <param name="propertyName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static List<string> GetListPropertyOrDefault(JsonElement element, string propertyName, List<string> defaultValue)
        {
            if (element.TryGetProperty(propertyName, out var property))
            {
                var result = new List<string>();
                foreach (var item in property.EnumerateArray())
                {
                    var addItem = item.GetString();
                    if (!string.IsNullOrEmpty(addItem))
                        result.Add(addItem);
                }
                return result;
            }
            return defaultValue;
        }

        /// <summary>
        /// Prepare ChatMessage object from json string
        /// </summary>
        /// <param name="element"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static List<ChatMessage> GenerateChatMessage(JsonElement element, string propertyName)
        {
            List<ChatMessage> chatMessages = new List<ChatMessage>();

            if (element.TryGetProperty(propertyName, out var property))
            {
                foreach (var item in property.EnumerateArray())
                {
                    var text = item.GetProperty("Text").GetString();
                    var sender = item.GetProperty("Sender").GetString();

                    if (sender != null && IsCategory(sender))
                    {
                        sender = sender.ToLower();
                        chatMessages.Add(new ChatMessage(new ChatRole(sender), text));
                    }
                }
            }

            return chatMessages;
        }

        /// <summary>
        /// Check if the value is a valid category
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool IsCategory(string value)
        {
            value = value.ToLower();

            return string.CompareOrdinal(ChatRole.Assistant.ToString(), value) == 0 ||
                   string.CompareOrdinal(ChatRole.System.ToString(), value) == 0 ||
                   string.CompareOrdinal(ChatRole.User.ToString(), value) == 0 ||
                   string.CompareOrdinal(ChatRole.Function.ToString(), value) == 0;
        }
    }
}

