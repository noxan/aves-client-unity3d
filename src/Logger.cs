using System.IO;


public class Logger {
    private static readonly Logger instance = new Logger();

    private StreamWriter writer;

    private Logger() {
        writer = File.AppendText(@".\aves-network.log");
    }

    public static void Log(string message) {
        instance.writer.WriteLine(message);
    }
}
