using AsnParser;

class Program
{
    static void Main()
    {
        string folderToMonitor = "path to folder";

        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = folderToMonitor;
        watcher.Filter = "*.txt";
        watcher.Created += OnFileCreated;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine($"Monitoring folder: {folderToMonitor}");
        Console.ReadLine();
    }

    static async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        string filePath = e.FullPath;
        Console.WriteLine($"New file detected: {filePath}");

        await ParseAndLoadData(filePath);
    }

    static async Task ParseAndLoadData(string filePath)
    {
        var service = new AsnParserService("HDR", "LINE");
        await service.ParseAnsLoadToDbFrom(filePath);
    }

}
