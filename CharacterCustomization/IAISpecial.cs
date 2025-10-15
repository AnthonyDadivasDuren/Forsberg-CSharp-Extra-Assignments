using CharacterCustomization.Characters;

namespace CharacterCustomization;

public interface IAISpecial
{
    
    // Lets AI call a special with context (defender known)
    int SpecialAttackAI(Character defender);
}