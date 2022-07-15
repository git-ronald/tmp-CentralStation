using CentralStation.Client.Extensions;
using CoreLibrary.ConstantValues;
using CoreLibrary.Delegates;
using CoreLibrary.Helpers;
using CoreLibrary.Models;
using CoreLibrary.SchedulerService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net;
using System.Text.Json;

namespace CentralStation.Client.Services
{
    // TODO: error handling where needed. Also: general error handler(s)?
    public class HubClientService : IHubClientService, IAsyncDisposable
    {
        private readonly AuthenticationStateProvider _authenticationState;
        private readonly NavigationManager _navigationManager;
        private readonly IAccessTokenProvider _accessTokenProvider;
        private readonly ISchedulerService _schedulerService;

        private readonly Guid _peerNodeId = Guid.NewGuid();
        //private readonly Dictionary<Guid, Func<object, Task>> _responseSubscribers = new();

        private HubConnection? _connection = null;
        private AccessTokenInfo _accessToken = new();
        private CancellationTokenSource _scheduleCancellation = new();

        public event EmptyAsyncHandler? OnConnectionRegistered;
        //public event AsyncEventHandlers.EmptyAsyncHandler? OnTestResponse;

        public delegate Task RegistrationUpdateDelegate(UpdatedRegistrations updates);
        public event RegistrationUpdateDelegate? OnRegistrationUpdate;

        public delegate Task PeerResponseDelegate(Guid peerConnectionId, string path, object? data);
        public event PeerResponseDelegate? OnPeerResponse;

        public delegate Task PeerErorDelegate(Guid peerConnectionId, string path, JsonElement? data, HttpStatusCode statusCode);
        public event PeerErorDelegate? OnPeerEror;

        public HubClientService(AuthenticationStateProvider authenticationState, NavigationManager navigationManager, IAccessTokenProvider accessTokenProvider, ISchedulerService schedulerService)
        {
            _authenticationState = authenticationState;
            _navigationManager = navigationManager;
            _accessTokenProvider = accessTokenProvider;
            _schedulerService = schedulerService;
        }

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;
        public string? HubConnectionId => _connection?.ConnectionId;

        public async Task StartConnection()
        {
            _scheduleCancellation = new CancellationTokenSource();
            _connection = await BuildHubConnection();

            if (_connection != null && _connection.State == HubConnectionState.Disconnected)
            {
                // The following StartAsync statement:
                // - yields an authotization error in the browser console: WebSocket connection to 'url' failed: HTTP Authentication failed; no valid credentials available
                // - that error could not be caught in a try-catch in current method (perhaps something to do with the token??)
                // - but the connection then continues to start successfully, after one try! (also: State == Connected)
                // TODO: figure out why the error occurs amd how to prevent it. For now, it doesn't seem like a big problem since everything seems to just work.
                await _connection.StartAsync();
            }
        }

        // TODO: also call this when token expires
        public async Task StopConnection()
        {
            _scheduleCancellation.Cancel();

            if (_connection != null)
            {
                await _connection.StopAsync();
                _connection = null;

            }
        }

        public async Task RequestPeer(Guid peerId, string path, object? data)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync(nameof(RequestPeer), peerId, path, data);
            }
        }

        private async Task HandlePeerResponse(Guid peerConnectionId, string path, object? data)
        {
            if (OnPeerResponse != null)
            {
                await OnPeerResponse(peerConnectionId, path, data);
            }
        }

        private async Task HandlePeerError(Guid peerConnectionId, string path, JsonElement? data, HttpStatusCode statusCode)
        {
            if (OnPeerEror != null)
            {
                await OnPeerEror(peerConnectionId, path, data, statusCode);
            }
        }

        private async Task<HubConnection?> BuildHubConnection()
        {
            if (!await _authenticationState.IsAuthenticated())
            {
                return null;
            }

            IHubConnectionBuilder? connectionBuilder = new HubConnectionBuilder()
                .WithUrl(_navigationManager.ToAbsoluteUri("/mainhub"), options =>
                {
                    options.AccessTokenProvider = GetToken;
                })
                .WithAutomaticReconnect();

            HubConnection connection = connectionBuilder.Build();
            AddConnectionEventHandlers(connection);
            return connection;
        }

        private async Task<string?> GetToken()
        {
            DateTime threshold = DateTime.UtcNow.AddMinutes(-3);

            if (_accessToken.Expiration > threshold)
            {
                return _accessToken.Value;
            }

            var accessTokenResult = await _accessTokenProvider.RequestAccessToken();
            if (accessTokenResult.TryGetToken(out AccessToken accessToken))
            {
                _accessToken.Value = accessToken.Value;
                _accessToken.Expiration = accessToken.Expires.UtcDateTime;
                return _accessToken.Value;
            }
            return null;
        }

        private void AddConnectionEventHandlers(HubConnection connection)
        {
            connection.On<TimeSpan>(SignalrMessages.RequestPeerRegistrationInfo, RequestPeerRegistrationInfo);

            connection.On<UpdatedRegistrations>(SignalrMessages.RegistrationUpdate, async updates =>
            {
                if (OnRegistrationUpdate != null)
                {
                    await OnRegistrationUpdate(updates);
                }
            });

            connection.On<Guid, string, object?>(SignalrMessages.PeerResponse, HandlePeerResponse);
            connection.On<Guid, string, JsonElement?, HttpStatusCode>(SignalrMessages.PeerError, HandlePeerError);
        }

        private async Task RequestPeerRegistrationInfo(TimeSpan signOfLifeEvent)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync(SignalrMessages.PeerRegistrationInfoResponse, new PeerRegistrationInfo
                {
                    PeerName = CoreConstants.FrontEndPeer,
                    PeerNodeId = _peerNodeId,
                    ConfirmedSignOfLifeEvent = signOfLifeEvent
                });
            }

            StartScheduler(signOfLifeEvent);
            await OnConnectionRegistered.InvokeHandlers();
        }

        private void StartScheduler(TimeSpan signOfLifeEvent)
        {
            Dictionary<TimeSpan, SchedulerTaskList> fixedTimeSchedule = new();

            foreach (int index in Enumerable.Range(0, 4))
            {
                TimeSpan fixedTime = signOfLifeEvent.Add(TimeSpan.FromHours(index * 6));
                // TODO: In the future this should be disabled for clients outside the home network.
                fixedTimeSchedule.Ensure(fixedTime).Add(async cancel =>
                {
                    if (_connection?.State == HubConnectionState.Connected)
                    {
                        await _connection.InvokeAsync(SignalrMessages.NotifySignOfLife, cancel);
                    }
                });
            }

            var _ = _schedulerService.Start(_scheduleCancellation.Token, fixedTimeSchedule);
        }

        //private async Task Connect(Func<HubConnection, Task> invoke)
        //{
        //    if (_connection != null && _connection.State == HubConnectionState.Connected)
        //    {
        //        await invoke(_connection);
        //    }
        //}
        ////private async Task Invoke(string methodName, CancellationToken cancellation = default)
        //{
        //    if (_connection != null && _connection.State == HubConnectionState.Connected)
        //    {
        //        await _connection.InvokeAsync(methodName, cancellation);
        //    }
        //}
        //private async Task Invoke<T>(string methodName, T arg, CancellationToken cancellation = default)
        //{
        //    if (_connection != null && _connection.State == HubConnectionState.Connected)
        //    {
        //        await _connection.InvokeAsync(methodName, arg, cancellation);
        //    }
        //}

        public async ValueTask DisposeAsync()
        {
            _scheduleCancellation.Cancel();
            _scheduleCancellation.Dispose();

            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
