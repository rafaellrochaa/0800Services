using ControladorServicos.Infra.RepositorioAgilus;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace ControladorServicos.Infra.ServicoUpload
{
    class HttpAccessException : Exception
    {
        public string MensagemHttp { get; set; }
        public int StatusHttp { get; set; }
        public HttpAccessException(string Mensagem, int Status)
        {
            MensagemHttp = Mensagem;
            StatusHttp = Status;
        }
    }
    public class LoginException : Exception
    {
    }

    public static class CetelemUtil
    {
        private static string contrato { get; set; }
        private static string usuario { get; set; }
        private static string senha { get; set; }
        private static CookieContainer AuthCookie { get; set; }
        private static string ViewState { get; set; }
        public static void LogarSite()
        {
            string UrlLogin = @"https://autorizador.cetelem.com.br/Login/AC.UI.LOGIN.aspx";

            //Obtendo cookies que serão usados nas operações pós login
            HttpWebRequest requestLogin = (HttpWebRequest)WebRequest.Create(UrlLogin);

            //Preenchendo Headers
            requestLogin.Host = "autorizador.cetelem.com.br";
            requestLogin.KeepAlive = true;
            requestLogin.Headers["Cache-Control"] = "no-cache";
            requestLogin.Headers["Origin"] = @"https://autorizador.cetelem.com.br";
            requestLogin.Headers.Add("X-Requested-With", "XMLHttpRequest");
            requestLogin.Headers.Add("X-MicrosoftAjax", "Delta=true");
            requestLogin.UserAgent = @"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
            requestLogin.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
            requestLogin.Accept = @"*/*";
            requestLogin.Referer = @"https://autorizador.cetelem.com.br/Login/AC.UI.LOGIN.aspx";
            requestLogin.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            requestLogin.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
            ServicePointManager.Expect100Continue = false;
            requestLogin.CookieContainer = AuthCookie; //Preenchendo os cookies da request com os cookies de autenticação
            requestLogin.Credentials = CredentialCache.DefaultCredentials;
            requestLogin.AutomaticDecompression = DecompressionMethods.GZip;

            //Configurando post
            requestLogin.Method = "POST";
            string dadosPost = @"ScriptManager1=updLogin%7CLKentrar&__LASTFOCUS=&__EVENTTARGET=LKentrar&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUKLTcwNzA2NTE4Ng9kFgICBA9kFgICAw9kFgJmD2QWBgIFDw8WAh4DUkVYZWQWBgIDDw8WBB4MRXJyb3JNZXNzYWdlBT5PIGNvbnRlw7pkbyBkbyBjYW1wbyBVc3VhcmlvIMOpIGRlIHByZWVuY2hpbWVudG8gb2JyaWdhdMOzcmlvLh4HRW5hYmxlZGdkZAIEDw8WBh4JRm9yZUNvbG9yDB4EXyFTQgIMHglCYWNrQ29sb3IMZGQCBg8PFgQeFFZhbGlkYXRpb25FeHByZXNzaW9uZR8BZWRkAgkPDxYCHwBlZBYIAgMPDxYCHwEFPE8gY29udGXDumRvIGRvIGNhbXBvIFNlbmhhIMOpIGRlIHByZWVuY2hpbWVudG8gb2JyaWdhdMOzcmlvLmRkAgQPDxYGHwMMHwQCDB8FDGRkAgYPDxYEHwZlHwFlZGQCBw8PFggeEVZhbGlkYXRlRW1wdHlUZXh0aB4YQ2xpZW50VmFsaWRhdGlvbkZ1bmN0aW9uBSxGSUFqYXhXZWJDb250cm9sc192YWxpZGFfQ2FyYWN0ZXJlc1Blcmlnb3Nvcx8BBUBVbSBjb25qdW50byBkZSBjYXJhY3RlcmVzIHBvdGVuY2lhbG1lbnRlIHBlcmlnb3NvIGZvaSB1dGlsaXphZG8uHwJnZGQCDQ8PFgIeBFRleHQFDzE2LjExMDMuMi41MzA4NmRkGAEFHl9fQ29udHJvbHNSZXF1aXJlUG9zdEJhY2tLZXlfXxYCBQ9FVXN1YXJpbyRDa19TZWwFDUVTZW5oYSRDa19TZWxaLNf8QOaca%2BeJ3MHO1MGLsrogGUPV5QXL%2Fa%2Fzi9pSSQ%3D%3D&__VIEWSTATEGENERATOR=CD142C37&__EVENTVALIDATION=%2FwEdAAQQZq3gH8KARsn2otTDiwAsbNt%2BMgjXnbJmB4v1BYfk33w0rFU7IYq2eh9fOaQI3repOKvkl00zk26eK11VLfCG790QFcMYRpJhFxO6kdlm%2Buzbw3KYf0eP2KT3cnWydBc%3D&EUsuario%24CAMPO=36662814865&ESenha%24CAMPO=g*S0lg%3DX&__ASYNCPOST=true&";
            byte[] byteArray = Encoding.UTF8.GetBytes(dadosPost);
            requestLogin.ContentLength = byteArray.Length;

            //Inserindo na minha request os dados do formulário
            Stream dataStream = requestLogin.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = (HttpWebResponse)requestLogin.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                string responseHtml = reader.ReadToEnd();

                if ((responseHtml.Contains("updatePanel") || responseHtml.Contains("updLogin")) && (!responseHtml.Contains("pageRedirect")))
                {
                    throw new LoginException();
                }
            }
        }
        public static void AcessarFormUpload(string contrato)
        {
            //27- do get na URI é o código do refinanciamento, como só trabalharão refin será sempre 27. O / ano faz parte do número do contrato.
            HttpWebRequest requestConsulta = (HttpWebRequest)WebRequest.Create(@"https://portal.cetelem.com.br/Web/UploadDocs_monitorarSolicitacao.aspx?contrato=" + contrato + "&acao=A&operador=36662814865&login=UFUNCAO");
            requestConsulta.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            requestConsulta.Headers.Add("Accept-Encoding", "gzip, deflate, sdch, br");
            requestConsulta.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
            requestConsulta.UserAgent = @"Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.71 Safari/537.36";
            requestConsulta.Referer = @"https://autorizador.cetelem.com.br/MenuWeb/Cadastro/Documentos/UI.Armazenar.aspx";
            requestConsulta.CookieContainer = AuthCookie;

            requestConsulta.Headers.Add("Upgrade-Insecure-Requests: 1");
            requestConsulta.KeepAlive = true;
            requestConsulta.Host = "portal.cetelem.com.br";

            var response = requestConsulta.GetResponse();

            using (WebResponse respostaConsulta = requestConsulta.GetResponse())
            {
                StreamReader sr = new StreamReader(respostaConsulta.GetResponseStream(), Encoding.Default);
                string responseHtml = sr.ReadToEnd();

                ViewState = ExtrairTexto(responseHtml, "__VIEWSTATE", "value=\"", ">");
            }
        }
        private static void RealizarUploadGet(string contrato)
        {
            string uri = @"https://portal.cetelem.com.br/Web/UploadDocs_monitorarSolicitacao.aspx?contrato=" + contrato + "&acao=A&operador=36662814865&login=UFUNCAO";
            HttpWebRequest requestUploadGet = (HttpWebRequest)WebRequest.Create(uri);
            requestUploadGet.AutomaticDecompression = DecompressionMethods.GZip;
            requestUploadGet.Method = "GET";
            requestUploadGet.Accept = "text/html, application/xhtml+xml, image/jxr, */*";
            requestUploadGet.Headers.Add("Accept-Encoding", "gzip, deflate");
            requestUploadGet.Headers.Add("Accept-Language", "pt-BR,pt;q=0.5");
            requestUploadGet.UserAgent = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393";

            requestUploadGet.Host = "portal.cetelem.com.br";
            requestUploadGet.KeepAlive = true;

            var cookies = new CookieContainer();

            requestUploadGet.CookieContainer = cookies;
            var response = (HttpWebResponse)requestUploadGet.GetResponse();

            using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
            {
                string responseHtml = reader.ReadToEnd();
                ViewState = ExtrairTexto(responseHtml, "__VIEWSTATE", "value=\"", "\" />");
            }
            AuthCookie = cookies;

        }
        public static bool RealizarUploadPost(string contrato, string nomeArquivo, byte[] documento, int codigoConvenio)
        {
            bool UploadRealizado = false;
            string cboTipoDocumento = codigoConvenio != 3 ? "152500" : "121226"; //Quando convênio é exército, o tipo de documento é comprovante de averbação
            try
            {
                RealizarUploadGet(contrato);
                string uri = @"https://portal.cetelem.com.br/Web/UploadDocs_monitorarSolicitacao.aspx?contrato=" + contrato + "&acao=A&operador=36662814865&login=UFUNCAO";

                HttpWebRequest requestUpload = (HttpWebRequest)WebRequest.Create(uri);
                string boundaryString = "----SomeRandomText"; ;//Separador dos meus campos dentro do meu body post
                ServicePointManager.Expect100Continue = false;

                //Headers do meu http Post
                requestUpload.Method = "POST";
                requestUpload.Host = "portal.cetelem.com.br";
                requestUpload.KeepAlive = true;
                requestUpload.Headers["Cache-Control"] = "max-age=0";
                requestUpload.Headers["Origin"] = "https://portal.cetelem.com.br";
                requestUpload.Headers.Add("Upgrade-Insecure-Requests", "1");
                requestUpload.UserAgent = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.99 Safari/537.36";
                requestUpload.ContentType = "multipart/form-data; boundary=" + boundaryString;
                requestUpload.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                requestUpload.Referer = uri;
                requestUpload.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                requestUpload.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");

                //Adicionais
                requestUpload.AutomaticDecompression = DecompressionMethods.GZip;
                requestUpload.CookieContainer = AuthCookie;


                // Memory Stream para receber dados do post
                MemoryStream postDataStream = new MemoryStream();
                StreamWriter postDataWriter = new StreamWriter(postDataStream);

                // Incluindo dados na ordem correta no meu body http Post
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n"); //boundary string no corpo do post entram mais 2 caracteres, no caso "--"
                postDataWriter.Write("Content-Disposition: form-data; name=\"__EVENTTARGET\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"__EVENTARGUMENT\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"__VIEWSTATE\"\r\n\r\n" + ViewState);
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"__VIEWSTATEGENERATOR\"\r\n\r\n79338D7A");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"txtSenha\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"txtconfirmarSenha\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"cboTipoDoc\"\r\n\r\n" + cboTipoDocumento);

                // Incluindo contexto de arquivo 
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition:form-data;"
                                        + " name=\"{0}\";"
                                        + " filename=\"{1}\""
                                        + "\r\nContent-Type:{2}\r\n\r\n",
                                        "FileUpCaminho", //name
                                        nomeArquivo, //Nome do arquivo
                                        "application/pdf"); //Tipo anexo, fiz dessa forma caso seja necessário parametrizar
                postDataWriter.Flush();

                //Inserindo bytes do arquivo no body do post
                postDataStream.Write(documento, 0, documento.Length);

                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"btnAnexar\"\r\n\r\nAnexar");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho1\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho2\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho3\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho4\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho5\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho6\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho7\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"HiddenCaminho8\"\r\n\r\n");
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Flush();
                requestUpload.ContentLength = postDataStream.Length;

                using (Stream s = requestUpload.GetRequestStream())
                {
                    postDataStream.WriteTo(s); //Setando meus dados na minha request
                }
                postDataStream.Close();

                var response = (HttpWebResponse)requestUpload.GetResponse();

                using (var rawResponse = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string htmlResponse = rawResponse.ReadToEnd();
                    string retornoResponse = ExtrairTexto(htmlResponse, "<span id=\"lblRetorno\"", ">", "</span>");

                    UploadRealizado = retornoResponse.Contains("Arquivo armazenado com sucesso!");

                    if (!UploadRealizado) //Se ele entrar houve algum erro de upload especificado no form.
                    {
                        if (retornoResponse.Contains("Já existe um arquivo com esse nome!"))
                        {
                            throw new Exception("Erro: " + retornoResponse);
                        }
                        else
                            throw new Exception("Erro " + retornoResponse);
                    }
                }
            }
            catch (WebException we)
            {
                if (we.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new HttpAccessException(we.Message, Convert.ToInt16(((HttpWebResponse)we.Response).StatusCode));
                }
                else
                    throw new Exception("Erro durante requisição da página de upload do portal cetelem. Provavelmente houve um erro na rede. Tentar novamente mais tarde.\nDetalhe: " + we.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return UploadRealizado;
        }

        public static void Logout()
        {
            HttpWebRequest requestLogout = (HttpWebRequest)WebRequest.Create(@"https://autorizador.cetelem.com.br/MenuWeb/UI.MENU.aspx");
            requestLogout.Host = "autorizador.cetelem.com.br";
            requestLogout.KeepAlive = true;
            requestLogout.Headers["Cache-Control"] = "max-age=0";
            requestLogout.Headers["Origin"] = @"https://autorizador.cetelem.com.br";
            requestLogout.Headers.Add("Upgrade-Insecure-Requests: 1");
            requestLogout.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 Safari/537.36";
            requestLogout.ContentType = @"application/x-www-form-urlencoded";
            requestLogout.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*; q = 0.8";
            requestLogout.Referer = @"https://autorizador.cetelem.com.br/MenuWeb/UI.MENU.aspx";
            requestLogout.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            requestLogout.Headers.Add("Accept-Language", "pt-BR,pt;q=0.8,en-US;q=0.6,en;q=0.4");
            requestLogout.CookieContainer = AuthCookie;
            requestLogout.Method = "POST";

            string dadosPost = @"ctl00_UcComunicadosPopup_ToolkitScriptManager1_HiddenField=&__EVENTTARGET=ctl00%24lk_Sair&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUKLTcyMjEyMDgwMQ9kFgJmD2QWAgIDD2QWDgIHDw8WAh4EVGV4dAULMzY2NjI4MTQ4NjVkZAIJDw8WAh8ABRAxNS8xMi8yMDE2IDIxOjA3ZGQCCw9kFgICAQ8PFgIfAAUIMTUvMTIvMTZkZAINDw8WAh8ABQ8xNi4xMTAzLjIuNTMwODZkZAIPDw8WAh8ABQlTUE9TNTMwMTNkZAITD2QWAmYPZBYCZg9kFgICAw8WAh4HVmlzaWJsZWhkAhUPZBYCAgEPPCsADQIADxYCHgtfIURhdGFCb3VuZGdkDBQrAAUFDzA6MCwwOjEsMDoyLDA6MxQrAAIWDB8ABQhDYWRhc3Rybx4FVmFsdWUFCENhZGFzdHJvHgdFbmFibGVkZx4KU2VsZWN0YWJsZWgeCERhdGFQYXRoBSQxNmNhMTUxMS1lMjc5LTRjMDgtYTQ5OS1jZjlmM2VlMjlkNTQeCURhdGFCb3VuZGcUKwACBSMwOjAsMTowLDA6MCwxOjAsMDowLDE6MCwwOjAsMTowLDA6MBQrAAIWDB8ABQpEb2N1bWVudG9zHwMFCkRvY3VtZW50b3MfBGcfBWgfBgUkOTY1N2E2ZWMtOTVkMC00MmQzLTlkMDctNDgwYTc3MjE4NTNjHwdnFCsAAwUHMDowLDA6MRQrAAIWDh8ABQlBcm1hemVuYXIfAwUJQXJtYXplbmFyHgtOYXZpZ2F0ZVVybAUuL01lbnVXZWIvQ2FkYXN0cm8vRG9jdW1lbnRvcy9VSS5Bcm1hemVuYXIuYXNweB8EZx8FZx8GBS4vbWVudXdlYi9jYWRhc3Ryby9kb2N1bWVudG9zL3VpLmFybWF6ZW5hci5hc3B4HwdnZBQrAAIWDh8ABQlDb25zdWx0YXIfAwUJQ29uc3VsdGFyHwgFLi9NZW51V2ViL0NhZGFzdHJvL0RvY3VtZW50b3MvVUkuQ29uc3VsdGFyLmFzcHgfBGcfBWcfBgUuL21lbnV3ZWIvY2FkYXN0cm8vZG9jdW1lbnRvcy91aS5jb25zdWx0YXIuYXNweB8HZ2QUKwACFgwfAAUIQ29uc3VsdGEfAwUIQ29uc3VsdGEfBGcfBWgfBgUkNzI2MmRiOTctMGJiNy00YTUwLTkwZDEtYTcyYTQ1ZDViM2Q3HwdnFCsABgUTMDowLDA6MSwwOjIsMDozLDA6NBQrAAIWDh8ABR9Db25zdWx0YSBQcm9wb3N0YSBDUCBDb25zaWduYWRvHwMFH0NvbnN1bHRhIFByb3Bvc3RhIENQIENvbnNpZ25hZG8fCAVBL01lbnVXZWIvQ29uc3VsdGEvUHJvcG9zdGFDb25zaWduYWRvL1VJLlByb3Bvc3RhQ1BDb25zaWduYWRvLmFzcHgfBGcfBWcfBgVBL21lbnV3ZWIvY29uc3VsdGEvcHJvcG9zdGFjb25zaWduYWRvL3VpLnByb3Bvc3RhY3Bjb25zaWduYWRvLmFzcHgfB2dkFCsAAhYOHwAFA0NEQx8DBQNDREMfCAUpL01lbnVXZWIvQ29uc3VsdGEvQ0RDL1VJLkNvbnN1bHRhQ0RDLmFzcHgfBGcfBWcfBgUpL21lbnV3ZWIvY29uc3VsdGEvY2RjL3VpLmNvbnN1bHRhY2RjLmFzcHgfB2dkFCsAAhYOHwAFHExhbsOnYW1lbnRvIGRlIE9ic2VydmHDp8O1ZXMfAwUcTGFuw6dhbWVudG8gZGUgT2JzZXJ2YcOnw7Vlcx8IBT8vTWVudVdlYi9Db25zdWx0YS9MYW5jYW1lbnRvT2JzZXJ2YWNhby9VSS5MYW5jYW1lbnRvT2JzZXJ2LmFzcHgfBGcfBWcfBgU%2FL21lbnV3ZWIvY29uc3VsdGEvbGFuY2FtZW50b29ic2VydmFjYW8vdWkubGFuY2FtZW50b29ic2Vydi5hc3B4HwdnZBQrAAIWDh8ABRdMaWJlcmHDp8OjbyBkZSBDcsOpZGl0bx8DBRdMaWJlcmHDp8OjbyBkZSBDcsOpZGl0bx8IBT0vTWVudVdlYi9Db25zdWx0YS9MaWJlcmFjYW9DcmVkaXRvL1VJLkxpYmVyYWNhb0NyZWRpdG9GSS5hc3B4HwRnHwVnHwYFPS9tZW51d2ViL2NvbnN1bHRhL2xpYmVyYWNhb2NyZWRpdG8vdWkubGliZXJhY2FvY3JlZGl0b2ZpLmFzcHgfB2dkFCsAAhYOHwAFIENvbXByb3ZhbnRlIGRlIENvbXByYSBkZSBEw612aWRhHwMFIENvbXByb3ZhbnRlIGRlIENvbXByYSBkZSBEw612aWRhHwgFRi9NZW51V2ViL0NvbnN1bHRhL0xpYmVyYWNhb0NyZWRpdG8vVUkuTGliZXJhY2FvQ3JlZGl0b0NvbXByb3ZhbnRlLmFzcHgfBGcfBWcfBgVGL21lbnV3ZWIvY29uc3VsdGEvbGliZXJhY2FvY3JlZGl0by91aS5saWJlcmFjYW9jcmVkaXRvY29tcHJvdmFudGUuYXNweB8HZ2QUKwACFgwfAAULUmVsYXTDs3Jpb3MfAwULUmVsYXTDs3Jpb3MfBGcfBWgfBgUkMDQ5MWJjMmItYWVjMS00MjExLTk0NDgtNTkzNWQzYTg2N2Y0HwdnFCsAAwUHMDowLDA6MRQrAAIWDh8ABQ5Eb2N1bWVudGHDp8Ojbx8DBQ5Eb2N1bWVudGHDp8Ojbx8IBTUvTWVudVdlYi9SZWxhdG9yaW9zL0RvY3VtZW50YWNhby9VSS5Eb2N1bWVudGFjYW8uYXNweB8EZx8FZx8GBTUvbWVudXdlYi9yZWxhdG9yaW9zL2RvY3VtZW50YWNhby91aS5kb2N1bWVudGFjYW8uYXNweB8HZ2QUKwACFg4fAAULUmVsYXTDs3Jpb3MfAwULUmVsYXTDs3Jpb3MfCAUjL01lbnVXZWIvUmVsYXRvcmlvcy9TSUMvVUkuU0lDLmFzcHgfBGcfBWcfBgUjL21lbnV3ZWIvcmVsYXRvcmlvcy9zaWMvdWkuc2ljLmFzcHgfB2dkFCsAAhYMHwAFCVNlcnZpw6dvcx8DBQlTZXJ2acOnb3MfBGcfBWgfBgUkNGU4YmQ2NWMtNzc4ZC00NWVhLWIwNGItNDJlNTI1ODlmYTQxHwdnFCsAAgUbMDowLDA6MSwxOjEsMDoxLDE6MSwwOjEsMToxFCsAAhYOHwAFFEFsdGVyYcOnw6NvIGRlIHNlbmhhHwMFFEFsdGVyYcOnw6NvIGRlIHNlbmhhHwgFKS9NZW51V2ViL1NlcnZpY29zL0FsdFNlbmhhL1VJLkFsdFNlbi5hc3B4HwRnHwVnHwYFKS9tZW51d2ViL3NlcnZpY29zL2FsdHNlbmhhL3VpLmFsdHNlbi5hc3B4HwdnZGRkmkgkE8RNWXT85pQjcBkHAI6QXGaLycERWWeTRBYMcNs%3D&__VIEWSTATEGENERATOR=942F433E&__EVENTVALIDATION=%2FwEdAAOzQlyd1n15juW8iYt9dVk3oWgvDFfFMGacRINqpbPg0pkIt2PFOnpEZtpIcsKGDmTl1t6HRaE1MoYjq4asS84rarOD5UHfPVKHtK7fxHauCA%3D%3D";
            byte[] byteArray = Encoding.UTF8.GetBytes(dadosPost);
            requestLogout.ContentLength = byteArray.Length;

            //Inserindo na minha request os dados do formulário
            Stream dataStream = requestLogout.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var response = requestLogout.GetResponse();

            using (WebResponse respostaLogout = requestLogout.GetResponse())
            {
                StreamReader sr = new StreamReader(respostaLogout.GetResponseStream(), Encoding.Default);
                string responseHtml = sr.ReadToEnd();
            }
        }

        private static string ExtrairTexto(string textoCompleto, string valorAncora, string delimitadorInicioValorProcurado, string delimitadorFinalValorProcurado)
        {
            int PosicaoAncora = textoCompleto.IndexOf(valorAncora);

            int PosicaoInicioValor = textoCompleto.IndexOf(delimitadorInicioValorProcurado, PosicaoAncora) + (delimitadorInicioValorProcurado.Length);

            int tamanhoTextoRetorno = -PosicaoInicioValor + textoCompleto.IndexOf(delimitadorFinalValorProcurado, PosicaoInicioValor);

            return textoCompleto.Substring(PosicaoInicioValor, tamanhoTextoRetorno);
        }

        public static void GravarDocumentosAutorizadorCetelem(object sender)
        {
            if (Utils.AcaoDesativada("Upload"))
            {
                return;
            }

            bool resultado = false;
            try
            {
                foreach (var c in BancoDados.ContratosComDocumentosAEnviar())
                {
                    if (true)//validação
                    {
                        BancoDados.AtualizaFaseAndamentoUpload(c.CodigoAf, c.NomeArquivo);
                        try
                        {
                            resultado = CetelemUtil.RealizarUploadPost(c.NumeroOperacao, c.NomeArquivo, c.Documento, c.CodigoConvenio);
                        }
                        catch (Exception ex)
                        {
                            BancoDados.AtualizaFaseErroUpload(c.CodigoAf, c.NomeArquivo, ex.Message);
                        }

                        if (resultado)
                        {
                            BancoDados.AtualizaFaseUploadFinalizado(c.CodigoAf);
                        }
                    }
                    else
                        BancoDados.AtualizaFaseErroUpload(c.CodigoAf, c.NomeArquivo, "");
                }
            }
            catch (Exception)
            {
                //Voltar a fase original
            }
        }
    }
}
