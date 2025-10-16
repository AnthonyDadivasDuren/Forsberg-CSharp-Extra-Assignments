namespace CharacterCustomization.Characters;

class Mage : Character, ICharacterActions, ISpecial, IAiSpecial
{
    public int Mana { get; set; }
    private int Intelligence { get; set; }
    public int HealingPotions { get; set; } = 3;
    
    public int MaxMana => 80 + (Level * 15);

    public Mage(string name, int level)
        : base(name, CalculateHealth(level), level)
    {
        RecalculateStats();
    }

    private static int CalculateHealth(int level) => 80 + (level * 15);

    private void RecalculateStats()
    {
        Mana = 100 + (Level * 25);
        Intelligence = 15 + (Level * 6);
        MaxHealth = CalculateHealth(Level);
        if (Health > MaxHealth) Health = MaxHealth;
    }

    protected override void OnLevelUp() => RecalculateStats();

    public int Attack()
    {
        int restored = 5 + (int)(Level * 0.5);
        Mana = Math.Min(MaxMana, Mana + restored);
        Console.WriteLine($"{Name} restores {restored} mana with a basic cast.");

        if (SharedRng.Next(1, 101) <= 5) { Console.WriteLine($"{Name}'s spell fizzles out!"); return 0; }

        bool crit = SharedRng.Next(1, 101) <= 20;
        int baseDmg = (int)(Intelligence * 1.2 + Level * 4 + SharedRng.Next(15, 30));
        int dmg = crit ? (int)(baseDmg * 1.6) : baseDmg;

        Console.WriteLine(crit ? $"{Name}'s fireball EXPLODES for {dmg}!" : $"{Name} hurls a fireball for {dmg}.");
        return dmg;
    }

    public void Defend()
    {
        if (Mana < 5) { Console.WriteLine($"{Name} lacks mana to defend!"); return; }
        Mana -= 5;
        Console.WriteLine($"{Name} conjures a barrier, reducing next damage. (-5 Mana)");
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
        int amount = 25;
        Mana += amount;
        Console.WriteLine($"{Name} meditates and regains {amount} mana. (Mana: {Mana})");
    }

    // SPECIAL: Arcane Burst — costs 40 mana, big hit + applies BURN for 2 turns (DoT)
   public int SpecialAttack(string choice)
{
    return choice switch
    {
        "1" => Frostbolt(),
        "2" => FireBlast(),
        "3" => ArcaneSurge(),
        "4" => ManaShield(),
        _   => 0
    };
}

public int SpecialAttackAi(Character defender)
{
    if (Mana >= 30 && defender.Health <= defender.MaxHealth * 6 / 10) return ArcaneSurge();
    if (Mana >= 20) return FireBlast();
    if (Mana >= 18 && Health <= MaxHealth * 5 / 10) return ManaShield();
    if (Mana >= 12) return Frostbolt();
    return 0;
}

private int Frostbolt()
{
    if (Mana < 12) { Console.WriteLine($"{Name} lacks Mana!"); return 0; }
    Mana -= 12;
    if (SharedRng.Next(1,101) <= 8) { Console.WriteLine($"{Name}'s spell fizzles!"); return 0; }

    int baseDmg = (int)(Intelligence * 1.0 + Math.Sqrt(Level) * 5 + SharedRng.Next(10, 18));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} casts Frostbolt for {dmg} damage and chills the foe.");
    LastSpecialOutcome = new SpecialOutcome { Effect="burn", Turns=1, Potency=Level * 5, SelfTarget=false }; // reuse as chip DoT
    return dmg;
}

private int FireBlast()
{
    if (Mana < 20) { Console.WriteLine($"{Name} lacks Mana!"); return 0; }
    Mana -= 20;
    if (SharedRng.Next(1,101) <= 12) { Console.WriteLine($"{Name}'s fire sputters out!"); return 0; }

    bool crit = SharedRng.Next(1,101) <= 22;
    int baseDmg = (int)(Intelligence * 1.5 + Math.Sqrt(Level) * 7 + SharedRng.Next(14, 24));
    int dmg = crit ? (int)(baseDmg * 1.5) : baseDmg;
    Console.WriteLine(crit ? $"{Name}'s Fire Blast critically sears for {dmg}!" : $"{Name}'s Fire Blast hits for {dmg}.");

    LastSpecialOutcome = new SpecialOutcome { Effect="burn", Turns=2, Potency=Level * 7, SelfTarget=false };
    return dmg;
}

private int ArcaneSurge()
{
    if (Mana < 30) { Console.WriteLine($"{Name} lacks Mana!"); return 0; }
    Mana -= 30;
    if (SharedRng.Next(1,101) <= 15) { Console.WriteLine($"{Name}'s surge destabilizes!"); return 0; }

    int baseDmg = (int)(Intelligence * 1.8 + Math.Sqrt(Level) * 8 + SharedRng.Next(16, 28));
    int dmg = baseDmg;
    int cap = (int)(Level * 20 * 0.45);
    if (dmg > cap) dmg = cap;

    Console.WriteLine($"{Name} unleashes Arcane Surge for {dmg} damage.");
    return dmg;
}

private int ManaShield()
{
    if (Mana < 18) { Console.WriteLine($"{Name} lacks Mana!"); return 0; }
    Mana -= 18;
    if (SharedRng.Next(1,101) <= 5) { Console.WriteLine($"{Name}'s ward falters!"); return 0; }

    int baseDmg = (int)(Intelligence * 0.6 + Math.Sqrt(Level) * 4 + SharedRng.Next(6, 12));
    int dmg = baseDmg;
    Console.WriteLine($"{Name} conjures a mana shield and deals {dmg} shock damage.");
    LastSpecialOutcome = new SpecialOutcome { Effect="shield", Turns=0, Potency=Level * 7, SelfTarget=true };
    return dmg;
}


    public override string ToString()
        => $"[Mage] {Name} | Lv {Level} | HP {Health}/{MaxHealth} | INT {Intelligence} | MANA {Mana} | Potions {HealingPotions}";

    
}