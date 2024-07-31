using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace Server.Model;
public class Server
{
    IPEndPoint endp;
    Socket socket;

    public Socket Client;
    public byte[] Buffer = new byte[1024];

    public Server(string ip, int port)
    {
        this.endp = new IPEndPoint(IPAddress.Parse(ip), port);
        this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        this.Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        this.socket.Bind(endp);
        this.socket.Listen(10);
    }
    public async Task<Socket> ClientAccept() => this.Client = await this.socket.AcceptAsync();

    public void Stop()
    {
        try
        {
            this.socket.Close();
            this.Client.Close();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
    public async Task<int> Receive() => await this.Client.ReceiveAsync(this.Buffer);
    public async void Send(string strSend)
    {
        if (this.socket != null)
            await this.Client.SendAsync(System.Text.Encoding.UTF8.GetBytes(strSend));
    }
}