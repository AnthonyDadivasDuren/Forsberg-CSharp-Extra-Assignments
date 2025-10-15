namespace CharacterCustomization;

public struct SpecialOutcome
{
    public string Effect;     // "stun", "burn", "bleed", "shield", "defend"
    public int    Turns;      // duration for DoTs or stun
    public int    Potency;    // how strong (e.g. bleed/burn per turn or shield value)
    public bool   SelfTarget; // true = apply to attacker, false = apply to defender
}