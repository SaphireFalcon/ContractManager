using HarmonyLib;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Patches
{
    [HarmonyPatch]
    internal static class UncompressedSavePatchLoad
    {
        private static Harmony? _harmony = new Harmony("ContractManager");

        public static void Patch()
        {
            Console.WriteLine("[CM] Patching UncompressedSave...");
            _harmony?.PatchAll(typeof(UncompressedSavePatchLoad).Assembly);
        }

        public static void Unload()
        {
            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;
        }

        [HarmonyPatch(typeof(KSA.UncompressedSave), nameof(KSA.UncompressedSave.Load))]
        [HarmonyPostfix]
        public static void LoadPostfix(ref KSA.UncompressedSave __instance)
        {
            Console.WriteLine($"[CM] UncompressedSavePatchLoad.LoadPostfix() loading data from '{__instance.Directory.FullName}'");
            ContractManager.data.LoadFrom(__instance.Directory.FullName);
        }
    }
}
