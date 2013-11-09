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
                if (!target.Equals(BitBoard.empty))
                {
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
        }

        
        //goto should go to the nearest non-occupied square
        //from the target bit board
        private static void missionGoTo(Unit u, BitArray b, bool walkThroughWater)
        {
            if (u.MovementLeft == 0)
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
						List<Tuple<int, int>> vals = new List<Tuple<int, int>>();
						vals.Add(Tuple.Create(0, -1));
						vals.Add(Tuple.Create(0, 1));
						vals.Add(Tuple.Create(-1, 0));
						vals.Add(Tuple.Create(1, 0));

						BitArray oppUnits = BitBoard.oppWorkers.Or(BitBoard.oppScouts).Or(BitBoard.oppTanks);

						foreach(Tuple<int, int> val in vals)
						{
							int x = u.X + val.Item1;
							int y = u.Y + val.Item2;

							if(BitBoard.GetBit(oppUnits, x, y)) 
							{
							}



						}
				}
		}
}
