using System;

namespace DCPU16.Assembler
{
    public interface IInstructionResolver
    {
        dynamic Resolve(string statement);
    }
}