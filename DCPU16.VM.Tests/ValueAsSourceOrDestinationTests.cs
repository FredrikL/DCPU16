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

        [Test]
        public void RegisterPointerValues()
        {
            for (int i = 0x0; i < 0x8; i++)
            {
                ushort[] program = {
                                       (ushort)(0x7c01 + (i << 4)), 0x1000,
                                       0x7de1, 0x1000, 0xdead,
                                       (ushort) (0x2001 + (i << 4) + (i << 10))
                                   };

                this.cpu.LoadProgram(program);
                this.cpu.Run();

                Assert.That(this.cpu.Registers[i], Is.EqualTo(0xdead));
            }
        }

        [Test]
        public void RegisterPointerValuesWithOffset()
        {
            for (int i = 0x0; i < 0x8; i++)
            {
                ushort[] program = {
                                       (ushort)(0x7c01 + (i << 4)), (ushort)(0x0000 + (i+1)),
                                       (ushort)(0x7D01 + (i << 4)), 0x1000, 0x2000,
                                       (ushort)(0x410b + (i <<4) + (i <<10) ), 0x1000, 0x1000
                                   };

                this.cpu.LoadProgram(program);
                this.cpu.Run();

                Assert.That(this.cpu.Registers[i], Is.EqualTo(i+1));
                Assert.That(this.cpu.Ram[0x1000+(i+1)], Is.EqualTo(0x0));
                Assert.That(this.cpu.ProgramCounter, Is.EqualTo(0x8));
            }
        }
    }
}