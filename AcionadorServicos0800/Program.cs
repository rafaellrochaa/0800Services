﻿using System.ServiceProcess;

namespace AcionadorServicos0800
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            // Terceiro comment
            ServicesToRun = new ServiceBase[]
            {
                new Agilus0800()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
