using System;
using System.Linq;
using NUnit.Framework;

namespace DCPU16.Assembler.Tests
{
    [TestFixture]
    public class InstructionTests
    {
        private AssemblerImpl assembler;

        [TestFixtureSetUp]
        public void Setup()
        {
            this.assembler = new AssemblerImpl();
        }

        [Test]
        public void Shl()
        {
            string asm = "SHL X, 4";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x9037));
        }

        [Test]
        public void LablesShouldNotCauseParserError()
        {
            string asm = ":troll SHL X, 4";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x9037));
        }

        [Test]
        public void Labels()
        {
            string asm = @":troll SHL X, 4
                                  SET PC, troll";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x9037));
            Assert.That(result[1], Is.EqualTo(0x7dc1));
            Assert.That(result[2], Is.EqualTo(0x0000));
        }
        
        [Test]
        public void LabelsAreABitComplicated()
        {
            string asm = @"    SET A, A
                        :Troll SHL X, 4
                               SET PC, Troll";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x0001));
            Assert.That(result[1], Is.EqualTo(0x9037));
            Assert.That(result[2], Is.EqualTo(0x7dc1));
            Assert.That(result[3], Is.EqualTo(0x0001));
        }

        [Test]
        public void ShouldSupprtJsr()
        {
            string asm = @"    JSR troll
                        :troll SHL X, 4
                               SET PC, troll";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x7c10));
            Assert.That(result[1], Is.EqualTo(0x0002));
            Assert.That(result[2], Is.EqualTo(0x9037));
            Assert.That(result[3], Is.EqualTo(0x7dc1));
            Assert.That(result[4], Is.EqualTo(0x0002));
        }

        [Test]
        public void ShouldBeAbleToAssemble1_1SpecProgram()
        {
            string asm = @"
                      SET A, 0x30        
                      SET [0x1000], 0x20 
                      SUB A, [0x1000]    
                      IFN A, 0x10        
                         SET PC, crash   
                      
                      SET I, 10          
                      SET A, 0x2000      
        :loop         SET [0x2000+I], [A]
                      SUB I, 1           
                      IFN I, 0           
                         SET PC, loop    
        
                      SET X, 0x4         
                      JSR testsub        
                      SET PC, crash      
        
        :testsub      SHL X, 4           
                      SET PC, POP        
        :crash        SET PC, crash";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result.Length, Is.EqualTo(28));

        //0000: 7c01 0030 7de1 1000 0020 7803 1000 c00d
        //0008: 7dc1 001a a861 7c01 2000 2161 2000 8463
        //0010: 806d 7dc1 000d 9031 7c10 0018 7dc1 001a
        //0018: 9037 61c1 7dc1 001a 0000 0000 0000 0000
            Assert.That(result[0], Is.EqualTo(0x7c01));
            Assert.That(result[1], Is.EqualTo(0x0030));
            Assert.That(result[2], Is.EqualTo(0x7de1));
            Assert.That(result[3], Is.EqualTo(0x1000));
            Assert.That(result[4], Is.EqualTo(0x0020));
            Assert.That(result[5], Is.EqualTo(0x7803));
            Assert.That(result[6], Is.EqualTo(0x1000));
            Assert.That(result[7], Is.EqualTo(0xc00d));
            Assert.That(result[8], Is.EqualTo(0x7dc1));
            Assert.That(result[9], Is.EqualTo(0x001a));
            Assert.That(result[10], Is.EqualTo(0xa861));
            Assert.That(result[11], Is.EqualTo(0x7c01));
            Assert.That(result[12], Is.EqualTo(0x2000));
            Assert.That(result[13], Is.EqualTo(0x2161));
            Assert.That(result[14], Is.EqualTo(0x2000));
            Assert.That(result[15], Is.EqualTo(0x8463));
            Assert.That(result[16], Is.EqualTo(0x806d));
            Assert.That(result[17], Is.EqualTo(0x7dc1));
            Assert.That(result[18], Is.EqualTo(0x000d));
            Assert.That(result[19], Is.EqualTo(0x9031));
            Assert.That(result[20], Is.EqualTo(0x7c10));
            Assert.That(result[21], Is.EqualTo(0x0018));
            Assert.That(result[22], Is.EqualTo(0x7dc1));
            Assert.That(result[23], Is.EqualTo(0x001a));
            Assert.That(result[24], Is.EqualTo(0x9037));
            Assert.That(result[25], Is.EqualTo(0x61c1));
            Assert.That(result[26], Is.EqualTo(0x7dc1));
            Assert.That(result[27], Is.EqualTo(0x001a));
        }

        [Test]
        public void ShouldSupportLabelInSet()
        {
            string asm = @"    BOR [A], [hello+I]
                        :hello DAT 0xdead";


            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x588a));
            Assert.That(result[1], Is.EqualTo(0x0002));
            Assert.That(result[2], Is.EqualTo(0xdead));
        }

        [Test]
        public void Troll()
        {
            string asm = @"    BOR [A], [Hello+I]
                        :Hello DAT 0xdead";


            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x588a));
            Assert.That(result[1], Is.EqualTo(0x0002));
            Assert.That(result[2], Is.EqualTo(0xdead));
        }

        [Test]
        public void TrollLol()
        {
            string asm = @"    JSR Hello
                        :Hello SHL X, 4
                               SET PC, Hello";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x7c10));
            Assert.That(result[1], Is.EqualTo(0x0002));
            Assert.That(result[2], Is.EqualTo(0x9037));
            Assert.That(result[3], Is.EqualTo(0x7dc1));
            Assert.That(result[4], Is.EqualTo(0x0002));
        }

         [Test]
        public void Sub()
        {
            string asm = "SUB A, [0x1000]";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x7803));
            Assert.That(result[1], Is.EqualTo(0x1000));
        }

        [Test]
        public void SubLiteral()
        {
            string asm = "SUB I, 1";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0x8463));
        }

        [Test]
        public void IFN()
        {
            string asm = "IFN A, 0x10";

            var result = this.assembler.Assemble(asm).ToArray();

            Assert.That(result[0], Is.EqualTo(0xc00d));
        }

        [Test]
        public void DatA()
        {
            var assembly = "DAT \"A\"";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0041));
        }

        [Test]
        public void DatAA()
        {
            var assembly = "DAT \"AA\"";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0041));
            Assert.That(result[1], Is.EqualTo(0x0041));
        }

        [Test]
        public void DatShouldSupportHex()
        {
            var assembly = "DAT 0x1234";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x1234));
        }

        [Test]
        public void DatShouldSupportMultipleItems()
        {
            var assembly = "DAT 0x1234,\"C\" ,0xaaaa";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x1234));
            Assert.That(result[1], Is.EqualTo(0x0043));
            Assert.That(result[2], Is.EqualTo(0xaaaa));
        }

        [Test]
        public void DatCanHaveLabels()
        {
            var assembly = @"
     SET A, A
:lol DAT 0x1234,""C"" ,0xaaaa
     SET PC, lol";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0001));
            Assert.That(result[1], Is.EqualTo(0x1234));
            Assert.That(result[2], Is.EqualTo(0x0043));
            Assert.That(result[3], Is.EqualTo(0xaaaa));
            Assert.That(result[4], Is.EqualTo(0x7dc1));
            Assert.That(result[5], Is.EqualTo(0x0001));
        }

        [Test]
        public void HandleLableInSecondPosition()
        {
            var assembly = @"
     SET A, A
:lol DAT 0x1234,""C"" ,0xaaaa
     SET [0x8000+I], [lol+i]";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0001));
            Assert.That(result[1], Is.EqualTo(0x1234));
            Assert.That(result[2], Is.EqualTo(0x0043));
            Assert.That(result[3], Is.EqualTo(0xaaaa));
            Assert.That(result[4], Is.EqualTo(0x5961));
            Assert.That(result[5], Is.EqualTo(0x8000));
            Assert.That(result[6], Is.EqualTo(0x0001));
        }
        [Test]
        public void ShouldHandleLabelsInBothPositions()
        {
            var assembly = @"
     SET A, A
:lol DAT 0x1234,""C"" ,0xaaaa
     SET [lol+I], [lol+i]";

            var result = this.assembler.Assemble(assembly);

            Assert.That(result[0], Is.EqualTo(0x0001));
            Assert.That(result[1], Is.EqualTo(0x1234));
            Assert.That(result[2], Is.EqualTo(0x0043));
            Assert.That(result[3], Is.EqualTo(0xaaaa));
            Assert.That(result[4], Is.EqualTo(0x5961));
            Assert.That(result[5], Is.EqualTo(0x0001));
            Assert.That(result[6], Is.EqualTo(0x0001));
        }

        [Test, Ignore ]
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

