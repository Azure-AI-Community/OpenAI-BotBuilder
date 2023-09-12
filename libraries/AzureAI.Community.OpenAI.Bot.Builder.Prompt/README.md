# AzureAI.Community.OpenAI.Bot.Builder.Prompt - Preview

## Description

This package contains OpenAI prompts to provide support for use in Bot Composer.

Currently, the following Prompts are available:

1. [Prompt Completions](#prompt-completions)
2. [Prompt ChatCompletions](#prompt-chatcompletions)
3. [DALL-E](#DALL-E)

## Changes for Bot Composer Target Framework NET 7.0

**Important Note:** Bot Composer now targets .NET 7.0.

## Composer Component Installation

Follow these steps to install the component:

1. Navigate to the Bot Framework Composer Package Manager.
2. Change the filter to Community packages.
3. Search for 'AzureAI' and install `AzureAI.Community.OpenAI.Bot.Builder.Prompt`.

   <img width="1175" alt="image" src="https://github.com/Azure-AI-Community/OpenAI-BotBuilder/assets/16264167/132cfb64-e357-43cc-b346-6e0c32c7e946">


After installing the package, you will find Azure OpenAI menus available in the main menu.

<img width="419" alt="image" src="https://github.com/Azure-AI-Community/OpenAI-BotBuilder/assets/16264167/c5be9020-98e0-4df7-9baf-d9073771e0cb">




## Prompt Completions

Prompt Completions: Returns textual completions as configured for a given prompt.

![Prompt Completions](https://github.com/Azure-AI-Community/OpenAI-BotBuilder/assets/16264167/285a6f7e-d4b9-48f6-82d9-5e0aba577659)

### Example

- **Endpoint:** https://github.com/Azure-AI-Community/OpenAI-BotBuilder
- **API Key:** 123131asdasd2132132
- **Deployment or Model Name:** davinici

#### Prompt Configuration

This section is used to pass the prompt and parameter configurations, and Prompt Configuration is in required JSON format.

**Note:** The "Prompt" parameter is a required field; the rest of the parameters are optional.

Here's a complete config structure example:

```json
{
    "Prompts": ["prompt1", "prompt2"],
    "GenerationSampleCount": 3,
    "Temperature": 0.75,
    "User": "AzureAICommunityUser",
    "Echo": true,
    "LogProbabilityCount": 1,
    "MaxTokens": 512,
    "TokenSelectionBiases": {
        "25996": -100,
        "35484": -100,
        "40058": -100,
        "15991": -100
    },
    "StopSequences": ["\n"],
    "ChoicesPerPrompt": 3,
    "FrequencyPenalty": 0.5,
    "NucleusSamplingFactor": 0.9,
    "PresencePenalty": 0.1
}
```
## output
To obtain the result, utilize `${turn.OpenAI}`.


## Prompt ChatCompletions

**Description:** Returns chat completions for the provided chat context message.

![Prompt ChatCompletions](https://github.com/Azure-AI-Community/OpenAI-BotBuilder/assets/16264167/4fabe194-81ac-4d41-b628-6f0afe30bc71)

## Example

- **Endpoint:** https://github.com/Azure-AI-Community/OpenAI-BotBuilder
- **API Key:** 123131asdasd2132132
- **Deployment or Model Name:** GPT4

### Prompt Chat Configuration

This section is used to pass the `ChatMessages` and parameter configurations, and Prompt Configuration is in the required JSON format.

**Note:** The "ChatMessages" parameter is a required field; the rest of the parameters are optional.

Here is a complete config structure:

```json
{
    "ChoiceCount": 1,
    "FrequencyPenalty": 0.5,
    "MaxTokens": 512,
    "NucleusSamplingFactor": 0.9,
    "PresencePenalty": 0.1,
    "StopSequences": ["\\n"],
    "Temperature": 0.75,
    "TokenSelectionBiases": {
        "25996": -100,
        "35484": -100,
        "40058": -100,
        "15991": -100
    },
    "User": "AzureAICommunityUser",
    "ChatMessages": [
        {
            "Text": "Hello, how are you?",
            "Sender": "user"
        },
        {
            "Text": "I'm fine, thank you. And you?",
            "Sender": "Assistant"
        }
    ]
}
```
## output
To obtain the result, utilize `${turn.OpenAI}`




## DALL-E

### DALL-E Image Generation

### Prompt Configuration

- **Endpoint:** Connect to OpenAI
- **API Key:** Your OpenAI API Key
- **Prompt:** Specify the type of image you want to create.
- **Image Size:** Desired image resolution size.
- **Image Count:** Number of images to generate.
- **User:** A unique identifier representing your end-user, which aids in monitoring and abuse detection.

  <img width="1289" alt="image" src="https://github.com/Azure-AI-Community/OpenAI-BotBuilder/assets/16264167/21119ef9-7382-4bee-9749-edd8c3688b74">



## output
To obtain the result, utilize `${turn.OpenAI}`

