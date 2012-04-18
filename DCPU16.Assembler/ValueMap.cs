using System.Collections.Generic;

namespace DCPU16.Assembler
{
    public class ValueMap : IValueMap
    {
        private Dictionary<dynamic, ushort> registerValueMap;

        public ValueMap()
        {
            this.registerValueMap = new Dictionary<dynamic, ushort>();
            this.registerValueMap.Add("A", 0x0);
            this.registerValueMap.Add("B", 0x1);
            this.registerValueMap.Add("C", 0x2);
            this.registerValueMap.Add("X", 0x3);
            this.registerValueMap.Add("Y", 0x4);
            this.registerValueMap.Add("Z", 0x5);
            this.registerValueMap.Add("I", 0x6);
            this.registerValueMap.Add("J", 0x7);
            this.registerValueMap.Add("POP", 0x18);
            this.registerValueMap.Add("PEEK", 0x19);
            this.registerValueMap.Add("PUSH", 0x1a);
            this.registerValueMap.Add("SP", 0x1b);
            this.registerValueMap.Add("PC", 0x1c);
            this.registerValueMap.Add("O", 0x1d);
        }

        public ushort this[string registerCode]
        {
            get { return registerValueMap[registerCode]; }
        }
    }
}