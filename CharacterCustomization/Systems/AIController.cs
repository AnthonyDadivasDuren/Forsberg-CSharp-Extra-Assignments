using CharacterCustomization.Characters;


namespace CharacterCustomization.Systems
{
    public static class AiController
    {
        public static string DecideEnemyAction(BattleState s, Character me, Character opp)
        {
            double myHpPct = (double)me.Health / me.MaxHealth;
            double oppHpPct = (double)opp.Health / opp.MaxHealth;
            bool specialReady = me is ISpecial && !s.IsOnCooldown(me);
            bool enemyDef = s.IsDefending(opp);

            // Emergency heal
            if (myHpPct <= 0.25 && UiSystem.GetPotions(me) > 0) return "3";

            // Recover if resource is low
            if (UiSystem.GetEnergy(me) < 10 && SharedRng.Next(1, 101) <= 60) return "4";

            // Use special when impactful
            if (specialReady)
            {
                if (oppHpPct > 0.4 && SharedRng.Next(1, 101) <= 60) return "5";
                if (enemyDef && SharedRng.Next(1, 101) <= 80) return "5";
            }

            // If special cooling down and behind on level → defend sometimes
            if (s.IsOnCooldown(me) && opp.Level > me.Level && SharedRng.Next(1, 101) <= 40)
                return "2";

            // If losing on HP → defend more often
            if (myHpPct < oppHpPct && SharedRng.Next(1, 101) <= 35)
                return "2";

            // Default: attack most of time
            return SharedRng.Next(1, 101) <= 80 ? "1" : "2";
        }
    }
}