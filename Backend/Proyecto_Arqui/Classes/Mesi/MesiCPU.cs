using Proyecto_Arqui.Controllers;
using System.Diagnostics;

namespace Proyecto_Arqui.Classes.Mesi
{
    public class MesiCPU
    {
        private int[] list_register;
        public MesiCache cache;
        public int id;
        public string instruction_to_execute;
        public MesiCPU(int _id)
        {
            list_register = new int[8];
            cache = new MesiCache();
            id = _id;
            Debug.WriteLine($"CPU:{id}, Starting thread");
            Console.WriteLine($"CPU:{id}, Starting thread");
            Thread thread = new Thread(new ThreadStart(check_interconnect));
            thread.Start();
            Debug.WriteLine($"thread surived: {thread.IsAlive}");
            Console.WriteLine($"thread surived: {thread.IsAlive}");
        }
        public void write(int address, int reg)
        {
            var write = cache.write_to_address(address, list_register[reg], id);
            write.updateCost();
            MesiController.set_execution(id ,write);

        }
        public Transaction_tracker read(int address, int reg)
        {
            var read = cache.get_from_address(address, id);
            read.updateCost();
            list_register[reg] = read.cache_resp[2];
            MesiController.set_execution(id, read);
            return read;
        }
        public void increment_reg(int reg)
        {
            list_register[reg]++;
        }
        public void execute_inst(string inst)
        {
            string[] inst_parsed = inst.Split(' ');
            string instruction_to_execute = inst_parsed[0];
            int reg_to_exec = short.Parse(inst_parsed[1]);
            int address_to_exec = short.Parse(inst_parsed[2]);
            if (string.Compare(instruction_to_execute, "read") == 0)
            {
                read(address_to_exec, reg_to_exec);
            }
            else if (string.Compare(instruction_to_execute, "write") == 0)
            {
                write(address_to_exec, reg_to_exec);
            }
            else if (string.Compare(instruction_to_execute, "increment") == 0)
            {
                increment_reg(reg_to_exec);
            }
        }

        private void check_interconnect()
        {
            while (true)
            {
                if (instruction_to_execute != null && MesiInterconnect.Instance.inst_active == false && MesiInterconnect.Instance.active_cpu == -1)
                {
                    lock (MesiInterconnect.Instance)
                    {
                        Debug.WriteLine($"CPU:{id}, blocking Interconnect and executing");
                        Console.WriteLine($"CPU:{id}, blocking Interconnect and executing");
                        MesiInterconnect.Instance.inst_active = true;
                        MesiInterconnect.Instance.active_cpu = id;
                        execute_inst(instruction_to_execute);
                        instruction_to_execute = null;
                        MesiInterconnect.Instance.active_cpu = -1;
                        MesiInterconnect.Instance.inst_active = false;
                    }

                }
                else
                {
                    Debug.WriteLine($"CPU:{id}, sleeping");
                    Console.WriteLine($"CPU:{id}, sleeping");
                    TimeSpan interval = new TimeSpan(0, 0, 2);
                    Thread.Sleep(interval);
                }
                
            }

        }

    }
}
