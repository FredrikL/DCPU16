using System;
using System.Collections.Generic;

namespace DCPU16.Assembler
{  
    public class InstructionBuilder : IInstructionBuilder
    {
        private Dictionary<OpCode, ushort> opCodes = new Dictionary<OpCode, ushort>();

        public InstructionBuilder()
        {
            opCodes.Add(OpCode.SET, 0x1);
            opCodes.Add(OpCode.ADD, 0x2);
            opCodes.Add(OpCode.SUB, 0x3);
            opCodes.Add(OpCode.MUL, 0x4);
            opCodes.Add(OpCode.DIV, 0x5);
            opCodes.Add(OpCode.MOD, 0x6);
            opCodes.Add(OpCode.SHL, 0x7);
            opCodes.Add(OpCode.SHR, 0x8);
            opCodes.Add(OpCode.AND, 0x9);
            opCodes.Add(OpCode.BOR, 0xa);
            opCodes.Add(OpCode.XOR, 0xb);
            opCodes.Add(OpCode.IFE, 0xc);
            opCodes.Add(OpCode.IFN, 0xd);
            opCodes.Add(OpCode.IFG, 0xe);
            opCodes.Add(OpCode.IFB, 0xf);
        }

        private static void DoOffset(dynamic instruction, List<ushort> ret, bool second = false)
        {
            if (instruction.values[second ? 1 : 0] is Tuple<ushort, ushort>)
            {
                var instone = instruction.values[second ? 1 : 0] as Tuple<ushort, ushort>;
                ret[0] = (ushort)(ret[0] + (instone.Item1 << (second ? 10 : 4)));
                ret.Add(instone.Item2);
            }
            else if(instruction.values[second ? 1 : 0] is string)
            {
                ret[0] = (ushort)(ret[0] + (0x1f << (second ? 10 : 4)));
                ret.Add(0x0); // placeholder
            }
            else
                ret[0] = (ushort)(ret[0] + (instruction.values[second ? 1 : 0] << (second ? 10 : 4)));
        }

        public ushort[] BuildInstruction(dynamic instruction)
        {
            var ret = new List<ushort>();
            ret.Add(opCodes[instruction.opcode]);
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