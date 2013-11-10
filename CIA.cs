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
                        missionGoToAttack(m.agent, target, m.walkThroughWater);
                        missionAttackInRange(m.agent);//todo actually implement goattack
                        break;
                    case Mission.missionTypes.defendAndTrench:
                        missionGoTo(m.agent, target, m.walkThroughWater);
                        missionAttackInRange(m.agent);
//                        missionTrenchAroundTarget(m.agent, target);
//                        missionAttackInRange(m.agent);
                        break;
                    case Mission.missionTypes.defendPumpStation:
                        //todo: implement
                        break;
										case Mission.missionTypes.buildTrench:
												missionBuildTrench(m.agent, target);
												break;
                }
            }
        }


        //goto should go to the nearest non-occupied square
        //from the target bit board
        private static void missionGoTo(Unit u, BitArray b, bool walkThroughWater)
        {
            if (u.MovementLeft == 0 || BitBoard.Equal(b, BitBoard.empty))
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

				private static void missionGoToAttack(Unit u, BitArray b, bool walkThroughWater)
        {
            if (u.MovementLeft == 0 || BitBoard.Equal(b, BitBoard.empty))
            {
                return;
            }

            List<Node> path = AStar.route(u.X, u.Y, b, !walkThroughWater);
            if (path.Count == 0)
            {
							if(BitBoard.GetBit(b, u.X, u.Y))
							{
								BitArray dest = BitBoard.GetPumpStation(b, u.X, u.Y);
								dest.And(BitBoard.GetNonDiagonalAdjacency(BitBoard.oppOccupiedTiles));
								if(!BitBoard.Equal(dest, BitBoard.empty))
								{
									path = AStar.route(u.X, u.Y, dest, !walkThroughWater);
									foreach (Node n in path) 
									{
										if (u.MovementLeft == 0) break;
										u.move(n.x, n.y);
									}
								}
							}
            }
            foreach (Node n in path)
            {
								bool stop = false;
                if (u.MovementLeft == 0) break;

                // Try to move
                // if you fail to move, 
                // curl up in a ball and cry
                if (!u.move(n.x, n.y))
                {
                    System.Console.WriteLine("Could not move from " + u.X + " " + u.Y + " to " + n.x + " " + n.y + "!!!");
                    break;
                }
								//Try to stop once we are close enough to attack
								foreach (Unit unit in AI.units)
                {
                    if (unit.Owner != u.Owner)
                    {
                        if (u.Range >= Misc.ManhattanDistance(u, unit))
                        {
													stop = true;
													break;
												}
                    }
                }

								if(stop) break;
            }
            BitBoard.UpdateUnits();
        }
        private static void missionAttackInRange(Unit u)
        {
            if (!u.HasAttacked)
						{
							  Unit goal = null;
                foreach (Unit unit in AI.units)
                {
                    if (unit.Owner != u.Owner)
                    {
                        if (u.Range >= Misc.ManhattanDistance(u, unit))
                        {
													if(goal == null || goal.HealthLeft > unit.HealthLeft)
													{
														goal = unit;
													}
                        }
                    }
                }
								if( goal != null)
									u.attack(goal);
						}
            BitBoard.UpdateUnits();
        }


        private static void missionTrenchAroundTarget(Unit u, BitArray target)
        {
            BitArray adj = BitBoard.GetAdjacency(target);
            if (!u.HasDug)
            {
                Tile minTile = null;
								List<Node> path = null;
                foreach (Tile tile in AI.tiles)
                {
                    if (BitBoard.GetBit(adj, tile.X, tile.Y) && 3 >= Misc.ManhattanDistance(u, tile))
                    {
											  BitArray pos = BitBoard.position[tile.X][tile.Y];
												BitArray dest = new BitArray(BitBoard.length, false).Or(pos).Or(BitBoard.GetNonDiagonalAdjacency(pos));
												List<Node> p = AStar.route(u.X, u.Y, dest);
												if (p.Count == 0 && !BitBoard.GetBit(dest, u.X, u.Y)) continue;

                        if (minTile == null)
                        {
                            minTile = tile;
														path = p;
                        }

                        if (tile.Depth < minTile.Depth)
                        {
                            minTile = tile;
														path = p;
                        }
                    }
                }

								if (minTile != null)
								{
//									BitArray pos = BitBoard.position[minTile.X][minTile.Y];
//									BitArray dest = new BitArray(BitBoard.length, false).Or(pos).Or(BitBoard.GetNonDiagonalAdjacency(pos));
//									List<Node> path = AStar.route(u.X, u.Y, dest);
									//int x = u.X;
									//int y = u.Y;

									Console.WriteLine(Misc.ManhattanDistance(u, minTile));
									foreach(Node n in path) 
									{
										if(u.MovementLeft == 0) break;
										u.move(n.x, n.y);
									}

									if(1 >= Misc.ManhattanDistance(u, minTile))
									{
										u.dig(minTile);
									}
								}
            }

            BitBoard.UpdateAll(); // May cause water to change
        }

				private static void missionBuildTrench(Unit u, BitArray target)
				{
					/*List<Tile> glaciers = new List<Tile>();
					foreach(Tile t in tiles)
					{
						if (t.WaterAmount > 1)
						{
							glaciers.Add(t);
						}
					}

					List<Node> path = null;
					foreach(Tile g in glaciers)
					{
						List<Node> p = AStar.route(g.X, g.Y, target);
						if(path == null)
						{
							path = p;
						}
						else if(p.Count < path.Count)
						{
							path = p;
						}
					}

					if(path == null) return;

					//have chosen a 


*/
					BitBoard.UpdateAll();
				}
    }
}
