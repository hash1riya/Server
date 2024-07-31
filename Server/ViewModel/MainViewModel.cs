using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Diagnostics;
using Server.Model;

namespace Server.ViewModel;
public partial class MainViewModel : ObservableObject
{
    public Model.Server Server;
    public Socket Client;

    [ObservableProperty]
    public bool isRunning = false;
    [ObservableProperty]
    public bool isNotRunning = true;
    [ObservableProperty]
    public bool isConnected = false;
    [ObservableProperty]
    private string ip = "127.0.0.1";
    [ObservableProperty]
    private int port = 1024;
    [ObservableProperty]
    private string message = string.Empty;

    public ObservableCollection<Message> MessageHistory { get; set; } = [];

    public async Task<Socket> ClientAccept()
    {
        Socket client = await this.Server.ClientAccept();
        this.IsConnected = true;
        this.MessageHistory.Add(new Message()
        {
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Content = $"{client.RemoteEndPoint.ToString()} connected!",
            Sender = "me:"
        });
        return client;
    }
    public async void Run()
    {
        string clientAdress = string.Empty;
        try
        {
            int bytesRead = 0;
            while (true)
            {
                this.Client = await ClientAccept();
                clientAdress = Client.RemoteEndPoint.ToString();
                while (Client.Connected)
                {
                    bytesRead = await Server.Receive(Client);
                    this.MessageHistory.Add(new Message()
                    {
                        Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        Content = System.Text.Encoding.UTF8.GetString(Server.Buffer, 0, bytesRead),
                        Sender = $"{clientAdress}:"
                    });
                }
            }
        }
        catch (SocketException ex)
        {
            Debug.WriteLine(ex.Message);
            if (IsRunning)
            {
                this.MessageHistory.Add(new Message()
                {
                    Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    Content = $"{clientAdress} disconnected",
                    Sender = "me:"
                });
                Run();
            }
            this.IsConnected = false;
        }
    }

    [RelayCommand]
    private void Start()
    {
        this.IsRunning = true;
        this.IsNotRunning = false;
        this.Server = new Model.Server(Ip, Port);
        this.MessageHistory.Add(new Message()
        {
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Content = "Server was setted up. Waiting for client...",
            Sender = "me:"
        });
        this.Run();
    }

    [RelayCommand]
    private void Stop()
    {
        this.IsRunning = false;
        this.IsNotRunning = true;
        this.IsConnected = false;
        this.Server.Stop();
        this.MessageHistory.Add(new Message()
        {
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Content = "Server was shutted down!",
            Sender = "me"
        });
    }

    [RelayCommand]
    private void Send()
    {
        this.Server.Send(this.Message, this.Client);
        this.MessageHistory.Add(new Message()
        {
            Time = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Sender = "me:",
            Content = this.Message
        });
        this.Message = string.Empty;
    }
}
