using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using AzureAI.Community.OpenAI.Bot.Builder.Prompt.PromptConfig;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureAI.Community.OpenAI.Bot.Builder.Prompt
{
    //Prompt Engineering in Bot Builder
    public class OpenAIPromptDialog : Dialog
    {
        public OpenAIPromptDialog([CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) : base()
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$Kind")] public const string Kind = "OpenAIPromptDialog";

        [JsonProperty("EndPoint")] public StringExpression EndPoint { get; set; }

        [JsonProperty("ApiKey")] public StringExpression ApiKey { get; set; }

        [JsonProperty("DeploymentOrModelName")]
        public StringExpression DeploymentOrModelName { get; set; }

        [JsonProperty("PromptConfiguration")] public StringExpression PromptConfiguration { get; set; }

        [JsonProperty("ErrorProperty")] public StringExpression ErrorProperty { get; set; }

        [JsonProperty("resultProperty")] public StringExpression ResultProperty { get; set; }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result = string.Empty;

            var endPoint = EndPoint?.GetValue(dc.State);
            if (endPoint == null)
            {
                SetResult(dc.Context, false, error: "Endpoint should not be empty.");
                return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
            }

            var apiKeyValue = ApiKey?.GetValue(dc.State);
            if (apiKeyValue == null)
            {
                SetResult(dc.Context, false, error: "API Key should not be empty.");
                return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
            }

            var deploymentValue = DeploymentOrModelName?.GetValue(dc.State);
            if (deploymentValue == null)
            {
                SetResult(dc.Context, false, error: "Deployment or Model Name should not be empty.");
                return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
            }

            var getPromptConfig = this.PromptConfiguration?.GetValue(dc.State)?.ToString();

            if (getPromptConfig == null)
            {
                SetResult(dc.Context, false, error: "Prompt configuration should not be empty.");
                return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
            }

            var promptConfig = new PromptGenerator();
            var promptOptions = promptConfig.GenerateOptions("GetCompletionsAsync", getPromptConfig);

            if (promptOptions == null)
            {
                SetResult(dc.Context, false, error: "Prompt configuration JSON is invalid.");
                return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
            }

            var openAI = new ConnectOpenAI.OpenAI(endPoint, apiKeyValue, deploymentValue);

            var jResult = await openAI.InvokeAsync(promptOptions.Value.invoke, promptOptions.Value.options);

            SetResult(dc.Context, true, jResult);

            return await dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
        }

        private void SetResult(ITurnContext turnContext, bool success, JObject result = null, string error = "")
        {
            var conversationExpire = new
            {
                IsSuccess = success,
                Result = result,
                Error = error
            };

            var expireDetails = JsonConvert.SerializeObject(conversationExpire);
            ObjectPath.SetPathValue(turnContext.TurnState, "turn.OpenAI", JObject.Parse(expireDetails));
        }
    }
}