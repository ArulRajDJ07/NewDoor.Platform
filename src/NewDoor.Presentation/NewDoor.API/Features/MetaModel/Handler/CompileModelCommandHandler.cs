
using NewDoor.API.Features.MetaModel.Command;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

public class CompileModelCommandHandler
    : IRequestHandler<CompileModelCommand, bool>
{
    private readonly IWebHostEnvironment _env;

    public CompileModelCommandHandler(IWebHostEnvironment env)
    {
        _env = env;
    }

    public Task<bool> Handle(CompileModelCommand request,CancellationToken cancellationToken)
    {
        var solutionRoot =
            Directory.GetParent(_env.ContentRootPath)!.FullName;

        var providerArg = request.Provider.ToString(); // SqlServer / Sqlite

        var psi = new ProcessStartInfo(
            "dotnet",
            $"run --project DoWhatta.ModelCompiler -- --provider {providerArg}")
        {
            WorkingDirectory = solutionRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var p = Process.Start(psi)!;
        p.WaitForExit();

        if (p.ExitCode != 0)
            throw new Exception(p.StandardError.ReadToEnd());

        return Task.FromResult(true);
    }

}
