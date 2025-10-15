namespace CharacterCustomization.Characters;



public abstract class Character
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel => Level * 100;
    
    public SpecialOutcome? LastSpecialOutcome { get; set; }
    public void ClearSpecialOutcome() => LastSpecialOutcome = null;

    // NEW: MaxHealth so we can cap heals & scale by level
    public int MaxHealth { get;  set; }

    protected Character(string name, int health, int level, int experience = 0)
    {
        Name = name;
        Health = health;
        MaxHealth = health;
        Level = level;
        Experience = experience;
    }

    public void GainExperience(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            Level++;
            OnLevelUp();
            System.Console.WriteLine($"🎉 {Name} leveled up! Now Level {Level}.");
        }
    }

    // Let subclasses re-compute stats (health/energy/scalars) on level up
    protected virtual void OnLevelUp()
    {
        // Default: bump MaxHealth a little and heal to new max
        MaxHealth += 10;
        Health = MaxHealth;
    }
}