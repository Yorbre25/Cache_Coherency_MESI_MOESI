namespace Proyecto_Arqui.Classes
{
    public abstract class CPU
    {
        public Cache cache;
        public int id;
        public string instruction_to_execute;
        public int[] list_register;

        public abstract CPU copy(); 
    }
}