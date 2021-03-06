﻿using System;

namespace DCPU16.VM
{
    public class Cpu : ICpu
    {
        private ISkipValue skipValue = new DefaultSkipValue();
        private IRegisters registers;
        private IRam ram;
        private IDestinationProvider destinationProvider;
        private ISourceProvider sourceProvider;
        private ICostCalculator costCalculator = new CostCalculator();

        private bool skipNext = false;

        private Instruction currentInstruction;

        public Cpu(IRegisters registers, IRam ram)
        {
            this.registers = registers;
            this.ram = ram;
            this.destinationProvider = new DestinationProvider(registers, ram);
            this.sourceProvider = new SourceProvider(registers, ram);
        }

        public void LoadProgram(ushort[] program)
        {
            this.registers.Reset();
            program.CopyTo(this.ram.Ram, 0);
        }        

        private Func<ushort> GetSource(byte value, int offset  = 0)
        {
            return this.sourceProvider.GetSource(value, offset);
        }

        private Action<ushort> GetDestination(byte value)
        {
            return this.destinationProvider.GetDestination(value, this.currentInstruction);
        }

        private Instruction GetInstruction()
        {
            ushort programCounter = this.registers.ProgramCounter;
            ushort word = this.ram.Ram[this.registers.ProgramCounter++];
            byte instruction = (byte)(0xf & word);
            byte a = (byte)(0x3f & (word >> 0x4));
            byte b = (byte)(0x3f & (word >> 0xa));

            if (this.skipValue.SkipValue(a)) this.registers.ProgramCounter++;
            if (this.skipValue.SkipValue(b)) this.registers.ProgramCounter++;

            return new Instruction(){a = a, b=b, instruction = instruction, raw = word, position =  programCounter};
        }

        private int GetCost(Instruction instruction)
        {
            return this.costCalculator.CalculateCost(instruction);
        }

        public int Tick()
        {
            if (skipNext)
            {
                GetInstruction();
                this.skipNext = false;
            }

#if DEBUG
            if (this.ram.Ram[this.registers.ProgramCounter] == 0) return 0;
#endif

            var ins = GetInstruction();
            this.currentInstruction = ins;
            int cost = GetCost(ins);

            switch (ins.instruction)
            {
                case 0x0:
                    switch (ins.a)
                    {
                        case 0x01:
                            this.Jsr(ins.b);
                            break;
                        default:
                            break;
                    }
                    break;

                case 0x1:
                    this.Set(ins.a, ins.b);
                    break;

                case 0x2:
                    DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x + y), (x, y) => (ushort) ((x + y) > 0xffff ? 0x0001 : 0x0000));
                    break;

                case 0x3:
                    DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x - y), (x, y) => (ushort) (x < y ? 0xffff : 0x0000));
                    break;

                case 0x4:
                    DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x*y), (x, y) => (ushort) (((x*y) >> 16) & 0xffff));
                    break;

                case 0x5:
                    DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x/y), (x, y) => (ushort) (((x << 16)/y) & 0xffff), (y) => y != 0, () => this.registers.OverFlow = 0);
                    break;

                case 0x6:
                    DoMathOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x%y), (x, y) => 0, (y) => y != 0, () => 0);
                    break;

                case 0x7:
                    DoMathOp(Tuple.Create(GetSource(ins.a)(), (ushort) ins.b), GetDestination(ins.a), (x, y) => (ushort) (x << y), (x, y) => (ushort) (((x << y) >> 16) & 0xffff));
                    break;

                case 0x8:
                    DoMathOp(Tuple.Create(GetSource(ins.a)(), (ushort) ins.b), GetDestination(ins.a), (x, y) => (ushort) (x >> y), (x, y) => (ushort) (((x << 16) >> y) & 0xffff));
                    break;

                case 0x9:
                    DoBinaryOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x & y));
                    break;

                case 0xa:
                    DoBinaryOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x | y));
                    break;

                case 0xb:
                    DoBinaryOp(ResolveSources(ins.a, ins.b), GetDestination(ins.a), (x, y) => (ushort) (x ^ y));
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

            return cost;
        }

        public void Run()
        {
            while (this.Tick() > 0) ;
        }

        private Tuple<ushort, ushort> ResolveSources(byte a, byte b)
        {
            short skipcount = 0;            
            if (this.skipValue.SkipValue(b))
                skipcount--;
            var bVal = GetSource(b, skipcount)();

            if (this.skipValue.SkipValue(a))
                skipcount--;
            var aVal = GetSource(a, skipcount)();
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
            short skipcount = 0;
            if (this.skipValue.SkipValue(a))
                skipcount--;
            var value = GetSource(a, skipcount)();
            this.Push((ushort)(this.registers.ProgramCounter + skipcount + 1)); // push location of next instruction
            this.registers.ProgramCounter = value;
            this.registers.ProgramCounterManipulated = true;
        }

        private void Set(byte a, byte b)
        {
            short skipcount = 0;            
            if (this.skipValue.SkipValue(b))
                skipcount--;
            var source = GetSource(b, skipcount);
            
            var dest = GetDestination(a);
            dest(source());
        }
    }
}
