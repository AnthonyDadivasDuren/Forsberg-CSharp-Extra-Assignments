namespace CharacterCustomization.Characters;



public abstract class Character(string name, int health, int level, int experience = 0)
{
    public string Name { get; set; } = name;
    public int Health { get; set; } = health;
    public int Level { get; set; } = level;
    public int Experience { get; private set; } = experience;
    public int ExperienceToNextLevel => Level * 100;
    
    public SpecialOutcome? LastSpecialOutcome { get; protected set; }
    public void ClearSpecialOutcome() => LastSpecialOutcome = null;
    
    public int MaxHealth { get;  set; } = health;

    public void GainExperience(int amount)
    {
        Experience += amount;
        while (Experience >= ExperienceToNextLevel)
        {
            Experience -= ExperienceToNextLevel;
            Level++;
            OnLevelUp();
            Console.WriteLine($"🎉 {Name} leveled up! Now Level {Level}.");
        }
    }

 
    protected virtual void OnLevelUp()
    {
        // Default: bump MaxHealth a little and heal to new max
        MaxHealth += 10;
        Health = MaxHealth;
    }
}