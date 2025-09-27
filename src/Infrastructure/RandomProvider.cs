using Application.Abstractions;

namespace Infrastructure;

public class RandomProvider : IRandomProvider
{
    private readonly Random _rng = new();
    public int Next(int minValue, int maxValue) => _rng.Next(minValue, maxValue);
    public double NextDouble() => _rng.NextDouble();
}