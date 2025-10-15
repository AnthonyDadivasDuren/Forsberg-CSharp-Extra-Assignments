using CharacterCustomization.Characters;


namespace CharacterCustomization
{
    // Interface for characters that have unique special attacks
    public interface ISpecial
    {
        /// <summary>
        /// Performs the special attack and returns the damage dealt.
        /// Special attacks can also apply unique effects like Stun, Burn, Bleed, etc.
        /// </summary>
        int SpecialAttack(string choice);
    }
}