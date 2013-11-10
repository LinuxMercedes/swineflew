using System;
using System.Collections;
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
                BitArray target = m.target();
                switch (m.missionType)
                {
                    case Mission.missionTypes.goTo:
                        missionGoTo(m.agent, target, m.walkThroughWater);
                        break;
                    case Mission.missionTypes.attackInRange:
                        missionAttackInRange(m.agent);
                        break;
                    case Mission.missionTypes.goAttack:
                        missionGoTo(m.agent, target, m.walkThroughWater);
                        missionAttackInRange(m.agent);//todo actually implement goattack
                        break;
                }
            }
        }


        //goto should go to the nearest non-occupied square
        //from the target bit board
        private static void missionGoTo(Unit u, BitArray b, bool walkThroughWater)
        {
            if (u.MovementLeft == 0 || b.Equals(BitBoard.empty))
            {
                return;
            }

            List<Node> path = AStar.route(u.X, u.Y, b, !walkThroughWater);
            if (path.Count == 0)
            {
                System.Console.WriteLine("No Path Found");
            }
            foreach (Node n in path)
            {
                if (u.MovementLeft == 0) break;

                // Try to move
                // if you fail to move, 
                // curl up in a ball and cry
                if (!u.move(n.x, n.y))
                {
                    System.Console.WriteLine("Could not move from " + u.X + " " + u.Y + " to " + n.x + " " + n.y + "!!!");
                    break;
                }
						}
						BitBoard.UpdateUnits();
        }

        private static void missionAttackInRange(Unit u)
        {
            if (!u.HasAttacked)
                foreach (Unit unit in AI.units)
                {
                    if (unit.Owner != u.Owner)
                    {
                        if (u.Range >= Misc.ManhattanDistance(u, unit))
                        {
                            u.attack(unit);
                        }
                    }
                }
            BitBoard.UpdateUnits();
        }
    }
}
