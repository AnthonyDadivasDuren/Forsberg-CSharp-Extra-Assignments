namespace CharacterCustomization;

public static class SharedRng
{
    private static readonly Random Rng = new Random();
    public static int Next(int min, int max) => Rng.Next(min, max);
}