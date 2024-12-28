using CommandLine;

using Microsoft.EntityFrameworkCore;

using UserService.Persistence;

namespace UserService.API.Extensions;

public record DatabaseOptions
{
	[Option('c', "connectionString", Required = true, HelpText = "Connection string for the database")]
	public string ConnectionString { get; init; } = string.Empty;
}

public static class CommandLineParser
{
	private static readonly Parser Parser;

	static CommandLineParser()
	{
		Parser = new Parser(config =>
		{
			config.IgnoreUnknownArguments = true;
			config.AutoHelp = true;
			config.HelpWriter = Console.Error;
		});
	}

	public static async Task RunMigration(string[] args)
	{
		var runningMigrationMode = IsRunningMigrationMode(args);

		if (runningMigrationMode)
		{
			var connectionString = GetConnectionString(args);

			var optionsBuilder = new DbContextOptionsBuilder<UserServiceDBContext>();
			optionsBuilder.UseNpgsql(connectionString);

			await using var dbContext = new UserServiceDBContext(optionsBuilder.Options);
			await dbContext.Database.MigrateAsync();

			return;
		}
	}

	public static bool IsRunningMigrationMode(string[] args)
	{
		return args.Contains("--RunMigrations");
	}

	public static string GetConnectionString(string[] args)
	{
		var connectionString = Parser.ParseArguments<DatabaseOptions>(args)
				.MapResult(
					options => options.ConnectionString,
					_ => string.Empty);

		if (string.IsNullOrEmpty(connectionString))
		{
			throw new ArgumentException("Command line arguments are invalid");
		}

		return connectionString;
	}
}