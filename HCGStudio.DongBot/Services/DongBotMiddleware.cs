namespace HCGStudio.DongBot.Services;

public class DongBotMiddleware
{
    private readonly Func<DongBotContext, Func<Task>, CancellationToken, Task> _invokeContent;

    public DongBotMiddleware(Func<DongBotContext, Func<Task>, CancellationToken, Task> invokeContent)
    {
        _invokeContent = invokeContent;
    }

    public async Task InvokeAsync(DongBotContext context, DongBotMiddleware[] middleware, int current,
        CancellationToken token)
    {
        await _invokeContent.Invoke(context,
            current == middleware.Length - 1
                ? () => Task.CompletedTask
                : () => middleware[current + 1]
                    .InvokeAsync(context, middleware, current + 1, token)
            , token);
    }
}