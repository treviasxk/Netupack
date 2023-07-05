using System.Security.Authentication;

class Program {
    static async Task Main(string[] args){
        await DownloadFile("https://github.com/treviasxk/Nethostfire/archive/refs/tags/v2.0.8.0.zip", "teste.zip");
        Console.ReadKey();
    }

    static async Task DownloadFile(string _url, string _file){
        using(FileStream fileStream = new FileStream(_file, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None)){
            long skip = fileStream.Length;
            HttpClient webClient = new HttpClient(new HttpClientHandler(){SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13});
            webClient.DefaultRequestHeaders.Add("Range", "bytes=" + skip + "-");
            var response = await webClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead);
            long? size = response.Content.Headers.ContentLength;

            using(var stream = response.Content.ReadAsStream()){
                var buffer = new byte[Convert.ToInt64(size)];
                fileStream.Seek(skip, SeekOrigin.Current);
                while(fileStream.Length < size + skip){
                    int bytesRead = await stream.ReadAsync(buffer, 0, 1024);
                    if(bytesRead == 0)
                        continue;
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
            fileStream.Close();
        }
    }
}