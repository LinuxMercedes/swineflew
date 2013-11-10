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
                    case Mission.missionTypes.attackAdjacent:
                        missionAttackAdjacent(m.agent);
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
						if(path.Count == 0)
						{
							System.Console.WriteLine("Wtfwlj;");
						}
						foreach(Node n in path)
						{
							if(u.MovementLeft == 0) break;

							u.move(n.x, n.y);
							System.Console.Write(n.x);
						  System.Console.Write(" ");
							System.Console.WriteLine(n.y);
						}
						BitBoard.Update();
        }

        private static void missionAttackAdjacent(Unit u)
        {
            if (!u.HasAttacked)
                foreach (Unit unit in AI.units)
                {
                    if (u.X == unit.X && (u.Y - 1 == unit.Y || u.Y + 1 == unit.Y))
                    {
                        u.attack(unit);
                        break;
                    }
                    else if (u.Y == unit.Y && (u.X - 1 == unit.X || u.X + 1 == unit.X))
                    {
                        u.attack(unit);
                        break;
                    }
                }
            BitBoard.Update();
        }
    }
}
