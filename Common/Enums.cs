using System;

namespace FTN.Common
{	
	public enum PhaseCode : short
	{
		Unknown = 0x0,
		N = 0x1,
		C = 0x2,
		CN = 0x3,
		B = 0x4,
		BN = 0x5,
		BC = 0x6,
		BCN = 0x7,
		A = 0x8,
		AN = 0x9,
		AC = 0xA,
		ACN = 0xB,
		AB = 0xC,
		ABN = 0xD,
		ABC = 0xE,
		ABCN = 0xF
	}

    public enum RegulatingControlModeKind : short
    {
        Voltage = 1,
		ActivePower = 2,
		ReactivePower = 3,
		CurrentFlow = 4,
		Fixed = 5,
		Admittance = 6,
		TimeScheduled = 7,
		Temperature = 8,
		PowerFactor = 9,
    }
}
