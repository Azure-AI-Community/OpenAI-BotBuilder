using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureAI.Community.OpenAI.Bot.Builder.Prompt
{
    public class AzureOpenAIBotPromptComponent : BotComponent
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddSingleton<DeclarativeType>(sp =>
            new DeclarativeType<PromptCompletionsDialog>(PromptCompletionsDialog.Kind));

            services.AddSingleton<DeclarativeType>(sp =>
                new DeclarativeType<PromptChatCompletionsDialog>(PromptChatCompletionsDialog.Kind));
        }
    }
}
