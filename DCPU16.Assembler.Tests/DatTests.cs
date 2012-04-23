using System.Linq;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class DatTests
    {

        private AssemblerImpl assembler;

        [SetUp]
        public void Setup()
        {
            this.assembler = new AssemblerImpl();
        }

        [Test]
        public void DatA()
        {
            var assembly = "DAT \"A\"";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0041));
        }

        [Test]
        public void DatAA()
        {
            var assembly = "DAT \"AA\"";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0041));
            Assert.That(result[1], Is.EqualTo(0x0041));
        }

        [Test]
        public void DatShouldSupportHex()
        {
            var assembly = "DAT 0x1234";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x1234));           
        }
    }
}