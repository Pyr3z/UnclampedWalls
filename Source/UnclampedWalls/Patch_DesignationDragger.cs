using Harmony;
using Verse;
using Xnope;
using System.Collections.Generic;

namespace UnclampedWalls
{
    [HarmonyPatch(typeof(DesignationDragger), "UpdateDragCellsIfNeeded")]
    public static class Patch_DesignationDragger
    {
        // Prefix patch:
        // diverts original method, copying functionality except for walls
        static bool Prefix(DesignationDragger __instance)
        {
            if (Find.DesignatorManager.SelectedDesignator.DraggableDimensions == 1)
            {
                // get ref to starting cell
                var startCell = (IntVec3)AccessTools.Field(typeof(DesignationDragger), "startDragCell").GetValue(__instance);
                var mouseCell = UI.MouseCell();

                // get ref to private list once
                var dragCells = (List<IntVec3>)AccessTools.Field(typeof(DesignationDragger), "dragCells").GetValue(__instance);
                dragCells.Clear();

                // get ref to failure reason
                var failReason = AccessTools.Field(typeof(DesignationDragger), "failureReasonInt");

                foreach (var cell in startCell.CellsInLineTo(mouseCell))
                {
                    // try to add cells to list
                    var _failReason = TryAddDragCellWithFailReason(cell, dragCells);

                    failReason.SetValue(__instance, _failReason);
                }

                // disable original function
                return false;
            }

            // defaulting to original function
            return true;
        }

        static string TryAddDragCellWithFailReason(IntVec3 cell, List<IntVec3> dragCells)
        {
            var report = Find.DesignatorManager.SelectedDesignator.CanDesignateCell(cell);

            if (report.Accepted)
            {
                dragCells.Add(cell);
                return null;
            }
            else
            {
                return report.Reason;
            }
        }
    }
}
