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

        [TestFixtureSetUp]
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

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x1151));
            Assert.That(result[1], Is.EqualTo(0x2000));
        }

         [Test]
        public void ShouldHandleasdasdasdns()
        {
            var asm = "SUB A, [0x1000]";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0x7803));
            Assert.That(result[1], Is.EqualTo(0x1000));
        }

        [Test]
        public void ShouldHandle3WordInstrictions()
        {
            var asm = "SET [0x1000], 0x20";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0x7de1));
            Assert.That(result[1], Is.EqualTo(0x1000));
            Assert.That(result[2], Is.EqualTo(0x0020));
        }

        [Test]
        public void ShouldBeAbleToSetProgramCounter()
        {
            var asm = "SET PC, 0x1000";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0x7dc1));
            Assert.That(result[1], Is.EqualTo(0x1000));
        }

        [Test]
        public void ShouldBeAbleTOSetLiterals()
        {
            var asm = "SET I, 10";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0xa861));
        }

        [Test]
        public void ShouldBeAbleToHandleMoreThanOneInstruction()
        {
            var asm = @"SET A, 0x30
                        SET [0x1000], 0x20";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0x7c01));
            Assert.That(result[1], Is.EqualTo(0x0030));
            Assert.That(result[2], Is.EqualTo(0x7de1));
            Assert.That(result[3], Is.EqualTo(0x1000));
            Assert.That(result[4], Is.EqualTo(0x0020));
        }

        [Test]
        public void ShouldBePossibleToPop()
        {
            var asm = "SET PC, POP";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0x61c1));
        }

        [Test]
        public void ShouldHandleComments()
        {
            var asm = @"; meh 9
                        SET PC, POP ;foo
                        SET PC, POP ;foo";

            var result = this.assembler.Assemble(asm);

            Assert.That(result[0], Is.EqualTo(0x61c1));
            Assert.That(result[1], Is.EqualTo(0x61c1));
        }
    }
}

