using System;
using System.Object;
using System.Collections.Generic;

class AStar
{
	class Node : IEquatable<Node>, IComparable
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
		}

		public override int CompareTo(Object obj)
		{
			var o = obj as Node;
			if (o == null) return 1;

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

		public override int GetHashcode()
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

	public BaseAI ai {get;set;}

	public AStar(BaseAI bai)
	{
		ai = bai;
	}
		
	// HEY THIS IS THE ROUTING FUNCTION
	static public route(uint x_start, uint y_start, Bb b, bool avoidWater = true)
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

			moves = genMoves(here, b, avoidWater);

			foreach(move in moves)
			{
				if(!closed.Contains(move))
				{
					open.Add(move);
				}
			}

			open.Sort();
		}
		
		public static bool isDestination(Node n, Bb b)
		{
			return b.get(n.x, n.y);
		}

		public static uint heuristic(Node n, Bb b)
		{
		}

		public static List<Node> genMoves(Node n, Bb b, bool avoidWater)
		{
			List vals<Tuple<uint, uint>> = new List<Tuple<uint, uint>>();
			vals.Add(Tuple.Create(0, -1));
			vals.Add(Tuple.Create(0, 1));
			vals.Add(Tuple.Create(-1, 0));
			vals.Add(Tuple.Create(1, 0));

			foreach(val in vals)
			{
			  uint x = val.Item1 + n.x;
				uint y = val.Item2 + n.y;
		
				// Validate coords
				if (x > ai.mapWidth()) continue;
				if (y > ai.mapHeight()) continue;
				if (x < 0) continue;
				if (y < 0) continue;

				// See if we can move to that location
				// other units
				// ice caps
				// enemy spawn bases
				// spawning tiles
				// water, if so chosen

			}
		}
	}

}
	