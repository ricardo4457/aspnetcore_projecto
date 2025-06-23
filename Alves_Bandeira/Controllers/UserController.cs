using Alves_Bandeira.Models;
using Microsoft.AspNetCore.Mvc;

namespace Alves_Bandeira.Controllers {
    public class UserController : Controller {
        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User userEnviado) {
            HelperUser helper = new HelperUser();
            // Validação do NIF
            if (string.IsNullOrEmpty(userEnviado.Nif) || userEnviado.Nif.Length != 9) {
                ViewBag.Erro = "NIF deve ter exatamente 9 dígitos numéricos";
                return View();
            }
            // Validação da password
            if (string.IsNullOrEmpty(userEnviado.Password)) {
                ViewBag.Erro = "Password é obrigatória";
                return View();
            }

            // Tenta fazer login
            User userAutenticado = helper.Login(userEnviado.Nif, userEnviado.Password);

            if (userAutenticado.Papel > 0) { // Login com sucesso
            // Guarda utilizador na sessão
                string json = helper.SerializeUser(userAutenticado);
                HttpContext.Session.SetString("user", json);
                return RedirectToAction("Index", "Fatura");
            }
            // Login falhou
            ViewBag.Erro = "Credenciais inválidas";
            return View();
        }

        [HttpGet]
        public IActionResult Logout() {
            HelperUser helper = new HelperUser();

            helper.Logout(HttpContext.Session); // Remove da sessão
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Registar() {
            return View();
        }

        [HttpPost]
        public IActionResult Registar(User novoUser) {

            HelperUser helper = new HelperUser();
            // Validação do NIF: 9 dígitos numéricos
            if (string.IsNullOrEmpty(novoUser.Nif) || novoUser.Nif.Length != 9 || !novoUser.Nif.All(char.IsDigit)) {
                ViewBag.Erro = "NIF deve ter exatamente 9 dígitos numéricos";
                return View(novoUser);
            }
            // Validação da password: mínimo 6 caracteres
            if (string.IsNullOrEmpty(novoUser.Password) || novoUser.Password.Length < 6) {
                ViewBag.Erro = "Password deve ter no mínimo 6 caracteres";
                return View(novoUser);
            }

            // Tenta registar o utilizador
            Boolean sucesso = helper.RegistarUser(novoUser);

            if (sucesso) {
                return RedirectToAction("Login");
            }

            return View(novoUser);
        }
    }
}
