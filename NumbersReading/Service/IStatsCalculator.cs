using System.Collections.Concurrent;

namespace NumbersReading.Service;

public interface IStatsCalculator
{
    double CalculateAverage(double averege, int count, int data);

    double CalculateStandardDeviation(double average, double averageSquare, int count, int data);

    int CalculateMode(ConcurrentDictionary<int, int> collection, int number);

    double CalculateMedian(ConcurrentDictionary<int, int> numbers);
}
