﻿using SC2APIProtocol;
using Sharky.DefaultBot;
using System.Collections.Generic;

namespace Sharky.Managers
{
    public class EnemyStrategyManager : SharkyManager
    {
        EnemyData EnemyData;
        EnemyAggressivityService EnemyAggressivityService;
        DebugService DebugService;

        public EnemyStrategyManager(DefaultSharkyBot defaultSharkyBot)
        {
            EnemyData = defaultSharkyBot.EnemyData;
            EnemyAggressivityService = defaultSharkyBot.EnemyAggressivityService;
            DebugService = defaultSharkyBot.DebugService;
        }

        public override IEnumerable<Action> OnFrame(ResponseObservation observation)
        {
            var frame = (int)observation.Observation.GameLoop;

            foreach (var enemyStrategy in EnemyData.EnemyStrategies.Values)
            {
                enemyStrategy.OnFrame(frame);
            }

            EnemyAggressivityService.Update(frame);

            DebugService.DrawText($"Enemy aggression {EnemyData.EnemyAggressivityData.ArmyAggressivity}");

            return new List<Action>();
        }
    }
}
