namespace DCPU16.Assembler
{
    public interface IInstructionBuilder
    {
        IInstruction BuildInstruction(dynamic instruction);
        IInstruction BuildExtendedInstruction(ushort opCode, dynamic instruction);
    }
}
