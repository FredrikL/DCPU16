using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using Piglet.Parser;
using Piglet.Parser.Configuration.Fluent;

namespace DCPU16.Assembler
{
    public enum OpCode
    {
        SET,ADD,SUB,MUL,DIV,MOD,SHL,SHR,AND,BOR,XOR,IFE,IFN,IFG,IFB
    }

    public class AssemblerImpl
    {
        private IInstructionBuilder instructionBuilder = new InstructionBuilder();
        private IInstructionResolver instructionResolver = new InstructionResolver();
        private IValueMap valueMap = new ValueMap();
        private Dictionary<string, OpCode> opCodes = new Dictionary<string, OpCode>();

        private IExpressionConfigurator basicRegister, registerPointer, nextWordAndRegister, nextWord, hex, nextWordLiteralDecimal, constants, label,
            nextWordAndRegisterUsingLabel;

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
       }

        public ushort[] GetAsUshorts(byte[] input)
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
            foreach (dynamic item in items.values)
            {               
                if(((IDictionary<String, object>)item).ContainsKey("String")) {
                    ushort[] x = GetAsUshorts(Encoding.ASCII.GetBytes(item.String));
                    return new RawData(x);
                }

                if(((IDictionary<String, object>)item).ContainsKey("Binary")) {
                    var data = UInt16.Parse(item.Binary.Substring(2), NumberStyles.HexNumber);
                    return new RawData(new ushort[]{data});
                }
            }

            throw new NotImplementedException("ParseData");
        }

        public ushort[] Assemble(string assembly)
        {
            var config = ParserFactory.Fluent();
            var asm = config.Rule();
            var instructions = config.Rule();
            SetupExpressions(config);
            var opCode = config.Expression();
            opCode.ThatMatches("SET|ADD|SUB|MUL|DIV|MOD|SHL|SHR|AND|BOR|XOR|IFE|IFN|IFG|IFB").AndReturns(f => opCodes[f]);

            
            var register = GetRegisterMap(config);
            var dataRules = DataParts(config);
            
            var label = config.Expression();
            label.ThatMatches(@":\w+").AndReturns(f => f.Substring(1));

            var labelWrapper = config.Rule();
            labelWrapper.IsMadeUp.By(label).As("label").Followed.By(instructions).As("ins").
                WhenFound(f => SetLabel(f.label, f.ins)).Or
                .By(instructions);
           
            instructions.IsMadeUp.By(opCode).As("opcode").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").WhenFound(f => this.instructionBuilder.BuildInstruction(f)).Or.
                By("JSR").Followed.By(register).As("values").
                WhenFound(f => this.instructionBuilder.BuildExtendedInstruction(0x01, f)).Or
                .By("DAT").Followed.ByListOf(dataRules).As("values").
                WhenFound(f => ParseData(f));

            asm.IsMadeUp.ByListOf<IInstruction>(labelWrapper).As("Result").ThatIs.WhenFound(f =>  f.Result);

            IParser<object> parser = config.CreateParser();
            List<IInstruction> instructionList = (List<IInstruction>)parser.Parse(assembly);

            return ResolveLables(instructionList).ToArray();
        }

        private IEnumerable<ushort> ResolveLables(List<IInstruction> instructions)
        {
            foreach (var instruction in instructions.Where(instruction => !instruction.IsResolved))
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
                .By(hex).As("Binary").WhenFound(f => f);

            return datarules;
        }        

        private IRule GetRegisterMap(IFluentParserConfigurator config)
        {
            var register = config.Rule();           
            label = config.Expression();
            label.ThatMatches(@"\w+").AndReturns(f => f);

            register.IsMadeUp.By(basicRegister).As("Name").WhenFound(f => this.valueMap[f.Name]).Or
                .By(registerPointer).As("Name").WhenFound(f => (ushort)(this.valueMap[f.Name] + 0x8)).Or
                .By(nextWordAndRegister).As("Instr").WhenFound(f => this.instructionResolver.Resolve(f.Instr)).Or
                .By(hex).As("Instr").WhenFound(f => this.instructionResolver.Resolve(f.Instr)).Or
                .By(label).As("Label").WhenFound(f => f.Label); //.Or
            return register;
        }

        private void SetupExpressions(IFluentParserConfigurator config)
        {
            basicRegister = config.Expression();
            basicRegister.ThatMatches(@"[ABCXYZIJ]|PC|SP|O|POP|PEEK|PUSH").AndReturns(f => f);

            registerPointer = config.Expression();
            registerPointer.ThatMatches(@"\[[ABCXYZIJ]\]").AndReturns(f => f.Substring(1, 1));

            nextWordAndRegister = config.Expression();
            nextWordAndRegister.ThatMatches(@"\[0x[0-9a-fA-F]{1:4}\+[ABCXYZIJ]\]|\[0x[0-9a-fA-F]{1:4}\]|[0-9a-fA-F]{1:2}|\[\w+\+[ABCXYZIJ]\]").AndReturns(f => f);

            hex = config.Expression();
            hex.ThatMatches(@"0x[0-9a-fA-F]{1:4}").AndReturns(f => f);
        }
    }
}