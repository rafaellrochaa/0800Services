using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControladorServicos.DominioServicoDownload
{
    public class DocumentoRetornoRotaLog
    {
        public byte[] Documento { get; set; }
        public string NomeDocumento { get; set; }
        public string ProposalId { get; set; }
    }
}