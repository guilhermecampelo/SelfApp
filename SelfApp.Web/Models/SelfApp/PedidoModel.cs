using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
	public class PedidoModel
	{
		#region Atributos

		public int Id { get; set; }
		public int IdCliente { get; set; }
		public decimal TotalPedido { get; set; }

		#endregion

		#region Metodos

		public static int RecuperarQuantidade()
		{
			var ret = 0;

			using (var conexao = new SqlConnection())
			{
				conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
				conexao.Open();
				using (var comando = new SqlCommand())
				{
					comando.Connection = conexao;
					comando.CommandText = "select count(*) from pedido where id_cliente = 1";
					ret = (int)comando.ExecuteScalar();
				}
			}

			return ret;
		}

		public static List<PedidoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "")
		{
			var ret = new List<PedidoModel>();

			using (var conexao = new SqlConnection())
			{
				conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
				conexao.Open();
				using (var comando = new SqlCommand())
				{
					var pos = (pagina - 1) * tamPagina;

					var filtroWhere = "";
					if (!string.IsNullOrEmpty(filtro))
					{
						filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
					}

					var paginacao = "";
					if (pagina > 0 && tamPagina > 0)
					{
						paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
							 pos > 0 ? pos - 1 : 0, tamPagina);
					}

					comando.Connection = conexao;
					comando.CommandText =
						 "select *" +
						 " from pedido" +
						 filtroWhere +
						 " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
						 paginacao;

					var reader = comando.ExecuteReader();
					while (reader.Read())
					{
						ret.Add(new PedidoModel
						{
							Id = (int)reader["id"],
							IdCliente = (int)reader["id_cliente"],
							TotalPedido = (decimal)reader["total_pedido"]
						});
					}
				}
			}

			return ret;
		}

		public static PedidoModel RecuperarPeloId(int id)
		{
			PedidoModel ret = null;

			using (var conexao = new SqlConnection())
			{
				conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
				conexao.Open();
				using (var comando = new SqlCommand())
				{
					comando.Connection = conexao;
					comando.CommandText = "select * from pedido where (id = @id)";

					comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

					var reader = comando.ExecuteReader();
					if (reader.Read())
					{
						ret = new PedidoModel
						{
							Id = (int)reader["id"],
							IdCliente = (int)reader["id_cliente"],
							TotalPedido = (decimal)reader["total_pedido"]
						};
					}
				}
			}

			return ret;
		}

		public static bool ExcluirPeloId(int id)
		{
			var ret = false;

			if (RecuperarPeloId(id) != null)
			{
				using (var conexao = new SqlConnection())
				{
					conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
					conexao.Open();
					using (var comando = new SqlCommand())
					{
						comando.Connection = conexao;
						comando.CommandText = "delete from pedido where (id = @id)";

						comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

						ret = (comando.ExecuteNonQuery() > 0);
					}
				}
			}

			return ret;
		}

		public int Salvar()
		{
			var ret = 0;

			var model = RecuperarPeloId(this.Id);

			using (var conexao = new SqlConnection())
			{
				conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
				conexao.Open();
				using (var comando = new SqlCommand())
				{
					comando.Connection = conexao;

					if (model == null)
					{
						comando.CommandText = "insert into pedido (id_cliente, total_pedido) values (@id_cliente, @total_pedido); select convert(int, scope_identity())";

						comando.Parameters.Add("@id_cliente", SqlDbType.Int).Value = this.IdCliente;
						comando.Parameters.Add("@total_pedido", SqlDbType.Decimal).Value = this.TotalPedido;

						ret = (int)comando.ExecuteScalar();
					}
					else
					{
						comando.CommandText = "update pais set id_cliente=@id_cliente, total_pedido=@total_pedido where id = @id";

						comando.Parameters.Add("@id_cliente", SqlDbType.Int).Value = this.IdCliente;
						comando.Parameters.Add("@total_pedido", SqlDbType.Decimal).Value = this.TotalPedido;
						comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

						if (comando.ExecuteNonQuery() > 0)
						{
							ret = this.Id;
						}
					}
				}
			}

			return ret;
		}

		#endregion
	}
}