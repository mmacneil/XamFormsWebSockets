

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace XFWebSocketServer
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly ConcurrentBag<WebSocket> WebSockets = new ConcurrentBag<WebSocket>();

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await httpContext.WebSockets.AcceptWebSocketAsync();

                WebSockets.Add(socket);

                while (socket.State == WebSocketState.Open)
                {
                    var token = CancellationToken.None;
                    var buffer = new ArraySegment<byte>(new byte[4096]);
                    var received = await socket.ReceiveAsync(buffer, token);

                    switch (received.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            // nothing to do for now...
                            break;

                        case WebSocketMessageType.Text:
                            var incoming = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
                            // get rid of trailing crap from buffer
                            incoming = incoming.Replace("\0", "");
                            var data = Encoding.UTF8.GetBytes("data from server :" + DateTime.Now.ToLocalTime() + " " + incoming);
                            buffer = new ArraySegment<byte>(data);

                            // send to all open sockets
                            await Task.WhenAll(WebSockets.Where(s => s.State == WebSocketState.Open)
                           .Select(s => s.SendAsync(buffer, WebSocketMessageType.Text, true, token)));
                           break;
                    }
                }
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }

    public static class RequestExtensions
    {
        public static IApplicationBuilder UseWebSocketHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
