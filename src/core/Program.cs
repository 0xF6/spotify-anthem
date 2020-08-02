namespace spotify.core
{
    using System;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO
            // temporary
            if (Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS") is null)
                throw new Exception();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(x => 
                    x.AddEnvironmentVariables())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://localhost:12903", "https://localhost:12904");
                    webBuilder.UseStartup<Startup>();
                });
    }
}
