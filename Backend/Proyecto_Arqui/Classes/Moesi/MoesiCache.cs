
using System.Diagnostics;
using System.Net;

namespace Proyecto_Arqui.Classes.Moesi
{
    public class MoesiCache:Cache
    {
        //STATES:
        //0 = M
        //4 = O
        //1 = E
        //2 = S
        //3 = I
        public MoesiCache()
        {
            memory = new List<List<int>>();
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
            memory.Add(new List<int>(new int[] { 3, 0, 0 }));
        }

        
        public override Transaction_tracker get_from_address(int address, int cpu_id)
        {
            List<int> cache_info = new List<int>();
            Transaction_tracker res = new Transaction_tracker();
            int pos_in_cache = where_in_cache(address);
            if (pos_in_cache != -1)
            {
                res.com_types["cache"] += 1;
                cache_info = memory[pos_in_cache];
            }else
            {

                int chache_to_replace = choose_cache_replacement();
                res.transactionList.Add(new transaction("READ_REQ", chache_to_replace, cpu_id, address, -1));
                var temp = MoesiInterconnect.Instance.get_address(address, cpu_id);
                res.transactionList.AddRange(temp.transactionList);
                cache_info = temp.cache_resp;
                res.add(temp.com_types);

                //write-back policy
                if (memory[chache_to_replace][0] == 0)
                {
                    res.transactionList.Add(MoesiInterconnect.Instance.write_to_memory(memory[chache_to_replace][1], memory[chache_to_replace][2], cpu_id));
                    res.com_types["memory"] += 1;
                }
                else if (memory[chache_to_replace][0] == 4)
                {
                    res.transactionList.Add(MoesiInterconnect.Instance.write_to_memory(memory[chache_to_replace][1], memory[chache_to_replace][2], cpu_id));
                    var temp1 = MoesiInterconnect.Instance.Invalidate(memory[chache_to_replace][1], cpu_id);
                    res.transactionList.AddRange(temp1.transactionList);
                    res.add(temp1.com_types);
                    res.com_types["memory"] += 1;
                }

                //replace memory
                res.transactionList.AddRange(state_for_tracker(cache_info, chache_to_replace, cpu_id).transactionList);
                memory[chache_to_replace][0] = cache_info[0];
                memory[chache_to_replace][1] = cache_info[1];
                memory[chache_to_replace][2] = cache_info[2];
                cache_info = memory[chache_to_replace];
            }
            res.cache_resp = cache_info;
            return res;
        }

        private Transaction_tracker state_for_tracker(List<int> memory, int pos_in_cache, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            if (memory[0] == 1)
            {
                res.transactionList.Add(new transaction("EX", pos_in_cache, cpu_id, memory[1], memory[2]));
            }
            else if (memory[0] == 2)
            {
                res.transactionList.Add(new transaction("SHARED", pos_in_cache, cpu_id, memory[1], memory[2]));
            }
            else if (memory[0] == 4)
            {
                res.transactionList.Add(new transaction("OWN", pos_in_cache, cpu_id, memory[1], memory[2]));
            }
            else
            {
                res.transactionList.Add(new transaction("MOD", pos_in_cache, cpu_id, memory[1], memory[2]));
            }

            return res;
        }
        public override Transaction_tracker write_to_address(int address, int value, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            int pos_in_cache = where_in_cache(address);
            if (pos_in_cache == -1)
            {
                Debug.WriteLine($"CPU {cpu_id} did not have address in cache\n");
                Console.WriteLine($"CPU {cpu_id} did not have address in cache\n");
                var temp1 = get_from_address(address, cpu_id);
                pos_in_cache = where_in_cache(address);
                res.transactionList.AddRange(temp1.transactionList);
                res.add(temp1.com_types);
            }
            else
            {
                res.com_types["cache"] += 1;
            }

            res.com_types["cache"] += 1;
            var temp = write_and_invalidate(value, address, pos_in_cache, cpu_id);
            res.transactionList.AddRange(temp.transactionList);
            res.add(temp.com_types);
            return res;
        }

        private Transaction_tracker write_and_invalidate(int value, int address, int pos_in_cache, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            //if already modified, same state just change value in cache
            if (memory[pos_in_cache][0] == 0)
            {
                res.transactionList.Add(new transaction("MOD", pos_in_cache, cpu_id, address, value));
                memory[pos_in_cache][2] = value;
            }

            //if already exlusive, change state from exclusive to Modified and value in cache
            else if (memory[pos_in_cache][0] == 1)
            {
                res.transactionList.Add(new transaction("MOD", pos_in_cache, cpu_id, address, value));
                memory[pos_in_cache][0] = 0;
                memory[pos_in_cache][2] = value;
            }
            //if already shared, change state from shared to Modified and value in cache.
            else if (memory[pos_in_cache][0] == 2)
            {
                res.transactionList.Add(new transaction("MOD", pos_in_cache, cpu_id, address, value));
                memory[pos_in_cache][0] = 0;
                memory[pos_in_cache][2] = value;
            }
            //if already in owned, change state from owned to modified.
            else if (memory[pos_in_cache][0] == 4)
            {
                res.transactionList.Add(new transaction("MOD", pos_in_cache, cpu_id, address, value));
                memory[pos_in_cache][0] = 0;
                memory[pos_in_cache][2] = value;
            }
            // invalidate all other caches values
            var temp = MoesiInterconnect.Instance.Invalidate(address, cpu_id);
            res.transactionList.AddRange(temp.transactionList);
            res.add(temp.com_types);
            return res;
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

        public override int where_in_cache(int address)
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
