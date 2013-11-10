﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BitBoard
{
  // player ids
  public static int myID;
  public static int oppID;

  // bitboard dimensions
  public static int width;
  public static int height;
  public static int length;

  // constant bitboards
  public static BitArray empty;
  public static BitArray full;
  public static BitArray[][] position;

  // team-specific occupancy bitboards
  public static BitArray myPumpStations;
  public static BitArray mySpawnBases;
  public static BitArray mySpawningSquares;
  public static BitArray myWorkers;
  public static BitArray myScouts;
  public static BitArray myTanks;
  public static BitArray myOccupiedTiles;
  public static BitArray myMotionTiles;
  public static BitArray myNonMotionTiles;
  public static BitArray oppPumpStations;
  public static BitArray oppSpawnBases;
  public static BitArray oppSpawningSquares;
  public static BitArray oppWorkers;
  public static BitArray oppScouts;
  public static BitArray oppTanks;
  public static BitArray oppOccupiedTiles;
  public static BitArray oppMotionTiles;
  public static BitArray oppNonMotionTiles;

  // neutral occupancy bitboards
  public static BitArray waterTiles;
  public static BitArray trenchTiles;
  public static BitArray dirtTiles;
  public static BitArray iceCaps;

  // objective-specific bitboards
  public static BitArray myConnectedPumpStations;
  public static BitArray oppConnectedPumpStations;

  // initializes and populates the bitboards
  public static void Initialize(AI ai)
  {
    // initialize player ids
    foreach (Player player in BaseAI.players)
    {
      if (player.Id == ai.playerID())
      {
        myID = player.Id;
      }
      else
      {
        oppID = player.Id;
      }
    }

    // initialize bitboard dimensions
    width = ai.mapWidth();
    height = ai.mapHeight();
    length = width * height;

    // initialize constant bitboards
    empty = new BitArray(length, false);
    full = new BitArray(length, true);

    position = new BitArray[width][];
    for (int i = 0; i < width; i++)
    {
      position[i] = new BitArray[height];
      for (int j = 0; j < height; j++)
      {
        position[i][j] = new BitArray(length, false);
        SetBit(position[i][j], i, j, true);
      }
    }

    // initialize type-specific bitboards
    myPumpStations = new BitArray(length, false);
    mySpawnBases = new BitArray(length, false);
    mySpawningSquares = new BitArray(length, false);
    myWorkers = new BitArray(length, false);
    myScouts = new BitArray(length, false);
    myTanks = new BitArray(length, false);
    oppPumpStations = new BitArray(length, false);
    oppSpawnBases = new BitArray(length, false);
    oppSpawningSquares = new BitArray(length, false);
    oppWorkers = new BitArray(length, false);
    oppScouts = new BitArray(length, false);
    oppTanks = new BitArray(length, false);
    waterTiles = new BitArray(length, false);
    trenchTiles = new BitArray(length, false);
    dirtTiles = new BitArray(length, false);
    iceCaps = new BitArray(length, false);

    foreach (Tile tile in BaseAI.tiles)
    {
      switch (tile.Owner)
      {
        case 2: // neutral tile
          if (tile.Depth == 0) // dirt tile
          {
            SetBit(dirtTiles, tile.X, tile.Y, true);
          }
          else if (tile.WaterAmount == 0) // trench tile
          {
            SetBit(trenchTiles, tile.X, tile.Y, true);
          }
          else // water tile
          {
            SetBit(waterTiles, tile.X, tile.Y, true);
          }
          break;
        case 3: // ice cap
          SetBit(iceCaps, tile.X, tile.Y, true);
          break;
        default:
          if (tile.Owner == myID) // my tile
          {
            if (tile.PumpID != -1) // my pump station
            {
              SetBit(myPumpStations, tile.X, tile.Y, true);
            }
            else if (tile.IsSpawning) // my spawning square
            {
              SetBit(mySpawningSquares, tile.X, tile.Y, true);
            }
            else // my spawn base
            {
              SetBit(mySpawnBases, tile.X, tile.Y, true);
            }
          }
          else if (tile.Owner == oppID) // opp tile
          {
            if (tile.PumpID != -1) // opp pump station
            {
              SetBit(oppPumpStations, tile.X, tile.Y, true);
            }
            else if (tile.IsSpawning) // opp spawning square
            {
              SetBit(oppSpawningSquares, tile.X, tile.Y, true);
            }
            else // opp spawn base
            {
              SetBit(oppSpawnBases, tile.X, tile.Y, true);
            }
          }
          break;
      }
    }

    foreach (Unit unit in BaseAI.units)
    {
      switch (unit.Type)
      {
        case (int)AI.Types.Worker:
          if (unit.Owner == myID) // my worker
          {
            SetBit(myWorkers, unit.X, unit.Y, true);
          }
          else if (unit.Owner == oppID) // opp worker
          {
            SetBit(oppWorkers, unit.X, unit.Y, true);
          }
          break;
        case (int)AI.Types.Scout:
          if (unit.Owner == myID) // my scout
          {
            SetBit(myScouts, unit.X, unit.Y, true);
          }
          else if (unit.Owner == oppID) // opp scout
          {
            SetBit(oppScouts, unit.X, unit.Y, true);
          }
          break;
        case (int)AI.Types.Tank:
          if (unit.Owner == myID) // my tank
          {
            SetBit(myTanks, unit.X, unit.Y, true);
          }
          else if (unit.Owner == oppID) // opp tank
          {
            SetBit(oppTanks, unit.X, unit.Y, true);
          }
          break;
      }
    }

    // initialize generic bitboards
    myOccupiedTiles = new BitArray(length, false).Or(myWorkers).Or(myScouts).Or(myTanks);
    oppOccupiedTiles = new BitArray(length, false).Or(oppWorkers).Or(oppScouts).Or(oppTanks);
    myNonMotionTiles = new BitArray(length, false).Or(myOccupiedTiles).Or(oppOccupiedTiles).Or(mySpawningSquares).Or(oppSpawnBases).Or(iceCaps);
    myMotionTiles = new BitArray(myNonMotionTiles).Xor(full);
    oppNonMotionTiles = new BitArray(length, false).Or(oppOccupiedTiles).Or(myOccupiedTiles).Or(oppSpawningSquares).Or(mySpawnBases).Or(iceCaps);
    oppMotionTiles = new BitArray(oppNonMotionTiles).Xor(full);

    // initialize special bitboards
    myConnectedPumpStations = GetConnectedPumpStations(myID);
    oppConnectedPumpStations = GetConnectedPumpStations(oppID);
  }

  // clears the data in the non-constant bitboards
  public static void ResetAll()
  {
    ResetUnits();

    myPumpStations.SetAll(false);
    mySpawningSquares.SetAll(false);
    oppPumpStations.SetAll(false);
    oppSpawningSquares.SetAll(false);
    waterTiles.SetAll(false);
    trenchTiles.SetAll(false);
    dirtTiles.SetAll(false);
    iceCaps.SetAll(false);
  }

  // clears the data in the unit bitboards
  public static void ResetUnits()
  {
    myWorkers.SetAll(false);
    myScouts.SetAll(false);
    myTanks.SetAll(false);
    myOccupiedTiles.SetAll(false);    
    oppWorkers.SetAll(false);
    oppScouts.SetAll(false);
    oppTanks.SetAll(false);
    oppOccupiedTiles.SetAll(false);
    myNonMotionTiles.SetAll(false);
    myMotionTiles.SetAll(false);
  }

  // populates the data in all bitboards for the current game state
  public static void PopulateAll()
  {
    // populate type-specific bitboards
    foreach (Tile tile in BaseAI.tiles)
    {
      switch (tile.Owner)
      {
        case 2: // neutral tile
          if (tile.Depth == 0) // dirt tile
          {
            SetBit(dirtTiles, tile.X, tile.Y, true);
          }
          else if (tile.WaterAmount == 0) // trench tile
          {
            SetBit(trenchTiles, tile.X, tile.Y, true);
          }
          else // water tile
          {
            SetBit(waterTiles, tile.X, tile.Y, true);
          }
          break;
        case 3: // ice cap
          SetBit(iceCaps, tile.X, tile.Y, true);
          break;
        default:
          if (tile.Owner == myID) // my tile
          {
            if (tile.PumpID != -1) // my pump station
            {
              SetBit(myPumpStations, tile.X, tile.Y, true);
            }
            else if (tile.IsSpawning) // my spawning square
            {
              SetBit(mySpawningSquares, tile.X, tile.Y, true);
            }
          }
          else if (tile.Owner == oppID) // opp tile
          {
            if (tile.PumpID != -1) // opp pump station
            {
              SetBit(oppPumpStations, tile.X, tile.Y, true);
            }
            else if (tile.IsSpawning) // opp spawning square
            {
              SetBit(oppSpawningSquares, tile.X, tile.Y, true);
            }
          }
          break;
      }
    }

    PopulateUnits();

    // populate special bitboards
    myConnectedPumpStations = GetConnectedPumpStations(myID);
    oppConnectedPumpStations = GetConnectedPumpStations(oppID);
  }

  // populates data in the unit bitboards for the current game state
  public static void PopulateUnits()
  {
    // populate individual unit bitboards
    foreach (Unit unit in BaseAI.units)
    {
      switch (unit.Type)
      {
        case (int)AI.Types.Worker:
          if (unit.Owner == myID) // my worker
          {
            SetBit(myWorkers, unit.X, unit.Y, true);
          }
          else if (unit.Owner == oppID) // opp worker
          {
            SetBit(oppWorkers, unit.X, unit.Y, true);
          }
          break;
        case (int)AI.Types.Scout:
          if (unit.Owner == myID) // my scout
          {
            SetBit(myScouts, unit.X, unit.Y, true);
          }
          else if (unit.Owner == oppID) // opp scout
          {
            SetBit(oppScouts, unit.X, unit.Y, true);
          }
          break;
        case (int)AI.Types.Tank:
          if (unit.Owner == myID) // my tank
          {
            SetBit(myTanks, unit.X, unit.Y, true);
          }
          else if (unit.Owner == oppID) // opp tank
          {
            SetBit(oppTanks, unit.X, unit.Y, true);
          }
          break;
      }
    }

    // populate dependent generic bitboards
    myOccupiedTiles = new BitArray(length, false).Or(myWorkers).Or(myScouts).Or(myTanks);
    oppOccupiedTiles = new BitArray(length, false).Or(oppWorkers).Or(oppScouts).Or(oppTanks);
    myNonMotionTiles = new BitArray(length, false).Or(myOccupiedTiles).Or(oppOccupiedTiles).Or(mySpawningSquares).Or(oppSpawnBases).Or(iceCaps);
    myMotionTiles = new BitArray(myNonMotionTiles).Xor(full);
    oppNonMotionTiles = new BitArray(length, false).Or(oppOccupiedTiles).Or(myOccupiedTiles).Or(oppSpawningSquares).Or(mySpawnBases).Or(iceCaps);
    oppMotionTiles = new BitArray(oppNonMotionTiles).Xor(full);
  }

  // updates all non-constant bitboards
  public static void UpdateAll()
  {
    ResetAll();
    PopulateAll();
  }

  // updates all unit bitboards
  public static void UpdateUnits()
  {
    ResetUnits();
    PopulateUnits();
  }

  // sets the value of a specific bit in a bitboard using (x,y) coordinates
  public static void SetBit(BitArray bitboard, int x, int y, bool value)
  {
    bitboard.Set((x * height) + y, value);
  }

  // gets the value of a specific bit in a bitboard using (x,y) coordinates
  public static bool GetBit(BitArray bitboard, int x, int y)
  {
    return bitboard.Get((x * height) + y);
  }

  // returns a bitboard of a pump station that exists on the specified coordinates,
  // or return empty if no pump station exists on the specified coordinates
  public static BitArray GetPumpStation(BitArray pumpStations, int x, int y)
  {
    // check to see if specified unit is on one of our pump stations
    BitArray test = new BitArray(length, false).Or(pumpStations);
    test.And(position[x][y]);
    if (test.Equals(empty))
    {
      return empty;
    }

    // create bitboard representing pump station the unit is standing on
    BitArray pumpStation = new BitArray(length, false).Or(position[x][y]);
    if (x - 1 >= 0)
    {
      pumpStation.Or(position[x - 1][y]);
      if (y - 1 >= 0)
      {
        pumpStation.Or(position[x - 1][y - 1]);
      }
      if (y + 1 < height)
      {
        pumpStation.Or(position[x - 1][y + 1]);
      }
    }
    if (x + 1 < width)
    {
      pumpStation.Or(position[x + 1][y]);
      if (y - 1 >= 0)
      {
        pumpStation.Or(position[x + 1][y - 1]);
      }
      if (y + 1 < height)
      {
        pumpStation.Or(position[x + 1][y + 1]);
      }
    }
    if (y - 1 >= 0)
    {
      pumpStation.Or(position[x][y - 1]);
    }
    if (y + 1 < height)
    {
      pumpStation.Or(position[x][y + 1]);
    }
    return pumpStation.And(pumpStations);
  }

  // returns a bitboard of all connected pump stations for the specified player
  public static BitArray GetConnectedPumpStations(int player)
  {
    // get bitboard of all pump stations for specified player
    BitArray pumpStations = new BitArray(length, false);
    if (player == myID)
    {
      pumpStations.Or(myPumpStations);
    }
    else if (player == oppID)
    {
      pumpStations.Or(oppPumpStations);
    }
    if (pumpStations.Equals(empty))
    {
      return empty;
    }

    // get connected pump stations
    BitArray connectedPumpStations = new BitArray(length, false);
    List<int> pumpStationIndeces = GetIndeces(pumpStations);
    foreach (int index in pumpStationIndeces)
    {
      // get current pump station
      BitArray currentPumpStation = GetPumpStation(pumpStations, index / height, index % height);
      if (currentPumpStation.Equals(empty))
      {
        continue;
      }

      // get starting points from current pump station's adjacency bitboard
      BitArray currentAdjacency = GetAdjacency(currentPumpStation).And(waterTiles);
      if (currentAdjacency.Equals(empty))
      {
        continue;
      }
      List<int> startingPoints = GetIndeces(currentAdjacency);

      // get path from starting points to nearest connected glacier
      foreach (int start in startingPoints)
      {
        BitArray invalidTiles = new BitArray(length, false).Or(waterTiles).Or(iceCaps).Xor(full);
        List<Node> path = AStar.route(start / height, start % height, iceCaps, false, invalidTiles);

        // if a path exists, add current pump station to connected pump stations bitboard
        if (path.Count != 0)
        {
          connectedPumpStations.Or(currentPumpStation);
        }
      }

      // remove current pump station from pump stations bitboard
      pumpStations.Xor(currentPumpStation);
    }

    return connectedPumpStations;
  }

  // returns the adjacency bitboard for a specified bitboard
  public static BitArray GetAdjacency(BitArray bitboard)
  {
    BitArray adjacency = new BitArray(bitboard.Length, false).Or(bitboard).Or(ShiftLeft(bitboard, 1)).Or(ShiftRight(bitboard, 1));
    return adjacency.Or(ShiftLeft(adjacency, width)).Or(ShiftRight(adjacency, width)).Xor(bitboard);
  }

  // returns the bit array shifted a specified number of bits to the left
  public static BitArray ShiftLeft(BitArray bitboard, int shift)
  {
    BitArray shiftedBitBoard = new BitArray(bitboard.Length, false);
    for (int i = bitboard.Length - 1; (i - shift) >= 0; i--)
    {
      shiftedBitBoard.Set((i - shift), bitboard.Get(i));
    }
    return shiftedBitBoard;
  }

  // returns the bit array shifted a specified number of bits to the right
  public static BitArray ShiftRight(BitArray bitboard, int shift)
  {
    BitArray shiftedBitBoard = new BitArray(bitboard.Length, false);
    for (int i = 0; (i + shift) < bitboard.Length; i++)
    {
      shiftedBitBoard.Set((i + shift), bitboard.Get(i));
    }
    return shiftedBitBoard;
  }

  // gets a list of indeces of the high bits in the specified bitboard
  public static List<int> GetIndeces(BitArray bitboard)
  {
    List<int> indeces = new List<int>(0);
    for (int i = 0; i < bitboard.Length; i++)
    {
      if (bitboard.Get(i))
      {
        indeces.Add(i);
      }
    }
    return indeces;
  }

  // gets the X coordinate from a bitboard index
  public static int GetX(int index)
  {
    return index / height;
  }

  // gets the X coordinate from a bitboard index
  public static int GetY(int index)
  {
    return index % height;
  }

  // prints the coordinates of the high bits in the specified bitboard
  public static void PrintBitBoard(BitArray bitboard)
  {
    for (int i = 0; i < bitboard.Length; i++)
    {
      if (bitboard.Get(i))
      {
        Console.WriteLine((i / height) + " " + (i % height));
      }
    }
  }
}