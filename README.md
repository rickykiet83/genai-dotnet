# GenAI for .NET

This repository is a hands-on space for learning how to integrate OpenAI, Ollama, and .NET's new [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai) (MEAI) abstractions. Its examples build a foundation for a wide range of generative AI applications, including chatbots, semantic search, Retrieval-Augmented Generation (RAG), and image analysis.

## Prerequisites

- .NET 10 SDK
- An API credential for the provider used by an example:
  - `OpenAI:ApiKey` for OpenAI models
  - `GitHubModels:Token` for GitHub Models
- Ollama installed and running locally for Ollama-based examples

Keep credentials out of source control. This repository uses [.NET user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for local development.

## Configure credentials

Set the credential required by the sample you want to run. For example, the current `TextCompletion` sample uses OpenAI:

```bash
dotnet user-secrets set "OpenAI:ApiKey" "your-openai-api-key" --project TextCompletion
```

For a GitHub Models example, set a GitHub token instead:

```bash
dotnet user-secrets set "GitHubModels:Token" "your-github-models-token" --project TextCompletion
```

## Run an example

```bash
dotnet run --project TextCompletion
```

The `TextCompletion` project demonstrates a simple `IChatClient` workflow using MEAI with an OpenAI chat model.

## Learning path

1. Start with text completion and chat clients.
2. Swap model providers while retaining MEAI abstractions.
3. Add embeddings and semantic search.
4. Build RAG pipelines over local or remote content.
5. Explore multimodal scenarios such as image analysis.
