using HarmonyLib;
using TaleWorlds.MountAndBlade;

namespace GenderDiversity
{
    public class SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            var harmony = new Harmony("mod.genderdiversity");
            harmony.PatchAll();
        }
    }
}
