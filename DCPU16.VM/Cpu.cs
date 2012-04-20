using System;

namespace DCPU16.VM
{
    public class Cpu
    {
        private ISkipValue skipValue = new DefaultSkipValue();
        private IRegisters registers;
        private IRam ram;

        private bool skipNext = false;
        private bool programCounterManupulated = false;

        public ushort[] Ram
        {
            get { return this.ram.Ram; }
        }

        public Cpu(IRegisters registers, IRam ram)
        {
            this.registers = registers;
            this.ram = ram;
        }

        public void LoadProgram(ushort[] program)
        {
            this.registers.Reset();
            program.CopyTo(this.ram.Ram, 0);
        }

        private void SkipNextInstruction()
        {
            var ins = GetInstruction();
            // determine if we should advance programcounter 1 or 2 additional steps
            if (this.skipValue.SkipValue(ins.a)) this.registers.ProgramCounter++;
            if (this.skipValue.SkipValue(ins.b)) this.registers.ProgramCounter++;
        }

        private Func<ushort> GetSource(byte value, int offset  = 0)
        {
            ushort val;

            if (value >= 0x20 && value <= 0x3f)
                return () => (ushort) (value - 0x20);

            switch(value)
            {            
                case 0x00:
                    return () => this.registers.A;
                case 0x01:
                    return () => this.registers.B;
                case 0x02:
                    return () => this.registers.C;
                case 0x03:
                    return () => this.registers.X;
                case 0x04:
                    return () => this.registers.Y;
                case 0x05:
                    return () => this.registers.Z;
                case 0x06:
                    return () => this.registers.I;
                case 0x07:
                    return () => this.registers.J;

                case 0x08:
                    return () => this.ram.Ram[this.registers.A];
                case 0x09:
                    return () => this.ram.Ram[this.registers.B];
                case 0x0a:
                    return () => this.ram.Ram[this.registers.C];
                case 0x0b:
                    return () => this.ram.Ram[this.registers.X];
                case 0x0c:
                    return () => this.ram.Ram[this.registers.Y];
                case 0x0d:
                    return () => this.ram.Ram[this.registers.Z];
                case 0x0e:
                    return () => this.ram.Ram[this.registers.I];
                case 0x0f:
                    return () => this.ram.Ram[this.registers.J];

                case 0x10:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.A];
                case 0x11:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.B];
                case 0x12:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.C];
                case 0x13:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.X];
                case 0x14:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.Y];
                case 0x15:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.Z];
                case 0x16:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.I];
                case 0x17:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val + this.registers.J];


                case 0x18:
                    return () => this.ram.Ram[this.registers.StackPointer++];

                case 0x19:
                    return () => this.ram.Ram[this.registers.StackPointer];

                case 0x1b:
                    return () => this.registers.StackPointer;

                case 0x1c:
                    return () => this.registers.ProgramCounter;

                case 0x1d:
                    return () => this.registers.OverFlow;

                case 0x1e:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return () => this.ram.Ram[val];

                case 0x1f:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
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
                    return v => this.registers.A = v;
                case 0x1:
                    return v => this.registers.B = v;
                case 0x2:
                    return v => this.registers.C = v;
                case 0x3:
                    return v => this.registers.X = v;
                case 0x4:
                    return v => this.registers.Y = v;
                case 0x5:
                    return v => this.registers.Z = v;
                case 0x6:
                    return v => this.registers.I = v;
                case 0x7:
                    return v => this.registers.J = v;

                case 0x08:
                    return v => this.ram.Ram[this.registers.A] = v;
                case 0x09:
                    return v => this.ram.Ram[this.registers.B] = v;
                case 0x0a:
                    return v => this.ram.Ram[this.registers.C] = v;
                case 0x0b:
                    return v => this.ram.Ram[this.registers.X] = v;
                case 0x0c:
                    return v => this.ram.Ram[this.registers.Y] = v;
                case 0x0d:
                    return v => this.ram.Ram[this.registers.Z] = v;
                case 0x0e:
                    return v => this.ram.Ram[this.registers.I] = v;
                case 0x0f:
                    return v => this.ram.Ram[this.registers.J] = v;

                case 0x10:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.A] = v;
                case 0x11:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.B] = v;
                case 0x12:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.C] = v;
                case 0x13:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.X] = v;
                case 0x14:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.Y] = v;
                case 0x15:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.Z] = v;
                case 0x16:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.I] = v;
                case 0x17:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val + this.registers.J] = v;

                case 0x1a:
                    return v => this.ram.Ram[--this.registers.StackPointer] = v;

                case 0x1b:
                    return v => this.registers.StackPointer = v;

                case 0x1c:
                    this.programCounterManupulated = true;
                    return v => this.registers.ProgramCounter = v;

                case 0x1d:
                    return v => this.registers.OverFlow = v;

                case 0x1e:
                    val = this.ram.Ram[this.registers.ProgramCounter + offset];
                    return v => this.ram.Ram[val] = v;

                case 0x1f:
                    return v => this.ram.Ram[this.registers.ProgramCounter + offset] = v;

                default:
                    throw new NotImplementedException("GetDestination");
            }
        }

        private Instruction GetInstruction()
        {
            ushort word = this.ram.Ram[this.registers.ProgramCounter];
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
                        DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x / y), (x, y) => (ushort)(((x << 16) / y) & 0xffff), (y) => y != 0, () => this.registers.OverFlow = 0);
                        break;

                    case 0x6:
                        DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort)(x % y), (x, y) => 0, (y) => y != 0, () => 0);
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
                    this.registers.ProgramCounter++;                    
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
            this.registers.OverFlow = overflowOp(values.Item1, values.Item2);
            destination(op(values.Item1, values.Item2));
        }

        private void DoMathOp(Tuple<ushort, ushort> values, Action<ushort> destination, Func<ushort, ushort, ushort> op, Func<ushort, ushort, ushort> overflowOp, Func<ushort, bool> opCondition, Func<ushort> noConditionOp)
        {
            if (opCondition(values.Item2))
            {
                DoMathOp(values, destination, op, overflowOp);
            }
            else
                destination(noConditionOp());
        }

        private void Push(ushort value)
        {
            this.ram.Ram[--this.registers.StackPointer] = value;
        }

        private void Jsr(byte a)
        {
            ushort skipcount = 0;
            if (this.skipValue.SkipValue(a))
                skipcount++;
            var value = GetSource(a, skipcount)();
            this.Push((ushort)(this.registers.ProgramCounter + skipcount + 1)); // push locaion of next instruction
            this.registers.ProgramCounter = value;
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
