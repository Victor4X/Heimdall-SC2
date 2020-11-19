﻿using Sharky.Managers;
using System;

namespace Sharky.EnemyStrategies
{
    public abstract class EnemyStrategy : IEnemyStrategy
    {
        public bool Active { get; private set; }
        public bool Detected { get; private set; }

        protected IChatManager ChatManager;
        protected EnemyStrategyHistory EnemyStrategyHistory;
        protected IUnitManager UnitManager;
        protected SharkyOptions SharkyOptions;

        public string Name()
        {
            return GetType().Name;
        }

        public void OnFrame(int frame)
        {
            var detected = Detect(frame);

            if (detected)
            {
                Active = true;

                if (!Detected)
                {
                    EnemyStrategyHistory.History[frame] = Name();
                    Console.WriteLine($"Detected: {Name()}");
                    Detected = true;
                    DetectedChat();
                }
            }
        }

        protected void DetectedChat()
        {
            ChatManager.SendChatType($"{Name()}-EnemyStrategy");
        }

        protected abstract bool Detect(int frame);
    }
}
