namespace Alves_Bandeira.Models {
    public class Fatura {

        public enum EstadoFatura {
            Valida,
            Invalida,
            Todas
        }
        public enum TipoCombustivel {
            Gasolina,
            Diesel,
            Gpl
        }

        private Guid _guidFatura;

        public string GuidFatura {
            get { return _guidFatura.ToString(); }
        }

        public string Nif { get; set; }

        public decimal Preco { get; set; }


        public TipoCombustivel TipoComb { get; set; }


        public EstadoFatura Estado { get; set; }

        public Fatura(string guidFatura) {
            Guid.TryParse(guidFatura, out _guidFatura);
            Nif = "000000000";
            Preco = 0.0M;
            TipoComb = TipoCombustivel.Gasolina;
            Estado = EstadoFatura.Invalida;
        }

        public Fatura() {
            _guidFatura = Guid.Empty;
            Nif = "000000000";
            Preco = 0.0M;
            TipoComb = TipoCombustivel.Gasolina;
            Estado = EstadoFatura.Invalida;
        }



    }
}
