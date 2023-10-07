namespace Proyecto_Arqui.Classes
{
    public class transaction
    {
        public transaction(string op = "", int pos_cache = -1, int cpu_num = -1, int address = -1, int value = -1)
        {
            Op = op;
            Pos_cache = pos_cache;
            Cpu_num = cpu_num;
            Address = address;
            Value = value;
        }
        public string Op { get; set; }
        public int Pos_cache { get; set; }
        public int Cpu_num { get; set; }
        public int Address { get; set; }
        public int Value { get; set; }
    }
}