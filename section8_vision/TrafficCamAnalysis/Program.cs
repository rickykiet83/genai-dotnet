using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient client =
    new OllamaApiClient(new Uri("http://localhost:11434"), "llava");


foreach (var imagePath in Directory.GetFiles("images", "*.jpg"))
{
    var name = Path.GetFileNameWithoutExtension(imagePath);

    var message = new ChatMessage(ChatRole.User, $$"""
                                                   Extract information from this image from camera {{name}}.

                                                       Respond with a JSON object in this form: {
                                                       "Status": string // One of these values: "Clear", "Flowing", "Congested", "Blocked",
                                                       "NumCars": number,
                                                       "NumTrucks": number
                                                   }
                                                   """);
    message.Contents.Add(new DataContent(File.ReadAllBytes(imagePath), "image/jpg"));

    var response = await client.GetResponseAsync<TrafficCamResult>([message]);

    if (response.TryGetResult(out var result))
    {
        Console.WriteLine($"{name} status: {result.Status} (cars: {result.NumCars}, trucks: {result.NumTrucks})");
    }
}

class TrafficCamResult
{
    public TrafficStatus Status { get; set; }
    public int NumCars { get; set; }
    public int NumTrucks { get; set; }

    public enum TrafficStatus
    {
        Clear,
        Flowing,
        Congested,
        Blocked
    };
}