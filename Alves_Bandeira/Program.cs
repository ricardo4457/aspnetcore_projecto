using Alves_Bandeira.Models;
using System.Threading;

namespace Alves_Bandeira
{

    public class Program
    {

        public static string Conetor = "";
        public static string SmtpIP = "";
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(20));
            builder.Services.AddMvc();
            var config = builder.Configuration.GetSection("Configuracao").Get<Configuracao>();
            Conetor = config.Conexao;
            SmtpIP = config.SmtpIP;

            var app = builder.Build();

            app.UseSession();
            app.UseStaticFiles();
            // Rotas da aplicação
            app.MapControllerRoute(
                           name: "default",
                           pattern: "{controller=Home}/{action=Index}/{id?}"
             );
            app.MapControllerRoute(
                          name: "default",
                          pattern: "{controller=Fatura}/{action=Index}/{id?}"
            );
            app.Run();
        }
    }
}
