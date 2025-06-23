using System;
using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace Alves_Bandeira.Models {
    public class HelperUser : HelperBase {

        public User SetGuest() {
            return new User {
                GuidUser = Guid.NewGuid(),
                Nome = "Visitante",
                Nif = "000000000",
                Papel = 0,
                Password = "admin123"
            };
        }

        public User Login(string nif, string password) {
            using var conexao = new SqlConnection(ConetorHerdado);
            using var comando = new SqlCommand("QUser_Login", conexao) {
                CommandType = CommandType.StoredProcedure
            };

            comando.Parameters.AddWithValue("@Nif", nif);
            comando.Parameters.AddWithValue("@Password", password);

            var adaptador = new SqlDataAdapter(comando);
            var dt = new DataTable();
            adaptador.Fill(dt);

            if (dt.Rows.Count == 1) {
                var linha = dt.Rows[0];
                return new User {
                    GuidUser = Guid.Parse(linha["GuidUser"].ToString()!),
                    Nome = linha["Nome"].ToString()!,
                    Nif = linha["Nif"].ToString()!,
                    Papel = Convert.ToInt32(linha["Papel"]),
                    Password = "" 
                };
            }

            return SetGuest();
        }

        public void Logout(ISession session) {
            session.Remove("user");
        }


        public Boolean RegistarUser(User user) {
            user.GuidUser = Guid.NewGuid();
            using var conexao = new SqlConnection(ConetorHerdado);
            using var comando = new SqlCommand("QUser_Registar", conexao) {
                CommandType = CommandType.StoredProcedure
            };

            comando.Parameters.AddWithValue("@GuidUser", user.GuidUser);
            comando.Parameters.AddWithValue("@Nome", user.Nome);
            comando.Parameters.AddWithValue("@Nif", user.Nif);
            comando.Parameters.AddWithValue("@Papel", 1);
            comando.Parameters.AddWithValue("@Password", user.Password);

            conexao.Open();
            int linhasAfetadas = comando.ExecuteNonQuery();
            return linhasAfetadas > 0;
        }

   
        public string SerializeUser(User user) {
            return JsonSerializer.Serialize(user);
        }

        public User? DeserializeUser(string json) {
            try {
                return JsonSerializer.Deserialize<User>(json);
            }
            catch {
                return null;
            }
        }


    }
}
