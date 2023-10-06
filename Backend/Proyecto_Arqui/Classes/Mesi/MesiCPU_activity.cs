namespace Proyecto_Arqui.Classes.Mesi
{
    public class MesiCPU_activity
    {
        static int CPUS = 0;
        public MesiCPU CPU { get; set; }
        public bool ran_inst { get; set; }
        public MesiCPU_activity(bool activity)
        {
            CPU = new MesiCPU(CPUS);
            CPUS += 1;
            ran_inst = activity;
        }
    }
}
