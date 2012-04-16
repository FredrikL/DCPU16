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
            ushort[] program = { 0x7c01, 0x0003,
                                 0x7c11, 0x0002,
                                 0x0404 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x06));
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

        [Test]
        public void And()
        {
            ushort[] program = { 0x7c01, 0x0003,
                                 0x7c11, 0x0002,
                                 0x0409 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x2));
        }

        [Test]
        public void Bor()
        {
            ushort[] program = { 0x7c21, 0x0001,
                                 0x7c71, 0x0002,
                                 0x1c2a }; // BOR C, J

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.C, Is.EqualTo(0x3));
        }

        [Test]
        public void Xor()
        {
            ushort[] program = {
                                   0x7c41, 0x0001,
                                   0x7c51, 0x0002,
                                   0x144b
                               };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.Y, Is.EqualTo(0x3));
        }

        [Test]
        public void Ife()
        {
            ushort[] program = { 0x7c01, 0x0001,
                                 0x7c11, 0x0001,
                                 0x040c,
                                 0x7c01, 0x0004};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x4));
        }

        [Test]
        public void IfeShouldSkipIfNOtEqual()
        {
            ushort[] program = { 0x7c01, 0x0001,
                                 0x7c11, 0x0002,
                                 0x040c,
                                 0x7c01, 0x0004};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x1));
        }

        [Test]
        public void Ifg()
        {
            ushort[] program = { 0x7c01, 0x0002,
                                 0x7c11, 0x0001,
                                 0x040e,
                                 0x7c01, 0x0004};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x4));
        }

        [Test]
        public void IfgShouldSkipIfNotGreater()
        {
            ushort[] program = { 0x7c01, 0x0001,
                                 0x7c11, 0x0001,
                                 0x040e,
                                 0x7c01, 0x0004};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x1));
        }

        [Test]
        public void Ifb()
        {
            ushort[] program = { 0x7c01, 0x0001,
                                 0x7c11, 0x0003,
                                 0x040f,
                                 0x7c01, 0x0004}; 

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x4));
        }

        [Test]
        public void IfbShouldSkip()
        {
            ushort[] program = { 0x7c01, 0x0001,
                                 0x7c11, 0x0002,
                                 0x040f,
                                 0x7c01, 0x0004};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x1));
        }
    }
}