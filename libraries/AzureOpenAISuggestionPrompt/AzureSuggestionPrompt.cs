using Azure;
using Azure.AI.OpenAI;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using static Microsoft.Bot.Builder.Dialogs.Prompts.PromptCultureModels;
using Choice = Microsoft.Bot.Builder.Dialogs.Choices.Choice;

namespace AzureAI.Community.OpenAI.Bot.Builder.SuggestionPrompt
{
    public class OpenAISuggestionPrompt : ChoicePrompt
    {
        private OpenAIClient client;

        private string openAIDeploymentName;
        private int openAIMaxTokens;
        private int openAIQuestions;

        public OpenAISuggestionPrompt(string dialogId, string key, string endPoint, string deploymentName,
            int maxTokens = 100,int noOfSuggestion=3, PromptValidator<FoundChoice> validator = null, string defaultLocale = null) : this(dialogId, new Dictionary<string, ChoiceFactoryOptions>(GetSupportedCultures().ToDictionary(culture => culture.Locale, culture =>
                new ChoiceFactoryOptions(culture.Separator, culture.InlineOr, culture.InlineOrMore, true))),
            validator,
            defaultLocale)
        {
            InitOpenAIClient(key, endPoint, deploymentName, maxTokens, noOfSuggestion);
        }

        public OpenAISuggestionPrompt(string dialogId, string key, string endPoint, string deploymentName,int maxTokens, int noOfSuggestion,Dictionary<string, ChoiceFactoryOptions> choiceDefaults, PromptValidator<FoundChoice> validator = null,
            string defaultLocale = null) : base(dialogId, validator)
        {
            InitOpenAIClient(key, endPoint, deploymentName, maxTokens, noOfSuggestion);
        }

        private void InitOpenAIClient(string key, string endPoint, string deploymentName, int maxTokens,int noOfSuggestion= 3)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key));
            
            if (string.IsNullOrEmpty(endPoint))
                throw new ArgumentNullException(nameof(endPoint));

            if (string.IsNullOrEmpty(deploymentName))
                throw new ArgumentNullException(nameof(deploymentName));

            if (maxTokens <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(maxTokens)} must be greather than 1");

            if (noOfSuggestion <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(noOfSuggestion)} must be greather than 1");

            this.openAIDeploymentName = deploymentName;
            this.openAIMaxTokens = maxTokens;
            this.openAIQuestions = noOfSuggestion;
            
            client = new OpenAIClient(new Uri(endPoint), new AzureKeyCredential(key));
        }

        private OpenAISuggestionPrompt(string dialogId, PromptValidator<FoundChoice> validator = null, string defaultLocale = null)
            : this(dialogId,new Dictionary<string, ChoiceFactoryOptions>(GetSupportedCultures().ToDictionary(culture => culture.Locale, culture =>
                            new ChoiceFactoryOptions(culture.Separator, culture.InlineOr, culture.InlineOrMore, true))),
                validator,
                defaultLocale)
        {
            
        }

        private OpenAISuggestionPrompt(string dialogId, Dictionary<string, ChoiceFactoryOptions> choiceDefaults, PromptValidator<FoundChoice> validator = null, 
            string defaultLocale = null)
            : base(dialogId, validator)
        {
         
        }

        public async Task<List<string>> GenerateQuestions(string prompt)
        {

            var systemMessage =
                $"You are an assistant that generates follow-up {this.openAIQuestions} questions.Don't provide any additional information like sure or okay etc.";

            var chatCompletionsOptions = new ChatCompletionsOptions()
            {
                Messages =
                {
                    new ChatMessage(ChatRole.System,systemMessage),
                    new ChatMessage(ChatRole.User, prompt),
                },
                MaxTokens = this.openAIMaxTokens
            };

            Response<ChatCompletions> response = await client.GetChatCompletionsAsync(
                deploymentOrModelName: openAIDeploymentName,
                chatCompletionsOptions);

            var suggestQuestions = response.Value.Choices[0].Message.Content;

            // Split the string based on the numbers followed by a dot
            string[] splitQuestions = suggestQuestions.Split('?');

            List<string> questionsList = new List<string>();
            foreach (var question in splitQuestions)
            {
                if (string.IsNullOrEmpty(question))
                    continue;

                var quest = question.Trim() + "?";
                questionsList.Add(quest);
            }

            return questionsList;

        }


        protected override async Task OnPromptAsync(ITurnContext turnContext, IDictionary<string, object> state, PromptOptions options, bool isRetry,
            CancellationToken cancellationToken = new CancellationToken())
        {

            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var userPrompt = turnContext.Activity.Text;

            if (string.IsNullOrEmpty(userPrompt))
                throw new ArgumentNullException("User Prompt should not be null");

            options.Choices = new List<Microsoft.Bot.Builder.Dialogs.Choices.Choice>();

            var choicesList = await GenerateQuestions(userPrompt);

            foreach (var suggestionPrompt in choicesList)
            {
                var choice = new Choice()
                {
                    Value = suggestionPrompt,
                    Synonyms = new List<string>() { suggestionPrompt },
                };  
                options.Choices.Add(choice);
            }

            await base.OnPromptAsync(turnContext, state, options, isRetry, cancellationToken);

        }
      
    }
}
