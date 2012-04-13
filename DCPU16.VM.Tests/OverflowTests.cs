using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    public class OverflowTests
    {
        private Cpu cpu;

        [SetUp]
        public void Setup()
        {
            this.cpu = new Cpu();
        }
    
        [Test]
        public void Sub()
        {
            ushort[] program = { 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.Overflow, Is.EqualTo(0xffff));
        }

        [Test]
        public void Add()
        {
            ushort[] program = { 0x7c01, 0xffff,
                                 0x7c11, 0xffff,
                                 0x0402 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.Overflow, Is.EqualTo(0x0001));
        }
    }
}