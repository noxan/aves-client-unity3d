using System;

public class Test {
    static void Main(string[] args) {
        Net net = new Net();
        net.Connect();
        while(true) {
            Tuple<NetEventType, Object> evt = net.PollNetworkEvent();
            if(evt != null) {
                Console.WriteLine(evt.GetFirst());
                switch(evt.GetFirst()) {
                    case NetEventType.CONNECT:
                        net.Write("ping\n");
                        break;
                    case NetEventType.DATA_READ:
                        Console.Write(evt.GetSecond());
                        net.Write("pong\n");
                        break;
                    case NetEventType.DISCONNECT:
                        Environment.Exit(0);
                        break;
                }
            }
        }
    }
}
