using HarmonyLib;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContractManager.Patches
{
    [HarmonyPatch]
    internal static class UniverseDataPatchWriteTo
    {
        // Patch a `Postfix` to write contract manager data after `KSA.UniverseData.WriteTo()` is called.
        // Note, can't do prefix or postfix to `KSA.UncompressedSave.Write()` because save directory would be deleted, or `CacheStrings` is already called.
        
        private static Harmony? _harmony = new Harmony("ContractManager");

        public static void Patch()
        {
            Console.WriteLine("[CM] Patching UniverseData...");
            _harmony?.PatchAll(typeof(UniverseDataPatchWriteTo).Assembly);
        }

        public static void Unload()
        {
            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;
        }

        [HarmonyPatch(typeof(KSA.UniverseData), nameof(KSA.UniverseData.WriteTo), typeof(DirectoryInfo))]
        [HarmonyPostfix]
        public static void WriteToPostfix(DirectoryInfo directory)
        {
            Console.WriteLine($"[CM] UniverseDataPatchWriteTo.WriteToPostfix() writing data to '{directory.FullName}'");
            ContractManager.data.WriteTo(directory);
        }
    }
}
