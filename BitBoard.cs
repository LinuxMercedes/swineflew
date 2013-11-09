using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BitBoard
{
  // map dimensions
  public static int width;
  public static int height;
  public static int length;

  // constant bitboards
  public static BitArray empty;
  public static BitArray full;

  // team specific occupancy bitboards
  public static BitArray myPumpStations;
  public static BitArray mySpawnBases;
  public static BitArray myWorkers;
  public static BitArray myScouts;
  public static BitArray myTanks;
  public static BitArray myOccupiedTiles;
  public static BitArray oppPumpStations;
  public static BitArray oppSpawnBases;
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
  public static void Initialize()
  {
    // initialize bitboard length
    length = width * height;

    // initialize constant bitboards
    empty = new BitArray(length, false);
    full = new BitArray(length, true);
    
    // initialize my occupancy bitboards


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
}