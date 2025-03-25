using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace MOVEit.Service
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }


        private void serviceProcessInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                ServiceController theController = new ServiceController("moveitserv");
                theController.Start();
            }
            catch (Exception ex)
            {
                //TODO: log error to cloud service
            }

        }
    }
}
