using System.IO;

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
    }
}
