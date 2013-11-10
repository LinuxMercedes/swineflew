using System;
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

  // team specific occupancy bitboards
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

  // initializes and populates the bitboard objects
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
    dirtTiles  = new BitArray(length, false);
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
    myOccupiedTiles = new BitArray(length, false);
    myOccupiedTiles.Or(myWorkers);
    myOccupiedTiles.Or(myScouts);
    myOccupiedTiles.Or(myTanks);
    oppOccupiedTiles = new BitArray(length, false);
    oppOccupiedTiles.Or(oppWorkers);
    oppOccupiedTiles.Or(oppScouts);
    oppOccupiedTiles.Or(oppTanks);
    myNonMotionTiles = new BitArray(length, false);
    myNonMotionTiles.Or(myOccupiedTiles);
    myNonMotionTiles.Or(oppOccupiedTiles);
    myNonMotionTiles.Or(mySpawningSquares);
    myNonMotionTiles.Or(oppSpawnBases);
    myNonMotionTiles.Or(iceCaps);
    myMotionTiles = new BitArray(myNonMotionTiles);
    myMotionTiles.Xor(full);
    oppNonMotionTiles = new BitArray(length, false);
    oppNonMotionTiles.Or(oppOccupiedTiles);
    oppNonMotionTiles.Or(myOccupiedTiles);
    oppNonMotionTiles.Or(oppSpawningSquares);
    oppNonMotionTiles.Or(mySpawnBases);
    oppNonMotionTiles.Or(iceCaps);
    oppMotionTiles = new BitArray(oppNonMotionTiles);
    oppMotionTiles.Xor(full);
  }

  // clears the data in the non-constant bitboard objects
  public static void Reset()
  {
    myPumpStations.SetAll(false);
    mySpawningSquares.SetAll(false);
    myWorkers.SetAll(false);
    myScouts.SetAll(false);
    myTanks.SetAll(false);
    oppPumpStations.SetAll(false);
    oppSpawningSquares.SetAll(false);
    oppWorkers.SetAll(false);
    oppScouts.SetAll(false);
    oppTanks.SetAll(false);
    waterTiles.SetAll(false);
    trenchTiles.SetAll(false);
    dirtTiles.SetAll(false);
    iceCaps.SetAll(false);
    myOccupiedTiles.SetAll(false);
    oppOccupiedTiles.SetAll(false);
    myNonMotionTiles.SetAll(false);
    myMotionTiles.SetAll(false);
    oppNonMotionTiles.SetAll(false);
    oppMotionTiles.SetAll(false);
  }

  // populates the data in the bitboard objects for the current game state
  public static void PopulateBitBoards()
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

    // populate generic bitboards
    myOccupiedTiles = new BitArray(length, false);
    myOccupiedTiles.Or(myWorkers);
    myOccupiedTiles.Or(myScouts);
    myOccupiedTiles.Or(myTanks);
    oppOccupiedTiles = new BitArray(length, false);
    oppOccupiedTiles.Or(oppWorkers);
    oppOccupiedTiles.Or(oppScouts);
    oppOccupiedTiles.Or(oppTanks);
    myNonMotionTiles = new BitArray(length, false);
    myNonMotionTiles.Or(myOccupiedTiles);
    myNonMotionTiles.Or(oppOccupiedTiles);
    myNonMotionTiles.Or(mySpawningSquares);
    myNonMotionTiles.Or(oppSpawnBases);
    myNonMotionTiles.Or(iceCaps);
    myMotionTiles = new BitArray(myNonMotionTiles);
    myMotionTiles.Xor(full);
    oppNonMotionTiles = new BitArray(length, false);
    oppNonMotionTiles.Or(oppOccupiedTiles);
    oppNonMotionTiles.Or(myOccupiedTiles);
    oppNonMotionTiles.Or(oppSpawningSquares);
    oppNonMotionTiles.Or(mySpawnBases);
    oppNonMotionTiles.Or(iceCaps);
    oppMotionTiles = new BitArray(oppNonMotionTiles);
    oppMotionTiles.Xor(full);

    // debug
    Console.WriteLine("Populate Complete.");
  }

  // updates the bitboards
  public static void Update()
  {
    Reset();
    PopulateBitBoards();
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