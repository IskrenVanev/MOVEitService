using MOVEit.Platform;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MOVEit.UI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window, IDisposable
    {
        private readonly MOVEitContext _context;

        public Settings()
        {
            InitializeComponent();
            _context = new MOVEitContext();

            LoadUserData();
        }

        private async void LoadUserData()
        {
            var user = await _context.Users.FirstOrDefaultAsync();

            if (user == null) return;

            syncDirectoryTb.Text = user.SyncDirectory;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var user = await _context.Users.FirstOrDefaultAsync();

                    if (user == null) return;

                    user.SyncDirectory = dialog.SelectedPath;
                    syncDirectoryTb.Text = user.SyncDirectory;
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await _context.SaveChangesAsync();
            MessageBox.Show("Diretory configuration saved successfully!");
        }
    }
}
