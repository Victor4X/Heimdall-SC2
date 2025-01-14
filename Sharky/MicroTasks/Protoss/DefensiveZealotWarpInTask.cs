﻿using SC2APIProtocol;
using Sharky.Builds.BuildingPlacement;
using Sharky.DefaultBot;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Sharky.MicroTasks
{
    public class DefensiveZealotWarpInTask : MicroTask
    {
        ActiveUnitData ActiveUnitData;
        SharkyOptions SharkyOptions;
        SharkyUnitData SharkyUnitData;
        MacroData MacroData;

        WarpInPlacement WarpInPlacement;

        public DefensiveZealotWarpInTask(DefaultSharkyBot defaultSharkyBot, bool enabled, float priority)
        {
            ActiveUnitData = defaultSharkyBot.ActiveUnitData;
            SharkyOptions = defaultSharkyBot.SharkyOptions;
            SharkyUnitData = defaultSharkyBot.SharkyUnitData;
            MacroData = defaultSharkyBot.MacroData;
            WarpInPlacement = (WarpInPlacement)defaultSharkyBot.WarpInPlacement;

            Priority = priority;

            UnitCommanders = new List<UnitCommander>();
            Enabled = enabled;
        }

        public override void ClaimUnits(ConcurrentDictionary<ulong, UnitCommander> commanders)
        {
        }

        public override IEnumerable<SC2APIProtocol.Action> PerformActions(int frame)
        {
            var commands = new List<SC2APIProtocol.Action>();

            if (MacroData.Minerals < 100 || MacroData.FoodLeft < 2) { return commands; }

            var idleWarpGate = ActiveUnitData.Commanders.Values.FirstOrDefault(c => c.UnitCalculation.Unit.UnitType == (uint)UnitTypes.PROTOSS_WARPGATE && c.WarpInOffCooldown(frame, SharkyOptions.FramesPerSecond, SharkyUnitData));
            if (idleWarpGate == null)
            {
                return commands;
            }

            foreach (var pylon in ActiveUnitData.SelfUnits.Values.Where(u => u.Unit.UnitType == (uint)UnitTypes.PROTOSS_PYLON && u.Unit.BuildProgress >= 1))
            {
                if (pylon.NearbyEnemies.Any(e => e.UnitClassifications.Contains(UnitClassification.ArmyUnit) && !e.Unit.IsFlying))
                {
                    if (pylon.TargetPriorityCalculation.GroundWinnability < 1 || !pylon.NearbyAllies.Any(a => a.UnitClassifications.Contains(UnitClassification.ArmyUnit)))
                    {
                        var location = WarpInPlacement.FindPlacementForPylon(pylon, 1);
                        if (location != null)
                        {
                            var action = idleWarpGate.Order(frame, Abilities.TRAINWARP_ZEALOT, location);
                            if (action != null)
                            {
                                commands.AddRange(action);
                                return commands;
                            }
                        }
                    }
                }
            }

            return commands;
        }
    }
}
