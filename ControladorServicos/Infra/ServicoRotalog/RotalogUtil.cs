using ControladorServicos.DominioServicoDownload;
using ControladorServicos.Infra.RepositorioAgilus;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;

namespace ControladorServicos.Infra.ServicoRotalog
{
    public static class RotalogUtil
    {
        public static void GravarAnexosBaixadosRotalog(/*object sender*/)
        {
            if (Utils.AcaoDesativada("Download"))
            {
                return;
            }
            
            var operacaoBancoDados = new BancoDados();
            DadosColetaProposta dadosDownloadDocumentos = operacaoBancoDados.ObterColetasDownload();

            List<string> coletasDuplicadas = dadosDownloadDocumentos._dados.SelectMany(x => x.Values).ToList();
            var coletas = coletasDuplicadas.Distinct();

            foreach (var codigoColeta in coletas)
            {
                List<string> propostas = dadosDownloadDocumentos._dados.Where(x => x.Values.Contains(codigoColeta)).SelectMany(v => v.Keys).ToList();
                if (propostas.Count > 0)
                {
                    var anexos = DownloadAnexosRotaLog(Convert.ToInt16(codigoColeta), propostas);

                    foreach (var anexo in anexos)
                    {
                        operacaoBancoDados.AtualizarFaseAfAgilus(116, codigoColeta, anexo.ProposalId); //Fase Mapeada no banco de dados como "Baixando documentos"
                        operacaoBancoDados.GravarAnexo(anexo.Documento, anexo.NomeDocumento, anexo.ProposalId);
                    }
                }
            }
        }
        private static List<DocumentoRetornoRotaLog> DownloadAnexosRotaLog(int codigoColeta, List<string> propostas)
        {
            var documentosBaixados = new List<DocumentoRetornoRotaLog>();

            DataTable DocumentosDownload = new Rotalog.WebServiceRotaSoapClient().ConsultaCaminhoDocumentos("USU00480", "ag12345", codigoColeta).Tables[0];

            if (DocumentosDownload.Columns.Contains("Tipo"))
            {
                throw new Exception(DocumentosDownload.Rows[0]["Descricao"].ToString());
            }
            else
            {
                foreach (DataRow anexo in DocumentosDownload.Rows)
                {
                    string nomeArquivo = Path.GetFileName(anexo["url_arquivo"].ToString());
                    string NumeroProposta = anexo["descricao"].ToString().Split(':').Last().Replace(".pdf", ""); //Obtendo o número da proposta no nome do arquivo.

                    if (propostas.Contains(NumeroProposta))
                    {
                        try
                        {
                            var documento = DownloadArquivo(anexo["url_arquivo"].ToString());
                            documentosBaixados.Add(new DocumentoRetornoRotaLog()
                            {
                                Documento = documento,
                                NomeDocumento = nomeArquivo,
                                ProposalId = NumeroProposta
                            });
                        }
                        catch (Exception e)
                        {
                            new BancoDados().AtualizaFaseErroDownload(NumeroProposta, e.Message);
                        }
                    }
                }
            }
            return documentosBaixados;
        }
        private static byte[] DownloadArquivo(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            //if (response.StatusCode == HttpStatusCode.OK ||
            //    response.StatusCode == HttpStatusCode.Moved ||
            //    response.StatusCode == HttpStatusCode.Redirect)
            {

                MemoryStream ms = new MemoryStream();
                using (Stream inputStream = response.GetResponseStream())
                {
                    inputStream.CopyTo(ms);
                    ms.Position = 0;
                }
                return ms.ToArray();
            }
        }
    }
}
