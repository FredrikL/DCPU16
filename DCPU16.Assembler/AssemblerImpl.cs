using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using Piglet.Parser;
using Piglet.Parser.Configuration.Fluent;

namespace DCPU16.Assembler
{
    public class AssemblerImpl
    {
        private Dictionary<dynamic, ushort> registerValueMap;

        public AssemblerImpl()
        {
            this.registerValueMap = new Dictionary<dynamic, ushort>();
            this.registerValueMap.Add("A", 0x0);
            this.registerValueMap.Add("B", 0x1);
            this.registerValueMap.Add("C", 0x2);
            this.registerValueMap.Add("X", 0x3);
            this.registerValueMap.Add("Y", 0x4);
            this.registerValueMap.Add("Z", 0x5);
            this.registerValueMap.Add("I", 0x6);
            this.registerValueMap.Add("J", 0x7);
        }

        public ushort[] BuildInstruction(dynamic instruction)
        {            
            if(instruction.values[0] is Tuple<ushort, ushort>)
            {
                var instone = instruction.values[0] as Tuple<ushort, ushort>;
                var x = new List<ushort>();
                x.Add((ushort)(0x01 + (instone.Item1 << 4) + (instruction.values[1] << 10)));
                x.Add(instone.Item2);
                return x.ToArray();
            }

            if (instruction.values[0] is ushort && instruction.values[1] is ushort) 
                return new ushort[]{ (ushort) (0x01 + (instruction.values[0] << 4) + (instruction.values[1] << 10))};

            return new ushort[] {}; // y u no ushorts?
        }

        public ushort[] Assemble(string assembly)
        {
            var config = ParserFactory.Fluent();
            var set = config.Rule();

            var register = GetRegisterMap(config);


            set.IsMadeUp.By("SET").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").
                WhenFound(f => BuildInstruction(f));

            var x = (ushort[])config.CreateParser().Parse(assembly);
            return x;
        }

        private Tuple<ushort, ushort> Meh(string x)
        {
            // item 1 instruction (a|b)
            // item 2 value for next word
            string regex = @"\[0x(\d{4})\+([ABCXYZIJ])\]";
            Match match = Regex.Match(x, regex);
            if(match.Success)
            {
                ushort first = (ushort)(this.registerValueMap[match.Groups[2].Value] + 0x10);
                ushort second = UInt16.Parse(match.Groups[1].Value, NumberStyles.HexNumber);
                return new Tuple<ushort, ushort>(first,second);
            }            
            throw new ArgumentException("Que?");
        }

        private IRule GetRegisterMap(IFluentParserConfigurator config)
        {
            var register = config.Rule();
            var basicRegister = config.Expression();
            basicRegister.ThatMatches(@"([ABCXYZIJ])").AndReturns(f => f);

            var registerPointer = config.Expression();
            registerPointer.ThatMatches(@"\[([ABCXYZIJ])\]").AndReturns(f => f.Substring(1, 1));

            var nextWordAndRegister = config.Expression();
            nextWordAndRegister.ThatMatches(@"\[0x\d{4}\+[ABCXYZIJ]\]").AndReturns(f => Meh(f));

            register.IsMadeUp.By(basicRegister).As("Name").WhenFound(f => this.registerValueMap[f.Name]).Or
                .By(registerPointer).As("Name").WhenFound(f => (ushort)(this.registerValueMap[f.Name] + 0x8)).Or
                .By(nextWordAndRegister).As("Instr").WhenFound(f => f.Instr);
            return register;
        }
    }
}