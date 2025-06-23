namespace Alves_Bandeira.Models {
    public class User {
        public Guid GuidUser { get; set; }
        public string Nome { get; set; }
        public string Nif { get; set; }
        public int Papel { get; set; }
        public string Password { get; set; }
    }
}
