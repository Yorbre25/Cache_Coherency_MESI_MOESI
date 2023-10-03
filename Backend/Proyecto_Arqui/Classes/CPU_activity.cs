namespace Proyecto_Arqui.Classes
{
    public class CPU_activity
    {
        static int CPUS = 0;
        public CPU CPU { get; set; }
        public bool ran_inst { get; set; }
        public CPU_activity(bool activity)
        {
            CPU = new CPU(CPUS);
            CPUS += 1;
            ran_inst = activity;
        }
    }
}
