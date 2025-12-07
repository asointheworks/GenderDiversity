using EveryoneFights.Configuration;
using System;
using System.Threading;
using TaleWorlds.Core;

namespace EveryoneFights.Core
{
    public static class GenderOverrideManager
    {
        private static readonly ThreadLocal<bool> _overrideActive = new ThreadLocal<bool>(() => false);
        private static readonly ThreadLocal<bool> _overrideValue = new ThreadLocal<bool>(() => false);

        // Lore-friendly exceptions: troops that should remain single-gender
        // Female-only troops (like Sword Sisters) are already handled - 
        // the game defines them with is_female="true", so ShouldBeFemale returns false for them.
        // 
        // Male-only troops need explicit exclusion:
        private static readonly string[] MaleOnlyTroopIds = new[]
        {
            // Skolderbroda (Nordic mercenary brotherhood based on Jomsvikings - male only by lore)
            // Troop IDs: skolderbrotva_tier_1, skolderbrotva_tier_2, skolderbrotva_tier_3
            "skolderbrotva",
            
            // Ghilman/Mamlukes (Aserai slave-soldiers - historically male only)
            // Troop IDs: aserai_mameluke_soldier, aserai_mameluke_regular, aserai_mameluke_heavy_cavalry, 
            //            aserai_mameluke_guard, aserai_vanguard_faris
            "aserai_mameluke",
            "vanguard_faris",
            
            // Ghulam variants (alternative Ghilman naming)
            "ghulam",
        };

        public static bool IsOverrideActive => _overrideActive.Value;
        public static bool OverrideIsFemale => _overrideValue.Value;

        public static void EnableOverride(BasicCharacterObject? character, int seed = 0)
        {
            _overrideActive.Value = true;
            _overrideValue.Value = ShouldBeFemale(character, seed);
        }

        public static void DisableOverride()
        {
            _overrideActive.Value = false;
            _overrideValue.Value = false;
        }

        public static bool ShouldBeFemale(BasicCharacterObject? character, int seed)
        {
            var settings = Settings.Instance;
            if (settings == null || !settings.Enabled)
                return false;
            if (character == null || character.IsFemale)
                return false;
            if (character.IsHero)
                return false;
            
            // Don't affect civilians (townspeople, villagers, etc.)
            if (IsCivilian(character))
                return false;
            
            if (settings.LoreFriendly && IsMaleOnlyTroop(character))
                return false;

            Random rand = seed != 0 ? new Random(seed) : new Random();
            return rand.Next(100) < settings.FemalePercentage;
        }

        private static bool IsCivilian(BasicCharacterObject character)
        {
            if (character.StringId == null) return false;
            string id = character.StringId.ToLowerInvariant();
            
            // Civilian StringId patterns - these should NOT be affected
            string[] civilianPatterns = new[]
            {
                "townsman",
                "townswoman", 
                "villager",
                "peasant",
                "beggar",
                "merchant",
                "shopworker",
                "barber",
                "blacksmith",
                "tavernkeeper",
                "tavern_wench",
                "musician",
                "dancer",
                "gambler",
                "arena_master",
                "weaponsmith",
                "armorer",
                "horse_merchant",
                "goods_merchant",
                "ransom_broker",
                "notable",
                "gang_leader",
                "artisan",
                "headman",
                "rural_notable",
                "sp_notable",
                "_civilian",
                "_noncombatant",
            };
            
            foreach (var pattern in civilianPatterns)
            {
                if (id.Contains(pattern))
                    return true;
            }
            return false;
        }

        private static bool IsMaleOnlyTroop(BasicCharacterObject character)
        {
            if (character.StringId == null) return false;
            string id = character.StringId.ToLowerInvariant();
            foreach (var maleOnlyId in MaleOnlyTroopIds)
            {
                if (id.Contains(maleOnlyId))
                {
                    EveryoneFights.SubModule.Log($"Lore-friendly exclusion: {character.StringId} matched '{maleOnlyId}'");
                    return true;
                }
            }
            return false;
        }

        public static int GenerateSeed(string? characterId, int contextSeed = 0)
        {
            if (string.IsNullOrEmpty(characterId)) return contextSeed;
            unchecked
            {
                int hash = characterId!.GetHashCode();
                return hash ^ (contextSeed * 397);
            }
        }
    }
}
