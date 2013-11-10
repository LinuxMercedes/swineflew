using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using CSClient;

/// <summary>
/// The class implementing gameplay logic.
/// </summary>
class AI : BaseAI
{
    // Enum for types of units you can spawn.
    public enum Types { Worker, Scout, Tank };

    public override string username()
    {
        return "SwineFlew";
    }

    public override string password()
    {
        return "password";
    }

    /// <summary>
    /// This function is called each time it is your turn.
    /// </summary>
    /// <returns>True to end your turn. False to ask the server for updated information.</returns>
    public override bool run()
    {
        try
        {
            ourRun();
        }
        catch (Exception ex)
        {
        }
        return true;
    }


    /// <summary>
    /// This function is called once, before your first turn.
    /// </summary>
    public override void init()
    {
        BitBoard.Initialize(this);
    }

    /// <summary>
    /// This function is called once, after your last turn.
    /// </summary>
    public override void end() { }

    public AI(IntPtr c) : base(c) { }

    //our code =================================================

    public static bool phase2;
    public static HashSet<int> defenders = new HashSet<int>();
    public static List<int> xSpawn = new List<int>();
    public static List<int> ySpawn = new List<int>();

    public void ourRun()
    {
        try
        {
            while (xSpawn.Count > 0)
            {
                foreach (Unit u in units)
                {
                    if (u.Owner == playerID())
                        if (u.X == xSpawn[0] && u.Y == ySpawn[0])
                        {
                            defenders.Add(u.Id);
                            break;
                        }
                }
                xSpawn.RemoveAt(0);
                ySpawn.RemoveAt(0);
            }
        }
        catch (Exception ex)
        {

        }

        try
        {
            BitBoard.UpdateAll();
            if (players[playerID()].WaterStored <= players[Math.Abs(playerID() - 1)].WaterStored && BitBoard.Equal(BitBoard.empty, new BitArray(BitBoard.length).Or(BitBoard.myConnectedPumpStations).Or(BitBoard.oppConnectedPumpStations)))
            {
                phase2 = true;
            }
        }
        catch (Exception ex) { }
        try
        {
            betterSpawn();
        }
        catch (Exception ex) { }
        BitBoard.UpdateAll();
        try
        {
            CIA.executeMissions(assignMissions());
        }
        catch (Exception ex) { }
    }

    public void spawnUnits()
    {
        int numberOfUnits = 0;

        // Get the number of units owned.
        for (int i = 0; i < units.Length; i++)
            if (units[i].Owner == playerID())
                numberOfUnits++;
        if (1 == playerID())
            for (int i = 0; i < tiles.Length; i++)
                innerloop(i);
        else
            for (int i = tiles.Length - 1; i >= 0; i--)
                innerloop(i);
    }

    private void innerloop(int i)
    {
        // If this tile is my spawn tile or my pump station...
        if (tiles[i].Owner == playerID())
        {
            int cost = Int32.MaxValue;
            for (int j = 0; j < unitTypes.Length; j++)

                if (turnNumber() <= 1 && unitTypes[j].Type == (int)Types.Worker)
                    cost = unitTypes[j].Cost;
                else if (turnNumber() > 1 && unitTypes[j].Type == (int)Types.Scout)
                    cost = unitTypes[j].Cost;

            // If there is enough oxygen to spawn the unit...
            if (players[playerID()].Oxygen >= cost)
            {
                // ...and nothing is spwning on the tile...
                if (!tiles[i].IsSpawning)
                {
                    bool canSpawn = true;

                    // If it's a pump station and it's not being seiged...
                    if (tiles[i].PumpID != -1)
                    {
                        // ...find the pump in the vector.
                        for (int j = 0; j < pumpStations.Length; j++)
                        {
                            // If it's being sieged, don't spawn.
                            if (pumpStations[j].Id == tiles[i].PumpID && pumpStations[j].SiegeAmount > 0)
                                canSpawn = false;
                        }
                    }

                    // If there is someone else on the tile, don't spawn.
                    for (int j = 0; j < units.Length; j++)
                        if (tiles[i].X == units[j].X && tiles[i].Y == units[j].Y)
                            canSpawn = false;
                    bool defender = false;
                    if (turnNumber() > 1 && BitBoard.GetBit(BitBoard.myConnectedPumpStations, tiles[i].X, tiles[i].Y))
                    {
                        defender = true;
                    }
                    foreach (Unit u in units)
                    {
                        if (defenders.Contains(u.Id)
                            && BitBoard.GetBit(BitBoard.myConnectedPumpStations, tiles[i].X, tiles[i].Y)
                            && BitBoard.GetBit(new BitArray(BitBoard.length, false).Or
                            (BitBoard.myConnectedPumpStations).Or
                            (BitBoard.GetAdjacency(BitBoard.myConnectedPumpStations)), u.X, u.Y)
                           )
                        {
                            canSpawn = false;
                        }
                    }

                    // If possible, spawn!
                    if (canSpawn)
                    {
                        if (defender)
                        {
                            tiles[i].spawn((int)Types.Tank);
                            xSpawn.Add(tiles[i].X);
                            ySpawn.Add(tiles[i].Y);
                        }
                        if (turnNumber() <= 1)
                            tiles[i].spawn((int)Types.Worker);
                        else
                            tiles[i].spawn((int)Types.Scout);
                    }
                }
            }
        }
    }

