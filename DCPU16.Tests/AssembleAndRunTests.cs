using DCPU16.Assembler;
using DCPU16.VM;
using NUnit.Framework;

namespace DCPU16.Tests
{
    [TestFixture]
    public class AssembleAndRunTests
    {
        private AssemblerImpl assembler;
        private Cpu cpu;
        private DefaultRegisters registers;
        private DefaultRam ram;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            this.assembler = new AssemblerImpl();
        }      

        [SetUp]
        public void Setup()
        {
            registers = new DefaultRegisters();
            ram = new DefaultRam();
            this.cpu = new Cpu(registers, ram);
        }

        [Test]
        public void HelloWorld()
        {
            var asm = @"; Assembler test for DCPU
; by Markus Persson

             set a, 0xbeef                        ; Assign 0xbeef to register a
             set [0x1000], a                      ; Assign memory at 0x1000 to value of register a
             ifn a, [0x1000]                      ; Compare value of register a to memory at 0x1000 ..
                 set PC, end                      ; .. and jump to end if they don't match

             set i, 0                             ; Init loop counter, for clarity
:nextchar    ife [data+i], 0                      ; If the character is 0 ..
                 set PC, end                      ; .. jump to the end
             set [0x8000+i], [data+i]             ; Video ram starts at 0x8000, copy char there
             add i, 1                             ; Increase loop counter
             set PC, nextchar                     ; Loop
  
:data        dat ""Hello world!"", 0                ; Zero terminated string

:end         add PC, 1                            ; Freeze the CPU forever";

            var program = this.assembler.Assemble(asm);

            Assert.That(program.Length, Is.GreaterThan(0));

            this.cpu.LoadProgram(program);

            this.cpu.Run();

            Assert.That(this.ram.Ram[0x8000], Is.EqualTo(0x0048));
            Assert.That(this.ram.Ram[0x8001], Is.EqualTo(0x0045));
            Assert.That(this.ram.Ram[0x8002], Is.EqualTo(0x004c));
            Assert.That(this.ram.Ram[0x8003], Is.EqualTo(0x004c));
            Assert.That(this.ram.Ram[0x8004], Is.EqualTo(0x004f));
            Assert.That(this.ram.Ram[0x8005], Is.EqualTo(0x0020));
        }
        
    }
}
