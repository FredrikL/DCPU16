using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DCPU16.Assembler
{
    public class InstructionResolver : IInstructionResolver
    {
        private IValueMap valueMap = new ValueMap();

        public dynamic Resolve(string statement)
        {
            // item 1 instruction (a|b)
            // item 2 value for next word
            Match match;// = Regex.Match(statement, @"\[0x(\d{4})\+([ABCXYZIJ])\]",RegexOptions.Compiled);
            //if(match.Success)
            //{
            //    ushort first = (ushort)(this.valueMap[match.Groups[2].Value] + 0x10);
            //    ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
            //    return new Tuple<ushort, ushort>(first,second);
            //}

            //match = Regex.Match(statement, @"\[0x(\d{4})\]",RegexOptions.Compiled);
            //if (match.Success)
            //{
            //    ushort first = (ushort)(0x1e);
            //    ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
            //    return new Tuple<ushort, ushort>(first, second);
            //}
           
            //match = Regex.Match(statement, @"^0x(\d{1,4})|(\d{1,4})$", RegexOptions.Compiled);
            //if (match.Success)
            //{
            //    ushort first = (ushort)(0x1f);
            //    ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);

            //    // Handle literal values 
            //    if(second <= 0x1f) 
            //        return (ushort) (second + 0x20);

            //    return new Tuple<ushort, ushort>(first, second);
            //}

            match = Regex.Match(statement, @"\[([\w\d_]+)\+([ABCXYZIJ])\]", RegexOptions.Compiled);
            if (match.Success)
            {
                ushort value = (ushort)(this.valueMap[match.Groups[2].Value] + 0x10);
                string label = match.Groups[1].Value;

                return new Tuple<ushort, string>(value, label);
            }
            throw new ArgumentException("Que?");
        }

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