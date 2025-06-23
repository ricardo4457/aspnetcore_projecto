using Alves_Bandeira.Models;
using Microsoft.AspNetCore.Mvc;

namespace Alves_Bandeira.Controllers {
    public class FaturaController : GenericBaseController {
        public IActionResult Index(string id, string? sortOrder = null) {
            // Determina que tipo de faturas mostrar baseado no parâmetro id
            Fatura.EstadoFatura estadoListagem;

            switch (id) {
                case "0":
                    estadoListagem = Fatura.EstadoFatura.Invalida;  // Só inválidas
                    break;
                case "1":
                    estadoListagem = Fatura.EstadoFatura.Valida; // Só válidas
                    break;
                default:
                    estadoListagem = Fatura.EstadoFatura.Todas; // Todas as faturas
                    break;
            }

            // Define se a ordenação é ascendente ou descendente
            Boolean ascendente = true;
            if (!string.IsNullOrEmpty(sortOrder)) {
                if (sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase)) {
                    ascendente = true;
                }
                else if (sortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase)) {
                    ascendente = false;
                }
            }

            HelperFatura helper = new HelperFatura();

            // Se o utilizador é admin (papel=2), vê todas as faturas
            // Se é cliente normal, só vê as suas faturas
            string filtroNif = (_user.Papel == 2) ? "000000000" : _user.Nif;

            List<Fatura> lista = helper.list(filtroNif, estadoListagem, ascendente);

            // Prepara dados para mostrar na página
            ViewBag.NumeroFaturas = helper.getNrFaturas(filtroNif);
            ViewBag.TotalFaturas = helper.getTotalFaturas(filtroNif);
            ViewBag.User = _user;

            return View(lista);
        }
        public IActionResult Detalhe(string id) {
            HelperFatura helper = new HelperFatura();
            Fatura? prenda = helper.get(id);
            if (prenda != null) {
                return View(prenda);
            }
            else {
                // Se não encontra a fatura, volta para a lista
                return RedirectToAction("Index", "Fatura");
            }
        }


        [HttpGet]
        public IActionResult Criar() {
            if ( _user.Papel == 2) { // Só admins podem cria
                ViewBag.User = _user;
                return View();
            }
            return RedirectToAction("Index", "Fatura");
        }

        [HttpPost]
        public IActionResult Criar(Fatura fatura) {
            if ( _user.Papel == 2) {// Verifica se é admin

                // Validação do NIF: deve ter 9 dígitos
                if (string.IsNullOrEmpty(fatura.Nif) || fatura.Nif.Length != 9 || !fatura.Nif.All(char.IsDigit)) {
                    ViewBag.Erro = "NIF deve ter exatamente 9 dígitos numéricos";
                    ViewBag.User = _user;
                    return View(fatura);
                }
                // Validação do preço: não pode ser negativo ou igual a zero
                if (fatura.Preco <= 0) {
                    ViewBag.Erro = "O preço não pode ser negativo ou igual a zero";
                    ViewBag.User = _user;
                    return View(fatura);
                }

                if (ModelState.IsValid) {
                    HelperFatura helper = new HelperFatura();
                    helper.save(fatura);
                    return RedirectToAction("Index", "Fatura");
                }
                ViewBag.User = _user;
                return View(fatura);
            }
            return RedirectToAction("Index", "Fatura");
        }

        [HttpGet]
        public IActionResult Editar(string id) {
            if (_user.Papel == 2) {   // Verifica se é admin
                HelperFatura helper = new HelperFatura();
                Fatura? fatura2Edit = helper.get(id);
                if (fatura2Edit != null) {
                    return View(fatura2Edit);
                }
                else {
                    return RedirectToAction("Index", "Fatura");
                }
            }
            else {
                return RedirectToAction("Index", "Fatura");
            }
        }

        [HttpPost]
        public IActionResult Editar(string id, Fatura faturaPostada) {
            if (_user.Papel == 2) { // Verifica se é admin
                 // Validação do NIF: deve ter 9 dígitos
                if (string.IsNullOrEmpty(faturaPostada.Nif) || faturaPostada.Nif.Length != 9) {
                    ViewBag.Erro = "NIF deve ter exatamente 9 dígitos numéricos";
                    return View(faturaPostada);
                }
                // Validação do preço: não pode ser negativo ou igual a zero
                if (faturaPostada.Preco < 0) {
                    ViewBag.Erro = "O preço não pode ser negativo";
                    return View(faturaPostada);
                }
                if (ModelState.IsValid) {
                    HelperFatura helper = new HelperFatura();
                    helper.save(faturaPostada, id);
                    return RedirectToAction("Index", "Fatura");
                }
                else {
                    return View(faturaPostada);
                }
            }
            else {
                return RedirectToAction("Index", "Fatura");
            }
        }

        public IActionResult Eliminar(string id) {
            if (_user.Papel == 2) {    // Elimina uma fatura (só admins)
                HelperFatura helper = new HelperFatura();
                helper.delete(id);
            }
            return RedirectToAction("Index", "Fatura");
        }
    }
}
