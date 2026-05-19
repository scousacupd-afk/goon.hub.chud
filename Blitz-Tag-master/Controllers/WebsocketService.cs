using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace Blitz_Tag.Controllers
{
    [ApiController]
    [Route("/api/MothershipWebsocket")]
    public class WebsocketService : ControllerBase
    {
        public static readonly ConcurrentDictionary<string, WebSocket> ConnectedClients = new();

        [HttpGet]
        public async Task<IActionResult> GetHost()
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                return NotFound();
            }

            if (!JsonWebToken.Verify(Request.Headers["x-mothership-token"], out var id) || id == null)
            {
                return StatusCode(403);
            }

            foreach (var client in ConnectedClients.ToList())
            {
                if (client.Value.State == WebSocketState.None || client.Value.State == WebSocketState.CloseSent || client.Value.State == WebSocketState.CloseReceived || client.Value.State == WebSocketState.Aborted)
                {
                    ConnectedClients.TryRemove(client.Key, out _);
                }
            }

            var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            ConnectedClients.TryAdd(id, webSocket);

            var buffer = new byte[1024];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine(ex.Message);
                if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing after exception", CancellationToken.None);
                }
            }
            finally
            {
                ConnectedClients.TryRemove(id, out _);
            }

            return new EmptyResult();
        }
    }
}
