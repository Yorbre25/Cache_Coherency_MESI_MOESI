namespace Proyecto_Arqui.Classes.Moesi
{
    public class MoesiCPU_activity
    {
        static int CPUS = 0;
        public MoesiCPU CPU { get; set; }
        public bool ran_inst { get; set; }
        public MoesiCPU_activity(bool activity)
        {
            CPU = new MoesiCPU(CPUS);
            CPUS += 1;
            ran_inst = activity;
        }
    }
}
