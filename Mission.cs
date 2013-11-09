using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CSClient
{
    class Mission
    {
        public enum missionTypes { goTo };
        public Unit agent;
        public Func<BitArray> target;
        public missionTypes missionType;
        public Mission(Unit agent, Func<BitArray> target, missionTypes missionType)
        {
            this.agent = agent;
            this.target = target;
            this.missionType = missionType;
        }
    }
}
