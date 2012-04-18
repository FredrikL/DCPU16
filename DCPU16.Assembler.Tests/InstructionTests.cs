using System.Linq;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class InstructionTests
    {
        private AssemblerImpl assembler;

        [SetUp]
        public void Setup()
        {
            this.assembler = new AssemblerImpl();
        }

        [Test]
        public void Shl()
        {
            string asm = "SHL X, 4";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x9037));
        }
    }
}