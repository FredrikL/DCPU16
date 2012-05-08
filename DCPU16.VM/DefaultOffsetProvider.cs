namespace DCPU16.VM
{
    internal class DefaultOffsetProvider : IOffsetProvider
    {
        private ISkipValue skipValue = new DefaultSkipValue();

        public short GetOffset(Instruction instruction)
        {
            short skipcount = 0;
            if (this.skipValue.SkipValue(instruction.b))
                skipcount--;
            if (this.skipValue.SkipValue(instruction.a))
                skipcount--;
            return skipcount;
        }
    }
}