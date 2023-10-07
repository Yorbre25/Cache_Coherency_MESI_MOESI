using Proyecto_Arqui.Classes.Moesi;
using Proyecto_Arqui.Classes.Mesi;
using System.Transactions;
using System.Runtime.Serialization.Formatters.Binary;

namespace Proyecto_Arqui.Classes
{
    public class front_end_data
    {
        public List<transaction> Transition_request { get; set; }
        
        public Dictionary<string,int> Report_data { get; set; }

        public int[] initial_Ram { get; set; }

        public List<CPU> initial_CPU_list { get; set; }

        public int[] final_Ram { get; set; }

        public List<CPU> final_CPU_list { get; set; }
        public front_end_data(MoesiInterconnect moesi)
        {
            initial_Ram = new int[16];
            final_Ram = new int[16];
            for (int i = 0; i < moesi.RAM.memory.Length; i++)
            {
                initial_Ram[i] = moesi.RAM.memory[i];
            }
            initial_CPU_list = new List<CPU>();
            final_CPU_list = new List<CPU>();
            initial_CPU_list.Add(moesi.CPU_lists[0].CPU.copy());
            initial_CPU_list.Add(moesi.CPU_lists[1].CPU.copy());
            initial_CPU_list.Add(moesi.CPU_lists[2].CPU.copy());
        }
        
        public front_end_data(MesiInterconnect mesi)
        {
            initial_Ram = new int[16];
            final_Ram = new int[16];
            for (int i = 0; i < mesi.RAM.memory.Length; i++)
            {
                initial_Ram[i] = mesi.RAM.memory[i];
            }
            initial_CPU_list = new List<CPU>();
            final_CPU_list = new List<CPU>();
            List<MesiCPU_activity> temp = mesi.CPU_lists;
            initial_CPU_list.Add(temp[0].CPU.copy());
            initial_CPU_list.Add(temp[1].CPU.copy());
            initial_CPU_list.Add(temp[2].CPU.copy());
        }

        public void update_data(MesiInterconnect mesi, Transaction_tracker res)
        {
            Transition_request = res.transactionList;
            Report_data = (Dictionary<string, int>?)mesi.Report_counts;
            for (int i = 0; i < mesi.RAM.memory.Length; i++)
            {
                final_Ram[i] = mesi.RAM.memory[i];
            }
            List<MesiCPU_activity> temp = mesi.CPU_lists;
            final_CPU_list.Add(temp[0].CPU.copy());
            final_CPU_list.Add(temp[1].CPU.copy());
            final_CPU_list.Add(temp[2].CPU.copy());
        }
        public void update_data(MoesiInterconnect moesi, Transaction_tracker res)
        {
            Transition_request = res.transactionList;
            Report_data = (Dictionary<string, int>?)moesi.Report_counts;
            for(int i = 0; i< moesi.RAM.memory.Length; i++)
            {
                final_Ram[i] = moesi.RAM.memory[i];
            }
            List<MoesiCPU_activity> temp = moesi.CPU_lists;
            final_CPU_list.Add(temp[0].CPU.copy());
            final_CPU_list.Add(temp[1].CPU.copy());
            final_CPU_list.Add(temp[2].CPU.copy());
        }

    }

    
}
