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
    /// This window allows users to configure and save their sync directory settings.
    /// </summary>
    public partial class Settings : Window
    {
        private readonly MOVEitContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// Loads user data from the database.
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            _context = new MOVEitContext();

            LoadUserData();
        }
        /// <summary>
        /// Loads the user data asynchronously and populates the sync directory field.
        /// </summary>
        private async void LoadUserData()
        {
            var user = await _context.Users.FirstOrDefaultAsync();

            if (user == null) return;

            syncDirectoryTb.Text = user.SyncDirectory;
        }
        /// <summary>
        /// Opens a folder browser dialog allowing the user to select a sync directory.
        /// Updates the user's sync directory in the UI and database.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the button click.</param>
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
        /// <summary>
        /// Saves the updated sync directory setting to the database asynchronously.
        /// Displays a message confirming the save action.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments for the button click.</param>
        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            await _context.SaveChangesAsync();
            MessageBox.Show("Diretory configuration saved successfully!");
        }
    }
}
