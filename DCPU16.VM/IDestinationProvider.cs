using System;

namespace DCPU16.VM
{
    public interface IDestinationProvider
    {
        Action<ushort> GetDestination(byte value, Instruction instruction);
    }
}