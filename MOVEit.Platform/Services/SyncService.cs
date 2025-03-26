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
    /// <summary>
    /// Service responsible for synchronizing files with MOVEit Transfer. 
    /// Monitors the user's sync directory for file changes and uploads new files.
    /// </summary>
    public class SyncService : IDisposable
    {
        private readonly AuthService _authService;
        private readonly MOVEitContext _context;
        private readonly FileService _fileService;

        private FileSystemWatcher _directoryWatcher;

        private Timer _loginTimer;
        /// <summary>
        /// Indicates whether the service is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }
        /// <summary>
        /// Indicates whether the service is currently syncing files.
        /// </summary>
        public bool IsSyncing { get; private set; }
        /// <summary>
        /// The authenticated user associated with the sync operation.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncService"/> class.
        /// </summary>
        public SyncService()
        {
            _authService = new AuthService();
            _context = new MOVEitContext();
            _fileService = new FileService();
        }

        /// <summary>
        /// Starts the sync service and begins periodic login checks.
        /// </summary>
        public void Start()
        {
            IsRunning = true;
            _loginTimer = new Timer(LoginTimer_Tick, null, 0, 30000);
        }

        /// <summary>
        /// Stops the sync service and cancels any ongoing sync operations.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Periodically checks if the user is authenticated and if syncing should continue.
        /// </summary>
        /// <param name="obj">The timer callback argument.</param>
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

        /// <summary>
        /// Subscribes to the user's sync directory and starts monitoring for changes.
        /// </summary>
        private void SubscribeDirectory()
        {
            _directoryWatcher = new FileSystemWatcher(User.SyncDirectory)
            {
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _directoryWatcher.Created += async (sender, e) => await DirectoryWatcher_Changed(sender, e);
            _directoryWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Handles changes to files in the monitored directory and uploads new files to MOVEit Transfer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments containing file change details.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        /// <exception cref="HttpRequestException">Thrown when the file upload request fails.</exception>
        public async Task<bool> DirectoryWatcher_Changed(object sender, FileSystemEventArgs e)
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
                        var defaultFolder = await _fileService.GetUserDefaultFolderIdAsync(token);
                        var client = new RestClient($"https://testserver.moveitcloud.com/api/v1/folders/{defaultFolder}/files");
                        RestRequest req = new RestRequest($"https://testserver.moveitcloud.com/api/v1/folders/{defaultFolder}/files", Method.Post);
                                                
                        req.AddHeader("Authorization", $"Bearer {token}");
                        req.AddFile("file", e.FullPath);

                        var response = await client.ExecuteAsync(req);

                        if (!response.IsSuccessStatusCode)
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        //TODO: Log error to server
                        throw;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Stops monitoring the sync directory by detaching the file system watcher.
        /// </summary>
        private void DetachWatcher()
        {
            _directoryWatcher?.Dispose();
        }

        /// <summary>
        /// Checks if the user is authenticated and the token is still valid.
        /// </summary>
        /// <returns>True if the user is authenticated, otherwise false.</returns>
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

        /// <summary>
        /// Refreshes the user's authentication token using the refresh token.
        /// </summary>
        /// <param name="user">The user whose token needs to be refreshed.</param>
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

        /// <summary>
        /// Disposes of the resources used by the sync service.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
            _loginTimer?.Dispose();
        }
    }
}
