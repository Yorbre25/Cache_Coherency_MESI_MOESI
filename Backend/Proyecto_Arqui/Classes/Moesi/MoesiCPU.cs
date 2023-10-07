using Proyecto_Arqui.Classes.Mesi;
using Proyecto_Arqui.Controllers;
using System.Diagnostics;

namespace Proyecto_Arqui.Classes.Moesi
{
    public class MoesiCPU:CPU
    {
        public bool exec_inst;

        public int id;
        public MoesiCPU(int _id)
        {

            list_register = new int[8];
            cache = new MoesiCache();
            id = _id;
            
            instrucctions = new List<string>();
            instrucctions_executed = new List<string>();
            generate_inst();
            Thread thread = new Thread(new ThreadStart(check_interconnect));
            thread.Start();
        }
        public void write(int address, int reg)
        {
            var write = cache.write_to_address(address, list_register[reg], id);
            write.updateCost();
            MoesiInterconnect.set_execution(id, write);

        }
        public Transaction_tracker read(int address, int reg)
        {
            var read = cache.get_from_address(address, id);
            read.updateCost();
            list_register[reg] = read.cache_resp[2];
            MoesiInterconnect.set_execution(id, read);
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
                if (exec_inst != false && MoesiInterconnect.Instance.inst_active == false && MoesiInterconnect.Instance.active_cpu == -1)
                {
                    lock(MoesiInterconnect.Instance)
                    {
                        Debug.WriteLine($"MoesiCPU:{id}, blocking Interconnect and executing");
                        Console.WriteLine($"MoesiCPU:{id}, blocking Interconnect and executing");
                        MoesiInterconnect.Instance.inst_active = true;
                        MoesiInterconnect.Instance.active_cpu = id;
                        if (instrucctions.Count <= 5)
                        {
                            reset();
                        }
                        execute_inst(instrucctions[0]);
                        instrucctions_executed.Add(instrucctions[0]);
                        instrucctions.RemoveAt(0);
                        exec_inst = false;
                        MoesiInterconnect.Instance.active_cpu = -1;
                        MoesiInterconnect.Instance.inst_active = false;
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
            var res = new MoesiCPU(7);
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
            if (this.instrucctions_executed.Count == 0)
            {
                res.instrucctions.Add(this.instrucctions[0]);
                res.instrucctions.Add(this.instrucctions[1]);
                res.instrucctions.Add(this.instrucctions[2]);
                res.instrucctions.Add(this.instrucctions[3]);
            }
            else
            {
                res.instrucctions.Add(new string(this.instrucctions_executed[this.instrucctions_executed.Count - 1]));
                res.instrucctions.Add(this.instrucctions[0]);
                res.instrucctions.Add(this.instrucctions[1]);
                res.instrucctions.Add(this.instrucctions[2]);
            }

            res.instrucctions_executed = new List<string>();
            return res;
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
        public override void reset()
        {
            instrucctions = new List<string>();
            instrucctions_executed = new List<string>();
            generate_inst();
        }
    }
}
