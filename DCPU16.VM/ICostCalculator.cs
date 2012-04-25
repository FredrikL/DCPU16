namespace DCPU16.VM
{
    public interface ICostCalculator
    {
        int CalculateCost(Instruction instruction);
    }
}