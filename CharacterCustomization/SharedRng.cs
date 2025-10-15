using System;

public static class SharedRng
{
    private static readonly Random _rng = new Random();
    public static int Next(int min, int max) => _rng.Next(min, max);
}