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
            ControladorServicos.Infra.Utils.CarregarConfiguracaoDeAcesso();
            timer1 = new Timer(new TimerCallback(tasks => ExecutarTasksServices0800()), null, 0, 180000);
        }

        protected override void OnStop()
        {

        }
        public void ExecutarTasksServices0800()
        {
            //Verificação feita para evitar o conflito entre a mesma tarefa.
            if (!tarefasExecutando.Contains("Download"))
            {
                Task.Run(() =>
                {
                    tarefasExecutando.Add("Download");
                    RotalogUtil.GravarAnexosBaixadosRotalog(this);
                    tarefasExecutando.Remove("Download");
                });
            }
            if (!tarefasExecutando.Contains("Upload"))
            {
                Task.Run(() =>
                {
                    tarefasExecutando.Add("Upload");
                    CetelemUtil.GravarDocumentosAutorizadorCetelem(this);
                    tarefasExecutando.Remove("Upload");
                });
            }
        }
    }
}
