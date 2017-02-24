using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControladorServicos.Infra.Geral
{
    public class DadosAcesso
    {
        public string Servidor { get; set; }
        public string BancoDeDados { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
