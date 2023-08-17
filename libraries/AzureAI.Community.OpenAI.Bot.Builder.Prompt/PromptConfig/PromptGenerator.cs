using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.AI.OpenAI;

namespace AzureAI.Community.OpenAI.Bot.Builder.Prompt.PromptConfig
{
    public class PromptGenerator
    {
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

        public (string invoke, CompletionsOptions options)? GenerateOptions(string invoke, string jsonString)
        {
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            JsonElement root = doc.RootElement;


            List<string> missingParameters = CheckMissingParameters(root, "Prompts");

            if (missingParameters.Count > 0)
            {
                Console.WriteLine($"Missing parameters: {string.Join(", ", missingParameters)}");
                return null;
            }

            //string invoke = GetInvokeType(root);

            if (string.IsNullOrEmpty(invoke))
                return null;

            CompletionsOptions completionsOptions = null;

            switch (invoke)
            {
                case "GetCompletionsAsync":
                    completionsOptions = GetCompletionsOptions(root);
                    break;
                case "GetChatCompletionsAsync":
                    break;
            }


            return (invoke, completionsOptions);
        }

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

        private static string GetInvokeType(JsonElement root)
        {
            string invoke = null;
            if (root.TryGetProperty("Invoke", out var invokeFunction))
            {
                invoke = invokeFunction.GetString();
            }

            return string.IsNullOrEmpty(invoke) ? null : invoke;
        }

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

        private static List<string> GetListPropertyOrDefault(JsonElement element, string propertyName,
            List<string> defaultValue)
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
    }
}