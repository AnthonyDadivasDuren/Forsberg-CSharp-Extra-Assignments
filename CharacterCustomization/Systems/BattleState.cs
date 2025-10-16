using CharacterCustomization.Characters;

namespace CharacterCustomization.Systems
{
    public class BattleState
    {
        public Character Player { get; }
        public Character Enemy  { get; }
        
        public int CapSpecialDamage(Character defender, int proposedDamage)
        {
            int cap = (int)(defender.MaxHealth * _diff.SpecialCapPct);
            return proposedDamage > cap ? cap : proposedDamage;
        }
        
        private readonly DifficultyMods _diff;
        
        public string DifficultyName() => _diff.Name;
        public double SpecialCapPercent() => _diff.SpecialCapPct;
        

        // Round counter & log 
        public int RoundNumber { get; private set; } = 1;
        public List<string> RoundLog { get; } = new();

        // Defend, stun
        private readonly HashSet<Character> _defending = new();
        private readonly HashSet<Character> _stunned = new();

        // DoTs and shields
        private readonly Dictionary<Character, (int turns, int dot)> _burns = new();
        private readonly Dictionary<Character, (int turns, int dot)> _bleeds = new();
        private readonly Dictionary<Character, int> _holyShield = new(); // absorbs next damage

        // Cooldowns
        private readonly Dictionary<Character, int> _specialCooldowns = new();

        // Damage multipliers (for arcade shop + difficulty)
        private readonly Dictionary<Character, double> _dmgBuff = new();

        public BattleState(Character player, Character enemy, double playerDmgMult, double enemyDmgMult, DifficultyMods diff)
        {
            Player = player;
            Enemy = enemy;
            _dmgBuff[player] = playerDmgMult <= 0 ? 1.0 : playerDmgMult;
            _dmgBuff[enemy]  = enemyDmgMult  <= 0 ? 1.0 : enemyDmgMult;
            _diff = diff;
        }

        private void NextRound() => RoundNumber++;

        // Apply shields/defend reductions
        public int ApplyHitModifiers(Character attacker, Character defender, int dmg)
        {
            if (dmg <= 0) return 0;

            if (_holyShield.TryGetValue(defender, out int shield) && shield > 0)
            {
                int absorbed = Math.Min(shield, dmg);
                _holyShield[defender] -= absorbed;
                dmg -= absorbed;
                RoundLog.Add($"{defender.Name}'s holy shield absorbs {absorbed} damage");
                if (_holyShield[defender] <= 0) _holyShield.Remove(defender);
            }

            if (_defending.Contains(defender))
            {
                int reduced = (int)Math.Ceiling(dmg * 0.5);
                RoundLog.Add($"{defender.Name} defended: damage reduced from {dmg} to {reduced}");
                dmg = reduced;
            }

            return dmg;
        }

        public int ApplyDamageBuff(Character attacker, int dmg)
        {
            if (dmg <= 0) return 0;
            double mult = _dmgBuff.GetValueOrDefault(attacker, 1.0);
            int boosted = (int)Math.Round(dmg * mult);
            if (mult > 1.0) RoundLog.Add($"{attacker.Name}'s damage buff x{mult:0.00} → {boosted}");
            return boosted;
        }

        // Status API
        public void SetDefend(Character c) => _defending.Add(c);
        private void ClearDefends() => _defending.Clear();
        public bool IsDefending(Character c) => _defending.Contains(c);

        public void ApplyStun(Character c, int turns) => _stunned.Add(c);
        public bool IsStunned(Character c) => _stunned.Contains(c);
        public void ClearStun(Character c) { if (_stunned.Contains(c)) _stunned.Remove(c); }

        // Scaled effects: scale DoT with stats
        public void ApplyBurn(Character c, int turns, int baseDot)
        {
            // burn scales stronger for high INT/FAITH casters
            int intelFaith = 0;
            if (c is Mage m) intelFaith = Math.Max(0, m.Mana / 5); 
            if (c is Paladin p) intelFaith = Math.Max(intelFaith, p.Zeal / 5);
            int dot = baseDot + c.Level * 2 + (int)(intelFaith * 0.2);
            _burns[c] = (turns, dot);
            RoundLog.Add($"{c.Name} is burning for {dot} per turn ({turns} turns)");
        }

        public void ApplyBleed(Character c, int turns, int baseDot)
        {
            int agiFactor = 0;
            if (c is Rogue r) agiFactor = r.Energy / 4;
            int dot = baseDot + c.Level * 2 + (int)(agiFactor * 0.2);
            _bleeds[c] = (turns, dot);
            RoundLog.Add($"{c.Name} is bleeding for {dot} per turn ({turns} turns)");
        }

        public void ApplyHolyShield(Character c, int amount)
        {
            // scale shield a bit with level
            amount += c.Level * 5;
            _holyShield[c] = amount;
            RoundLog.Add($"{c.Name} gains a holy shield for {amount}");
        }
        
