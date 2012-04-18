namespace DCPU16.Assembler
{
    public interface IValueMap
    {
        ushort this[string registerCode] { get; }
    }
}