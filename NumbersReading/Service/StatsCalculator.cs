using System.Collections.Concurrent;

namespace NumbersReading.Service;

// I do not find extracting to the separate class necessary in this case.
// I would just use the methods in the DataConsumer Program class
// I did it for the sake of it being a task
public class StatsCalculator : IStatsCalculator
{
    public double CalculateAverage(double average, int count, int data)
    {
        return (average * count + data) / (count + 1);
    }

    public double CalculateStandardDeviation(double average, double averageSquare, int count, int data)
    {
        var newAverage = CalculateAverage(average, count, data);
        var newAverageSquare = CalculateAverage(averageSquare, count, data * data);
        return Math.Sqrt(newAverageSquare - newAverage * newAverage);
    }

    public int CalculateMode(ConcurrentDictionary<int, int> collection, int number)
    {
        collection.AddOrUpdate(number, 1, (key, oldValue) => oldValue + 1);

        var maxCount = 0;
        var mode = 0;

        Parallel.ForEach(collection, item =>
        {
            if (item.Value > maxCount)
            {
                maxCount = item.Value;
                mode = item.Key;
            }
        });

        return mode;
    }

    public double CalculateMedian(ConcurrentDictionary<int, int> numbers)
    {
        var collection = numbers.AsParallel().SelectMany(x => Enumerable.Repeat(x.Key, x.Value)).Order();
        var size = collection.Count();

        if (size % 2 == 0)
        {
            var medianElements = collection.Skip(size / 2 - 1).Take(2).ToArray();
            return (medianElements[0] + medianElements[1]) / 2.0;
        }

        return collection.ElementAt(size / 2);
    }

}
