using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
	public class BandejaModel
	{
		#region Atributos

		public int Id { get; set; }
		public string Nome { get; set; }
		public TamanhoBandeja TamanhoBandeja { get; set; }
		public decimal TotalBandeja { get; set; }
		public int IdPedido { get; set; }

		#endregion

		#region Metodos



		#endregion
	}
}