using System;
using System.Globalization;

namespace DCPU16.Assembler
{
    public class InstructionResolver : IInstructionResolver
    {
        private IValueMap valueMap = new ValueMap();       

        public Tuple<ushort, ushort> ResolveRegisterPointer(string hex, ushort reg)
        {
            ushort first = (ushort)(reg + 0x10);
            ushort second = UInt16.Parse(hex.Substring(2), NumberStyles.HexNumber);
            return new Tuple<ushort, ushort>(first, second);
        }

        public Tuple<ushort, ushort> ResolveNextWord(string hex)
        {
            ushort first = (ushort)(0x1e);
            ushort second = UInt16.Parse(hex.Substring(2), NumberStyles.HexNumber);
            return new Tuple<ushort, ushort>(first, second);
        }

        public dynamic ResolveHex(string hex)
        {
            ushort first = (ushort)(0x1f);
            ushort second = UInt16.Parse(hex.Substring(2), NumberStyles.HexNumber);

            if (second < 0x20)
                return (ushort) (second + 0x20);
            return new Tuple<ushort, ushort>(first, second);
        }

        public ushort ResolveLiteralValue(string digit)
        {
            return (ushort)( UInt16.Parse(digit) + 0x20);
        }

        public Tuple<ushort, string> ResolveLabelAndRegister(string label, string reg)
        {
            ushort value = (ushort)(this.valueMap[reg] + 0x10);
            return new Tuple<ushort, string>(value, label);
        }
    }
}