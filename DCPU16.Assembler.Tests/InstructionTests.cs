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

        [Test]
        public void LablesShouldNotCauseParserError()
        {
            string asm = ":troll SHL X, 4";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x9037));
        }

        [Test]
        public void Labels()
        {
            string asm = @":troll SHL X, 4
                                  SET PC, troll";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x9037));
            Assert.That(result[1], Is.EqualTo(0x7dc1));
            Assert.That(result[2], Is.EqualTo(0x0000));
        }

        [Test, Ignore]
        public void LabelsAreABitComplicated()
        {
            string asm = @"    SET A, A
                        :troll SHL X, 4
                               SET PC, troll";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x0001));
            Assert.That(result[1], Is.EqualTo(0x9037));
            Assert.That(result[2], Is.EqualTo(0x7dc1));
            Assert.That(result[3], Is.EqualTo(0x0001));
        }
    }
}