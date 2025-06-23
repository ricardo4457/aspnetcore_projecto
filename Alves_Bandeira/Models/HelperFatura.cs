using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;

namespace Alves_Bandeira.Models {
    public class HelperFatura : HelperBase {
        public List<Fatura> list(string? nif = null, Fatura.EstadoFatura estado = Fatura.EstadoFatura.Todas, Boolean ascendente = true) {
            DataTable dt = new DataTable();
            List<Fatura> saida = new List<Fatura>();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(ConetorHerdado);

            comando.CommandType = CommandType.StoredProcedure;
            comando.Connection = conexao;

            // Escolhe o procedimento correto para ordenação
            // Seleciona o procedimento baseado na ordenação
            if (ascendente == null) {
                comando.CommandText = "QFatura_List"; // Sem ordenação específica
            }
            else if (ascendente == true) {
                comando.CommandText = "QFatura_ListAsc"; // Ordenação ascendente
            }
            else {
                comando.CommandText = "QFatura_ListDesc"; // Ordenação descendente
            }
            // Adiciona filtros se necessário
            if (!string.IsNullOrEmpty(nif) && nif != "000000000") {
                comando.Parameters.AddWithValue("@Nif", nif);
            }
            else {
                comando.Parameters.AddWithValue("@Nif", DBNull.Value);
            }

            if (estado != Fatura.EstadoFatura.Todas) {
                comando.Parameters.AddWithValue("@Estado", estado);
            }
            else {
                comando.Parameters.AddWithValue("@Estado", DBNull.Value);
            }

            adapter.SelectCommand = comando;
            adapter.Fill(dt);

            foreach (DataRow linha in dt.Rows) {
                Fatura fatura = new Fatura(linha["guidFatura"].ToString());

                fatura.Nif = linha["Nif"].ToString();
                fatura.Preco = Convert.ToDecimal(linha["Preco"]);
                fatura.TipoComb = (Fatura.TipoCombustivel)Convert.ToByte(linha["TipoCombustivel"]);
                fatura.Estado = (Fatura.EstadoFatura)Convert.ToByte(linha["Estado"]); ;
                saida.Add(fatura);
            }
            return saida;
        }

        public void delete(string guidFatura2Del) {
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(ConetorHerdado);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Connection = conexao;
            comando.CommandText = "QFatura_Delete";
            comando.Parameters.AddWithValue("@GuidFatura", guidFatura2Del);
            conexao.Open();
            comando.ExecuteNonQuery();
            conexao.Close();
            conexao.Dispose();
        }

        public Fatura? get(string guidFatura) {
            DataTable dt = new DataTable();
            SqlDataAdapter telefone = new SqlDataAdapter();
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(ConetorHerdado);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Connection = conexao;
            comando.CommandText = "QFatura_Get";
            comando.Parameters.AddWithValue("@GuidFatura", guidFatura);

            telefone.SelectCommand = comando;
            telefone.Fill(dt);

            if (dt.Rows.Count == 1) {
                DataRow linha = dt.Rows[0];
                Fatura fatura = new Fatura("" + linha["guidFatura"].ToString());
                fatura.Nif = linha["Nif"].ToString();
                fatura.Preco = Convert.ToDecimal(linha["Preco"]);
                fatura.TipoComb = (Fatura.TipoCombustivel)Convert.ToByte(linha["TipoCombustivel"]);
                fatura.Estado = (Fatura.EstadoFatura)Convert.ToByte(linha["Estado"]);
                return fatura;
            }
            return null;
        }

        public Boolean save(Fatura faturaSent, string guidFatura = "") {
            Boolean result = false;
            Fatura? fatura2Save;
            string instrucaoSQL = "";

            // Se não tem GUID, é uma fatura nova
            if (guidFatura.IsNullOrEmpty()) {
                fatura2Save = new Fatura();

            }
            else {
                fatura2Save = get(guidFatura);
            }

            if (fatura2Save != null) {
                // Copia dados da fatura enviada
                fatura2Save.Nif = faturaSent.Nif;
                fatura2Save.Preco = faturaSent.Preco;
                fatura2Save.TipoComb = faturaSent.TipoComb;
                // Determina se é INSERT ou UPDATE
                if (fatura2Save.GuidFatura == Guid.Empty.ToString()) {
                    fatura2Save.Estado = Fatura.EstadoFatura.Invalida;
                    instrucaoSQL = "QFatura_Insert";
                }
                else {
                    fatura2Save.Estado = faturaSent.Estado;
                    instrucaoSQL = "QFatura_Update";
                
                }

                SqlCommand comando = new SqlCommand();
                SqlConnection conexao = new SqlConnection(ConetorHerdado);
                comando.CommandType = CommandType.StoredProcedure;
                comando.CommandText = instrucaoSQL;
                comando.Connection = conexao;
                // Adiciona parâmetros
                comando.Parameters.AddWithValue("@Nif", fatura2Save.Nif);
                comando.Parameters.AddWithValue("@Preco", fatura2Save.Preco);
                comando.Parameters.AddWithValue("@TipoComb", fatura2Save.TipoComb);
                comando.Parameters.AddWithValue("@Estado", fatura2Save.Estado);

                if (instrucaoSQL == "QFatura_Update") {
                    comando.Parameters.AddWithValue("@GuidFatura", fatura2Save.GuidFatura);
                }
                conexao.Open();
                comando.ExecuteNonQuery();
                conexao.Close();
                conexao.Dispose();
                result = true;
            }
            return result;
        }

        public int getNrFaturas(string? nif = null) {
            int nrFaturas = 0;
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(ConetorHerdado);
            try {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Connection = conexao;
                comando.CommandText = "QFatura_GetNumero";
                // Filtro por NIF se especificado
                if (!string.IsNullOrEmpty(nif) && nif != "000000000") {
                    comando.Parameters.AddWithValue("@Nif", nif);
                }
                else {
                    comando.Parameters.AddWithValue("@Nif", DBNull.Value);
                }

                conexao.Open();
                nrFaturas = Convert.ToInt32(comando.ExecuteScalar());
            }
            catch (Exception ex) {
                nrFaturas = 0; // Se há erro, retorna 0
            }
            conexao.Close();
            conexao.Dispose();
            return nrFaturas;
        }

        public decimal getTotalFaturas(string? nif = null) {
            decimal totalFaturas = 0;
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(ConetorHerdado);
            try {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Connection = conexao;
                comando.CommandText = "QFaturas_GetTotalPreco";
                // Filtro por NIF se especificado
                if (!string.IsNullOrEmpty(nif) && nif != "000000000") {
                    comando.Parameters.AddWithValue("@Nif", nif);
                }
                else {
                    comando.Parameters.AddWithValue("@Nif", DBNull.Value);
                }

                conexao.Open();
                totalFaturas = Convert.ToDecimal(comando.ExecuteScalar());
            }
            catch (Exception ex) {
                totalFaturas = 0.0M; // Se há erro, retorna 0
            }
            conexao.Close();
            conexao.Dispose();
            return totalFaturas;
        }


    }
}
