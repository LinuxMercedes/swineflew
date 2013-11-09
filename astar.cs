using System;
using System.Collections.Generic;

class AStar
{
	// HEY THIS IS THE ROUTING FUNCTION
	static public List<Node> route(uint x_start, uint y_start, BitBoard b, bool avoidWater = true)
	{
		List<Node> open = new List<Node>();
		HashSet<Node> closed = new HashSet<Node>();
		
		Node start = Node(x_start, y_start);
		start.g = 0;
		start.h = heuristic(start, Bb);
		start.f = g + h;
		open.Add(start);

		while(open.Count > 0 && !isDestination(open[0], b))
		{
			Node here = open[0];
			open.RemoveAt(0);
			closed.Add(here);

			List<Node> moves = genMoves(here, b, avoidWater);

			foreach(Node move in moves)
			{
				if(!closed.Contains(move))
				{
					open.Add(move);
				}
			}

			open.Sort();
		}
		
		// Path found, or none exists
		List<Node> path = new List<Node>();
		if(open.Count > 0)
		{
			path.Add(open[0]);
			while(path[-1].p != null)
			{
				path.Add(path[-1].p);
			}
		}
		return path.Reverse();
	}
	
	public static bool isDestination(Node n, BitArray b, BitBoard bb)
	{
		return getBb(n.x, n.y, b, bb);
	}

	public static uint heuristic(Node n, BitArray b, BitBoard bb)
	{
		uint h = bb.width + bb.height + 1; //Max distance
		for(uint x = 0; x < bb.width; x++)
		{
			for(uint y = 0; y < bb.height; y++)
			{
				if(getBb(x, y, b, bb))
				{
					uint d = Abs(x - n.x) + Abs(y - n.y);
					if(d < h) h = d;
				}
			}
		}
		return h;
	}

	public static List<Node> genMoves(Node n, BitBoard bb, bool avoidWater)
	{
		List<Node> moves = new List<Node>();
		List<Tuple<uint, uint>> vals = new List<Tuple<uint, uint>>();
		vals.Add(Tuple.Create(0, -1));
		vals.Add(Tuple.Create(0, 1));
		vals.Add(Tuple.Create(-1, 0));
		vals.Add(Tuple.Create(1, 0));

		foreach(Tuple<uint, uint> val in vals)
		{
			uint x = val.Item1 + n.x;
			uint y = val.Item2 + n.y;
	
			// Validate coords
			if (x > b.width) continue;
			if (y > b.height) continue;
			if (x < 0) continue;
			if (y < 0) continue;

			// See if we can move to that location
			// other units
			if (getBb(x, y, b.myWorkers.And(b.myScouts).And(b.myTanks), b)) continue;
			if (getBb(x, y, b.oppWorkers.And(b.oppScouts).And(b.oppTanks), b)) continue;

			// ice caps
			if (getBb(x, y, b.iceCaps, b)) continue;

			// enemy spawn bases
			if (getBb(x, y, b.oppSpawnBases, b)) continue;

			// spawning tiles
			// TODO
			if (getBb(x, y, b.FUCK, b)) continue;

			// water, if so chosen
			if (avoidWater && getBb(x, y, b.waterTiles, b)) continue;

			// Woop, we have a valid move
			Node move = Node(x, y, n);
			move.h = heuristic(n, b);
			move.f = move.h + move.g;
			moves.Add(move);
		}

		return moves;
	}

	public static bool getBb(uint x, uint y, BitArray b, BitBoard bb)
	{
		return bb.getVal(x, y, b);
	}
}
class Node : IEquatable<Node>, IComparable<Node>
{
	public uint f{get;set;}
	public uint g{get;set;}
	public uint h{get;set;}

	public Node parent{get;set;}

	public uint x{get;set;}
	public uint y{get;set;}

	public Node(uint xv, uint yv, Node p = null)
	{
		x = xv;
		y = yv;
		parent = p;
		if(p != null)
		{
			g = p.g + 1;
		}
	}

	public override int CompareTo(Node o)
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
		return 1000 * x + y;
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
