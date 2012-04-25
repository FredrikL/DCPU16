namespace DCPU16.VM
{
    public class CostCalculator : ICostCalculator
    {
        public int CalculateCost(Instruction instruction)
        {
            return this.OpCodeCost(instruction) + ValueCost(instruction);
        }

        private int OpCodeCost(Instruction instruction)
        {
            switch (instruction.instruction)
            {
                case 0x0:
                    switch (instruction.a)
                    {
                        case 0x01:
                            return 3;
                    }
                    return 0;
                case 0x01:
                    return 1;

                case 0x06:
                case 0x07:
                case 0x08:
                    return 3;
                case 0x09:
                case 0x0a:
                case 0x0b:
                    return 1;
                
                default:
                    return 2;
            }
        }

        private int ValueCost(Instruction instruction)
        {
            if (instruction.instruction == 0x0) // extended
                return ValueCost(instruction.b);

            return ValueCost(instruction.a) + ValueCost(instruction.b);
        }

        private int ValueCost(byte value)
        {
            if ((value >= 0x10 && value <= 0x17) || value == 0x1e || value == 0x1f)
                return 1;
            return 0;
        }
    }
}