using CharacterCustomization.Characters;

namespace CharacterCustomization;

public interface IAiSpecial
{
    
    // Lets AI call a special with context (defender known)
    int SpecialAttackAi(Character defender);
}