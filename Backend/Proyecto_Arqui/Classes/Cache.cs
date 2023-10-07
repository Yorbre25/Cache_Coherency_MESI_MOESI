namespace Proyecto_Arqui.Classes
{
    public abstract class Cache
    {
        public List<List<int>> memory;

        public abstract Transaction_tracker write_to_address(int address, int v, int id);
        public abstract Transaction_tracker get_from_address(int address, int id);

        public abstract int where_in_cache(int address);
    }
}