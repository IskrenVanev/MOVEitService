using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MOVEit.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MOVEitSyncService");

            if (!Directory.Exists(dbPath))
                Directory.CreateDirectory(dbPath);

            AppDomain.CurrentDomain.SetData("DataDirectory", dbPath);
        }
    }
}
