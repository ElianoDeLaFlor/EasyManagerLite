namespace EasyManagerLibrary
{
    public class Module
    {
        public int Id { get; set; }
        public string Libelle { get; set; }

        public Module(){}

        public Module(int id,string lib)
        {
            Id = id;
            Libelle = lib;
          
        }
    }
}
