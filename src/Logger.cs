using System.IO;


public class Logger {
    private static readonly StreamWriter writer = File.AppendText(@".\aves-network.log");

    private Logger() {}


    public static void Log(string message) {
        writer.WriteLine(message);
    }
}
