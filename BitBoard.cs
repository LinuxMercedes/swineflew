using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSClient
{
  class BitBoard
  {
    // length of each BitArray (width * height)
    public static readonly int length;

    // constant bitboards
    public static readonly BitArray empty = new BitArray(length, false);
    public static readonly BitArray full = new BitArray(length, true);

    // occupancy bitboards

  }
}
