using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class ValueAsSourceOrDestinationTests
    {
        private Cpu cpu;

        [SetUp]
        public void Setup()
        {
            this.cpu = new Cpu();
        }

        [Test]
        public void WriteToDestination()
        {
            // B
            ushort load = (ushort) 0x7c01 + (0x01 << 4);
            ushort instruction = (ushort) (0x01 << 10) + (0x01 << 4) + 0x01;

            ushort[] program = {   load, 0xdead,
                                   instruction
                               };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.B, Is.EqualTo(0xdead));
        }

        [Test]
        public void LiteralValues()
        {
            for (int i = 0x20; i <= 0x3f; i++)
            {
                ushort instruction = (ushort)( (i << 10) + 0x01);
                ushort[] program = {
                                       instruction
                                   };

                this.cpu.LoadProgram(program);                
                this.cpu.Run();

                Assert.That(this.cpu.A, Is.EqualTo(i - 0x20));
            }
        }
    }
}