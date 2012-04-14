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
                                 0x0403B };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x01));
        }
    }
}