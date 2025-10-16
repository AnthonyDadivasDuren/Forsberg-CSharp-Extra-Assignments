namespace CharacterCustomization
{
    // Interface for characters that have unique special attacks
    public interface ISpecial
    {
        // Performs the special attack and returns the damage dealt.
        // Special attacks can also apply unique effects like Stun, Burn, Bleed, etc.
        int SpecialAttack(string choice);
    }
}