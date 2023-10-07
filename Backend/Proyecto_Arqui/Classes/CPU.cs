namespace Proyecto_Arqui.Classes
{
    public abstract class CPU
    {
        public Cache cache;
        public int[] list_register;
        public List<string> instrucctions;
        public List<string> instrucctions_executed;

        public abstract void reset();
        public abstract CPU copy(); 
    }
}