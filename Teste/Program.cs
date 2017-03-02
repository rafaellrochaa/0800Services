using ControladorServicos.Infra.ServicoRotalog;
using ControladorServicos.Infra;
using System;
using System.Collections.Generic;

namespace Teste
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> tarefasExecutando = new List<string>();

            ControladorServicos.Infra.Utils.CarregarConfiguracaoDeAcesso();
            RotalogUtil.GravarAnexosBaixadosRotalog(null);
            Console.Write("Serviço Executado");
            Console.ReadKey();
        }
    }
}
