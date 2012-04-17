using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class AssemblerSETTests
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

        [Test]
        public void ShouldSupportSetRegisterPointer()
        {
            var assembly = "SET [C], [X]";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result.Single(), Is.EqualTo(0x2ca1));
        }

        [Test]
        public void SetShouldHandleNextWordPlusRegister()
        {
            var assembly = "SET [0x2000+Z], Y";

            var result = this.assembler.Assemble(assembly).ToArray();

            Assert.That(result[0], Is.EqualTo(0x1151));
            Assert.That(result[1], Is.EqualTo(0x2000));
        }

        [Test]
        public void ShouldHandle3WordInstrictions()
        {
            var asm = "SET [0x1000], 0x20";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x7de1));
            Assert.That(result[1], Is.EqualTo(0x1000));
            Assert.That(result[2], Is.EqualTo(0x0020));
        }

    }
}

