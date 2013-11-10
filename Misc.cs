using System;

public class Misc
{
    public static int ManhattanDistance(Unit u1, Unit u2)
    {
        return Math.Abs(u2.X - u1.X) + Math.Abs(u2.Y - u1.Y);
    }
}
