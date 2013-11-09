using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BitBoard
{
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
  public static BitArray oppPumpStations;
  public static BitArray oppSpawnBases;
  public static BitArray oppSpawningSquares;
  public static BitArray oppWorkers;
  public static BitArray oppScouts;
  public static BitArray oppTanks;
  public static BitArray oppOccupiedTiles;

  // neutral occupancy bitboards
  public static BitArray waterTiles;
  public static BitArray trenchTiles;
  public static BitArray dirtTiles;
  public static BitArray iceCaps;
  public static BitArray occupiedTiles;
  public static BitArray unoccupiedTiles;

  // initializes and populates the bitboard objects
  public static void Initialize(AI ai)
  {
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
        position[i][j].Set((width * i) + j, true);
      }
    }
    
    // initialize type-specific occupancy bitboards
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
        case 0: // my tile
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
          break;
        case 1: // opp tile
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
          break;
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
      }
    }
  }

  // clears the data in the non-constant bitboard objects
  public static void Reset()
  {
    // do not clear objects in the following bitboards:
    // 1) my/oppPumpStations
    // 2) my/oppSpawnBases
  }

  // updates the data in the bitboard objects for the current game state
  public static void Update()
  {
    // do not update objects in the following bitboards:
    // 1) my/oppPumpStations
    // 2) my/oppSpawnBases
  }

  // sets the value of a specific bit in a bitboard using (x,y) coordinates
  public static void SetBit(BitArray bitboard, int x, int y, bool value)
  {
    bitboard.Set((x * width) + y, value);
  }
}