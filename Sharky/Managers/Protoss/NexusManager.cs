﻿using SC2APIProtocol;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Sharky.Managers.Protoss
{
    public class NexusManager : SharkyManager
    {
        ActiveUnitData ActiveUnitData;
        SharkyUnitData SharkyUnitData;
        ChronoData ChronoData;
        float OverchargeRangeSquared = 100;
        float RestoreRangeSquared = 36;

        public NexusManager(ActiveUnitData activeUnitData, SharkyUnitData sharkyUnitData, ChronoData chronoData)
        {
            ActiveUnitData = activeUnitData;
            SharkyUnitData = sharkyUnitData;
            ChronoData = chronoData;
        }

        public override IEnumerable<SC2APIProtocol.Action> OnFrame(ResponseObservation observation)
        {
            var actions = new List<SC2APIProtocol.Action>();

            var nexuses = ActiveUnitData.Commanders.Values.Where(c => c.UnitCalculation.Unit.UnitType == (uint)UnitTypes.PROTOSS_NEXUS && c.UnitCalculation.Unit.BuildProgress == 1).OrderByDescending(c => c.UnitCalculation.Unit.Energy);
            foreach (var nexus in nexuses)
            {
                var action = Overcharge(nexus, (int)observation.Observation.GameLoop);
                if (action != null)
                {
                    actions.AddRange(action);
                }
                else
                {
                    action = ChronoBoost(nexus, (int)observation.Observation.GameLoop);
                    if (action != null)
                    {
                        actions.AddRange(action);
                        return actions;
                    }               
                }

                if (nexus.UnitRole != UnitRole.Defend && nexus.UnitCalculation.NearbyAllies.Any(a => a.Unit.UnitType == (uint)UnitTypes.PROTOSS_SHIELDBATTERY && Vector2.DistanceSquared(nexus.UnitCalculation.Position, a.Position) <= 64))
                {
                    nexus.UnitRole = UnitRole.Defend;
                }
            }

            return actions;
        }

        List<SC2APIProtocol.Action> Overcharge(UnitCommander nexus, int frame)
        {
            if (nexus.UnitCalculation.Unit.Energy >= 50)
            {
                foreach (var shieldBattery in nexus.UnitCalculation.NearbyAllies.Where(u => u.Unit.UnitType == (uint)UnitTypes.PROTOSS_SHIELDBATTERY && u.Unit.BuildProgress == 1 && Vector2.DistanceSquared(nexus.UnitCalculation.Position, u.Position) < OverchargeRangeSquared).OrderBy(u => u.Unit.Energy))
                {
                    if (shieldBattery.NearbyAllies.Any(a => a.EnemiesInRangeOf.Count() > 0 && a.Unit.Shield < 5 && Vector2.DistanceSquared(shieldBattery.Position, a.Position) < RestoreRangeSquared))
                    {
                        return nexus.Order(frame, Abilities.BATTERYOVERCHARGE, null, shieldBattery.Unit.Tag);
                    }
                }
            }

            return null;
        }

        List<SC2APIProtocol.Action> ChronoBoost(UnitCommander nexus, int frame)
        {
            if (nexus.UnitRole == UnitRole.Defend && nexus.UnitCalculation.Unit.Energy < 100) { return null; } // save for overcharge or recall

            if (nexus.UnitCalculation.Unit.Energy >= 50)
            {
                foreach (var upgrade in ChronoData.ChronodUpgrades)
                {
                    var upgradeData = SharkyUnitData.UpgradeData[upgrade];
                    var building = ActiveUnitData.SelfUnits.Where(u => u.Value.Unit.IsPowered && !u.Value.Unit.BuffIds.Contains((uint)Buffs.CHRONOBOOST) && upgradeData.ProducingUnits.Contains((UnitTypes)u.Value.Unit.UnitType) && u.Value.Unit.Orders.Any(o => o.AbilityId == (uint)upgradeData.Ability)).FirstOrDefault().Value;
                    if (building != null)
                    {
                        return nexus.Order(frame, Abilities.CHRONOBOOST, null, building.Unit.Tag);
                    }
                }

                foreach (var unit in ChronoData.ChronodUnits)
                {
                    var trainingData = SharkyUnitData.TrainingData[unit];
                    var building = ActiveUnitData.SelfUnits.Where(u => (u.Value.Unit.IsPowered || u.Value.Unit.UnitType == (uint)UnitTypes.PROTOSS_NEXUS) && !u.Value.Unit.BuffIds.Contains((uint)Buffs.CHRONOBOOST) && trainingData.ProducingUnits.Contains((UnitTypes)u.Value.Unit.UnitType) && u.Value.Unit.Orders.Any(o => o.AbilityId == (uint)trainingData.Ability)).FirstOrDefault().Value;
                    if (building != null)
                    {
                        return nexus.Order(frame, Abilities.CHRONOBOOST, null, building.Unit.Tag);
                    }
                }
            }

            return null;
        }
    }
}
