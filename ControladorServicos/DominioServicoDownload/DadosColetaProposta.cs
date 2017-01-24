using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
