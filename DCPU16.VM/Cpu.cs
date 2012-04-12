using System;
using System.Diagnostics;

namespace DCPU16.VM
{
    public class Cpu
    {
        private ushort[] ram = new ushort[0x10000];

        // registers
        private ushort a, b, c, x, y, z, i, j;

        private ushort programCounter = 0x0;
        private ushort stackPointer = 0xffff;
        private ushort overflow = 0x0;
        
        
        public ushort[] Ram
        {
            get { return this.ram; }
        }

        public ushort A { get { return this.a; }}
        public ushort I { get { return this.i; } }
        public ushort ProgramCounter { get { return this.programCounter; } }

        public Cpu()
        {
            Reset();
        }

        private void Reset()
        {
            this.programCounter = 0x0;
            this.stackPointer = 0xffff;
            this.overflow = 0x0;
            this.a = 0x0;
            this.b = 0x0;
            this.c = 0x0;
            this.x = 0x0;
            this.y = 0x0;
            this.z = 0x0;
            this.i = 0x0;
            this.j = 0x0;
        }

        public void LoadProgram(ushort[] program)
        {
            program.CopyTo(ram, 0);
        }

        private Func<ushort> GetSource(byte value)
        {
            ushort val;

            if (value >= 0x20 && value <= 0x3f)
                return () => (ushort) (value - 0x20);

            switch(value)
            {            
                case 0x00:
                    return () => this.a;
                case 0x01:
                    return () => this.b;
                case 0x02:
                    return () => this.c;

                case 0x03:
                    return () => this.x;
                case 0x04:
                    return () => this.y;
                case 0x05:
                    return () => this.z;

                case 0x06:
                    return () => this.i;
                case 0x07:
                    return () => this.j;

                case 0x08:
                    return () => this.ram[this.a];

                case 0x1e:
                    val = this.ram[this.programCounter++];
                    return () => this.ram[val];

                case 0x1f:
                    val = this.ram[this.programCounter++];
                    return () => val;             

                default:
                    throw new NotImplementedException();
            }            
        }

        private Action<ushort> GetDestination( byte value)
        {
            ushort val;
            switch(value)
            {
                case 0x0:
                    return v => this.a = v;
                case 0x1:
                    return v => this.b = v;
                case 0x2:
                    return v => this.c = v;
                case 0x3:
                    return v => this.x = v;
                case 0x4:
                    return v => this.y = v;
                case 0x5:
                    return v => this.z = v;
                case 0x6:
                    return v => this.i = v;
                case 0x7:
                    return v => this.j = v;

                case 0x16:
                    val = this.ram[this.programCounter++];
                    return v => this.ram[val + this.i] = v;

                case 0x1c:
                    return v => this.programCounter = v;

                case 0x1e:
                    val = this.ram[this.programCounter++];
                    return v => this.ram[val] = v;

                default:
                    throw new NotImplementedException();
            }
        }

        private Instruction GetInstruction()
        {
            ushort word = ram[this.programCounter++];
            byte instruction = (byte)(0xf & word);
            byte a = (byte)(0x3f & (word >> 0x4));
            byte b = (byte)(0x3f & (word >> 0xa));
   
            return new Instruction(){a = a, b=b, instruction = instruction};
        }

        public void Run()
        {
            while (true)
            {
                var ins = GetInstruction();

                switch (ins.instruction)
                {
                    case 0x0:
                        Debug.WriteLine("Non-basic opcodes");
                        return; //escape
                        break;

                    case 0x1:
                        Debug.WriteLine("SET");
                        this.Set(ins.a, ins.b);
                        break;

                    case 0x2:
                        Debug.WriteLine("ADD");
                        break;

                    case 0x3:
                        Debug.WriteLine("SUB");
                        this.Sub(ins.a, ins.b);
                        break;

                    case 0x4:
                        Debug.WriteLine("MUL");
                        break;

                    case 0x5:
                        Debug.WriteLine("DIV");
                        break;

                    case 0x6:
                        Debug.WriteLine("MOD");
                        break;

                    case 0x7:
                        Debug.WriteLine("SHL");
                        break;

                    case 0x8:
                        Debug.WriteLine("SHR");
                        break;

                    case 0x9:
                        Debug.WriteLine("AND");
                        break;

                    case 0xa:
                        Debug.WriteLine("BOR");
                        break;

                    case 0xb:
                        Debug.WriteLine("XOR");
                        break;

                    case 0xc:
                        Debug.WriteLine("IFE");
                        break;

                    case 0xd:
                        Debug.WriteLine("IFN");
                        this.Ifn(ins.a, ins.b);
                        break;

                    case 0xe:
                        Debug.WriteLine("IFG");
                        break;

                    case 0xf:
                        Debug.WriteLine("IFB");
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private bool SkipValue(byte value)
        {
            if ((
                    (value >= 0x10)  && (value <= 0x17)
                )
                || value == 0x1e 
                || value == 0x1f)
                return true;
            return false;
        }

        private void SkipNextInstruction()
        {
            var ins = GetInstruction();
            // determine if we should advance programcounter 1 or 2 additional steps
            if (SkipValue(ins.a)) this.programCounter++;
            if (SkipValue(ins.b)) this.programCounter++;
        }

        private void Ifn(byte a, byte b)
        {
            var aVal = GetSource(a)();
            var bVal = GetSource(b)();

            if (aVal == bVal)
                this.SkipNextInstruction();
        }

        private void Sub(byte a, byte b)
        {
            ushort aVal = GetSource(a)();

            ushort bVal = GetSource(b)();

            var dest = GetDestination(a);

            var res = (ushort)(aVal - bVal);
            //todo overflow check

            dest(res);
        }

        private void Set(byte a, byte b)
        {
            var dest = GetDestination(a);
            var source = GetSource(b);

            dest(source());
        }
    }
}
