using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class LiteDocService : IHostedService
{
    private LiteDoc liteDoc;
    private ICommand command;
    private ILogger logger;
    private IHostApplicationLifetime lifetime;

    public LiteDocService(
        LiteDoc liteDoc,
        ICommand command,
        ILogger<LiteDocService> logger,
        IHostApplicationLifetime lifetime
    )
    {
        this.liteDoc = liteDoc;
        this.command = command;
        this.logger = logger;
        this.lifetime = lifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        switch (this.command.Command)
        {
            case "run":
                this.logger.LogInformation("Running LiteDoc once...");
                await this.liteDoc.Run();
                this.lifetime.StopApplication();
                return;
            case "watch":
                this.logger.LogInformation("LiteDoc watching...");
                this.liteDoc.Watch();
                return;
            case "new":
                this.logger.LogInformation("Creating new LiteDoc workspace...");
                await this.liteDoc.New();
                this.lifetime.StopApplication();
                return;
            default:
                this.logger.LogInformation("Invalid command; usage: litedoc [run | watch | new] <workspace-path>");
                this.lifetime.StopApplication();
                return;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("LiteDoc: done.");
        return Task.CompletedTask;
    }
}