        private void Regen(Character c, bool isPlayer)
        {
            // health regen
            int baseHp = 3 + (int)(c.Level * 0.8);
            int hp = (int)(baseHp * (isPlayer ? _diff.PlayerRegenMult : _diff.EnemyRegenMult));
            if (hp > 0)
            {
                int before = c.Health;
                c.Health = Math.Min(c.MaxHealth, c.Health + hp);
                int gained = c.Health - before;
                if (gained > 0) RoundLog.Add($"{c.Name} regenerates {gained} health.");
            }

            // resource regen
            int baseRes = 2 + (int)(c.Level * 0.4);
            int res = (int)(baseRes * (isPlayer ? _diff.PlayerRegenMult : _diff.EnemyRegenMult));

            switch (c)
            {
                case Warrior w:
                    int beforeSta = w.Stamina;
                    w.Stamina = Math.Min(w.Stamina + res, w.MaxStamina);
                    int gSta = w.Stamina - beforeSta;
                    if (gSta > 0) RoundLog.Add($"{c.Name} regenerates {gSta} stamina.");
                    break;
                case Mage m:
                    int beforeMana = m.Mana;
                    m.Mana = Math.Min(m.Mana + res, m.MaxMana);
                    int gMana = m.Mana - beforeMana;
                    if (gMana > 0) RoundLog.Add($"{c.Name} regenerates {gMana} mana.");
                    break;
                case Rogue r:
                    int beforeEnergy = r.Energy;
                    r.Energy = Math.Min(r.Energy + res, r.MaxEnergy);
                    int gEnergy = r.Energy - beforeEnergy;
                    if (gEnergy > 0) RoundLog.Add($"{c.Name} regenerates {gEnergy} energy.");
                    break;
                case Paladin p:
                    int beforeZeal = p.Zeal;
                    p.Zeal = Math.Min(p.Zeal + res, p.MaxZeal);
                    int gZeal = p.Zeal - beforeZeal;
                    if (gZeal > 0) RoundLog.Add($"{c.Name} regenerates {gZeal} zeal.");
                    break;
            }
        }

        public void TickEndOfRound()
        {
            TickBurns();
            TickBleeds();
            ConsumeHolyShields();
            ClearDefends();
            TickCooldowns();
            Regen(Player, true);
            Regen(Enemy, false);
            NextRound();
        }

        private void TickBurns()
        {
            var keys = new List<Character>(_burns.Keys);
            foreach (var k in keys)
            {
                var (t, dot) = _burns[k];
                if (t > 0)
                {
                    k.Health = Math.Max(0, k.Health - dot);
                    RoundLog.Add($"{k.Name} suffers burn damage: {dot} (HP {k.Health}/{k.MaxHealth})");
                    _burns[k] = (t - 1, dot);
                }
                if (_burns.ContainsKey(k) && _burns[k].turns <= 0) _burns.Remove(k);
            }
        }

        private void TickBleeds()
        {
            var keys = new List<Character>(_bleeds.Keys);
            foreach (var k in keys)
            {
                var (t, dot) = _bleeds[k];
                if (t > 0)
                {
                    k.Health = Math.Max(0, k.Health - dot);
                    RoundLog.Add($"{k.Name} takes bleed damage: {dot} (HP {k.Health}/{k.MaxHealth})");
                    _bleeds[k] = (t - 1, dot);
                }
                if (_bleeds.ContainsKey(k) && _bleeds[k].turns <= 0) _bleeds.Remove(k);
            }
        }

        private void ConsumeHolyShields() { /* shield persists until consumed */ }

        // Cooldowns
        public void SetCooldown(Character c, int turns) => _specialCooldowns[c] = turns;
        public bool IsOnCooldown(Character c) => _specialCooldowns.TryGetValue(c, out int t) && t > 0;
        public int GetCooldown(Character c) => _specialCooldowns.GetValueOrDefault(c, 0);

        private void TickCooldowns()
        {
            var keys = new List<Character>(_specialCooldowns.Keys);
            foreach (var c in keys)
            {
                _specialCooldowns[c]--;
                if (_specialCooldowns[c] <= 0) _specialCooldowns.Remove(c);
            }
        }

        public string StatusText(Character c)
        {
            var parts = new List<string>();
            if (_defending.Contains(c)) parts.Add("DEFENDING");
            if (_stunned.Contains(c)) parts.Add("STUNNED");
            if (_burns.TryGetValue(c, out var burn)) parts.Add($"BURN({burn.turns})");
            if (_bleeds.TryGetValue(c, out var bleed)) parts.Add($"BLEED({bleed.turns})");
            if (_holyShield.TryGetValue(c, out var value)) parts.Add($"SHIELD({value})");
            return parts.Count == 0 ? "" : $"[{string.Join(", ", parts)}]";
        }
    }
   
}
