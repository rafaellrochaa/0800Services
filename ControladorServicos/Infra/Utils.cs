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
            StreamReader sr = new StreamReader(System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "/config.txt");

            return sr.ReadToEnd().Contains(nomeServicoVerificacao);
        }

        public static void CarregarConfiguracaoDeAcesso()
        {
            try
            {
                preencherDadosAcesso();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static DadosAcesso preencherDadosAcesso()
        {
            var xml = new XmlDocument();
            var dados = new DadosAcesso();
            xml.Load(getArquivo());
            lerDadosXml(xml, dados);

            return dados;
        }
        private static void lerDadosXml(XmlNode xml, DadosAcesso dados)
        {
            dados.Servidor = xml.SelectSingleNode("/Configuracao/Servidor/Endereco").InnerText;
            dados.BancoDeDados = xml.SelectSingleNode("/Configuracao/Servidor/BancoDeDados").InnerText;
            dados.Usuario = xml.SelectSingleNode("/Configuracao/Servidor/Usuario").InnerText;
            dados.Senha = xml.SelectSingleNode("/Configuracao/Servidor/Senha").InnerText;

            if (String.IsNullOrEmpty(dados.Servidor) || String.IsNullOrEmpty(dados.BancoDeDados) ||
                String.IsNullOrEmpty(dados.Usuario) || String.IsNullOrEmpty(dados.Senha))
            {
                throw new Exception("Dados de acesso inválidos.");
            }
        }
        private static string getArquivo()
        {
            string arquivo = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + Path.DirectorySeparatorChar + "conexao.xml";
            arquivo = arquivo.Substring(6, arquivo.Length - 6);

            if (!File.Exists(arquivo))
            {
                throw new Exception("Não foi encontrado nenhum arquivo de configuração para acesso ao banco de dados. Contate o administrador do sistema.");
            }
            return arquivo;
        }
    }
}
