using ControladorServicos.Infra;
using ControladorServicos.Infra.ServicoRotalog;
using ControladorServicos.Infra.ServicoUpload;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace AcionadorServicos0800
{
    public partial class Agilus0800 : ServiceBase
    {
        Timer timer1;
        List<string> tarefasExecutando = new List<string>();
        public Agilus0800()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Utils.CarregarConfiguracaoDeAcesso();
            timer1 = new Timer(new TimerCallback(tasks => ExecutarTasksServices0800()), null, 0, 60000);
        }

        protected override void OnStop()
        {
            tarefasExecutando.Clear();
        }
        public void ExecutarTasksServices0800()
        {
            //Verificação feita para evitar o conflito entre a mesma tarefa.
            if (!tarefasExecutando.Contains("Download"))
            {
                Task.Run(() =>
                {
                    try
                    {
                        tarefasExecutando.Add("Download");
                        RotalogUtil.GravarAnexosBaixadosRotalog();
                    }
                    finally
                    {
                        tarefasExecutando.Remove("Download");
                    }
                });
            }
            if (!tarefasExecutando.Contains("Upload"))
            {
                Task.Run(() =>
                {
                    try
                    {
                        tarefasExecutando.Add("Upload");
                        CetelemUtil.GravarDocumentosAutorizadorCetelem();
                    }
                    finally
                    {
                        tarefasExecutando.Remove("Upload");
                    }
                });
            }
        }
    }
}
