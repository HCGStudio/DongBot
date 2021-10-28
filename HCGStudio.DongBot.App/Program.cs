using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureServices((context, services) => { });
await builder.Build().RunAsync();