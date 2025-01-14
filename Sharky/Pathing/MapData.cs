﻿using Sharky.Builds.BuildingPlacement;
using System.Collections.Generic;

namespace Sharky.Pathing
{
    public class MapData
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public Dictionary<int, Dictionary<int, MapCell>> Map { get; set; }
        public string MapName { get; set; }
        public List<WallData> WallData { get; set; }
    }
}
