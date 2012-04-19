using System;

namespace DCPU16.VM
{
    public class Cpu
    {
        private ISkipValue skipValue = new DefaultSkipValue();

        private ushort[] ram = new ushort[0x10000];

        // registers
        private ushort a, b, c, x, y, z, i, j;

        private ushort programCounter = 0x0;
        private ushort stackPointer = 0xffff;
        private ushort overflow = 0x0;

        private bool skipNext = false;
        private bool programCounterManupulated = false;

        public ushort[] Ram
        {
            get { return this.ram; }
        }

        public ushort A { get { return this.a; } }
        public ushort B { get { return this.b; } }
        public ushort C { get { return this.c; } }
        public ushort X { get { return this.x; } }
        public ushort Y { get { return this.y; } }
        public ushort Z { get { return this.z; } }
        public ushort I { get { return this.i; } }
        public ushort J { get { return this.j; } }

        public ushort[] Registers
        {
            get { return new ushort[] {a, b, c, x, y, z, i, j}; }
        }

        public ushort ProgramCounter { get { return this.programCounter; } }
        public ushort StackPointer { get { return this.stackPointer; } }
        public ushort Overflow { get { return this.overflow; } }

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
            this.Reset();
            program.CopyTo(ram, 0);
        }

        private void SkipNextInstruction()
        {
            var ins = GetInstruction();
            // determine if we should advance programcounter 1 or 2 additional steps
            if (this.skipValue.SkipValue(ins.a)) this.programCounter++;
            if (this.skipValue.SkipValue(ins.b)) this.programCounter++;
        }

        private Func<ushort> GetSource(byte value, int offset  = 0)
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
                case 0x09:
                    return () => this.ram[this.b];
                case 0x0a:
                    return () => this.ram[this.c];
                case 0x0b:
                    return () => this.ram[this.x];
                case 0x0c:
                    return () => this.ram[this.y];
                case 0x0d:
                    return () => this.ram[this.z];
                case 0x0e:
                    return () => this.ram[this.i];
                case 0x0f:
                    return () => this.ram[this.j];

