using MOVEit.Platform;
using MOVEit.Platform.Models;
using MOVEit.Platform.Services;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MOVEit.UI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IDisposable
    {
        private readonly MOVEitContext _context;
        private readonly AuthService _authService;
        private readonly SyncService _syncService;

        public Login()
        {
            _context = new MOVEitContext();
            _authService = new AuthService();
            _syncService = new SyncService();
            CheckLogin();

            InitializeComponent();

        }

        private void CheckLogin()
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null) return;

            if (string.IsNullOrEmpty(user.Token)) return;

            var expireMinutes = (user.ExpiresIn - DateTime.Now).TotalMinutes;

            if (expireMinutes <= 0)
            {
                _context.Users.RemoveRange(_context.Users.ToList());
                _context.SaveChanges();
                return;
            };

            if (expireMinutes <= 5)
            {
                Task.WaitAll(new[] {Task.Run( async () => {
                    var response = await _authService.Authenticate(GrantType.RefreshToken, oldToken: user.RefreshToken);

                    user.Token = response.AccessToken;
                    user.ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn);
                    user.RefreshToken = response.RefreshToken;

                    await _context.SaveChangesAsync();
                })});
            }

            Task.WaitAll(new[] {Task.Run( async () => {
                FileService s = new FileService();
                await s.GetFolderContents(user.Token, 236041452);
            }) });

            

            Settings settings = new Settings();
            settings.Show();


            // Start the SyncService after login check
            _syncService.Start();

            Close();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TokenResponseDTO response = await _authService.Authenticate(GrantType.Password,
                   userName: usernameTb.Text,
                   password: passwordTb.Password);

                if (response != null)
                {
                    _context.Users.Add(new Platform.Entities.User
                    {
                        Token = response.AccessToken,
                        ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn),
                        RefreshToken = response.RefreshToken
                    });

                    await _context.SaveChangesAsync();

                    Settings settings = new Settings();
                    settings.Show();

                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please, check your credentials or contact service provider.");
            }
        }
    }
}
