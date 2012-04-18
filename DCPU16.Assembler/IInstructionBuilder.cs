namespace DCPU16.Assembler
{
    public interface IInstructionBuilder
    {
        ushort[] BuildInstruction(ushort opCode, dynamic instruction);
        ushort[] BuildExtendedInstruction(ushort opCode, dynamic instruction);
    }
}
