using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class RotatingMachine : RegulatingCondEq
    {
        public RotatingMachine(long globalId) : base(globalId)
        {
        }
    }
}
