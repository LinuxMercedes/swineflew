using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CSClient
{
    class Mission
    {
        public enum missionTypes { goTo, attackAdjacent };
        public Unit agent;
        public Func<BitArray> target;
        public missionTypes missionType;
        public bool walkThroughWater;

        public Mission(Unit agent, Func<BitArray> target, missionTypes missionType, bool walkThroughWater = false)
        {
            this.agent = agent;
            this.target = target;
            this.missionType = missionType;
            this.walkThroughWater = walkThroughWater;
        }
    }
}
