using GenderDiversity.Configuration;
using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GenderDiversity.Patches
{
    [HarmonyPatch]
    internal static class SpawnPatch
    {
        private static readonly Random _random = new Random();
        private static bool _overrideFemale = false;

        // List of troop IDs that should remain male-only for lore reasons
        private static readonly string[] MaleOnlyTroops = new[]
        {
            // Skolderbroda (Sturgian elite infantry - male warrior brotherhood)
            "sturgian_warrior_son",
            "sturgian_heroic_line_breaker",
            // Ghilman (Aserai slave soldiers - historically male)
            "aserai_mameluke_soldier",
            "aserai_mameluke_heavy_cavalry",
            "aserai_mameluke_guard",
            "aserai_vanguard_faris",
            // Add Nord equivalents if War Sails has male-only units
            "nord_huscarl",
        };

        private static bool IsMaleOnlyTroop(BasicCharacterObject character)
        {
            if (character?.StringId == null) return false;
            
            string id = character.StringId.ToLowerInvariant();
            foreach (var maleOnly in MaleOnlyTroops)
            {
                if (id.Contains(maleOnly))
                    return true;
            }
            return false;
        }

        private static bool ShouldBeFemale(BasicCharacterObject character, int seed)
        {
            var settings = Settings.Instance;
            if (settings == null || !settings.Enabled)
                return false;

            // Already female? Don't override
            if (character.IsFemale)
                return false;

            // Heroes keep their defined gender
            if (character.IsHero)
                return false;

            // Check lore exceptions
            if (settings.LoreFriendly && IsMaleOnlyTroop(character))
                return false;

            // Use seed for consistent gender per troop instance
            var rand = seed != 0 ? new Random(seed) : _random;
            return rand.Next(100) < settings.FemalePercentage;
        }

        // Patch Mission.SpawnAgent to enable gender override during spawn
        [HarmonyPatch(typeof(Mission), nameof(Mission.SpawnAgent))]
        private static class MissionSpawnAgentPatch
        {
            static void Prefix(AgentBuildData agentBuildData)
            {
                if (agentBuildData?.AgentCharacter == null)
                    return;

                int seed = agentBuildData.AgentOrigin?.Seed ?? 0;
                _overrideFemale = ShouldBeFemale(agentBuildData.AgentCharacter, seed);
            }

            static void Postfix()
            {
                _overrideFemale = false;
            }
        }

        // Patch IsFemale getter to return true when override is active
        [HarmonyPatch(typeof(BasicCharacterObject), nameof(BasicCharacterObject.IsFemale), MethodType.Getter)]
        private static class IsFemaleGetterPatch
        {
            static void Postfix(ref bool __result)
            {
                if (_overrideFemale)
                    __result = true;
            }
        }
    }
}
