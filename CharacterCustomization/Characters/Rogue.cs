namespace CharacterCustomization.Characters;

class Rogue : Character, ICharacterActions, ISpecial, IAiSpecial
{
    private int Agility { get; set; }
    public int Energy { get; set; }
    public int HealingPotions { get; set; } = 3;
    
    public int MaxEnergy => 40 + (Level * 8);

    public Rogue(string name, int level)
        : base(name, 85 + (level * 16), level)
    {
        Agility = 18 + (level * 7);
        Energy  = 80 + (level * 15);
        MaxHealth = 85 + (Level * 16);
    }

    public int Attack()
    {
        
        int restored = 5 + (int)(Level * 0.5);
        Energy = Math.Min(Energy + restored, MaxEnergy);
        Console.WriteLine($"{Name} recovers {restored} stamina with a basic slash.");

        // Higher miss chance (unreliable style)
        if (SharedRng.Next(1, 101) <= 12)
        {
            Console.WriteLine($"{Name} missteps and misses!");
            return 0;
        }
        
        bool crit = SharedRng.Next(1, 101) <= 30;

        int baseDmg = (int)(Agility * 0.8 + Level * 3 + SharedRng.Next(5, 15));
        int dmg = crit ? (int)(baseDmg * 1.5) : baseDmg;

        Console.WriteLine(crit
            ? $"{Name} lands a precise CRITICAL strike for {dmg} damage!"
            : $"{Name} slashes swiftly for {dmg} damage!");

        return dmg;
    }   

    public void Defend()
    {
        if (Energy < 5) { Console.WriteLine($"{Name} lacks energy to evade!"); return; }
        Energy -= 5;
        Console.WriteLine($"{Name} prepares to Evade, reducing next damage. (-5 Energy)");
    }

    public void Heal()
    {
        if (HealingPotions <= 0)
        {
            Console.WriteLine($"{Name} has no healing potions left!");
            return;
        }

        HealingPotions--;
        int healAmount = 15 + Level * 5;
        Health = Math.Min(MaxHealth, Health + healAmount);

        Console.WriteLine($"{Name} drinks a potion and heals {healAmount} HP. ({Health}/{MaxHealth})");
    }

    public void Recover()
    {
        int amount = 20;
        Energy += amount;
        Console.WriteLine($"{Name} catches breath, +{amount} energy. (Energy: {Energy})");
    }

    // SPECIAL: Shadow Cut — heavy hit + BLEED (2 turns)
    public int SpecialAttack(string choice)
    {
        return choice switch
        {
            "1" => ShadowCut(),
            "2" => Ambush(),
            "3" => DeathMark(),
            "4" => SmokeBomb(),
            _   => 0
        };
    }

public int SpecialAttackAi(Character defender)
{
    if (Energy >= 30 && defender.Health <= defender.MaxHealth * 6 / 10) return DeathMark();
    if (Energy >= 20) return Ambush();
    if (Energy >= 12) return ShadowCut();
    if (Energy >= 15) return SmokeBomb();
    return 0;
}

private int ShadowCut()
{
    if (Energy < 12) { Console.WriteLine($"{Name} lacks Energy!"); return 0; }
    Energy -= 12;
    if (SharedRng.Next(1,101) <= 12) { Console.WriteLine($"{Name} misses the cut!"); return 0; }

    int baseDmg = (int)(Agility * 0.9 + Math.Sqrt(Level) * 5 + SharedRng.Next(8, 14));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} slices and applies a bleeding wound.");
    LastSpecialOutcome = new SpecialOutcome { Effect="bleed", Turns=2, Potency=Level * 6, SelfTarget=false };
    return dmg;
}

private int Ambush()
{
    if (Energy < 20) { Console.WriteLine($"{Name} lacks Energy!"); return 0; }
    Energy -= 20;
    if (SharedRng.Next(1,101) <= 15) { Console.WriteLine($"{Name}'s ambush fails!"); return 0; }

    bool crit = SharedRng.Next(1,101) <= 35;
    int baseDmg = (int)(Agility * 1.2 + Math.Sqrt(Level) * 6 + SharedRng.Next(10, 18));
    int dmg = crit ? (int)(baseDmg * 1.6) : baseDmg;
    Console.WriteLine(crit ? $"{Name} lands a critical ambush for {dmg}!" : $"{Name} ambushes for {dmg} damage.");
    return dmg;
}

private int DeathMark()
{
    if (Energy < 30) { Console.WriteLine($"{Name} lacks Energy!"); return 0; }
    Energy -= 30;
    if (SharedRng.Next(1,101) <= 20) { Console.WriteLine($"{Name}'s mark fails!"); return 0; }

    int baseDmg = (int)(Agility * 1.4 + Math.Sqrt(Level) * 7 + SharedRng.Next(12, 20));
    int dmg = baseDmg;
    int cap = (int)(Level * 20 * 0.45);
    if (dmg > cap) dmg = cap;

    Console.WriteLine($"{Name} strikes a Death Mark for {dmg} damage.");
    return dmg;
}

private int SmokeBomb()
{
    if (Energy < 15) { Console.WriteLine($"{Name} lacks Energy!"); return 0; }
    Energy -= 15;
    if (SharedRng.Next(1,101) <= 5) { Console.WriteLine($"{Name} fumbles the bomb!"); return 0; }

    int baseDmg = (int)(Agility * 0.5 + Math.Sqrt(Level) * 3 + SharedRng.Next(4, 10));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} vanishes in smoke, preparing to evade.");
    LastSpecialOutcome = new SpecialOutcome { Effect="defend", Turns=1, Potency=0, SelfTarget=true };
    return dmg;
}


    public override string ToString()
        => $"[Rogue] {Name} | Lv {Level} | HP {Health}/{MaxHealth} | AGI {Agility} | EN {Energy} | Potions {HealingPotions}";
}