namespace DCPU16.VM
{
    public interface IRam
    {
        ushort[] Ram { get; set; }
    }

    public class DefaultRam : IRam
    {
        public DefaultRam()
        {
            this.Ram = new ushort[0x10000];
        }

        public ushort[] Ram{ get; set; }
    }
}