                case 0x10:
                    val = this.ram[this.programCounter+offset];
                    return () => this.ram[val + this.a];
                case 0x11:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.b];
                case 0x12:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.c];
                case 0x13:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.x];
                case 0x14:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.y];
                case 0x15:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.z];
                case 0x16:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.i];
                case 0x17:
                    val = this.ram[this.programCounter + offset];
                    return () => this.ram[val + this.a];


                case 0x18:
                    return () => this.ram[this.stackPointer++];

                case 0x19:
                    return () => this.ram[this.stackPointer];

                case 0x1b:
                    return () => this.programCounter;

                case 0x1c:
                    return () => this.programCounter;

                case 0x1d:
                    return () => this.overflow;

                case 0x1e:
                    val = this.ram[this.programCounter+offset];
                    return () => this.ram[val];

                case 0x1f:
                    val = this.ram[this.programCounter+offset];
                    return () => val;             

                default:
                    throw new NotImplementedException("GetSource");
            }            
        }

        private Action<ushort> GetDestination(byte value)
        {
            ushort val;
            ushort offset = 0;
            if (this.skipValue.SkipValue(value))
                offset = 1;
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

                case 0x08:
                    return v => this.ram[this.a] = v;
                case 0x09:
                    return v => this.ram[this.b] = v;
                case 0x0a:
                    return v => this.ram[this.c] = v;
                case 0x0b:
                    return v => this.ram[this.x] = v;
                case 0x0c:
                    return v => this.ram[this.y] = v;
                case 0x0d:
                    return v => this.ram[this.z] = v;
                case 0x0e:
                    return v => this.ram[this.i] = v;
                case 0x0f:
                    return v => this.ram[this.j] = v;

                case 0x10:
                    val = this.ram[this.programCounter+offset];
                    return v => this.ram[val + this.a] = v;
                case 0x11:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.b] = v;
                case 0x12:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.c] = v;
                case 0x13:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.x] = v;
                case 0x14:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.y] = v;
                case 0x15:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.z] = v;
                case 0x16:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.i] = v;
                case 0x17:
                    val = this.ram[this.programCounter + offset];
                    return v => this.ram[val + this.j] = v;

                case 0x1a:
                    return v => this.ram[--this.stackPointer] = v;

                case 0x1b:
                    return v => this.stackPointer = v;

                case 0x1c:
                    this.programCounterManupulated = true;
                    return v => this.programCounter = v;

                case 0x1d:
                    return v => this.overflow = v;

                case 0x1e:
                    val = this.ram[this.programCounter+offset];
                    return v => this.ram[val] = v;

                case 0x1f:
                    return v => this.ram[this.programCounter + offset] = v;

                default:
                    throw new NotImplementedException("GetDestination");
            }
        }

        private Instruction GetInstruction()
        {
            ushort word = ram[this.programCounter];
            byte instruction = (byte)(0xf & word);
            byte a = (byte)(0x3f & (word >> 0x4));
            byte b = (byte)(0x3f & (word >> 0xa));
   
            return new Instruction(){a = a, b=b, instruction = instruction, raw = word};
        }

        public void Run()
        {
            while (true)
            {                
                if(skipNext)
                {
                    this.SkipNextInstruction();
                    this.skipNext = false;
                }

                var ins = GetInstruction();

                switch (ins.instruction)
                {
                    case 0x0:
                        switch(ins.a)
                        {
                            case 0x01:
                                this.Jsr(ins.b);
                                break;
                            default:
#if DEBUG
                                // for testing, remove once full implementation is done                                
                                return;
#else
                                break;
#endif 
                        }                                              
                        break;

                    case 0x1:
                        this.Set(ins.a, ins.b);
                        break;

                    case 0x2:
                        DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x + y), (x, y) => (ushort)((x + y) > 0xffff ? 0x0001 : 0x0000));
                        break;

                    case 0x3:
                        DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x - y), (x, y) => (ushort)(x < y ? 0xffff : 0x0000));
                        break;

                    case 0x4:
                        DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x * y), (x, y) => (ushort)(((x * y) >> 16) & 0xffff));
                        break;

                    case 0x5:
                        this.Div(ins.a, ins.b);
                        break;

                    case 0x6:
                        this.Mod(ins.a, ins.b);
                        break;

                    case 0x7:
                        DoMathOp(Tuple.Create(GetSource(ins.a)(), (ushort)ins.b), GetDestination(ins.a), (x, y) => (ushort)(x << y), (x, y) => (ushort)(((x << y) >> 16) & 0xffff));
                        break;

                    case 0x8:
                        DoMathOp(Tuple.Create(GetSource(ins.a)(), (ushort)ins.b), GetDestination(ins.a), (x, y) => (ushort)(x >> y), (x, y) => (ushort)(((x << 16) >> y) & 0xffff));
                        break;

                    case 0x9:
                        DoBinaryOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x & y));
                        break;

                    case 0xa:
                        DoBinaryOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x | y));
                        break;

                    case 0xb:
                        DoBinaryOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x ^ y));
                        break;

                    case 0xc:
                        ShouldSkipNextIf(ResolveSources(ins.a, ins.b), (x, y) => (x != y));
                        break;

                    case 0xd:
                        ShouldSkipNextIf(ResolveSources(ins.a, ins.b), (x, y) => (x == y));
                        break;

                    case 0xe:
                        ShouldSkipNextIf(ResolveSources(ins.a, ins.b), (x, y) => (x <= y));
                        break;

                    case 0xf:
                        ShouldSkipNextIf(ResolveSources(ins.a, ins.b), (x, y) => (x & y) == 0);
                        break;

                    default:
                        throw new NotImplementedException("Run");
                }
                if (!this.programCounterManupulated)
                {
                    // in case we're using jsr don't increment programcounter here
                    // it's at the correct position
                    SkipNextInstruction();
                    this.programCounter++;                    
                }
                else
                    this.programCounterManupulated = false;
            }
        }

        private Tuple<ushort, ushort> ResolveSources(byte a, byte b)
        {
            ushort skipcount = 0;
            if (this.skipValue.SkipValue(a))
                skipcount++;
            var aVal = GetSource(a, skipcount)();
            if (this.skipValue.SkipValue(b))
                skipcount++;
            var bVal = GetSource(b, skipcount)();

            return Tuple.Create(aVal, bVal);
        }

        private void ShouldSkipNextIf(Tuple<ushort, ushort> values, Func<ushort,ushort,bool> comp)
        {
            this.skipNext = comp(values.Item1, values.Item2);
        }

        private void DoBinaryOp(Tuple<ushort, ushort> values, Action<ushort> destination, Func<ushort,ushort,ushort> op)
        {
            destination(op(values.Item1, values.Item2));
        }

        private void DoMathOp(Tuple<ushort, ushort> values, Action<ushort> destination, Func<ushort, ushort, ushort> op, Func<ushort, ushort, ushort> overflowOp)
        {
            this.overflow = overflowOp(values.Item1, values.Item2);
            destination(op(values.Item1, values.Item2));
        }

        private void Mod(byte a, byte b)
        {
            var values = ResolveSources(a, b);
            var dest = GetDestination(a);

            if(values.Item2 == 0)               
                dest(this.overflow = 0);
            else
                dest((ushort) (values.Item1 % values.Item2));
        }

        private void Div(byte a, byte b)
        {
            var values = ResolveSources(a, b);
            var dest = GetDestination(a);

            if (values.Item2 == 0)
                dest(this.overflow = 0);
            else
            {
                this.overflow = (ushort)(((values.Item1 << 16) / values.Item2) & 0xffff);
                dest((ushort)(values.Item1 / values.Item2));
            }
        }

        private void Push(ushort value)
        {
            this.ram[--this.stackPointer] = value;
        }

        private void Jsr(byte a)
        {
            ushort skipcount = 0;
            if (this.skipValue.SkipValue(a))
                skipcount++;
            var value = GetSource(a, skipcount)();
            this.Push((ushort)(this.programCounter + skipcount + 1)); // push locaion of next instruction
            this.programCounter = value;
            this.programCounterManupulated = true;
        }

        private void Set(byte a, byte b)
        {
            ushort skipcount = 0;
            if (this.skipValue.SkipValue(a))
                skipcount++;
            if (this.skipValue.SkipValue(b))
                skipcount++;
            var source = GetSource(b, skipcount);

            var dest = GetDestination(a);
            dest(source());
        }
    }
}
