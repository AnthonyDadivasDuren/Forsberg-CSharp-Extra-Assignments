using CharacterCustomization.Characters;


namespace CharacterCustomization.Systems
{
    public static class ExperienceSystem
    {
        public static void AwardExperience(Character winner, Character loser)
        {
            int levelDiff = loser.Level - winner.Level;
            int baseXp = 50 + (loser.Level * 20);
            int bonus = Math.Max(0, levelDiff * 10);
            int xp = baseXp + bonus;

            Console.WriteLine($"{winner.Name} gains {xp} XP! ({winner.Experience}/{winner.ExperienceToNextLevel})");
            winner.GainExperience(xp);

            if (winner.Level > 50) winner.Level = 50; // max level cap
        }
    }
}