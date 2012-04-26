using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Piglet.Parser;
using Piglet.Parser.Configuration.Fluent;

namespace DCPU16.Assembler
{
    public class AssemblerImpl
    {
        private IInstructionBuilder instructionBuilder = new InstructionBuilder();
        private IInstructionResolver instructionResolver = new InstructionResolver();
        private IValueMap valueMap = new ValueMap();
        private Dictionary<string, OpCode> opCodes = new Dictionary<string, OpCode>();

        private IExpressionConfigurator constants, registers, instructionsToResolve, hex, label, digit;

        private IParser<object> parser;

        public AssemblerImpl()
        {
            opCodes.Add("SET", OpCode.SET);
            opCodes.Add("ADD", OpCode.ADD);
            opCodes.Add("SUB", OpCode.SUB);
            opCodes.Add("MUL", OpCode.MUL);
            opCodes.Add("DIV", OpCode.DIV);
            opCodes.Add("MOD", OpCode.MOD);
            opCodes.Add("SHL", OpCode.SHL);
            opCodes.Add("SHR", OpCode.SHR);
            opCodes.Add("AND", OpCode.AND);
            opCodes.Add("BOR", OpCode.BOR);
            opCodes.Add("XOR", OpCode.XOR);
            opCodes.Add("IFE", OpCode.IFE);
            opCodes.Add("IFN", OpCode.IFN);
            opCodes.Add("IFG", OpCode.IFG);
            opCodes.Add("IFB", OpCode.IFB);
            parser = CreateParser();
        }

        private static ushort[] GetAsUshorts(byte[] input)
        {
            List<ushort> ret = new List<ushort>();
            foreach (var b in input)
            {
                ret.Add(b);
            }
            return ret.ToArray();
        }

        public IInstruction ParseData(dynamic items)
        {
            List<ushort> parts = new List<ushort>();
            foreach (dynamic item in items.values)
            {               
                if(((IDictionary<String, object>)item).ContainsKey("String")) {
                    ushort[] x = GetAsUshorts(Encoding.ASCII.GetBytes(item.String));
                    parts.AddRange(x);
                }

                if(((IDictionary<String, object>)item).ContainsKey("Binary")) {
                    var data = UInt16.Parse(item.Binary.Substring(2), NumberStyles.HexNumber);
                    parts.Add(data);
                }

                if (((IDictionary<String, object>)item).ContainsKey("Digit"))
                {
                    var data = UInt16.Parse(item.Digit);
                    parts.Add(data);
                }
            }
            return new RawData(parts.ToArray());
        }

        public ushort[] Assemble(string assembly)
        {            
            List<IInstruction> instructionList = (List<IInstruction>)parser.Parse(assembly.ToUpper());
            return ResolveLables(instructionList).ToArray();
        }

        private IParser<object> CreateParser()
        {
            var config = ParserFactory.Fluent();
            var asm = config.Rule();
            var instructions = config.Rule();
            var opCode = config.Expression();
            opCode.ThatMatches("SET|ADD|SUB|MUL|DIV|MOD|SHL|SHR|AND|BOR|XOR|IFE|IFN|IFG|IFB").AndReturns(f => opCodes[f]);
            var jsr = config.Expression();
            jsr.ThatMatches("JSR").AndReturns(f => null);
            var dat = config.Expression();
            dat.ThatMatches("DAT").AndReturns(f => null);

            SetupExpressions(config);

            var register = GetRegisterMap(config);
            var dataRules = DataParts(config);
            var lbl = config.Rule();
            lbl.IsMadeUp.By(label);

            config.Ignore(";[^\\n]+[\\n|\0]");

            var labelWrapper = config.Rule();
            labelWrapper.IsMadeUp.By(":").Followed.By(lbl).As("label").Followed.By(instructions).As("ins").
                WhenFound(f => SetLabel(f.label, f.ins)).Or
                .By(instructions);

            instructions.IsMadeUp.By(opCode).As("opcode").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").WhenFound(f => this.instructionBuilder.BuildInstruction(f)).Or.
                By(jsr).Followed.By(lbl).As("values").
                WhenFound(f => this.instructionBuilder.BuildExtendedInstruction(0x01, f)).Or
                .By(dat).Followed.ByListOf(dataRules).As("values").ThatIs.SeparatedBy(",").
                WhenFound(f => ParseData(f));

            asm.IsMadeUp.ByListOf<IInstruction>(labelWrapper).As("Result").ThatIs.WhenFound(f => f.Result);

            return config.CreateParser();            
        }

        private IEnumerable<ushort> ResolveLables(List<IInstruction> instructions)
        {
            foreach (var instruction in instructions.Where(instruction =>!instruction.IsResolved))
            {
                instruction.ResolveLables(instructions.ToArray());
            }

            return instructions.SelectMany(instruction => instruction.AsBinary);
        }

        private IInstruction SetLabel(string label, IInstruction instruction)
        {            
            instruction.Label = label;
            return instruction;
        }

        private IRule DataParts(IFluentParserConfigurator config)
        {
            var datarules = config.Rule();

            datarules.IsMadeUp.By(config.QuotedString).As("String").WhenFound(f => f).Or
                .By(hex).As("Binary").WhenFound(f => f).Or
                .By(digit).As("Digit").WhenFound(f => f);

            return datarules;
        }        

        private IRule GetRegisterMap(IFluentParserConfigurator config)
        {
            var register = config.Rule();

            register.IsMadeUp.By(constants).As("Name").WhenFound(f => this.valueMap[f.Name]).Or.
                By(registers).As("Name").WhenFound(f => this.valueMap[f.Name]).Or.
                By("[").Followed.By(registers).As("Name").Followed.By("]").WhenFound(f => (ushort) (this.valueMap[f.Name] + 0x8)).Or.
                By("[").Followed.By(hex).As("Hex").Followed.By("+").Followed.By(register).As("Reg").Followed.By("]").WhenFound(f => this.instructionResolver.ResolveRegisterPointer(f.Hex, f.Reg)).Or.
                By("[").Followed.By(hex).As("Hex").Followed.By("]").WhenFound(f => this.instructionResolver.ResolveNextWord(f.Hex)).Or.
                By(hex).As("Hex").WhenFound(f => this.instructionResolver.ResolveHex(f.Hex)).Or.
                By(digit).As("Digit").WhenFound(f => this.instructionResolver.ResolveLiteralValue(f.Digit)).Or.
                By("[").Followed.By(label).As("Label").Followed.By("+").Followed.By(registers).As("Reg").Followed.By("]").WhenFound(f => this.instructionResolver.ResolveLabelAndRegister(f.Label, f.Reg)).Or.
                By(label);
            return register;
        }

        private void SetupExpressions(IFluentParserConfigurator config)
        {
            constants = config.Expression();
            constants.ThatMatches("PC|SP|O|POP|PEEK|PUSH").AndReturns(f => f);

            registers = config.Expression();
            registers.ThatMatches(@"[ABCXYZIJ]").AndReturns(f => f);

            digit = config.Expression();
            digit.ThatMatches(@"\d|1\d|0\d").AndReturns(f =>f);

            hex = config.Expression();
            hex.ThatMatches(@"0[xX][0-9a-fA-F]{1:4}").AndReturns(f => f);

            label = config.Expression();
            label.ThatMatches(@"[a-zA-Z]\w+").AndReturns(f => f);
        }
    }
}