using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleEstoque.Web.Models;

namespace ControleEstoque.Web.Controllers
{
	public class PedidoController : Controller
	{
		private const int _quantMaxLinhasPorPagina = 5;

		public ActionResult Index()
		{
			ViewBag.Pratos = PratoModel.RecuperarLista(1, 9999);

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult PedidoPagina(int pagina, int tamPag, string ordem)
		{
			var lista = PedidoModel.RecuperarLista(pagina, tamPag, ordem: ordem);

			return Json(lista);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult RecuperarPais(int id)
		{
			return Json(PedidoModel.RecuperarPeloId(id));
		}

		[HttpPost]
		//[Authorize(Roles = "Gerente,Administrativo")]
		[ValidateAntiForgeryToken]
		public JsonResult ExcluirPedido(int id)
		{
			return Json(PedidoModel.ExcluirPeloId(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult SalvarPedido(PedidoModel model)
		{
			var resultado = "OK";
			var mensagens = new List<string>();
			var idSalvo = string.Empty;

			if (!ModelState.IsValid)
			{
				resultado = "AVISO";
				mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
			}
			else
			{
				try
				{
					var id = model.Salvar();
					if (id > 0)
					{
						idSalvo = id.ToString();
					}
					else
					{
						resultado = "ERRO";
					}
				}
				catch (Exception ex)
				{
					resultado = "ERRO";
				}
			}

			return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
		}
	}
}