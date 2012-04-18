using System.Linq;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class ComparisonTests
    {
        private AssemblerImpl assembler;

        [SetUp]
        public void Setup()
        {
            this.assembler = new AssemblerImpl();
        }

        [Test]
        public void IFN()
        {
            string asm = "IFN A, 0x10";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0xc00d));
        }
    }
}