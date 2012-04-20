using System;

namespace DCPU16.VM
{
    public class SourceProvider : ISourceProvider
    {
        private IRegisters registers;
        private IRam ram;

        public SourceProvider(IRegisters registers, IRam ram)
        {
            this.registers = registers;
            this.ram = ram;
        }

        public Func<ushort> GetSource(byte value, int offset = 0)
        {
            ushort val;

            if (value >= 0x20 && value <= 0x3f)
                return () => (ushort)(value - 0x20);

            switch (value)
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
    }
}