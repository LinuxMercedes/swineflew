using System;
using System.Collections;
using System.Collections.Generic;

class AStar
{
	static public List<Node> route(int x_start, int y_start, BitArray b, bool avoidWater = true)
	{
		List<Node> open = new List<Node>();
		HashSet<Node> closed = new HashSet<Node>();
		
		System.Console.WriteLine("Starting from " + x_start + " " + y_start);
		Node start = new Node(x_start, y_start);
		start.g = 0;
		start.h = heuristic(start, b);
		start.f = start.g + start.h;
		open.Add(start);

		long iters = 0;
		while(open.Count > 0 && !isDestination(open[0], b) && iters < 20000)
		{
			Node here = open[0];
			open.RemoveAt(0);
			closed.Add(here);

			System.Console.WriteLine("Here: " + here.x + " " + here.y + " f: " + here.f);

			List<Node> moves = genMoves(here, b, avoidWater);

			foreach(Node move in moves)
			{
				if(!closed.Contains(move))
				{
					open.Add(move);
				}
			}

			open.Sort();
			iters++;
		}
		
		// Path found, or none exists
		List<Node> path = new List<Node>();
		if(open.Count > 0)
		{
			path.Add(open[0]);
			while(path[path.Count - 1].parent != null)
			{
				path.Add(path[path.Count - 1].parent);
			}
		}
		path.Reverse();

		return path;
	}
	
	public static bool isDestination(Node n, BitArray b)
	{
		return getBb(n.x, n.y, b);
	}

	public static int heuristic(Node n, BitArray b)
	{
		int h = BitBoard.width + BitBoard.height + 1; //Max distance
		for(int x = 0; x < BitBoard.width; x++)
		{
			for(int y = 0; y < BitBoard.height; y++)
			{
				if(getBb(x, y, b))
				{
					int d = Math.Abs(x - n.x) + Math.Abs(y - n.y);
					if(d < h) h = d;
				}
			}
		}
		System.Console.WriteLine("Heuristic for " + n.x + " " + n.y + ": " + h);
		return h;
	}

	public static List<Node> genMoves(Node n, BitArray b, bool avoidWater)
	{
		List<Node> moves = new List<Node>();
		List<Tuple<int, int>> vals = new List<Tuple<int, int>>();
		vals.Add(Tuple.Create(0, -1));
		vals.Add(Tuple.Create(0, 1));
		vals.Add(Tuple.Create(-1, 0));
		vals.Add(Tuple.Create(1, 0));

		foreach(Tuple<int, int> val in vals)
		{
			int x = val.Item1 + n.x;
			int y = val.Item2 + n.y;
	
			// Validate coords
			if (x > BitBoard.width) continue;
			if (y > BitBoard.height) continue;
			if (x < 0) continue;
			if (y < 0) continue;

			// See if we can move to that location
			if (getBb(x, y, BitBoard.iceCaps
						.Or(BitBoard.myWorkers)
						.Or(BitBoard.myScouts)
						.Or(BitBoard.myTanks)
						.Or(BitBoard.mySpawningSquares)
						.Or(BitBoard.oppSpawnBases)
						.Or(BitBoard.oppWorkers)
						.Or(BitBoard.oppScouts)
						.Or(BitBoard.oppTanks)
						)
				 )
			{
				System.Console.WriteLine("Cannot move to " + x + " " + y);
				System.Console.WriteLine(getBb(x, y, BitBoard.iceCaps));
				System.Console.WriteLine(getBb(x, y, BitBoard.myWorkers));
				System.Console.WriteLine(getBb(x, y, BitBoard.myScouts));
				System.Console.WriteLine(getBb(x, y, BitBoard.myTanks));
				System.Console.WriteLine(getBb(x, y, BitBoard.mySpawningSquares));
				System.Console.WriteLine(getBb(x, y, BitBoard.oppSpawnBases));
				System.Console.WriteLine(getBb(x, y, BitBoard.oppWorkers));
				System.Console.WriteLine(getBb(x, y, BitBoard.oppScouts));
				System.Console.WriteLine(getBb(x, y, BitBoard.oppTanks));
				continue;
			}

			// Avoid water, if so chosen
			if (avoidWater && getBb(x, y, BitBoard.waterTiles)) continue;

			// Woop, we have a valid move
			Node move = new Node(x, y, n);
			move.h = heuristic(n, b);
			move.f = move.h + move.g;
			System.Console.WriteLine("Adding " + x + " " + y + " f: " + move.f);
			moves.Add(move);
		}

		return moves;
	}

	public static bool getBb(int x, int y, BitArray b)
	{
		return BitBoard.GetBit(b, x, y);
	}
}

class Node : IEquatable<Node>, IComparable<Node>
{
	public int f{get;set;}
	public int g{get;set;}
	public int h{get;set;}

	public Node parent{get;set;}

	public int x{get;set;}
	public int y{get;set;}

	public Node(int xv, int yv, Node p = null)
	{
		x = xv;
		y = yv;
		parent = p;
		if(p != null)
		{
			g = p.g + 1;
		}
	}

	int IComparable<Node>.CompareTo(Node o)
	{
		if(f == o.f) return 0;
		if(f < o.f) return -1;
		return 1;
	}

	public override bool Equals( Object obj )
	{
		var other = obj as Node;
		if( other == null ) return false;

		return Equals (other);
	}

	public override int GetHashCode()
	{
		return 1000 * (int) x + (int) y;
	}

	public bool Equals( Node other )
	{
		if( other == null )
		{
			return false;
		}

		if( ReferenceEquals (this, other) )
		{
			return true;
		}

		return x == other.x && y == other.y;
	}
}
