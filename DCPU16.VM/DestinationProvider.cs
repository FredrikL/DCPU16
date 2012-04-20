using System;

namespace DCPU16.VM
{
    public class DestinationProvider : IDestinationProvider
    {
        private ISkipValue skipValue = new DefaultSkipValue();
        private IRegisters registers;
        private IRam ram;

        public DestinationProvider(IRegisters registers, IRam ram)
        {
            this.registers = registers;
            this.ram = ram;
        }

        public Action<ushort> GetDestination(byte value)
        {
            ushort val;
            ushort offset = 0;
            if (this.skipValue.SkipValue(value))
                offset = 1;
            switch (value)
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
                    this.registers.ProgramCounterManipulated= true;
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
    }
}