using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using System.ClientModel;

// get credentials from user secrets
IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var credential = new ApiKeyCredential(config["GitHubModels:Token"] ??
                                      throw new InvalidOperationException(
                                          "Missing configuration: GitHubModels:Token."));
var endpoint = config["GitHubModels:Endpoint"] ??
               throw new InvalidOperationException("Missing configuration: GitHubModels:Endpoint.");
var model = config["GitHubModels:Model"] ??
            throw new InvalidOperationException("Missing configuration: GitHubModels:Model.");
var options = new OpenAIClientOptions()
{
    Endpoint = new Uri(endpoint)
};

// create a chat client
IChatClient client =
    new OpenAIClient(credential, options).GetChatClient(model).AsIChatClient();

#region Basic Completion

//// send prompt and get response
// string prompt = "What is AI ? explain max 20 word";
// Console.WriteLine($"user >>> {prompt}");
//
// ChatResponse response = await client.GetResponseAsync(prompt);
//
// Console.WriteLine($"assistant >>> {response}");
// Console.WriteLine($"Tokens used: in={response.Usage?.InputTokenCount}, out={response.Usage?.OutputTokenCount}");

#endregion

#region Streaming

//
// string prompt = "What is AI ? explain max 200 word";
// Console.WriteLine($"user >>> {prompt}");
//
// var responseStream = client.GetStreamingResponseAsync(prompt);
// await foreach (var message in responseStream)
// {
//     Console.Write(message.Text);
// }

#endregion

#region Classification

//
// var classificationPrompt = """
// Please classify the following sentences into categories: 
// - 'complaint' 
// - 'suggestion' 
// - 'praise' 
// - 'other'.
//
// 1) "I love the new layout!"
// 2) "You should add a night mode."
// 3) "When I try to log in, it keeps failing."
// 4) "This app is decent."
// """;
//
// Console.WriteLine($"user >>> {classificationPrompt}");
//
// ChatResponse classificationResponse = await client.GetResponseAsync(classificationPrompt);
//
// Console.WriteLine($"assistant >>>\n{classificationResponse}");

#endregion

#region Summarization

//
// var summaryPrompt = """
// Summarize the following blog in 1 concise sentences:
//
// "Microservices architecture is increasingly popular for building complex applications, but it comes with additional overhead. It's crucial to ensure each service is as small and focused as possible, and that the team invests in robust CI/CD pipelines to manage deployments and updates. Proper monitoring is also essential to maintain reliability as the system grows."
// """;
//
// Console.WriteLine($"user >>> {summaryPrompt}");
//
// ChatResponse summaryResponse = await client.GetResponseAsync(summaryPrompt);
//
// Console.WriteLine($"assistant >>> \n{summaryResponse}");

#endregion

#region Sentiment Analysis

//
// var analysisPrompt = """
//         You will analyze the sentiment of the following product reviews. 
//         Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews.
//
//         I bought this product and it's amazing. I love it!
//         This product is terrible. I hate it.
//         I'm not sure about this product. It's okay.
//         I found this product based on the other reviews. It worked for a bit, and then it didn't.
//         """;
//
// Console.WriteLine($"user >>> {analysisPrompt}");
//
// ChatResponse responseAnalysis = await client.GetResponseAsync(analysisPrompt);
//
// Console.WriteLine($"assistant >>> \n{responseAnalysis}");

#endregion

#region Structured output

//
// var carListings = new[]
// {
//     "Experience luxury with this brand-new 2024 BMW X5 SUV. Available for sale at $92,500. Equipped with xDrive AWD, panoramic sunroof, leather interior, adaptive cruise control, and wireless Apple CarPlay. A premium family SUV delivering exceptional comfort and performance.",
//     "Looking for an affordable commuter? This used 2018 Mazda 3 hatchback has travelled 72,000 km and is available for sale for $19,800. Features include reverse camera, blind spot monitoring, keyless entry, Bluetooth connectivity, and alloy wheels. A reliable daily driver that's economical and fun to drive.",
//     "Lease this new 2025 Hyundai Ioniq 5 EV today! Monthly lease equivalent starts from $580. Enjoy ultra-fast charging, dual 12-inch displays, adaptive cruise control, heated front seats, and Vehicle-to-Load (V2L) functionality. A modern electric SUV built for the future.",
//     "Own a well-maintained used 2020 Ford Ranger Wildtrak ute for $46,900. Powered by a 2.0L Bi-Turbo diesel engine with 4WD capability. Includes tow package, roller shutter, Apple CarPlay, satellite navigation, and leather seats. Perfect for both work and weekend adventures.",
//     "Now available: a new 2024 Kia Sportage GT-Line. Purchase price is $51,990. This stylish SUV includes a panoramic sunroof, ventilated leather seats, 360-degree camera, premium Harman Kardon audio system, and advanced driver assistance technologies."
// };
//
// foreach (var listingText in carListings)
// {
//     var response = await client.GetResponseAsync<CarDetails>(
//         $"""
//         Convert the following car listing into a JSON object matching this C# schema:
//         Condition: "New" or "Used"
//         Make: (car manufacturer)
//         Model: (car model)
//         Year: (four-digit year)
//         ListingType: "Sale" or "Lease"
//         Price: integer only
//         Features: array of short strings
//         TenWordSummary: exactly ten words to summarize this listing
//
//         Here is the listing:
//         {listingText}
//         """);
//
//     if (response.TryGetResult(out var info))
//     {
//         // Convert the CarDetails object to JSON for display
//         Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(
//             info, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
//     }
//     else
//     {
//         Console.WriteLine("Response was not in the expected format.");
//     }
// }
//
// class CarDetails
// {
//     public required string Condition { get; set; }  // e.g. "New" or "Used"
//     public required string Make { get; set; }
//     public required string Model { get; set; }
//     public int Year { get; set; }
//     public CarListingType ListingType { get; set; }
//     public int Price { get; set; }
//     public required string[] Features { get; set; }
//     public required string TenWordSummary { get; set; }
// }
//
// [JsonConverter(typeof(JsonStringEnumConverter))]
// enum CarListingType { Sale, Lease }

#endregion

#region ChatApp

// Start the conversation with context for the AI model
List<ChatMessage> chatHistory = new()
{
    new ChatMessage(ChatRole.System, """
                                         You are a friendly hiking enthusiast who helps people discover fun hikes in their area.
                                         You introduce yourself when first saying hello.
                                         When helping people out, you always ask them for this information
                                         to inform the hiking recommendation you provide:

                                         1. The location where they would like to hike
                                         2. What hiking intensity they are looking for

                                         You will then provide three suggestions for nearby hikes that vary in length
                                         after you get that information. You will also share an interesting fact about
                                         the local nature on the hikes when making a recommendation. At the end of your
                                         response, ask if there is anything else you can help with.
                                     """)
};

while (true)
{
    // Get user prompt and add to chat history
    Console.WriteLine("Your prompt:");
    var userPrompt = Console.ReadLine();
    chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));

    // Stream the AI response and add to chat history
    Console.WriteLine("AI Response:");
    var response = "";
    await foreach (var item in
                   client.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(item.Text);
        response += item.Text;
    }

    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
    Console.WriteLine();
}

#endregion