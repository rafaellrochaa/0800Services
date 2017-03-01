using System.Collections.Generic;

namespace ControladorServicos.DominioServicoDownload
{
    public class DadosColetaProposta
    {
        public DadosColetaProposta(List<IDictionary<string, string>> dadosDocumentoDownload)
        {
            _dados = dadosDocumentoDownload;
        }
        public List<IDictionary<string, string>> _dados { get; }
    }
}
