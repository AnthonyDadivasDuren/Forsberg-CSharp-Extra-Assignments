namespace CharacterCustomization.Characters;

class Paladin : Character, ICharacterActions, ISpecial, IAiSpecial
{
    private int Faith { get; set; }
    public int Zeal { get; set; }
    public int HealingPotions { get;  set; } = 3;
    
    public int MaxZeal => 50 + (Level * 10);

    public Paladin(string name, int level)
        : base(name, 110 + (level * 22), level)
    {
        Faith = 12 + (level * 5);
        Zeal  = 60 + (level * 12);
        MaxHealth = 110 + (Level * 22);
    }

    public int Attack()
    {
        int restored = 5 + (int)(Level * 0.5);
        Zeal = Math.Min(Zeal + restored, Zeal); 
        Console.WriteLine($"{Name} recovers {restored} stamina with a basic holy strike.");

        
        if (SharedRng.Next(1, 101) <= 10)
        {
            Console.WriteLine($"{Name}'s holy strike misses!");
            return 0;
        }

        // Moderate crit chance
        bool crit = SharedRng.Next(1, 101) <= 20;
        
        int baseDmg = (int)(Faith * 1.5 + Level * 3 + SharedRng.Next(8, 18));

        int dmg = crit ? (int)(baseDmg * 1.5) : baseDmg;

        Console.WriteLine(crit
            ? $"{Name}'s radiant smite CRITICALLY strikes for {dmg} holy damage!"
            : $"{Name} smites the foe with divine power for {dmg} damage!");

        return dmg;
    }
    public void Defend()
    {
        if (Zeal < 6) { Console.WriteLine($"{Name} lacks zeal to brace!"); return; }
        Zeal -= 6;
        Console.WriteLine($"{Name} raises a holy guard, reducing next damage. (-6 Zeal)");
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
        int amount = 18;
        Zeal += amount;
        Console.WriteLine($"{Name} renews zeal by {amount}. (Zeal: {Zeal})");
    }

    // SPECIAL: Holy Smite — big hit + temporary self-shield (handled in battle)
   public int SpecialAttack(string choice)
{

    return choice switch
    {
        "1" => HolySmite(),
        "2" => DivineShield(),
        "3" => Judgement(),
        "4" => Consecration(),
        _   => 0
    };
}

public int SpecialAttackAi(Character defender)
{
    if (Zeal >= 24 && defender.Health <= defender.MaxHealth * 4 / 10) return Judgement();
    if (Zeal >= 28) return Consecration();
    if (Zeal >= 18 && Health <= MaxHealth * 6 / 10) return DivineShield();
    if (Zeal >= 14) return HolySmite();
    return 0;
}

private int HolySmite()
{
    if (Zeal < 14) { Console.WriteLine($"{Name} lacks Zeal!"); return 0; }
    Zeal -= 14;
    if (SharedRng.Next(1,101) <= 10) { Console.WriteLine($"{Name}'s smite misses!"); return 0; }

    int baseDmg = (int)(Faith * 1.0 + Math.Sqrt(Level) * 5 + SharedRng.Next(8, 16));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} smites for {dmg} damage and sanctifies the foe.");
    LastSpecialOutcome = new SpecialOutcome { Effect="burn", Turns=1, Potency=Level * 6, SelfTarget=false };
    return dmg;
}

private int DivineShield()
{
    if (Zeal < 18) { Console.WriteLine($"{Name} lacks Zeal!"); return 0; }
    Zeal -= 18;
    if (SharedRng.Next(1,101) <= 5) { Console.WriteLine($"{Name}'s prayer fails!"); return 0; }

    int baseDmg = (int)(Faith * 0.6 + Math.Sqrt(Level) * 4 + SharedRng.Next(6, 12));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} is blessed with a radiant barrier and deals {dmg} damage.");
    LastSpecialOutcome = new SpecialOutcome { Effect="shield", Turns=0, Potency=Level * 8, SelfTarget=true };
    return dmg;
}

private int Judgement()
{
    if (Zeal < 24) { Console.WriteLine($"{Name} lacks Zeal!"); return 0; }
    Zeal -= 24;
    if (SharedRng.Next(1,101) <= 18) { Console.WriteLine($"{Name}'s judgement fails!"); return 0; }

    int baseDmg = (int)(Faith * 1.4 + Math.Sqrt(Level) * 7 + SharedRng.Next(12, 20));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} delivers Judgement for {dmg} damage and stuns the foe.");
    LastSpecialOutcome = new SpecialOutcome { Effect="stun", Turns=1, Potency=0, SelfTarget=false };
    return dmg;
}

private int Consecration()
{
    if (Zeal < 28) { Console.WriteLine($"{Name} lacks Zeal!"); return 0; }
    Zeal -= 28;
    if (SharedRng.Next(1,101) <= 20) { Console.WriteLine($"{Name}'s consecration fizzles!"); return 0; }

    int baseDmg = (int)(Faith * 1.2 + Math.Sqrt(Level) * 6 + SharedRng.Next(10, 18));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} consecrates the ground for {dmg} damage and burns the foe.");
    LastSpecialOutcome = new SpecialOutcome { Effect="burn", Turns=2, Potency=Level * 7, SelfTarget=false };
    return dmg;
}

    public override string ToString()
        => $"[Paladin] {Name} | Lv {Level} | HP {Health}/{MaxHealth} | FAI {Faith} | ZEAL {Zeal} | Potions {HealingPotions}";
}