using System.Collections.Generic;
using System.Linq;

namespace DCPU16.Assembler
{
    public class Instruction : IInstruction
    {
        private readonly ushort opcode;
        private readonly ushort[] additional;
        private readonly string[] labels;
        
        public string Label { get; set; }
        public bool IsResolved { get; set; }
        
        public int Size
        {
            get { return 1 + this.additional.Length; }
        }

        public Instruction(ushort opcode, ushort[] additional, string[] labels =null)
        {
            this.opcode = opcode;
            this.additional = additional;
            this.labels = labels;
            IsResolved = (this.labels == null || this.labels.Length ==0);
        }

        public void ResolveLables(IInstruction[] instructions)
        {
            if (IsResolved) return;
            int pos = 0;
            for(int i = 0; i < instructions.Length; i++)
            {
                if(this.labels.AsEnumerable().Any(x => x == instructions[i].Label))
                {
                    if (this.additional.Length == 1)
                    {
                        this.additional[0] = (ushort) pos;
                        this.IsResolved = true;
                    }
                }
                pos += instructions[i].Size;
            }
        }

        public ushort[] AsBinary { get
        {
            var x = new List<ushort>();
            x.Add(opcode);x.AddRange(additional);
            return x.ToArray();
        }}
    }
}