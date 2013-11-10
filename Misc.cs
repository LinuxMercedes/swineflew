using System;

public class Misc
{
    public static int ManhattanDistance(Mappable u1, Mappable u2)
    {
        return Math.Abs(u2.X - u1.X) + Math.Abs(u2.Y - u1.Y);
    }

    public static int ManhattanDistance(int s1, int s2)
    {
      return Math.Abs(BitBoard.GetX(s2) - BitBoard.GetX(s1)) + Math.Abs(BitBoard.GetY(s2) - BitBoard.GetY(s1));
    }
}
