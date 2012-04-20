using System;

namespace DCPU16.VM
{
    public interface ISourceProvider
    {
        Func<ushort> GetSource(byte value, int offset = 0);
    }
}