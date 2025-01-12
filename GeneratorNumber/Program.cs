var filePath = "numbers.txt";
var random = new Random();

var cts = new CancellationTokenSource();
var token = cts.Token;

var writerThread = new Thread(() =>
{
    while (!token.IsCancellationRequested)
    {
        var randomNumber = random.Next(1, 100);
        File.AppendAllText(filePath, randomNumber + Environment.NewLine);
        Console.WriteLine($"Number: {randomNumber}");

        Thread.Sleep(500);
    }
});

writerThread.Start();
Console.WriteLine("Press any key to continue...");
Console.ReadKey();

cts.Cancel();

writerThread.Join();



