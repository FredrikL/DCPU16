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
            switch(value)
            {            
                case 0x00:
                    return () => this.a;

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

                case 0x1e:
                    val = this.ram[this.programCounter++];
                    return v => this.ram[val] = v;

                default:
                    throw new NotImplementedException();
            }
        }

        public void Run()
        {
            while (true)
            {
                ushort word = ram[this.programCounter++];
                byte instruction = (byte) (0xf & word);
                byte a = (byte) (0x3f & (word >> 4));
                byte b = (byte) (0x3f & (word >> 10));

                switch (instruction)
                {
                    case 0x0:
                        Debug.WriteLine("Non-basic opcodes");
                        return; //escape
                        break;

                    case 0x1:
                        Debug.WriteLine("SET");
                        this.Set(a, b);
                        break;

                    case 0x2:
                        Debug.WriteLine("ADD");
                        break;

                    case 0x3:
                        Debug.WriteLine("SUB");
                        this.Sub(a, b);
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
