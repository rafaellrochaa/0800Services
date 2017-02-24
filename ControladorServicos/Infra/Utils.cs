using ControladorServicos.Infra.Geral;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ControladorServicos.Infra
{
    public static class Utils
    {
        public static bool AcaoDesativada(string nomeServicoVerificacao)
        {
            //Validar arquivo de configuração, caso exista o nome do serviço no arquivo, retorna true

            /* Nomes dos serviços até o momento
             * Download (Documentos Rotalog)
             * Upload (Documentos Cetelem)
             * Cancelamento (Serviço que valida cancelamento de coletas.
             */
            StreamReader sr = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory.ToString()+"/config.txt");
            
            return sr.ReadToEnd().Contains(nomeServicoVerificacao);
        }

        public DadosAcesso LerConfiguracao()
        {
            string arquivo = getArquivo();
            DadosAcesso dados = new DadosAcesso();

            XmlDocument xml = new XmlDocument();
            xml.Load(arquivo);
            dados.Servidor = xml.SelectSingleNode("/Configuracao/Servidor/Endereco").InnerText;
            dados.BancoDeDados = xml.SelectSingleNode("/Configuracao/Servidor/BancoDeDados").InnerText;
            dados.Usuario = xml.SelectSingleNode("/Configuracao/Servidor/Usuario").InnerText;
            dados.Senha = xml.SelectSingleNode("/Configuracao/Servidor/Senha").InnerText;

            ValidarArquivo();
        }

        public void ValidarArquivo()
        {
            string arquivo = getArquivo();

            if (!File.Exists(arquivo))
            {
                throw new Exception();
            }
            
            DadosAcesso dados = new DadosAcesso();
            XmlDocument xml = new XmlDocument();
            dados.Servidor = xml.SelectSingleNode("/Configuracao/Servidor/Endereco").InnerText;
            dados.BancoDeDados = xml.SelectSingleNode("/Configuracao/Servidor/BancoDeDados").InnerText;
            dados.Usuario = xml.SelectSingleNode("/Configuracao/Servidor/Usuario").InnerText;
            dados.Senha = xml.SelectSingleNode("/Configuracao/Servidor/Senha").InnerText;

            if(String.IsNullOrEmpty(dados.Servidor) || String.IsNullOrEmpty(dados.BancoDeDados) || 
                String.IsNullOrEmpty(dados.Usuario) || String.IsNullOrEmpty(dados.Senha))
            {

            }

            }
        }

        private static string getArquivo()
        {
            string arquivo = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + Path.DirectorySeparatorChar + "conexao.xml";
            arquivo = arquivo.Substring(6, arquivo.Length - 6);

            return arquivo;
        }
    }
}
