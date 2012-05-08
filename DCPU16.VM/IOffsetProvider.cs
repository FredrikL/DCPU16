namespace DCPU16.VM
{
    internal interface IOffsetProvider
    {
        short GetOffset(Instruction instruction);
    }
}