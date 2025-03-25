using MOVEit.Platform.Entities;
using MOVEit.Platform.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MOVEit.Platform.Services
{
    public class SyncService : IDisposable
    {
        private readonly AuthService _authService;
        private readonly MOVEitContext _context;
        private readonly FileService _fileService;

        private FileSystemWatcher _directoryWatcher;

        private Timer _loginTimer;

        public bool IsRunning { get; private set; }

        public bool IsSyncing { get; private set; }

        public User User { get; private set; }

        public SyncService()
        {
            _authService = new AuthService();
            _context = new MOVEitContext();
            _fileService = new FileService();
        }

        public void Start()
        {
            _loginTimer = new Timer(LoginTimer_Tick, null, 0, 30000);

            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private void LoginTimer_Tick(object obj)
        {
            if (!IsRunning) return;

            if (CheckAuth())
            {
                if (!string.IsNullOrEmpty(User.SyncDirectory))
                {
                    if (!IsSyncing)
                    {
                        IsSyncing = true;
                        SubscribeDirectory();
                    }
                }
            }
            else
            {
                if (IsSyncing)
                {
                    DetachWatcher();
                    User = null;
                    IsSyncing = false;
                }
            }
        }

        private void SubscribeDirectory()
        {
            //_directoryWatcher = new FileSystemWatcher(User.SyncDirectory);
            _directoryWatcher = new FileSystemWatcher(User.SyncDirectory)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _directoryWatcher.Changed += DirectoryWatcher_Changed;
           // _directoryWatcher.Changed += async (sender, e) => await DirectoryWatcher_Changed(sender, e);
            _directoryWatcher.EnableRaisingEvents = true;
        }

        public async void DirectoryWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (!File.GetAttributes(e.FullPath).HasFlag(FileAttributes.Directory))
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    // upload file 
                    try
                    {
                        CheckAuth();
                        var user = _context.Users.FirstOrDefault();
                        string token = user.Token;
                        string folderId = user.SyncDirectory;
                        var client = new RestClient($"https://testserver.moveitcloud.com/api/v1/{folderId}/files");
                        RestRequest req = new RestRequest();

                        req.AddHeader("Authorization", $"Bearer {token}");
                        req.AddFile("file", e.FullPath);
                        var response = await client.GetAsync(req);

                        if (!response.IsSuccessStatusCode)
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: Log error to server
                        throw;
                    }
                }
            }
            return;
        }

        private void DetachWatcher()
        {
            _directoryWatcher?.Dispose();
        }

        private bool CheckAuth()
        {
            var user = _context.Users.FirstOrDefault();

            if (user == null) return false;

            if (string.IsNullOrEmpty(user.Token)) return false;

            if ((user.ExpiresIn - DateTime.Now).TotalSeconds <= 0) return false;

            if ((user.ExpiresIn - DateTime.Now).Minutes <= 5)
                RefreshToken(user);

            User = user;

            return true;
        }

        private void RefreshToken(User user)
        {
            Task.WaitAll(new[] {Task.Run( async () => {
                    var response = await _authService.Authenticate(GrantType.RefreshToken, oldToken: user.RefreshToken);
                user.Token = response.AccessToken;
                    user.ExpiresIn = DateTime.Now.AddSeconds(response.ExpiresIn);
                    user.RefreshToken = response.RefreshToken;

                    await _context.SaveChangesAsync();
                })});
        }

        public void Dispose()
        {
            _context?.Dispose();
            _loginTimer?.Dispose();
        }
    }
}
