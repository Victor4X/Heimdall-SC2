﻿using Sharky.Managers;
using Sharky.Pathing;

namespace Sharky.MicroControllers.Protoss
{
    public class ZealotMicroController : IndividualMicroController
    {
        public ZealotMicroController(MapDataService mapDataService, UnitDataManager unitDataManager, ActiveUnitData activeUnitData, DebugManager debugManager, IPathFinder sharkyPathFinder, BaseData baseData, SharkyOptions sharkyOptions, DamageService damageService, MicroPriority microPriority, bool groupUpEnabled) 
            : base(mapDataService, unitDataManager, activeUnitData, debugManager, sharkyPathFinder, baseData, sharkyOptions, damageService, microPriority, groupUpEnabled)
        {

        }
    }
}
