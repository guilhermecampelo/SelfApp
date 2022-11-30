using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
	//[Authorize(Roles = "Gerente,Administrativo,Operador")]
	public class PratoController : Controller
	{		
		private const int _quantMaxLinhasPorPagina = 5;

		public ActionResult Index()
		{
			ViewBag.ListaTamPag = new SelectList(new int[] { _quantMaxLinhasPorPagina, 10, 15, 20 }, _quantMaxLinhasPorPagina);
			ViewBag.QuantMaxLinhasPorPagina = _quantMaxLinhasPorPagina;
			ViewBag.PaginaAtual = 1;

			var lista = PratoModel.RecuperarLista(ViewBag.PaginaAtual, _quantMaxLinhasPorPagina);
			var quant = PratoModel.RecuperarQuantidade();

			var difQuantPaginas = (quant % ViewBag.QuantMaxLinhasPorPagina) > 0 ? 1 : 0;
			ViewBag.QuantPaginas = (quant / ViewBag.QuantMaxLinhasPorPagina) + difQuantPaginas;

			return View(lista);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult PratoPagina(int pagina, int tamPag, string ordem)
		{
			var lista = PratoModel.RecuperarLista(pagina, tamPag, ordem: ordem);

			return Json(lista);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult RecuperarPrato(int id)
		{
			return Json(PratoModel.RecuperarPeloId(id));
		}

		[HttpPost]
		//[Authorize(Roles = "Gerente,Administrativo")]
		[ValidateAntiForgeryToken]
		public JsonResult ExcluirPrato(int id)
		{
			return Json(PratoModel.ExcluirPeloId(id));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public JsonResult SalvarPrato()
		{
			var resultado = "OK";
			var mensagens = new List<string>();
			var idSalvo = string.Empty;

			var nomeArquivoImagem = "";
			HttpPostedFileBase arquivo = null;
			if (Request.Files.Count > 0)
			{
				arquivo = Request.Files[0];
				nomeArquivoImagem = Guid.NewGuid().ToString() + ".jpg";
			}

			var model = new PratoModel()
			{
				Id = int.Parse(Request.Form["Id"]),
				Nome = Request.Form["Nome"],
				Descricao = Request.Form["Descricao"],
				PrecoVenda = decimal.Parse(Request.Form["PrecoVenda"]),
				Ativo = (Request.Form["Ativo"] == "true"),
				Imagem = nomeArquivoImagem
			};

			if (!ModelState.IsValid)
			{
				resultado = "AVISO";
				mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
			}
			else
			{
				try
				{
					var nomeArquivoImagemAnterior = "";
					if (model.Id > 0)
					{
						nomeArquivoImagemAnterior = PratoModel.RecuperarImagemPeloId(model.Id);
					}

					var id = model.Salvar();
					if (id > 0)
					{
						idSalvo = id.ToString();
						if (!string.IsNullOrEmpty(nomeArquivoImagem) && arquivo != null)
						{
							var diretorio = Server.MapPath("~/Content/Imgs");

							var caminhoArquivo = Path.Combine(diretorio, nomeArquivoImagem);
							arquivo.SaveAs(caminhoArquivo);

							if (!string.IsNullOrEmpty(nomeArquivoImagemAnterior))
							{
								var caminhoArquivoAnterior = Path.Combine(diretorio, nomeArquivoImagemAnterior);
								System.IO.File.Delete(caminhoArquivoAnterior);
							}
						}
					}
					else
					{
						resultado = "ERRO";
					}
				}
				catch (Exception)
				{
					resultado = "ERRO";
				}
			}

			return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
		}
	}
}