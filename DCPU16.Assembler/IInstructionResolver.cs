using System;

namespace DCPU16.Assembler
{
    public interface IInstructionResolver
    {
        Tuple<ushort, ushort> Resolve(string statement);
    }
}