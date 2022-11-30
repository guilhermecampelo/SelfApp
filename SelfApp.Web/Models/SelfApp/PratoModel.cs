using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
	public class PratoModel
	{
		#region Atributos

		public int Id { get; set; }

		[Required(ErrorMessage = "Preencha o nome.")]
		[MaxLength(60, ErrorMessage = "O nome pode ter no máximo 60 caracteres.")]
		public string Nome { get; set; }

		[Required(ErrorMessage = "Preencha a descrição.")]
		[MaxLength(120, ErrorMessage = "A descrição pode ter no máximo 120 caracteres.")]
		public string Descricao { get; set; }

		[Required(ErrorMessage = "Preencha o preço de venda.")]
		public decimal PrecoVenda { get; set; }

		public string Imagem { get; set; }

		public bool Ativo { get; set; }

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
					comando.CommandText = "select count(*) from prato";
					ret = (int)comando.ExecuteScalar();
				}
			}

			return ret;
		}

		private static PratoModel MontarPrato(SqlDataReader reader)
		{
			return new PratoModel
			{
				Id = (int)reader["id"],
				Nome = (string)reader["nome"],
				Descricao = (string)reader["descricao"],
				PrecoVenda = (decimal)reader["preco_venda"],
				Ativo = (bool)reader["ativo"],
				Imagem = (string)reader["imagem"],
			};
		}

		public static List<PratoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", bool somenteAtivos = false)
		{
			var ret = new List<PratoModel>();

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
						filtroWhere = string.Format(" where (lower(nome) like '%{0}%')", filtro.ToLower());
					}

					if (somenteAtivos)
					{
						filtroWhere = (string.IsNullOrEmpty(filtroWhere) ? " where" : " and") + "(ativo = 1)";
					}

					var paginacao = "";
					if (pagina > 0 && tamPagina > 0)
					{
						paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
							 pos > 0 ? pos - 1 : 0, tamPagina);
					}

					comando.Connection = conexao;
					comando.CommandText = "select * from prato" + filtroWhere 
						+ " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") + paginacao;

					var reader = comando.ExecuteReader();
					while (reader.Read())
					{
						ret.Add(MontarPrato(reader));
					}
				}
			}

			return ret;
		}

		public static PratoModel RecuperarPeloId(int id)
		{
			PratoModel ret = null;

			using (var conexao = new SqlConnection())
			{
				conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
				conexao.Open();
				using (var comando = new SqlCommand())
				{
					comando.Connection = conexao;
					comando.CommandText = "select * from prato where (id = @id)";

					comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

					var reader = comando.ExecuteReader();
					if (reader.Read())
					{
						ret = MontarPrato(reader);
					}
				}
			}

			return ret;
		}

		public static string RecuperarImagemPeloId(int id)
		{
			string ret = "";

			using (var conexao = new SqlConnection())
			{
				conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
				conexao.Open();
				using (var comando = new SqlCommand())
				{
					comando.Connection = conexao;
					comando.CommandText = "select imagem from prato where (id = @id)";

					comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

					var reader = comando.ExecuteReader();
					if (reader.Read())
					{
						ret = (string)reader["imagem"];
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
						comando.CommandText = "delete from prato where (id = @id)";

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
						comando.CommandText =
							 "insert into prato " +
							 "(nome, descricao, preco_venda, ativo, imagem) values " +
							 "(@nome, @descricao, @preco_venda, @ativo, @imagem); select convert(int, scope_identity())";

						comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
						comando.Parameters.Add("@descricao", SqlDbType.VarChar).Value = this.Descricao;
						comando.Parameters.Add("@preco_venda", SqlDbType.Decimal).Value = this.PrecoVenda;
						comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
						comando.Parameters.Add("@imagem", SqlDbType.VarChar).Value = this.Imagem;

						ret = (int)comando.ExecuteScalar();
					}
					else
					{
						comando.CommandText =
							 "update prato set nome = @nome, descricao = @descricao, preco_venda=@preco_venda, " +
							 "ativo=@ativo, imagem=@imagem where id = @id";

						comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
						comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
						comando.Parameters.Add("@descricao", SqlDbType.VarChar).Value = this.Descricao;
						comando.Parameters.Add("@preco_venda", SqlDbType.Decimal).Value = this.PrecoVenda;
						comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
						comando.Parameters.Add("@imagem", SqlDbType.VarChar).Value = this.Imagem;

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