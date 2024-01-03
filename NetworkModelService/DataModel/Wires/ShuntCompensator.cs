using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class ShuntCompensator : RegulatingCondEq
    {
        public ShuntCompensator(long globalId) : base(globalId)
        {
        }
    }
}