:end         sub PC, 1                            ; Freeze the CPU forever";

            var result = this.assembler.Assemble(asm);

            //Assert.That(result.Length, Is.EqualTo(0x85c3));

            Assert.That(result[0], Is.EqualTo(0x7c01));
            Assert.That(result[1], Is.EqualTo(0xbeef));
            Assert.That(result[2], Is.EqualTo(0x01e1));
            Assert.That(result[3], Is.EqualTo(0x1000));
            Assert.That(result[4], Is.EqualTo(0x780d));
            Assert.That(result[5], Is.EqualTo(0x1000));
            Assert.That(result[6], Is.EqualTo(0x7dc1));
            Assert.That(result[7], Is.EqualTo(0x0021)); //0x21
            Assert.That(result[8], Is.EqualTo(0x8061));
            Assert.That(result[9], Is.EqualTo(0x816c));
            Assert.That(result[10], Is.EqualTo(0x00da));
            Assert.That(result[11], Is.EqualTo(0x7dc1));
            Assert.That(result[12], Is.EqualTo(0x0021));
            Assert.That(result[13], Is.EqualTo(0x5961));
            Assert.That(result[14], Is.EqualTo(0x8000));
            Assert.That(result[15], Is.EqualTo(0x00da));
            Assert.That(result[16], Is.EqualTo(0x8462));
            Assert.That(result[17], Is.EqualTo(0x7dc1));
            Assert.That(result[18], Is.EqualTo(0x0009));
            Assert.That(result[19], Is.EqualTo(0x00da));
            Assert.That(result[20], Is.EqualTo(0x0048));
            Assert.That(result[21], Is.EqualTo(0x0065));
            Assert.That(result[22], Is.EqualTo(0x006c));
            Assert.That(result[23], Is.EqualTo(0x006c));
            Assert.That(result[24], Is.EqualTo(0x006f));
            Assert.That(result[25], Is.EqualTo(0x0020));
            Assert.That(result[26], Is.EqualTo(0x0077));
            Assert.That(result[27], Is.EqualTo(0x006f));
            Assert.That(result[28], Is.EqualTo(0x0072));
            Assert.That(result[29], Is.EqualTo(0x006c));
            Assert.That(result[30], Is.EqualTo(0x0064));
            Assert.That(result[31], Is.EqualTo(0x0021));
            Assert.That(result[32], Is.EqualTo(0x0000));
            Assert.That(result[33], Is.EqualTo(0x85c3));
        }
    }
}