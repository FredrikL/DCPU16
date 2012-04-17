using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DCPU16.Assembler
{
    public class InstructionResolver : IInstructionResolver
    {
        private IValueMap valueMap = new ValueMap();

        public Tuple<ushort, ushort> Resolve(string statement)
        {
            // item 1 instruction (a|b)
            // item 2 value for next word
            Match match = Regex.Match(statement, @"\[0x(\d{4})\+([ABCXYZIJ])\]");
            if(match.Success)
            {
                ushort first = (ushort)(this.valueMap[match.Groups[2].Value] + 0x10);
                ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                return new Tuple<ushort, ushort>(first,second);
            }

            match = Regex.Match(statement, @"\[0x(\d{4})\]");
            if (match.Success)
            {
                ushort first = (ushort)(0x1e);
                ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                return new Tuple<ushort, ushort>(first, second);
            }

            match = Regex.Match(statement, @"0x(\d{1,4})");
            if (match.Success)
            {
                ushort first = (ushort)(0x1f);
                ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                return new Tuple<ushort, ushort>(first, second);
            }   
            throw new ArgumentException("Que?");
        }
    }
}