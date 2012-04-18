using System;
using System.Collections.Generic;

namespace DCPU16.Assembler
{
    public class InstructionBuilder : IInstructionBuilder
    {
      
        private static void DoOffset(dynamic instruction, List<ushort> ret, bool second = false)
        {
            if (instruction.values[second ? 1 : 0] is Tuple<ushort, ushort>)
            {
                var instone = instruction.values[second ? 1 : 0] as Tuple<ushort, ushort>;
                ret[0] = (ushort)(ret[0] + (instone.Item1 << (second ? 10 : 4)));
                ret.Add(instone.Item2);
            }
            else
                ret[0] = (ushort)(ret[0] + (instruction.values[second ? 1 : 0] << (second ? 10 : 4)));
        }

        public ushort[] BuildInstruction(ushort opCode, dynamic instruction)
        {
            var ret = new List<ushort>();
            ret.Add(opCode);
            DoOffset(instruction, ret);
            DoOffset(instruction, ret, true);

            return ret.ToArray();
        }

        public ushort[] BuildExtendedInstruction(ushort opCode, dynamic instruction)
        {
            throw new NotImplementedException();
        }
    }
}