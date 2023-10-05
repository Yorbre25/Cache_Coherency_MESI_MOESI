using System.Diagnostics;
using System.Net;

namespace Proyecto_Arqui.Classes.Mesi
{
    public class MesiCache
    {
        //STATES:
        //0 = M
        //1 = E
        //2 = S
        //3 = I
        public List<List<int>> memory;
        public MesiCache()
        {
            memory = new List<List<int>>();
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));

        }


        public Transaction_tracker get_from_address(int address, int cpu_id)
        {
            List<int> cache_info;
            Transaction_tracker res = new Transaction_tracker();
            int pos_in_cache = where_in_cache(address);
            if (pos_in_cache != -1)
            {
                res.cost += 1;
                Debug.WriteLine($"CPU {cpu_id} has data in cache\n");
                Console.WriteLine($"CPU {cpu_id} has data in cache\n");
                cache_info = memory[pos_in_cache];
            }
            else
            {
                int chache_to_replace = choose_cache_replacement();
                var temp = MesiInterconnect.Instance.get_address(address, cpu_id);
                cache_info = temp.cache_resp;
                res.cost += temp.cost;

                //write-back policy
                if (memory[chache_to_replace][0] == 0)
                {
                    MesiInterconnect.Instance.write_to_memory(memory[chache_to_replace][1], memory[chache_to_replace][2], cpu_id);
                }
                memory[chache_to_replace][0] = cache_info[0];
                memory[chache_to_replace][1] = cache_info[1];
                memory[chache_to_replace][2] = cache_info[2];
                cache_info = memory[chache_to_replace];
            }
            
            res.cache_resp = cache_info;
            return res;
        }
        public Transaction_tracker write_to_address(int address, int value, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            int pos_in_cache = where_in_cache(address);
            if (pos_in_cache == -1)
            {
                Debug.WriteLine($"CPU {cpu_id} did not have address in cache\n");
                Console.WriteLine($"CPU {cpu_id} did not have address in cache\n");
                int chache_to_replace = choose_cache_replacement();
                var temp1 = get_from_address(address, cpu_id);
                pos_in_cache = chache_to_replace;
                res.cost = temp1.cost;
            }
            else
            {
                Debug.WriteLine($"CPU {cpu_id} has data in cache\n");
                Console.WriteLine($"CPU {cpu_id} has data in cache\n");
                res.cost = 1;
            }

            var temp = write_and_invalidate(value, address, pos_in_cache, cpu_id);
            res.cost += temp.cost;
            res.cache_resp = temp.cache_resp;
            return res;
        }

        private Transaction_tracker write_and_invalidate(int value, int address, int pos_in_cache, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
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
                memory[pos_in_cache][0] = 0;
                memory[pos_in_cache][2] = value;
            }
            // invalidate all other caches values
            return MesiInterconnect.Instance.Invalidate(address, cpu_id);
        }

        private int choose_cache_replacement()
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i][0] == 3)
                {
                    return i;
                }
            }
            Random rnd = new Random();
            return rnd.Next(memory.Count);
        }

        public int where_in_cache(int address)
        {
            for (int i = 0; i < memory.Count; i++)
            {
                if (memory[i][1] == address && memory[i][0] != 3)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
