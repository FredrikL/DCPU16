using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace DCPU16.VM.Tests
{
    [TestFixture]
    public class CpuSpec1_1Tests
    {
        // based on example code in 1.1 spec

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

        [Test]
        public void ShouldBeAbleToCompareAndSetProgramCounter()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.ProgramCounter, Is.EqualTo(0x000a)); 
        }

        [Test]
        public void ShouldSetITo10()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();
            
            Assert.That(this.cpu.I, Is.EqualTo(10));
        }

        [Test]
        public void ShouldSetATo0x2000()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.A, Is.EqualTo(0x2000));
        }

        [Test]
        public void ShouldBeAbleToSetOffsetWithValueFromI()
        {
            this.cpu.Ram[0x2000] = 0xff;
            this.cpu.Ram[0x2000 + 10] = 0x00;

            ushort[] program = {
                                   0x7c01, 0x0030,
                                   0x7de1, 0x1000, 0x0020,
                                   0x7803, 0x1000,
                                   0xc00d,
                                   0x7dc1, 0x001a,
                                   0xa861,
                                   0x7c01, 0x2000,
                                   0x2161, 0x2000
                               };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.Ram[0x2000 + 10], Is.EqualTo(0xff));
        }

        [Test]
        public void SubI()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.I, Is.EqualTo(9));
        }
    

        [Test]
        public void DoLoop()
        {
             ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d };

            this.cpu.LoadProgram(program);

            this.cpu.Run();
            
            Assert.That(this.cpu.I, Is.EqualTo(0x0));
            Assert.That(this.cpu.ProgramCounter, Is.EqualTo(0x13));
        }

        [Test]
        public void SetXTo0x4()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d,
                                 0x9031};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.X, Is.EqualTo(0x4));
        }

        [Test]
        public void JsrShouldSetProgramCounter()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d,
                                 0x9031,
                                 0x7c10, 0x0018};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.ProgramCounter, Is.EqualTo(0x18));
        }

        [Test]
        public void JsrShouldSetPushCurrentLocationToStack()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d,
                                 0x9031,
                                 0x7c10, 0x0018,
                                 0x7dc1, 0x001a };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.StackPointer, Is.EqualTo(0xfffe));
            Assert.That(this.cpu.Ram[this.cpu.StackPointer], Is.EqualTo(0x16));
        }

        [Test]
        public void ShlShouldWork()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d,
                                 0x9031,
                                 0x7c10, 0x0018,
                                 0x7dc1, 0x001a,
                                 0x9037 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.X, Is.EqualTo(0x40));
        }

        [Test]
        public void ShouldBeAbleToPop()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d,
                                 0x9031,
                                 0x7c10, 0x0018,
                                 0x7dc1, 0x001a,
                                 0x9037,
                                 0x61c1 };

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.cpu.StackPointer, Is.EqualTo(0xffff));
            Assert.That(this.cpu.ProgramCounter, Is.EqualTo(0x1a));
        }

        [Test, Ignore("Infinite loop in your tdd")]
        public void ShouldDoInfinteLoop()
        {
            ushort[] program = { 0x7c01, 0x0030,
                                 0x7de1, 0x1000, 0x0020,
                                 0x7803, 0x1000,
                                 0xc00d,
                                 0x7dc1, 0x001a,
                                 0xa861,
                                 0x7c01, 0x2000,
                                 0x2161, 0x2000,
                                 0x8463,
                                 0x806d,
                                 0x7dc1, 0x000d,
                                 0x9031,
                                 0x7c10, 0x0018,
                                 0x7dc1, 0x001a,
                                 0x9037,
                                 0x61c1,
                                 0x7dc1, 0x001a};

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.Fail("Infinite loop wasn't infinte");
        }
    }
}
