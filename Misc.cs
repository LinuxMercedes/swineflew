using System;

public class Misc
{
    public static int ManhattanDistance(Mappable u1, Mappable u2)
    {
        return Math.Abs(u2.X - u1.X) + Math.Abs(u2.Y - u1.Y);
    }
}
