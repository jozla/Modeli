using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class StaticVarCompensator : RegulatingCondEq
    {
        public StaticVarCompensator(long globalId) : base(globalId)
        {
        }
    }
}
