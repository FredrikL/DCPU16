using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class BasicInstructionsTests
    {
        private Cpu cpu;

        [SetUp]
        public void Setup()
        {
            this.cpu = new Cpu();
        }

        [Test]
        public void Add()
        {
            ushort[] program = { 0x7c01, 0x0001,
                                 0x7c11, 0x0001,
                                 0x0402 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x02));
        }

        [Test]
        public void Sub()
        {
            ushort[] program = { 0x7c01, 0x0002,
                                 0x7c11, 0x0001,
                                 0x0403 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x01));
        }

        [Test]
        public void Mul()
        {
            ushort[] program = { 0x7c01, 0x0002,
                                 0x7c11, 0x0002,
                                 0x0404 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x04));
        }

        [Test]
        public void Div()
        {
            ushort[] program = { 0x7c01, 0x0004,
                                 0x7c11, 0x0002,
                                 0x0405 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x02));
        }

        [Test]
        public void Mod()
        {
            ushort[] program = { 0x7c01, 0x0003,
                                 0x7c11, 0x0002,
                                 0x0406 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x01));
        }

        [Test]
        public void Shl()
        {
            ushort[] program = { 0x7c01, 0x0004,
                                 0x9007 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x0040));
            Assert.That(this.cpu.Overflow, Is.EqualTo(0x0));
        }

        [Test]
        public void Shr()
        {
            ushort[] program = { 0x7c01, 0x0040,
                                 0x9008 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x0004));
            Assert.That(this.cpu.Overflow, Is.EqualTo(0x0));
        }
    }
}