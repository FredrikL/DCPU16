using System;
using Piglet.Parser;

namespace DCPU16.Assembler
{
    public class AssemblerImpl
    {
        public ushort[] Assemble(string assembly)
        {
            var config = ParserFactory.Fluent();
            var set = config.Rule();
            var register = config.Rule();

            register.IsMadeUp.By("A").WhenFound(f => (ushort)0x0)
                                        .Or
                                        .By("B").WhenFound(f => (ushort)0x1);
            set.IsMadeUp.By("SET").Followed.ByListOf(register).As("registers").ThatIs.SeparatedBy(",").WhenFound(f => (ushort)(0x01 + (f.registers[0] << 4) + (f.registers[1] << 10)));

            var x = (ushort)config.CreateParser().Parse(assembly);
            return new ushort[] {x};
        }
    }
}