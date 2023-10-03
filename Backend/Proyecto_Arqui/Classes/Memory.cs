namespace Proyecto_Arqui.Classes
{
    public sealed class Memory
    {
        private readonly static Memory _instance = new Memory();
        private int[] memory;
        private Memory() {
            memory = new int[32] {9,8,7,6,5,4,3,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        }

        public static Memory Instance
        {
            get { return _instance; }
        }
        
        public int get_from_address(int address)
        {
            return memory[address];
        }
        public void write_to_address(int address,int value)
        {
            memory[address] = value;
        }
    }
}
