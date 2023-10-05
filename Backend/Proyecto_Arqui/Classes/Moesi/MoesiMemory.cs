namespace Proyecto_Arqui.Classes.Moesi
{
    public sealed class MoesiMemory
    {
        private readonly static MoesiMemory _instance = new MoesiMemory();
        private int[] memory;
        private MoesiMemory()
        {
            memory = new int[16] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, 0, 0, 0, 0, 0, 0};
        }

        public static MoesiMemory Instance
        {
            get { return _instance; }
        }

        public int get_from_address(int address)
        {
            return memory[address];
        }
        public void write_to_address(int address, int value)
        {
            memory[address] = value;
        }
    }
}
