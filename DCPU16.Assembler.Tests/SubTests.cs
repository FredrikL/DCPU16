using System.Linq;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class SubTests
    {
        private AssemblerImpl assembler;

        [SetUp]
        public void Setup()
        {
            this.assembler = new AssemblerImpl();
        }

        [Test]
        public void Sub()
        {
            string asm = "SUB A, [0x1000]";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x7803));
            Assert.That(result[1], Is.EqualTo(0x1000));
        }
    }
}