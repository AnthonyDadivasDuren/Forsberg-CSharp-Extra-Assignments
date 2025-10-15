namespace CharacterCustomization.Systems;


    // Multipliers > 1.0 = more; < 1.0 = less
    public sealed class DifficultyMods
    {
        public string Name { get; }
        public double PlayerRegenMult { get; }
        public double EnemyRegenMult  { get; }
        public double EnemyDamageMult { get; }
        public double PotionHealMult  { get; }
        public double SpecialCapPct   { get; } // cap as % of target max HP

        public DifficultyMods(string name, double pRegen, double eRegen, double eDmg, double pot, double capPct)
        {
            Name = name;
            PlayerRegenMult = pRegen;
            EnemyRegenMult  = eRegen;
            EnemyDamageMult = eDmg;
            PotionHealMult  = pot;
            SpecialCapPct   = capPct;
        }

        public static DifficultyMods Easy   => new("Easy",   pRegen:1.4, eRegen:0.8, eDmg:0.90, pot:1.35, capPct:0.35);
        public static DifficultyMods Normal => new("Normal", pRegen:1.0, eRegen:1.0, eDmg:1.00, pot:1.00, capPct:0.38);
        public static DifficultyMods Hard   => new("Hard",   pRegen:0.8, eRegen:1.2, eDmg:1.12, pot:0.85, capPct:0.40);
    }

