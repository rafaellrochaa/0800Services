using ControladorServicos.DominioServicoDownload;
using ControladorServicos.DominioServicoUpload;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ControladorServicos.Infra.RepositorioAgilus
{
    public class BancoDados
    {
        private static readonly string
             DataSource = "192.168.5.12", //Alterar banco de dados
            InitialCatalog = "dbAgilus",
            UserID = "suporte_agilus",
            Password = "@gilus2016";

        public void AtualizarFaseAfAgilus(int fase, string codigoColeta, string ProposalId)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();
                try
                {
                    var cmd = new SqlCommand("execute pr_atualizar_fase_proposta @faf_codigo, @numero_contrato, @codigo_coleta", conexao);
                    cmd.Parameters.Add("@faf_codigo", SqlDbType.Int).Value = fase;
                    cmd.Parameters.Add("@numero_contrato", SqlDbType.VarChar, 50).Value = ProposalId;
                    cmd.Parameters.Add("@codigo_coleta", SqlDbType.VarChar, 50).Value = codigoColeta;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }

        public void GravarAnexo(byte[] anexo, string nomeArquivo, string ProposalId)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();
                try
                {
                    var cmd = new SqlCommand("execute pr_grava_anexo_rotalog @anexo, @nome_arquivo, @numero_proposta", conexao);

                    cmd.Parameters.Add("@anexo", SqlDbType.VarBinary, -1).Value = anexo;
                    cmd.Parameters.Add("@nome_arquivo", SqlDbType.VarChar, 200).Value = nomeArquivo;
                    cmd.Parameters.Add("@numero_proposta", SqlDbType.VarChar, 50).Value = ProposalId;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }
        }
        public DadosColetaProposta ObterColetasDownload()
        {
            List<IDictionary<string, string>> dadosAColetar = new List<IDictionary<string, string>>();

            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();

                try
                {
                    var cmd = new SqlCommand("execute pr_coletas_download", conexao);
                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        var dados = new Dictionary<string, string>();
                        dados.Add(dr[1].ToString(), dr[0].ToString());
                        dadosAColetar.Add(dados);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Ocorreu um problema durante a atualização das fases no banco de dados. Detalhe do erro: " + e.Message);
                }
            }

            return new DadosColetaProposta(dadosAColetar);
        }
        public void AtualizaFaseErroDownload(string codigoContrato, string erroDownload)
        {
            SqlConnection conexao = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password));

            using (conexao)
            {
                conexao.Open();
                var cmd = new SqlCommand("execute pr_atualiza_fase_download_erro @codigo_contrato, @erro_download", conexao);
                cmd.Parameters.Add("@codigo_contrato", SqlDbType.VarChar, 50).Value = codigoContrato;
                cmd.Parameters.Add("@erro_download", SqlDbType.VarChar, 1000).Value = erroDownload;
                cmd.ExecuteNonQuery();
            }
        }

        //Métodos do serviço de upload
        public static List<Contrato> ContratosComDocumentosAEnviar()
        {
            using (IDbConnection db = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password)))
            {
                return db.Query<Contrato>("execute pr_documentos_para_cetelem", null, null, true, 0, null).ToList();
            }
        }

        public static void AtualizaFaseAndamentoUpload(int codigoAf, string nomeArquivo)
        {
            var p = new DynamicParameters();
            p.Add("@af_codigo", codigoAf);
            p.Add("@nome_arquivo", nomeArquivo);

            using (IDbConnection db = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password)))
            {
                db.Execute("pr_atualiza_fase_upload_cetelem_em_andamento", p, commandType: CommandType.StoredProcedure);
            }
        }

        public static void AtualizaFaseUploadFinalizado(int codigoAf)
        {
            var p = new DynamicParameters();
            p.Add("@af_codigo", codigoAf);

            using (IDbConnection db = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password)))
            {
                db.Execute("pr_atualiza_fase_upload_cetelem_finalizado", p, commandType: CommandType.StoredProcedure);
            }
        }

        public static void AtualizaFaseErroUpload(int codigoAf, string nomeArquivo, string erroPortal)
        {
            var p = new DynamicParameters();
            p.Add("@af_codigo", codigoAf);
            p.Add("@nome_arquivo", nomeArquivo);
            p.Add("@erro_portal", erroPortal);

            using (IDbConnection db = new SqlConnection(String.Format("Data Source= {0}; Initial Catalog={1}; User ID={2}; Password={3}", DataSource, InitialCatalog, UserID, Password)))
            {
                db.Execute("pr_atualiza_fase_upload_erro", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
