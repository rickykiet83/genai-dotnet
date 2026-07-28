// get credentials from user secrets

using System.ClientModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;

IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

string token = config["OpenAI:ApiKey"]
               ?? throw new InvalidOperationException("OpenAI:ApiKey is required");

var credentials = new ApiKeyCredential(token);

// The OpenAI SDK uses https://api.openai.com/v1 by default.
const string model = "gpt-5-mini";

// create a chat client
IChatClient client = new OpenAIClient(credentials)
    .GetChatClient(model).AsIChatClient();

// send prompt and get response
string prompt = "What is AI? explain max 30 words";
Console.WriteLine($"user >>> {prompt}");

ChatResponse response = await client.GetResponseAsync(prompt).ConfigureAwait(false);

Console.WriteLine($"assistant >>> {response}");
Console.WriteLine($"Token used: in={response.Usage?.InputTokenCount}; out={response.Usage?.OutputTokenCount}");
