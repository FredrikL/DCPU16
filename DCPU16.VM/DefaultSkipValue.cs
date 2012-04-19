namespace DCPU16.VM
{
    public class DefaultSkipValue : ISkipValue
    {
        public bool SkipValue(byte value)
        {
            if (((value >= 0x10) && (value <= 0x17))
                || value == 0x1e
                || value == 0x1f)
                return true;
            return false;
        }
    }
}