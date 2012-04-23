namespace DCPU16.Assembler
{
    public class RawData : IInstruction
    {
        private readonly ushort[] data;
        private readonly string[] labels;

        public RawData(ushort[] data, string[] labels = null)
        {
            this.data = data;
            this.labels = labels;
            this.IsResolved = true;
        }

        public string Label { get; set; }
        public bool IsResolved { get; set; }

        public int Size
        {
            get { return this.data.Length; }
        }

        public void ResolveLables(IInstruction[] instructions)
        {
            //    if (IsResolved) return;
            //    int pos = 0;
            //    for (int i = 0; i < instructions.Length; i++)
            //    {
            //        if (this.labels.AsEnumerable().Any(x => x == instructions[i].Label))
            //        {
            //            if (this.additional.Length == 1)
            //            {
            //                this.additional[0] = (ushort)pos;
            //                this.IsResolved = true;
            //            }
            //        }
            //        pos += instructions[i].Size;
            //    }
        }

        public ushort[] AsBinary
        {
            get { return this.data; }
        }
    }
}