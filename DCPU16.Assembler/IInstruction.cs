namespace DCPU16.Assembler
{
    public interface IInstruction
    {
        ushort[] AsBinary { get; }
        string Label { get; set; }
        int Size { get; }
        bool IsResolved { get; }

        void ResolveLables(IInstruction[] instructions);
    }
}