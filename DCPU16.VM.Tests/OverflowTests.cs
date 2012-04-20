using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class OverflowTests
    {
        private Cpu cpu;
        private DefaultRegisters registers;
        private DefaultRam ram;

        [SetUp]
        public void Setup()
        {
            registers = new DefaultRegisters();
            ram = new DefaultRam();
            this.cpu = new Cpu(registers, ram);
        }
    
        [Test]
        public void Sub()
        {
            ushort[] program = { 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.OverFlow, Is.EqualTo(0xffff));
        }

        [Test]
        public void Add()
        {
            ushort[] program = { 0x7c01, 0xffff,
                                 0x7c11, 0xffff,
                                 0x0402 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.OverFlow, Is.EqualTo(0x0001));
        }

        [Test]
        public void Mul()
        {
            ushort[] program = { 0x7c01, 0xffff,
                                 0x7c11, 0xffff,
                                 0x0404 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.OverFlow, Is.EqualTo(0xfffe));
        }

        [Test]
        public void DivByZero()
        {
            ushort[] program = { 0x7c01, 0xffff,
                                 0x7c11, 0x0000,
                                 0x0405 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.OverFlow, Is.EqualTo(0x0));
            Assert.That(this.registers.A, Is.EqualTo(0x0));
        }

        [Test]
        public void Div()
        {
            ushort[] program = { 0x7c01, 0x0002,
                                 0x7c11, 0xffff,
                                 0x0405 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.A, Is.EqualTo(0x0000));
            Assert.That(this.registers.OverFlow, Is.EqualTo(0x2));
        }

        [Test]
        public void Mod()
        {
            ushort[] program = { 0x7c01, 0x0003,
                                 0x7c11, 0x0002,
                                 0x0406 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.A, Is.EqualTo(0x0001));
        }

        [Test]
        public void ModByZero()
        {
            ushort[] program = { 0x7c01, 0x0003,
                                 0x7c11, 0x0000,
                                 0x0406 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.A, Is.EqualTo(0x0000));
        }

        [Test]
        public void ShlOverFlow()
        {
            ushort[] program = { 0x7c01, 0xffff,
                                 0x9007 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.A, Is.EqualTo(0xfff0));
            Assert.That(this.registers.OverFlow, Is.EqualTo(0x000f));
        }

        [Test]
        public void ShrOverFlow()
        {
            ushort[] program = { 0x7c01, 0x000f,
                                 0x9008 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.registers.A, Is.EqualTo(0x0000));
            Assert.That(this.registers.OverFlow, Is.EqualTo(0xf000));
        }
    }
}