<<<<<<< HEAD
=======
﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CSClient
{
    class Mission
    {
        public static enum missionTypes { goTo };
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
>>>>>>> a131fc6570fdbd14c8c9371db73ab622494b5d20
