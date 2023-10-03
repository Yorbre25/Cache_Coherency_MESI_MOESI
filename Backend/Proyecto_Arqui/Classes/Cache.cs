using System.Diagnostics;
using System.Net;

namespace Proyecto_Arqui.Classes
{
    public class Cache
    {
        //STATES:
        //0 = M
        //1 = E
        //2 = S
        //3 = I
        public List<List<int>> memory;
        public Cache()
        {
            memory = new List<List<int>>();
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));

        }


        public List<int> get_from_address(int address, int cpu_id)
        {
            int pos_in_cache = where_in_cache(address);
            if(pos_in_cache != - 1) {
                Debug.WriteLine($"CPU {cpu_id} has data in cache\n");
                Console.WriteLine($"CPU {cpu_id} has data in cache\n");
                return memory[pos_in_cache];
            }

            int chache_to_replace = choose_cache_replacement();
            List<int> cache_info = Interconnect.Instance.get_address(address, cpu_id);

            memory[chache_to_replace][0] = cache_info[0];
            memory[chache_to_replace][1] = cache_info[1];
            memory[chache_to_replace][2] = cache_info[2];
            
            return cache_info;
        }
        public void write_to_address(int address, int value, int cpu_id)
        {
            int pos_in_cache = where_in_cache(address);
            if (pos_in_cache == -1)
            {
                Debug.WriteLine($"CPU {cpu_id} did not have address in cache\n");
                Console.WriteLine($"CPU {cpu_id} did not have address in cache\n");
                int chache_to_replace = choose_cache_replacement();
                List<int> cache_info = get_from_address(address, cpu_id);
                pos_in_cache = chache_to_replace;
            }
            else
            {
                Debug.WriteLine($"CPU {cpu_id} has data in cache\n");
                Console.WriteLine($"CPU {cpu_id} has data in cache\n");
            }

            write_and_invalidate(value,address,pos_in_cache,cpu_id);
        }

        private void write_and_invalidate(int value,int address,int pos_in_cache, int cpu_id)
        {
            //if already modified, same state just change value in cache
            if (memory[pos_in_cache][0] == 0)
            {
                memory[pos_in_cache][2] = value;
            }

            //if already exlusive, change state from exclusive to Modified and value in cache
            else if (memory[pos_in_cache][0] == 1)
            {
                memory[pos_in_cache][0] = 0;
                memory[pos_in_cache][2] = value;
            }
            //if already shared, change state from shared to Modified and value in cache.
            else if (memory[pos_in_cache][0] == 2)
            {
                memory[pos_in_cache][0] = 1;
                memory[pos_in_cache][2] = value;
                Interconnect.Instance.write_to_memory(address, value, cpu_id);
            }
            // invalidate all other caches values
            Interconnect.Instance.Invalidate(address, cpu_id);
        }

        private int choose_cache_replacement()
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i][0] == 3) {
                    return i; 
                }
            }
            Random rnd = new Random();
            return rnd.Next(memory.Count);
        }

        public int where_in_cache(int address)
        {
            for( int i = 0; i < memory.Count; i++)
            {
                if (memory[i][1] == address && (memory[i][0] != 3)){
                    return i;
                }
            }
            return -1;
        }
    }
}
