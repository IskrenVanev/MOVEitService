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
    public partial class Login : Window
    {
        private readonly MOVEitContext _context;
        private readonly AuthService _authService;
        private readonly SyncService _syncService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// Checks user authentication status and starts the synchronization service if logged in.
        /// </summary>
        public Login()
        {
            _context = new MOVEitContext();
            _authService = new AuthService();
            _syncService = new SyncService();
            CheckLogin();

            InitializeComponent();

        }
        /// <summary>
        /// Checks if a user is already logged in and handles token expiration.
        /// If the token is expired or close to expiration, it refreshes the token.
        /// If authenticated, it opens the settings window and starts the synchronization service.
        /// </summary>
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

            _syncService.Start();

            Close();
        }
        /// <summary>
        /// Handles the login button click event.
        /// Authenticates the user using the provided credentials.
        /// If successful, stores the authentication token and opens the settings window.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">Event arguments.</param>
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
