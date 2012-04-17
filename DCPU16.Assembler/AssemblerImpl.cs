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
            var set = config.Rule();

            var register = GetRegisterMap(config);

            set.IsMadeUp.By("SET").Followed.ByListOf(register).As("values").
                ThatIs.SeparatedBy(",").
                WhenFound(f => this.instructionBuilder.BuildInstruction(0x01, f));

            var x = (ushort[])config.CreateParser().Parse(assembly);
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
            nextWordAndRegister.ThatMatches(@"\[0x\d{4}\+[ABCXYZIJ]\]").AndReturns(f => this.instructionResolver.Resolve(f));

            register.IsMadeUp.By(basicRegister).As("Name").WhenFound(f => this.valueMap[f.Name]).Or
                .By(registerPointer).As("Name").WhenFound(f => (ushort)(this.valueMap[f.Name] + 0x8)).Or
                .By(nextWordAndRegister).As("Instr").WhenFound(f => f.Instr);
            return register;
        }
    }
}