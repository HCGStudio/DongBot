using System.Collections.Immutable;
using System.Diagnostics;
using HCGStudio.DongBot.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HCGStudio.DongBot.Services;

public class DongHostedServices : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CancellationTokenSource _tokenSource = new();
    private Task? _task;


    public DongHostedServices(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void Dispose()
    {
        _tokenSource.Dispose();
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
        _task = StartListen();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _tokenSource.Cancel();

        Debug.Assert(_task is not null);
        await _task;
        Dispose();
    }

    public async Task StartListen()
    {
        var token = _tokenSource.Token;
        await using var scope = _serviceProvider.CreateAsyncScope();
        var services = scope.ServiceProvider;
        var provider = services.GetRequiredService<IMessageProvider>();
        var middleware = services.GetServices<DongBotMiddleware>().ToImmutableArray();
        while (!token.IsCancellationRequested)
        {
            var message = await provider.MessageChannel.Reader.ReadAsync(token);
            await HandleNext(message, middleware, token);
        }
    }

    public async ValueTask HandleNext(IMessage message, ImmutableArray<DongBotMiddleware> middleware,
        CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var services = scope.ServiceProvider;
    }
}