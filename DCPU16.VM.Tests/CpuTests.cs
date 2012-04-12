using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class CpuTests
    {
        private Cpu cpu;

        [SetUp]
        public void Setup()
        {
            this.cpu = new Cpu();
        }

        [Test]
        public void ShouldLoadProgram()
        {
            ushort[] program = { 0x7c01, 0x0030 };

            this.cpu.LoadProgram(program);

            Assert.AreEqual(0x7c01, this.cpu.Ram[0]);
            Assert.AreEqual(0x0030, this.cpu.Ram[1]);
        }

        [Test]
        public void SetRegisterATo0x30()
        {
            ushort[] program = {0x7c01, 0x0030};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x30));
        }

        [Test]
        public void SetRamAddressTo0x20()
        {
            ushort[] program = { 0x7de1, 0x1000, 0x0020 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.Ram[0x1000], Is.EqualTo(0x20));
        }

        [Test]
        public void ShouldBeAbleToSub()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x10));
        }
    }
}
