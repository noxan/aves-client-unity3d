using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;


public class Net {
    public const int BUFFER_SIZE = 1024;

    private byte[] byteBuffer = new byte[BUFFER_SIZE];
    private TcpClient tcpClient;
    private NetworkStream stream;

    private Thread connectThread;
    private Thread readThread;

    private string host = "127.0.0.1";
    private int port = 1666;

    private bool eventDriven = false;

    private Queue<Tuple<NetEventType, Object>> networkEvents;
    private List<NetEventListener> listeners;

    public void AddNetEventListener(NetEventListener listener) {
        listeners.Add(listener);
    }

    private void fireNetEvent(NetEventType type, Object data) {
        foreach(NetEventListener listener in listeners) {
            listener(type, data);
        }
    }

    public Net() {
        Initialize();
    }

    public Net(bool eventDriven) {
        this.eventDriven = eventDriven;
        Initialize();
    }

    private void Initialize() {
        listeners = new List<NetEventListener>();
        networkEvents = new Queue<Tuple<NetEventType, Object>>();
    }

    public void Connect(string host) {
        this.host = host;
        Connect();
    }

    public void Connect(string host, int port) {
        this.host = host;
        this.port = port;
        Connect();
    }

    public void Connect() {
        Logger.Log("Connect: Start");
        connectThread = new Thread(new ThreadStart(ConnectThread));
        connectThread.Start();
    }

    private void ConnectThread() {
        try {
            tcpClient = new TcpClient();
            tcpClient.Client.Connect(this.host, this.port);
            stream = tcpClient.GetStream();

            readThread = new Thread(new ThreadStart(ReadThread));
            readThread.Start();

            fireNetEvent(NetEventType.CONNECT, null);
            Logger.Log("Connect: Successful");
        } catch(SocketException socketException) {
            Logger.Log("Connect: SocketException " + socketException);
        } catch(Exception exception) {
            Logger.Log("Connect: Exception " + exception);
        }
    }

    private void ReadThread() {
        while(true) {
            try {
                if(stream.DataAvailable) {
                    int num = stream.Read(byteBuffer, 0, BUFFER_SIZE);
                    if(num < 1) {
                        Logger.Log("Read: End of stream, connection closed.");
                        break;
                    }
                    HandleData(byteBuffer, num);
                }
            } catch(Exception exception) {
                Logger.Log("Read: " + exception);
                break;
            }
        }
    }

    private void HandleData(byte[] buf, int size) {
        string data = Encoding.ASCII.GetString(buf, 0, size);
        Logger.Log(string.Format("DataHandler: {0}", data));
        fireNetEvent(NetEventType.DATA_READ, data);
    }

    public void Write(string message) {
        byte[] array = Encoding.ASCII.GetBytes(message);
        stream.Write(array, 0, array.Length);
        fireNetEvent(NetEventType.DATA_WRITE, message);
    }

    public bool IsConnected() {
        return tcpClient!=null && tcpClient.Client.Connected;
    }

    public void Disconnect() {
        stream.Close();
        tcpClient.Close();
        readThread.Abort();
        connectThread.Abort();
        fireNetEvent(NetEventType.DISCONNECT, null);
        Logger.Log("Disconnect: Successful");
    }
}
