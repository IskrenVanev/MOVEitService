using MOVEit.Platform;
using MOVEit.Platform.Entities;
using MOVEit.Platform.Models;
using MOVEit.Platform.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MOVEit.Service
{
    public partial class Service : ServiceBase
    {
        private readonly SyncService syncService;

        public Service()
        {
            InitializeComponent();
            syncService = new SyncService();
        }

        protected override void OnStart(string[] args)
        {
            syncService.Start();
        }

        protected override void OnStop()
        {
            syncService.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                syncService?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
