using System;
using DiscordRPC;
using DiscordRPCButton = DiscordRPC.Button;

namespace VxPresence.Services
{
    public class DiscordPresenceService : IDisposable
    {
        private DiscordRpcClient? _client;
        private readonly string _clientId;

        public DiscordPresenceService(string clientId)
        {
            _clientId = clientId;
        }

        public void Initialize()
        {
            if (string.IsNullOrEmpty(_clientId)) return;

            _client = new DiscordRpcClient(_clientId);
            _client.Initialize();
        }

        public void Update(string details, string state, DateTime startTime, bool isMedia, string? largeIcon = null, string? githubUrl = null)
        {
            if (_client == null || !_client.IsInitialized) return;

            var presence = new RichPresence
            {
                Details = details,
                State = state,
                Timestamps = Timestamps.FromTimeSpan(DateTime.UtcNow - startTime),
                Assets = new Assets
                {
                    LargeImageKey = largeIcon ?? "app_icon",
                    LargeImageText = "VxPresence Engine"
                }
            };

            if (!string.IsNullOrEmpty(githubUrl))
            {
                presence.Buttons = new[]
                {
                    new DiscordRPCButton { Label = "GitHub Repository", Url = githubUrl }
                };
            }

            _client.SetPresence(presence);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}