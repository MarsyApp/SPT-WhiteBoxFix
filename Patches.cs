using EFT.InventoryLogic;
using System.Collections.Generic;
using System.Reflection;
using Aki.Reflection.Patching;
using System;
using System.Linq;

namespace WhiteBoxFix
{
    class Patcher
    {
        public static void PatchAll()
        {
            new PatchManager().RunPatches();
        }
    }

    public class PatchManager
    {
        public PatchManager()
        {
            this._patches = new List<ModulePatch>
            {
                new ItemViewPatches.DraggedItemViewMethodCheckItem()
            };
        }

        public void RunPatches()
        {
            foreach (ModulePatch patch in this._patches)
            {
                patch.Enable();
            }
        }

        private readonly List<ModulePatch> _patches;
    }

    public static class ItemViewPatches
    {
        public class DraggedItemViewMethodCheckItem : ModulePatch
        {
            protected override MethodBase GetTargetMethod()
            {
                return typeof(ItemFilter).GetMethod("CheckItem", BindingFlags.Public | BindingFlags.Static);
            }
            
            [PatchPrefix]
            private static void PatchPrefix(Item item, ref string[] acceptableNodes)
            {
                bool containsNull = Array.Exists(acceptableNodes, s => s == null);

                if (containsNull)
                {
                    Logger.LogError("--------------------------------");
                    Logger.LogError("WhiteBoxFix: Found null in array");
                    Logger.LogError("--------------------------------");
                    Logger.LogError("Start clean up");
                    acceptableNodes = acceptableNodes.Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    Logger.LogError("End clean up");
                    
                    containsNull = Array.Exists(acceptableNodes, s => s == null);
                    
                    if (containsNull)
                    {
                        Logger.LogError("--------------------------------");
                        Logger.LogError("WhiteBoxFix: Found null in array after clean up");
                        Logger.LogError("--------------------------------");
                    }
                    else
                    {
                        Logger.LogError("--------------------------------");
                        Logger.LogError("WhiteBoxFix: No null found in array after clean up");
                        Logger.LogError("--------------------------------");
                    }
                }
            }
        }
    }
}
