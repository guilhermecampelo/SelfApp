using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
	public class DivisoriaModel
	{
		#region Atributos

		public int Id { get; set; }
		public string Nome { get; set; }		
		public TamanhoDiv TamanhoDiv { get; set; }
		public int IdPrato { get; set; }
		public int IdBandeja { get; set; }
		public int IdPedido { get; set; }

		#endregion

		#region Metodos



		#endregion
	}
}