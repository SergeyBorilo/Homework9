using System.Collections.Concurrent;
using NumbersReading.Service;

var filePath = "numbers.txt";
var numberFrequency = new ConcurrentDictionary<int, int>();
var numbers = new ConcurrentQueue<int>();
var average = 0.0;
var averageSquare = 0.0;
var standardDeviation = 0.0;
var mode = 0.0;
var median = 0.0;

var cts = new CancellationTokenSource();
var token = cts.Token;

var readThread = new Thread(() =>
{
    while (!token.IsCancellationRequested)
    {
        lock (filePath)
        {
            if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                IEnumerable<int> fileNumbers;

                fileNumbers = File.ReadLines(filePath)
                    .Select(line => int.TryParse(line, out var num) ? (int?)num : null)
                    .Where(num => num.HasValue)
                    .Select(num => num.Value)
                    .ToList();
                File.WriteAllText(filePath, string.Empty);

                foreach (var number in fileNumbers) numbers.Enqueue(number);
            }
        }

        Thread.Sleep(500);
    }
});

var calculationThread = new Thread(() =>
{
    var count = 0;
    while (!token.IsCancellationRequested)
    {
        var statsCalculator = new StatsCalculator();
        while (numbers.TryDequeue(out var number))
        {
            count++;
            standardDeviation = statsCalculator.CalculateStandardDeviation(average, averageSquare, count, number);
            average = statsCalculator.CalculateAverage(average, count - 1, number);
            averageSquare = statsCalculator.CalculateAverage(averageSquare, count - 1, number * number);
            mode = statsCalculator.CalculateMode(numberFrequency, number);
            median = statsCalculator.CalculateMedian(numberFrequency);

            Console.WriteLine($"Number: {number}");

            Console.WriteLine($"Average: {average}, Standard Deviation: {standardDeviation}, Mode: {mode}, Median: {median}");
        }

        Thread.Sleep(100);
    }
});

readThread.Start();
calculationThread.Start();

cts.Cancel();

readThread.Join();
calculationThread.Join();

Console.WriteLine("Program finished");
