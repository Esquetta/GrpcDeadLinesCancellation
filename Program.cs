
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDeadLinesCancellationServer;
using System.Threading;

var channel = GrpcChannel.ForAddress("http://localhost:5209");
var messageClient = new Message.MessageClient(channel);

//var response = await messageClient.SendMessageAsync(new MessageRequest { Message = "Furkana ,selam olsun..." },deadline:DateTime.UtcNow.AddSeconds(5));
//Console.WriteLine(response.Message);

//Console.WriteLine("asdasd");

CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
AsyncClientStreamingCall<MessageRequest, MessageResponse> call = messageClient.SendMessage(cancellationToken: cancellationTokenSource.Token);


var t1 = Task.Run(async () =>
{
    int count = 0;
    while (true)
        await call.RequestStream.WriteAsync(new MessageRequest { Message = $"mesaj {count++}" });
});


var t2 = Task.Run(async () =>
{
    ConsoleKey consoleKey = Console.ReadKey().Key;
    if (consoleKey == ConsoleKey.D)
    {
        call.Dispose();
        Console.WriteLine("Süreç dispose ile durduruldu.");
    }
    else if (consoleKey == ConsoleKey.C)
    {
        cancellationTokenSource.Cancel();
        Console.WriteLine("Süreç callationtoken ile durduruldu.");
    }

});


await Task.WhenAll(t1, t2);