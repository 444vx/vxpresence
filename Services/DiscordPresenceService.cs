using System;
using DiscordRPC;

namespace VxPresence.Services
{
    public class DiscordPresenceService : IDisposable
    {
        private readonly DiscordRpcClient _client;

        public DiscordPresenceService(string clientId)
        {
            _client = new DiscordRpcClient(clientId);
            _client.Initialize();
        }

        public void Update(string details, string state, DateTime? startTime = null, bool isMedia = false, string? processName = null, string? githubUrl = null)
        {
            if (!_client.IsInitialized) return;

            var presence = new RichPresence
            {
                Details = details,
                State = state,
                Timestamps = startTime.HasValue ? new Timestamps(startTime.Value) : null,
                Assets = new Assets
                {
                    LargeImageKey = isMedia ? "media" : "desktop",
                    LargeImageText = isMedia ? "Media Playing" : "VxPresence Engine",
                    SmallImageKey = "vx_logo",
                    SmallImageText = "VxPresence v1.0"
                }
            };

            if (!string.IsNullOrWhiteSpace(githubUrl))
            {
                presence.Buttons = new Button[]
                {
                    new Button { Label = "⚡ Get VxPresence App", Url = githubUrl }
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