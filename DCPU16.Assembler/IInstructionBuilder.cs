namespace DCPU16.Assembler
{
    public interface IInstructionBuilder
    {
        ushort[] BuildInstruction(dynamic instruction);
        ushort[] BuildExtendedInstruction(ushort opCode, dynamic instruction);
    }
}
