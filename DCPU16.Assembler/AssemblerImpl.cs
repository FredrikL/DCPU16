using System;
using System.Collections.Generic;
using System.Linq;
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
     
        public ushort[] Assemble(string assembly)
        {
            var config = ParserFactory.Fluent();
            var asm = config.Rule();
            var instructions = config.Rule();

            var opCode = config.Expression();
            opCode.ThatMatches("SET|ADD|SUB|MUL|DIV|MOD|SHL|SHR|AND|BOR|XOR|IFE|IFN|IFG|IFB").AndReturns(f => opCodes[f]);

            var register = GetRegisterMap(config);

            var label = config.Expression();
            label.ThatMatches(@":\w+").AndReturns(f => f.Substring(1));

            var labelWrapper = config.Rule();
            labelWrapper.IsMadeUp.By(label).As("label").Followed.By(instructions).As("ins").
                WhenFound(f => SetLabel(f.label, f.ins)).Or
                .By(instructions);
           
            instructions.IsMadeUp.By(opCode).As("opcode").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").WhenFound(f => this.instructionBuilder.BuildInstruction(f)).Or.
                By("JSR").Followed.By(register).As("values").
                WhenFound(f => this.instructionBuilder.BuildExtendedInstruction(0x01, f));

            asm.IsMadeUp.ByListOf<Instruction>(labelWrapper).As("Result").ThatIs.WhenFound(f =>  f.Result);

            IParser<object> parser = config.CreateParser();
            List<Instruction> instructionList = (List<Instruction>)parser.Parse(assembly);

            return ResolveLables(instructionList).ToArray();
        }

        private IEnumerable<ushort> ResolveLables(List<Instruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                if(!instruction.IsResolved)
                {
                    instruction.ResolveLables(instructions.ToArray());
                }
            }

            return instructions.SelectMany(instruction => instruction.AsBinary);
        }

        private Instruction SetLabel(string label, Instruction bar)
        {            
            bar.Label = label;
            return bar;
        }

        private IRule GetRegisterMap(IFluentParserConfigurator config)
        {
            var register = config.Rule();
            var basicRegister = config.Expression();
            basicRegister.ThatMatches(@"([ABCXYZIJ])").AndReturns(f => f);

            var registerPointer = config.Expression();
            registerPointer.ThatMatches(@"\[([ABCXYZIJ])\]").AndReturns(f => f.Substring(1, 1));

            var nextWordAndRegister = config.Expression();
            nextWordAndRegister.ThatMatches(@"\[0x\d{1:4}\+[ABCXYZIJ]\]").AndReturns(f => this.instructionResolver.Resolve(f));

            var nextWord = config.Expression();
            nextWord.ThatMatches(@"\[0x\d{1:4}\]").AndReturns(f => this.instructionResolver.Resolve(f));

            var nextWordLiteral = config.Expression();
            nextWordLiteral.ThatMatches(@"0x\d{1:4}").AndReturns(f => this.instructionResolver.Resolve(f));
            var nextWordLiteralDecimal = config.Expression();
            nextWordLiteralDecimal.ThatMatches(@"\d{1:2}").AndReturns(f => this.instructionResolver.Resolve(f));

            var constants = config.Expression();
            constants.ThatMatches("PC|SP|O|POP|PEEK|PUSH").AndReturns(f => f);

            var label = config.Expression();
            label.ThatMatches(@"\w+").AndReturns(f => f);

            register.IsMadeUp.By(basicRegister).As("Name").WhenFound(f => this.valueMap[f.Name]).Or
                .By(registerPointer).As("Name").WhenFound(f => (ushort)(this.valueMap[f.Name] + 0x8)).Or
                .By(nextWordAndRegister).As("Instr").WhenFound(f => f.Instr).Or
                .By(nextWord).As("Instr").WhenFound(f => f.Instr).Or
                .By(nextWordLiteral).As("Instr").WhenFound(f => f.Instr).Or
                .By(nextWordLiteralDecimal).As("Instr").WhenFound(f => f.Instr).Or
                .By(constants).As("Name").WhenFound(f => this.valueMap[f.Name]).Or
                .By(label).As("Label").WhenFound(f => f.Label);
            return register;
        }
    }
}