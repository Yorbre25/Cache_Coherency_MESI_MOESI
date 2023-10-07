using Proyecto_Arqui.Classes.Mesi;
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
        public IDictionary<string, int> Report_counts;
        public MoesiMemory RAM = MoesiMemory.Instance;
        public List<string> CPUinstrucction1 = new List<string>();
        public List<string> CPUinstrucction2 = new List<string>();
        public List<string> CPUinstrucction3 = new List<string>();
        private static List<Transaction_tracker> response;

        public List<MoesiCPU_activity> CPU_lists;

        private MoesiInterconnect()
        {
            CPU_lists = new List<MoesiCPU_activity>();

            CPU_lists.Add(new MoesiCPU_activity(false));
            CPU_lists.Add(new MoesiCPU_activity(false));
            CPU_lists.Add(new MoesiCPU_activity(false));
            Report_counts = new Dictionary<string, int>();
            Report_counts.Add("ReadReq", 0);
            Report_counts.Add("WriteReq", 0);
            Report_counts.Add("INV", 0);
            response = new List<Transaction_tracker>();
            response.Add(new Transaction_tracker());
            response.Add(new Transaction_tracker());
            response.Add(new Transaction_tracker());
        }
        public static Transaction_tracker get_execution(int cpu_id)
        {
            return response[cpu_id];
        }

        public static void set_execution(int cpu_id, Transaction_tracker exec)
        {
            response[cpu_id] = exec;
        }
        public static MoesiInterconnect Instance
        {
            get { return _instance; }
        }

        public Transaction_tracker get_address(int address, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            List<int> response = new List<int>();
            if (Report_counts.ContainsKey("ReadReq"))
            {
                Report_counts["ReadReq"] += 1;
            }
            int pos_in_cpu_list = find_in_caches(address, cpu_id);
            if (pos_in_cpu_list != -1)
            {
                int pos_in_cache = CPU_lists[pos_in_cpu_list].CPU.cache.where_in_cache(address);
                var temp = set_all_shared(address, cpu_id);

                var value = CPU_lists[pos_in_cpu_list].CPU.cache.memory[pos_in_cache][2];
                response.Add(2);
                response.Add(address);
                response.Add(value);
                res.transactionList.Add(new transaction("RESP", pos_in_cache, pos_in_cpu_list, address, value));
                res.add(temp);
                res.cache_resp = response;
                return res;
            }
            else
            {
                int value_in_mem = MoesiMemory.Instance.get_from_address(address);
                response.Add(1);
                response.Add(address);
                response.Add(value_in_mem);
                res.com_types["memory"] += 1;
                res.transactionList.Add(new transaction("RESP", -1, 3, address, value_in_mem));
                res.cache_resp = response;
                return res;
            }
        }

        public Transaction_tracker set_all_shared(int address, int cpu_id)
        {
            Transaction_tracker res = new Transaction_tracker();
            int amount_of_shared = 0;
            int pos_in_cache;
            for (int i = 0; i < CPU_lists.Count; i++)
            {
                if (i != cpu_id)
                {
                    pos_in_cache = CPU_lists[i].CPU.cache.where_in_cache(address);
                    if (pos_in_cache != -1 && CPU_lists[i].CPU.cache.memory[pos_in_cache][0] != 3)
                    {
                        //change state from modified or owned to owned in CPU
                        if (CPU_lists[i].CPU.cache.memory[pos_in_cache][0] == 0)
                        {
                            res.com_types["pe"] += 1;
                            res.transactionList.Add(new transaction("OWN", pos_in_cache, i, address, CPU_lists[i].CPU.cache.memory[pos_in_cache][2]));
                            CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 4;
                        }//all ready in owned, no change needed
                        else if (CPU_lists[i].CPU.cache.memory[pos_in_cache][0] == 4)
                        {
                            res.transactionList.Add(new transaction("OWN", pos_in_cache, i, address, CPU_lists[i].CPU.cache.memory[pos_in_cache][2]));
                            CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 4;
                        }//all ready in shared, ignore
                        else if (CPU_lists[i].CPU.cache.memory[pos_in_cache][0] == 2)
                        {
                            if (amount_of_shared == 0)
                            {
                                res.com_types["pe"] += 1;
                            }
                            amount_of_shared++;
                            res.transactionList.Add(new transaction("SHARED", pos_in_cache, i, address, CPU_lists[i].CPU.cache.memory[pos_in_cache][2]));
                            CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 2;
                        }
                        //any other case, pass to shared.
                        else{
                            res.transactionList.Add(new transaction("SHARED", pos_in_cache, i, address, CPU_lists[i].CPU.cache.memory[pos_in_cache][2]));
                            res.com_types["pe"] += 1;
                            CPU_lists[i].CPU.cache.memory[pos_in_cache][0] = 2;
                        }
                    }
                }
            }
            return res;
        }

        public transaction write_to_memory(int address, int value, int cpu_id)
        {
            List<transaction> res = new List<transaction>();
            if (Report_counts.ContainsKey("WriteReq"))
            {
                Report_counts["WriteReq"] += 1;
            }
            MoesiMemory.Instance.write_to_address(address, value);
            return new transaction("WRITE_REQ", -1, cpu_id, address, value);
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
            foreach (MoesiCPU_activity cpu_struct in CPU_lists)
            {
                int cache_pos = 0;
                if (cpu_id != cpu_struct.CPU.id)
                {
                    foreach (var cache_mem in cpu_struct.CPU.cache.memory)
                    {
                        if (cache_mem[1] == address && cache_mem[0] != 3)
                        {
                            if (Report_counts.ContainsKey("INV"))
                            {
                                Report_counts["INV"] += 1;
                            }
                            tracker.com_types["pe"] += 1;
                            cache_mem[0] = 3;
                            tracker.transactionList.Add(new transaction("INV", cache_pos, cpu_struct.CPU.id, address, cache_mem[2]));
                            break;
                        }
                        cache_pos++;
                    }
                }
            }
            return tracker;
        }

        public void generate_inst()
        {
            Random rnd = new Random();
            int tot_inst = 30;
            CPUinstrucction1 = new List<string>();
            while (CPUinstrucction1.Count < tot_inst)
            {
                int inst = rnd.Next(0, 3);
                //read
                if (inst == 0)
                {
                    CPUinstrucction1.Add($"read {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//write
                else if (inst == 1)
                {
                    CPUinstrucction1.Add($"write {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//inq
                else
                {
                    CPUinstrucction1.Add($"increment {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }
            }
            CPUinstrucction2 = new List<string>();
            while (CPUinstrucction2.Count < tot_inst)
            {
                int inst = rnd.Next(0, 3);
                //read
                if (inst == 0)
                {
                    CPUinstrucction2.Add($"read {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//write
                else if (inst == 1)
                {
                    CPUinstrucction2.Add($"write {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//inq
                else
                {
                    CPUinstrucction2.Add($"increment {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }
            }
            CPUinstrucction3 = new List<string>();
            while (CPUinstrucction3.Count < tot_inst)
            {
                int inst = rnd.Next(0, 3);
                //read
                if (inst == 0)
                {
                    CPUinstrucction3.Add($"read {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//write
                else if (inst == 1)
                {
                    CPUinstrucction3.Add($"write {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//inq
                else
                {
                    CPUinstrucction3.Add($"increment {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }
            }
        }

        public Transaction_tracker run_all()
        {
            Report_counts = new Dictionary<string, int>();
            Report_counts.Add("ReadReq", 0);
            Report_counts.Add("WriteReq", 0);
            Report_counts.Add("INV", 0);
            var temp = new Transaction_tracker();
            for (int i = 0; i < 30; i++)
            {
                CPU_lists[0].CPU.exec_inst = true;
                CPU_lists[1].CPU.exec_inst = true;
                CPU_lists[2].CPU.exec_inst = true;

                Thread.Sleep(100);
                temp.transactionList.AddRange(response[0].transactionList);
                temp.transactionList.AddRange(response[1].transactionList);
                temp.transactionList.AddRange(response[2].transactionList);

                temp.cost += response[0].cost;
                temp.cost += response[1].cost;
                temp.cost += response[2].cost;

                temp.com_types["pe"] += response[0].com_types["pe"];
                temp.com_types["cache"] += response[0].com_types["cache"];
                temp.com_types["memory"] += response[0].com_types["memory"];

                temp.com_types["pe"] += response[1].com_types["pe"];
                temp.com_types["cache"] += response[1].com_types["cache"];
                temp.com_types["memory"] += response[1].com_types["memory"];

                temp.com_types["pe"] += response[2].com_types["pe"];
                temp.com_types["cache"] += response[2].com_types["cache"];
                temp.com_types["memory"] += response[2].com_types["memory"];
            }
            temp.updateCost();
            return temp;
        }

        public Transaction_tracker run_all(bool cpu1, bool cpu2, bool cpu3)
        {
            var temp = new Transaction_tracker();
            for (int i = 0; i < 30; i++)
            {
                if (cpu1)
                {
                    CPU_lists[0].CPU.exec_inst = true;
                }
                if (cpu2)
                {
                    CPU_lists[1].CPU.exec_inst = true;
                }
                if (cpu3)
                {
                    CPU_lists[1].CPU.exec_inst = true;
                }
                Thread.Sleep(100);

                if (cpu1)
                {
                    temp.transactionList.AddRange(response[0].transactionList);
                    temp.cost += response[0].cost;
                    temp.com_types["pe"] += response[0].com_types["pe"];
                    temp.com_types["cache"] += response[0].com_types["cache"];
                    temp.com_types["memory"] += response[0].com_types["memory"];
                }

                if (cpu2)
                {
                    temp.transactionList.AddRange(response[1].transactionList);
                    temp.cost += response[1].cost;
                    temp.com_types["pe"] += response[1].com_types["pe"];
                    temp.com_types["cache"] += response[1].com_types["cache"];
                    temp.com_types["memory"] += response[1].com_types["memory"];
                }

                if (cpu3)
                {
                    temp.transactionList.AddRange(response[2].transactionList);
                    temp.cost += response[2].cost;
                    temp.com_types["pe"] += response[2].com_types["pe"];
                    temp.com_types["cache"] += response[2].com_types["cache"];
                    temp.com_types["memory"] += response[2].com_types["memory"];
                }

            }
            temp.updateCost();
            return temp;
        }

        public Transaction_tracker run(bool cpu1, bool cpu2, bool cpu3)
        {
            var temp = new Transaction_tracker();
            if (cpu1)
            {
                CPU_lists[0].CPU.exec_inst = true;
                Thread.Sleep(100);
                temp.transactionList.AddRange(response[0].transactionList);
                temp.cost += response[0].cost;
                temp.com_types["pe"] += response[0].com_types["pe"];
                temp.com_types["cache"] += response[0].com_types["cache"];
                temp.com_types["memory"] += response[0].com_types["memory"];
            }

            if (cpu2)
            {
                CPU_lists[1].CPU.exec_inst = true;
                Thread.Sleep(100);
                temp.transactionList.AddRange(response[1].transactionList);
                temp.cost += response[1].cost;

                temp.com_types["pe"] += response[1].com_types["pe"];
                temp.com_types["cache"] += response[1].com_types["cache"];
                temp.com_types["memory"] += response[1].com_types["memory"];
            }

            if (cpu3)
            {
                CPU_lists[0].CPU.exec_inst = true;
                Thread.Sleep(100);
                temp.transactionList.AddRange(response[2].transactionList);
                temp.cost += response[2].cost;
                temp.com_types["pe"] += response[2].com_types["pe"];
                temp.com_types["cache"] += response[2].com_types["cache"];
                temp.com_types["memory"] += response[2].com_types["memory"];
            }
            temp.updateCost();
            return temp;
        }
    }
}
