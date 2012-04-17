using Piglet.Parser;
using Piglet.Parser.Configuration.Fluent;

namespace DCPU16.Assembler
{
    public class AssemblerImpl
    {
        private IInstructionBuilder instructionBuilder = new InstructionBuilder();
        private IInstructionResolver instructionResolver = new InstructionResolver();
        private IValueMap valueMap = new ValueMap();

        public ushort[] Assemble(string assembly)
        {
            var config = ParserFactory.Fluent();
            var instructions = config.Rule();

            var register = GetRegisterMap(config);

            // Prescan source to find jump lables ?

            instructions.IsMadeUp.By("SET").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").
                WhenFound(f => this.instructionBuilder.BuildInstruction(0x01, f)).Or
                .By("SUB").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").
                WhenFound(f => this.instructionBuilder.BuildInstruction(0x03, f));

            IParser<object> parser = config.CreateParser();
            var x = (ushort[])parser.Parse(assembly);
            return x;
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

            var pc = config.Expression();
            pc.ThatMatches("PC").AndReturns(f => f);

            register.IsMadeUp.By(basicRegister).As("Name").WhenFound(f => this.valueMap[f.Name]).Or
                .By(registerPointer).As("Name").WhenFound(f => (ushort)(this.valueMap[f.Name] + 0x8)).Or
                .By(nextWordAndRegister).As("Instr").WhenFound(f => f.Instr).Or
                .By(nextWord).As("Instr").WhenFound(f => f.Instr).Or
                .By(nextWordLiteral).As("Instr").WhenFound(f => f.Instr).Or
                .By(pc).As("Name").WhenFound(f => this.valueMap[f.Name]);
            return register;
        }
    }
}