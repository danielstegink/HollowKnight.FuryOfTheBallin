using DanielSteginkUtils.Utilities;

namespace FuryOfTheBallin.Helpers
{
    /// <summary>
    /// Handles damage dealt by friendly Marmu
    /// </summary>
    public class BallinDamage : DamageEnemies
    {
        public BallinDamage()
        {
            attackType = AttackTypes.Spell; // Marmu is a literal ghost, so this feels right
            damageDealt = GetBallDamage();
            direction = 2;
            magnitudeMult = 1f;
        }

        private int GetBallDamage()
        {
            // This charm creates an enemy that bounces around colliding w enemies
            // The most comparable ability would be Defender's Crest, which deals
            // 3 dmg every 0.3 seconds to nearby enemies, or 10 dmg per second
            float dmgPerSecond = 3 / 0.3f;

            // Marmu covers a much wider area, so we can justify reducing that bonus to 10%, or
            // 1 dmg per second
            dmgPerSecond /= 10;

            // However, Marmu is bouncing around and will likely miss a given target repeatedly
            // If we assume he hits a random target once per second instead of every 0.3 seconds, then we have
            // 3.33 dmg per attack for 1 notch
            float damage = dmgPerSecond / 0.3f;

            // Fury of the Ballin' costs 1 notch
            // FOTF costs 2 and increases nail damage dealt by 75%, so FOTB should be equivalent to a 37.5% nail dmg boost
            float furyBonusAsNail = 0.375f;

            // 37.5% nail damage is worth 3.75 notchs
            float furyBonusAsNotches = furyBonusAsNail / NotchCosts.NailDamagePerNotch();

            // So if we require the player to be at 1 HP, 1 notch is worth 3.75 notches
            // If we multiply this cost against our damage, Marmu should deal 12.5 damage
            return (int)(damage * furyBonusAsNotches);
        }
    }
}