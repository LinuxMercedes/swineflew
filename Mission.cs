using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CSClient
{
    class Mission
    {
        Unit agent;
        BitArray target;
        public Mission(Unit agent)
        {
            this.agent = agent;
        }
    }
}
