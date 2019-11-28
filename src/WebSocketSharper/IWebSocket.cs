using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using WebSocketSharper.Net;

namespace WebSocketSharper
{
    public interface IWebSocket
    {
        CompressionMethod Compression { get; set; }
        IEnumerable<Cookie> Cookies { get; }
        NetworkCredential Credentials { get; }
        bool EmitOnPing { get; set; }
        bool EnableRedirection { get; set; }
        string Extensions { get; }
        bool IsAlive { get; }
        bool IsSecure { get; }
        ILogger Log { get; }
        IObservable<WebMessage> MessageReceived { get; }
        string Origin { get; set; }
        string Protocol { get; }
        WebSocketState ReadyState { get; }
        ClientSslConfiguration SslConfiguration { get; }
        Uri Url { get; }
        TimeSpan WaitTime { get; set; }

        event EventHandler<CloseEventArgs> OnClose;
        event EventHandler<ErrorEventArgs> OnError;
        event EventHandler<MessageEventArgs> OnMessage;
        event EventHandler OnOpen;

        void Accept();
        void AcceptAsync();
        void Close();
        void Close(CloseStatusCode code);
        void Close(CloseStatusCode code, string reason);
        void Close(ushort code);
        void Close(ushort code, string reason);
        void CloseAsync();
        void CloseAsync(CloseStatusCode code);
        void CloseAsync(CloseStatusCode code, string reason);
        void CloseAsync(ushort code);
        void CloseAsync(ushort code, string reason);
        void Connect();
        void ConnectAsync();
        bool Ping();
        bool Ping(string message);
        void Send(byte[] data);
        void Send(FileInfo fileInfo);
        void Send(Stream stream, int length);
        void Send(string data);
        void SendAsync(byte[] data, Action<bool> completed);
        void SendAsync(FileInfo fileInfo, Action<bool> completed);
        void SendAsync(Stream stream, int length, Action<bool> completed);
        void SendAsync(string data, Action<bool> completed);
        void SetCookie(Cookie cookie);
        void SetCredentials(string username, string password, bool preAuth);
        void SetProxy(string url, string username, string password);
    }
}