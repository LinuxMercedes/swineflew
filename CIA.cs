using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSClient
{
    class CIA
    {
        public static void executeMissions(List<Mission> missions)
        {
            foreach (Mission m in missions)
            {
                switch (m.missionType)
                {
                    case Mission.missionTypes.goTo:
                        missionGoTo(m);
                        break;
                }
            }
        }

        //nathan implement the following
        private static void missionGoTo(Mission m)
        {

        }
    }
}
