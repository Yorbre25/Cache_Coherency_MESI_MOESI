using System;
using System.Diagnostics;
using System.Net;
using System.Reflection.Metadata;

namespace Proyecto_Arqui.Classes.Moesi
{
    public sealed class MoesiInterconnect
    {

        private readonly static MoesiInterconnect _instance = new MoesiInterconnect();
        public bool inst_active;
        public int active_cpu = -1;


        public List<MoesiCPU_activity> CPU_lists;

        private MoesiInterconnect()
        {
            CPU_lists = new List<MoesiCPU_activity>();

            CPU_lists.Add(new MoesiCPU_activity(false));
            CPU_lists.Add(new MoesiCPU_activity(false));
            CPU_lists.Add(new MoesiCPU_activity(false));
        }

        public static MoesiInterconnect Instance
        {
            get { return _instance; }
        }

        public List<int> get_address(int address, int cpu_id)
        {
            List<int> response = new List<int>();
            int pos_in_cpu_list = find_in_caches(address, cpu_id);
            if (pos_in_cpu_list != -1)
            {
                int pos_in_cache = CPU_lists[pos_in_cpu_list].CPU.cache.where_in_cache(address);
                set_all_shared(address, cpu_id);
                Console.WriteLine($"CPU {pos_in_cpu_list} passing information to CPU {cpu_id}\n");
                Debug.WriteLine($"CPU {pos_in_cpu_list} passing information to CPU {cpu_id}\n");
                response.Add(2);
                response.Add(address);
                response.Add(CPU_lists[pos_in_cpu_list].CPU.cache.memory[pos_in_cache][2]); 
                return response;
            }
            else
            {
                int value_in_mem = MoesiMemory.Instance.get_from_address(address);
                response.Add(1);
                response.Add(address);
                response.Add(value_in_mem);
                Console.WriteLine($"Memory passing information to CPU {cpu_id}\n");
                Debug.WriteLine($"Memory passing information to CPU {cpu_id}\n");
                return response;
            }
        }

        public void set_all_shared(int address, int cpu_id)
        {
            int pos_in_cache;
            for (int i = 0; i < CPU_lists.Count; i++)
            {
                if (i != cpu_id)
                {
                    pos_in_cache = CPU_lists[i].CPU.cache.where_in_cache(address);
                    if (pos_in_cache != -1 && CPU_lists[i].CPU.cache.memory[pos_in_cache][0] != 3)
                    {
                        //change state from modified or owned to owned in CPU
                        if (CPU_lists[i].CPU.cache.memory[pos_in_cache][0] == 0 || CPU_lists[i].CPU.cache.memory[pos_in_cache][0] == 4)
                        {
                            CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 4;
                        }

                        else{
                            CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 2;
                        }
                    }
                }
            }

        }
        public void pass_inst(string inst, int cpu_id, int reg, int address)
        {
            string inst_to_execute = inst + $" {reg} {address}";
            CPU_lists[cpu_id].CPU.instruction_to_execute = inst_to_execute;
        }

        public void write_to_memory(int address, int value, int cpu_id)
        {
            Console.WriteLine($"CPU {cpu_id} writting to memory");
            MoesiMemory.Instance.write_to_address(address, value);
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

        public void Invalidate(int address, int cpu_id)
        {
            foreach (MoesiCPU_activity cpu_struct in CPU_lists)
            {
                if (cpu_id != cpu_struct.CPU.id)
                {
                    foreach (var cache_mem in cpu_struct.CPU.cache.memory)
                    {
                        if (cache_mem[1] == address && cache_mem[0] != 3)
                        {
                            cache_mem[0] = 3;
                            Console.WriteLine($"invalidating memory in to CPU {cpu_struct.CPU.id}\n");
                            Debug.WriteLine($"invalidating memory in to CPU {cpu_struct.CPU.id}\n");
                            break;
                        }
                    }
                }
            }
        }
    }
}
