using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class AssemblerTests
    {
        private AssemblerImpl assembler;

        [SetUp]
        public void Setup()
        {
            this.assembler = new AssemblerImpl();
        }

        [Test]
        public void ShouldSupportSimpleSet()
        {
            var assembly = "SET A, A";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result.Single(), Is.EqualTo(0x0001));
        }

        [Test]
        public void ShouldSupportSimpleSetB()
        {
            var assembly = "SET A, B";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result.Single(), Is.EqualTo(0x0401));
        }

    }
}
