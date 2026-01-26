using Clean.Architecture.Template.Api.Extensions;
using Clean.Architecture.Template.Application;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

var colorTheme = ConsoleTheme.None;

if (builder.Environment.IsDevelopment())
{
    colorTheme = AnsiConsoleTheme.Sixteen;
}

builder.Host.UseSerilog((ctx, lc) =>
    {
        lc.Enrich.WithComputed("SourceContextName", "Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)");
        lc.ReadFrom.Configuration(ctx.Configuration);
        lc.Enrich.FromLogContext();
        lc.Destructure.ByTransforming<Exception>(ex => new
        {
            ex.Message,
            ex.StackTrace,
            ex.InnerException,
            ex.Data
        });
        lc.WriteTo.Console(theme: colorTheme, applyThemeToRedirectedOutput: true, outputTemplate: "{SourceContextName}: [{Timestamp:HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}");
    }
);

if (!builder.Environment.IsProduction())
{
    // path is path of .env file relative to this project
    var dotenv = Path.Combine(Directory.GetCurrentDirectory(), "../../.env");
    DotEnv.Load(dotenv);
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCoreApplication();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
