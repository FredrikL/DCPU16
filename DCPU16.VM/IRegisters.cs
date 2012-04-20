namespace DCPU16.VM
{
    public interface IRegisters
    {
        ushort ProgramCounter { get; set; }
        ushort StackPointer { get; set; }
        ushort OverFlow { get; set; }

        ushort A { get; set; }
        ushort B { get; set; }
        ushort C { get; set; }
        ushort X { get; set; }
        ushort Y { get; set; }
        ushort Z { get; set; }
        ushort I { get; set; }
        ushort J { get; set; }

        ushort[] Registers { get; }

        void Reset();
        bool ProgramCounterManipulated { get; set; }
    }
}