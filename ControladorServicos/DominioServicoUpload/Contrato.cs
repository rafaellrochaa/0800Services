using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControladorServicos.DominioServicoUpload
{
    public class Contrato
    {
        public int CodigoAf { get; set; }
        public string CodigoContrato { get; set; }
        public string NumeroOperacao { get; set; }
        public string NomeArquivo { get; set; }
        public byte[] Documento { get; set; }
        public int CodigoConvenio { get; set; }
    }
}