    public void betterSpawn()
    {
        if (turnNumber() <= 1)
        {
            HashSet<int> pumpid = new HashSet<int>();
            List<int> pumpStationIndexes = BitBoard.GetIndexes(BitBoard.myConnectedPumpStations);

            foreach (Tile t in tiles)
            {
                if (t.Owner != playerID()) continue;
                if (t.PumpID == -1) continue;
                if (pumpid.Contains(t.PumpID)) continue;
                if (t.IsSpawning) continue;
                bool temp = true;
                foreach (int i in pumpStationIndexes)
                {
                    if (BitBoard.GetX(i) == t.X && BitBoard.GetY(i) == t.Y)
                    {
                        temp = false;
                        break;
                    }
                }
                if (temp) continue;
                temp = false;
                foreach (Unit u in units)
                {
                    if (u.X == t.X && u.Y == t.Y)
                    {
                        temp = true;
                        break;
                    }
                }
                if (temp) continue;


                pumpid.Add(t.PumpID);
                t.spawn((int)Types.Tank);
                xSpawn.Add(t.X);
                ySpawn.Add(t.Y);
            }
        }
        spawnUnits();
    }

    public List<Mission> assignMissions()
    {
        List<Mission> offensivemissions = new List<Mission>();
        List<Mission> defensivemissions = new List<Mission>();
        List<Mission> attackmissions = new List<Mission>();
        bool first = true;
        foreach (Unit u in units)
        {
            try
            {
                if (u.Owner == playerID())
                {
                    if (defenders.Contains(u.Id))
                    {
                        defensivemissions.Add(new Mission(u, /*() => BitBoard.GetPumpStation(BitBoard.myConnectedPumpStations, u.X, u.Y)*/
                                                        () => BitBoard.myConnectedPumpStations, Mission.missionTypes.defendAndTrench));
                        //missions.Add(new Mission(u,
                        //    () => BitBoard.GetPumpStation(new BitArray(BitBoard.length, false).Or(BitBoard.myConnectedPumpStations).Or(BitBoard.oppConnectedPumpStations),
                        //    u.X, u.Y), Mission.missionTypes.defendPumpStation));
                    }
                    else if (u.Type == (int)Types.Scout || u.Type == (int)Types.Worker)
                    {
                        if (!BitBoard.Equal(BitBoard.oppConnectedPumpStations, BitBoard.empty))
                        {
                            if (first)
                            {
                                first = false;
                                foreach (Unit e in units)
                                {
                                    if (e.Owner != u.Owner && e.Type == (int)Types.Tank)
                                    {
                                        offensivemissions.Add(new Mission(u, () => BitBoard.position[e.X][e.Y], Mission.missionTypes.goAttack));
                                    }
                                }
                            }
                            offensivemissions.Add(new Mission(u, () => BitBoard.oppConnectedPumpStations, Mission.missionTypes.goAttack));
                            offensivemissions.Add(new Mission(u, () => BitBoard.oppConnectedPumpStations, Mission.missionTypes.goAttack, true));
                        }
                        else
                        {
                            offensivemissions.Add(new Mission(u, () => BitBoard.oppOccupiedTiles, Mission.missionTypes.goAttack));
                        }
                    }
                    attackmissions.Add(new Mission(u, () => BitBoard.empty, Mission.missionTypes.attackInRange));
                }
            }
            catch (Exception ex) { }
        }
        offensivemissions.AddRange(defensivemissions);
        offensivemissions.AddRange(attackmissions);
        return offensivemissions;
    }
}
