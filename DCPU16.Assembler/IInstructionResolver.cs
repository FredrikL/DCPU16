using System;

namespace DCPU16.Assembler
{
    public interface IInstructionResolver
    {
        Tuple<ushort, ushort> ResolveRegisterPointer(string hex, ushort reg);
        Tuple<ushort, ushort> ResolveNextWord(string hex);
        dynamic ResolveHex(string hex);
        ushort ResolveLiteralValue(string digit);
        Tuple<ushort, string> ResolveLabelAndRegister(string label, string reg);
    }
}