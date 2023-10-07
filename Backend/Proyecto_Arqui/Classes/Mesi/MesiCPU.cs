using Proyecto_Arqui.Classes.Moesi;
using Proyecto_Arqui.Controllers;
using System.Diagnostics;

namespace Proyecto_Arqui.Classes.Mesi
{
    public class MesiCPU: CPU
    {

        public bool exec_inst;

        public int id;
        public MesiCPU(int _id)
        {
            list_register = new int[8];
            cache = new MesiCache();
            id = _id;
            instrucctions = new List<string>();
            instrucctions_executed = new List<string>();
            generate_inst();
            Debug.WriteLine($"MesiCPU:{id}, Starting thread");
            Console.WriteLine($"MesiCPU:{id}, Starting thread");
            Thread thread = new Thread(new ThreadStart(check_interconnect));
            thread.Start();
        }
        public void write(int address, int reg)
        {
            var write = cache.write_to_address(address, list_register[reg], id);
            write.updateCost();
            MesiInterconnect.set_execution(id ,write);
        }
        public void generate_inst()
        {
            this.instrucctions = new List<string>();
            while (instrucctions.Count < 30)
            {
                Random rnd = new Random();
                int inst = rnd.Next(0, 3);
                //read
                if (inst == 0)
                {
                    instrucctions.Add($"read {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//write
                else if (inst == 1)
                {
                    instrucctions.Add($"write {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }//inq
                else
                {
                    instrucctions.Add($"increment {rnd.Next(0, 8)} {rnd.Next(0, 16)}");
                }
            }
        }

        public Transaction_tracker read(int address, int reg)
        {
            var read = cache.get_from_address(address, id);
            read.updateCost();
            list_register[reg] = read.cache_resp[2];
            MesiInterconnect.set_execution(id, read);
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
                if (exec_inst != false && MesiInterconnect.Instance.inst_active == false && MesiInterconnect.Instance.active_cpu == -1)
                {
                    lock (MesiInterconnect.Instance)
                    {
                        Debug.WriteLine($"MesiCPU:{id}, blocking Interconnect and executing");
                        Console.WriteLine($"MesiCPU:{id}, blocking Interconnect and executing");
                        MesiInterconnect.Instance.inst_active = true;
                        MesiInterconnect.Instance.active_cpu = id;
                        if(instrucctions.Count <= 5)
                        {
                            reset();
                        }
                        execute_inst(instrucctions[0]);
                        instrucctions_executed.Add(instrucctions[0]);
                        instrucctions.RemoveAt(0);
                        exec_inst = false;
                        MesiInterconnect.Instance.active_cpu = -1;
                        MesiInterconnect.Instance.inst_active = false;
                    }
                }
                else
                {
                    TimeSpan interval = new TimeSpan(0, 0, 0, 0, 1);
                    Thread.Sleep(interval);
                }
            }
        }

        public override CPU copy()
        {
            var res = new MesiCPU(7);
            for (int i = 0; i < this.cache.memory.Count; i++)
            {
                res.cache.memory[i][0] = this.cache.memory[i][0];
                res.cache.memory[i][1] = this.cache.memory[i][1];
                res.cache.memory[i][2] = this.cache.memory[i][2];
            }
            for (int i = 0; i < this.list_register.Length; i++)
            {
                res.list_register[i] = this.list_register[i];
            }

            res.instrucctions = new List<string>();
            res.instrucctions.Add(this.instrucctions[0]);
            res.instrucctions.Add(this.instrucctions[1]);
            res.instrucctions.Add(this.instrucctions[2]);
            res.instrucctions.Add(this.instrucctions[3]);
            res.instrucctions_executed = new List<string>();
            return res;
        }

        public override void reset()
        {
            instrucctions = new List<string>();
            instrucctions_executed = new List<string>();
            generate_inst();
        }
    }
}
