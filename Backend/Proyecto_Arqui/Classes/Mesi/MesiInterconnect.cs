using System;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;

namespace Proyecto_Arqui.Classes.Mesi
{
    public sealed class MesiInterconnect
    {

        private readonly static MesiInterconnect _instance = new MesiInterconnect();
        public bool inst_active;
        public int active_cpu = -1;


        public List<MesiCPU_activity> CPU_lists;

        private MesiInterconnect()
        {
            CPU_lists = new List<MesiCPU_activity>();

            CPU_lists.Add(new MesiCPU_activity(false));
            CPU_lists.Add(new MesiCPU_activity(false));
            CPU_lists.Add(new MesiCPU_activity(false));
        }

        public static MesiInterconnect Instance
        {
            get { return _instance; }
        }

        public Transaction_tracker get_address(int address, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            int pos_in_cpu_list = find_in_caches(address, cpu_id);
            if (pos_in_cpu_list != -1)
            {
                int pos_in_cache = CPU_lists[pos_in_cpu_list].CPU.cache.where_in_cache(address);
                set_all_shared(address, cpu_id);
                Console.WriteLine($"CPU {pos_in_cpu_list} passing information to CPU {cpu_id}\n");
                Debug.WriteLine($"CPU {pos_in_cpu_list} passing information to CPU {cpu_id}\n");
                res.cost += 3;
                res.cache_resp = CPU_lists[pos_in_cpu_list].CPU.cache.memory[pos_in_cache];
            }
            else
            {
                int value_in_mem = MesiMemory.Instance.get_from_address(address);
                List<int> response = new List<int>();
                response.Add(1);
                response.Add(address);
                response.Add(value_in_mem);
                Console.WriteLine($"Memory passing information to CPU {cpu_id}\n");
                Debug.WriteLine($"Memory passing information to CPU {cpu_id}\n");
                res.cost += 9;
                res.cache_resp = response;
            }
            return res;
        }

        public Transaction_tracker set_all_shared(int address, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            bool save_to_mem = false;
            int cpu_to_write = 0;
            int pos_in_cache;
            for (int i = 0; i < CPU_lists.Count; i++)
            {
                if (i != cpu_id)
                {
                    pos_in_cache = CPU_lists[i].CPU.cache.where_in_cache(address);
                    if (pos_in_cache != -1 && CPU_lists[i].CPU.cache.memory[pos_in_cache][0] != 3)
                    {
                        if (CPU_lists[i].CPU.cache.memory[pos_in_cache][0] == 0)
                        {
                            save_to_mem = true;
                            cpu_to_write = i;
                        }
                        CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 2;
                    }
                }
            }
            //write to mem if a value was modified in another cache.
            if (save_to_mem)
            {
                pos_in_cache = CPU_lists[cpu_to_write].CPU.cache.where_in_cache(address);
                res.cost += 9;
                write_to_memory(address, CPU_lists[cpu_to_write].CPU.cache.memory[pos_in_cache][2], cpu_to_write);
            }
            return res;

        }
        public void pass_inst(string inst, int cpu_id, int reg, int address)
        {
            string inst_to_execute = inst + $" {reg} {address}";
            CPU_lists[cpu_id].CPU.instruction_to_execute = inst_to_execute;
        }

        public void write_to_memory(int address, int value, int cpu_id)
        {
            Console.WriteLine($"CPU {cpu_id} writting to memory");
            MesiMemory.Instance.write_to_address(address, value);
        }
        private int find_in_caches(int address, int id_of_Caller)
        {
            for (int i = 0; i < CPU_lists.Count; i++)
            {
                if (i != id_of_Caller)
                {
                    int pos_in_cache = CPU_lists[i].CPU.cache.where_in_cache(address);
                    if (pos_in_cache != -1 && CPU_lists[i].CPU.cache.memory[pos_in_cache][0] != 3)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public Transaction_tracker Invalidate(int address, int cpu_id)
        {
            Transaction_tracker tracker = new Transaction_tracker();
            foreach (MesiCPU_activity cpu_struct in CPU_lists)
            {
                if (cpu_id != cpu_struct.CPU.id)
                {
                    foreach (var cache_mem in cpu_struct.CPU.cache.memory)
                    {
                        if (cache_mem[1] == address && cache_mem[0] != 3)
                        {
                            tracker.cost += 3;
                            cache_mem[0] = 3;
                            Console.WriteLine($"invalidating memory in to CPU {cpu_struct.CPU.id}\n");
                            Debug.WriteLine($"invalidating memory in to CPU {cpu_struct.CPU.id}\n");
                            break;
                        }
                    }
                }
            }
            return tracker;
        }
    }
}
