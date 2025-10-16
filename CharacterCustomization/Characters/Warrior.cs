namespace CharacterCustomization.Characters;

class Warrior : Character, ICharacterActions, ISpecial, IAiSpecial

{
    private int Strength { get; set; }
    public int Stamina { get; set; }
    public int HealingPotions { get; set; } = 3;
    
    public int MaxStamina => 60 + (Level * 12);

    public Warrior(string name, int level)
        : base(name, CalculateHealth(level), level)
    {
        RecalculateStats();
    }

    private static int CalculateHealth(int level) => 100 + (level * 20);

    private void RecalculateStats()
    {
        Strength = 10 + (Level * 5);
        Stamina  = 50 + (Level * 10);
        MaxHealth = CalculateHealth(Level);
        if (Health > MaxHealth) Health = MaxHealth;
    }

    protected override void OnLevelUp() => RecalculateStats();

    
    public int Attack()
    {
     
        int restored = 5 + (int)(Level * 0.5);
         Stamina = Math.Min(Stamina + restored, Stamina); 
         Console.WriteLine($"{Name} recovers {restored} stamina with a basic strike.");

        // Small miss chance
        if (SharedRng.Next(1, 101) <= 8)
        {
            Console.WriteLine($"{Name} swings and misses!");
            return 0;
        }

        // Low crit chance, high multiplier
        bool crit = SharedRng.Next(1, 101) <= 15;

        int baseDmg = (int)(Strength * 1.5 + Level * 3 + SharedRng.Next(10, 20));
        int dmg = crit ? (int)(baseDmg * 1.75) : baseDmg;

        Console.WriteLine(crit
            ? $"{Name} delivers a devastating CRITICAL blow for {dmg} damage!"
            : $"{Name} slams the enemy for {dmg} damage!");

        return dmg;
    }

    public void Defend()
    {
        if (Stamina < 5) { Console.WriteLine($"{Name} is too exhausted to defend!"); return; }
        Stamina -= 5;
        Console.WriteLine($"{Name} raises a shield, reducing next damage taken. (-5 Stamina)");
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
        int amount = 15;
        Stamina += amount;
        Console.WriteLine($"{Name} takes a breather and regains {amount} stamina. (Stamina: {Stamina})");
    }

    // SPECIAL: Power Strike — costs 30 stamina, high damage + STUN (skip target next turn)
   public int SpecialAttack(string choice)
{
    return choice switch
    {
        "1" => ShieldBash(),
        "2" => PowerStrike(),
        "3" => Whirlwind(),
        "4" => RallyingGuard(),
        _   => 0
    };
}

public int SpecialAttackAi(Character defender)
{
    if (Stamina >= 28 && defender.Health <= defender.MaxHealth * 6 / 10) return Whirlwind();
    if (Stamina >= 12 && defender.Health <= defender.MaxHealth * 3 / 10) return ShieldBash(); 
    if (Stamina >= 18) return PowerStrike();
    if (Stamina >= 15) return RallyingGuard();
    return 0;
}

// Specials
private int ShieldBash()
{
    if (Stamina < 12) { Console.WriteLine($"{Name} lacks Stamina!"); return 0; }
    Stamina -= 12;
    if (SharedRng.Next(1,101) <= 15) { Console.WriteLine($"{Name}'s Shield Bash misses!"); return 0; }

    int baseDmg = (int)(Strength * 1.0 + Math.Sqrt(Level) * 5 + SharedRng.Next(6, 12));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} uses Shield Bash for {dmg} damage and stuns the foe!");
    LastSpecialOutcome = new SpecialOutcome { Effect="stun", Turns=1, Potency=0, SelfTarget=false };
    return dmg;
}

private int PowerStrike()
{
    if (Stamina < 18) { Console.WriteLine($"{Name} lacks Stamina!"); return 0; }
    Stamina -= 18;
    if (SharedRng.Next(1,101) <= 20) { Console.WriteLine($"{Name}'s Power Strike misses!"); return 0; }

    bool crit = SharedRng.Next(1,101) <= 18;
    int baseDmg = (int)(Strength * 1.6 + Math.Sqrt(Level) * 7 + SharedRng.Next(10, 18));
    int dmg = crit ? (int)(baseDmg * 1.5) : baseDmg;
    Console.WriteLine(crit ? $"{Name}'s Power Strike critically hits for {dmg}!" : $"{Name} strikes powerfully for {dmg} damage.");
    return dmg;
}

private int Whirlwind()
{
    if (Stamina < 28) { Console.WriteLine($"{Name} lacks Stamina!"); return 0; }
    Stamina -= 28;
    if (SharedRng.Next(1,101) <= 25) { Console.WriteLine($"{Name}'s Whirlwind misses!"); return 0; }

    int baseDmg = (int)(Strength * 1.9 + Math.Sqrt(Level) * 8 + SharedRng.Next(12, 22));
    int dmg = baseDmg;
    int cap = (int)(Level * 20 * 0.45);
    if (dmg > cap) dmg = cap;

    Console.WriteLine($"{Name} spins into a Whirlwind for {dmg} damage!");
    return dmg;
}

private int RallyingGuard()
{
    if (Stamina < 15) { Console.WriteLine($"{Name} lacks Stamina!"); return 0; }
    Stamina -= 15;
    if (SharedRng.Next(1,101) <= 10) { Console.WriteLine($"{Name}'s strike glances off!"); return 0; }

    int baseDmg = (int)(Strength * 0.8 + Math.Sqrt(Level) * 4 + SharedRng.Next(4, 10));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} strikes and braces for impact, gaining a small shield.");
    LastSpecialOutcome = new SpecialOutcome { Effect="shield", Turns=0, Potency=Level * 6, SelfTarget=true };
    return dmg;
}

    public override string ToString()
        => $"[Warrior] {Name} | Lv {Level} | HP {Health}/{MaxHealth} | STR {Strength} | STA {Stamina} | Potions {HealingPotions}";
}
