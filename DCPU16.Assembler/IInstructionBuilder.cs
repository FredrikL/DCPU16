namespace DCPU16.Assembler
{
    public interface IInstructionBuilder
    {
        Instruction BuildInstruction(dynamic instruction);
        Instruction BuildExtendedInstruction(ushort opCode, dynamic instruction);
    }
}
