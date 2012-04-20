using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class ValueAsSourceOrDestinationTests
    {
        private Cpu cpu;
        private DefaultRegisters registers = new DefaultRegisters();

        [SetUp]
        public void Setup()
        {
            this.cpu = new Cpu(registers);
        }

        [Test]
        public void WriteToDestination()
        {
            for (int i = 0x8; i < 0x10; i++)
            {
                ushort load = (ushort)( 0x7c01 + ((i-8) << 4));
                ushort instruction = (ushort) (((i-8) << 10) + (i << 4) + 0x01);

                ushort[] program = {
                                       load, 0xdead,
                                       instruction
                                   };

                this.cpu.LoadProgram(program);

                this.cpu.Run();

                Assert.That(this.registers.Registers[i - 8], Is.EqualTo(0xdead));
                Assert.That(this.cpu.Ram[0xdead], Is.EqualTo(0xdead));
            }
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

                Assert.That(this.registers.A, Is.EqualTo(i - 0x20));
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

                Assert.That(this.registers.Registers[i], Is.EqualTo(0xdead));
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

                Assert.That(this.registers.Registers[i], Is.EqualTo(i + 1));
                Assert.That(this.cpu.Ram[0x1000+(i+1)], Is.EqualTo(0x0));
                Assert.That(this.registers.ProgramCounter, Is.EqualTo(0x8));
            }
        }
    }
}