using System.Security.Authentication;

class Program {
    static async Task Main(string[] args){
        Console.WriteLine("Downloading...");
        await DownloadFileAsync("https://github.com/treviasxk/Nethostfire/archive/refs/heads/master.zip", "Nethostfire.zip");
        Console.ReadKey();
    }

    static async Task DownloadFileAsync(string _url, string _file){
        // Get content info download
        HttpClient webClient = new HttpClient(new HttpClientHandler(){SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13});
        var response = await webClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);
        long? size = response.Content.Headers.ContentLength;

        // Check resume download
        bool acceptRanges = response.Headers.AcceptRanges.Contains("bytes");
        if(!acceptRanges)
            File.Delete(_file);

        // Read and Write file
        using(FileStream fileStream = new FileStream(_file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)){
            // Get file size
            long skip = fileStream.Length;

            // Resume download
            if(acceptRanges){
                webClient.DefaultRequestHeaders.Add("Range", "bytes=" + skip + "-");
                response = await webClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);
            }

            // Get file size download
            long? sizePart = response.Content.Headers.ContentLength;
            using(var stream = response.Content.ReadAsStream()){
                var buffer = new byte[Convert.ToInt64(sizePart)];
                fileStream.Seek(skip, SeekOrigin.Current);
                while(fileStream.Length < sizePart + skip){
                    int bytesRead = 0;
                    bytesRead = await stream.ReadAsync(buffer, 0, 1024);
                    if(bytesRead == 0)
                        continue;
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            if(size == fileStream.Length)
                Console.WriteLine("Download Complete!");
            else
                Console.WriteLine("Download falid!");
            fileStream.Close();
        }
    }
}