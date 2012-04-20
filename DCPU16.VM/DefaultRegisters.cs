namespace DCPU16.VM
{
    public class DefaultRegisters : IRegisters
    {
        public ushort ProgramCounter { get; set; }
        public ushort StackPointer { get; set; }
        public ushort OverFlow { get; set; }
        public ushort A { get; set; }
        public ushort B { get; set; }
        public ushort C { get; set; }
        public ushort X { get; set; }
        public ushort Y { get; set; }
        public ushort Z { get; set; }
        public ushort I { get; set; }
        public ushort J { get; set; }

        public ushort[] Registers { get { return new ushort[]{A,B,C,X,Y,Z,I,J};} }

        public void Reset()
        {
            this.ProgramCounter = 0x0;
            this.StackPointer = 0xffff;
            this.OverFlow = 0x0;
        }
    }
}