using Brokers.Extensions;
using Extensions.Authorization;
using Extensions.Common;
using Extensions.Exceptions;
using Extensions.HealthCheck;
using Extensions.Host;
using Extensions.Logging;
using Extensions.Mapper;
using Extensions.Swagger;
using UserService.API.Extensions;
using UserService.Application.Extensions;
using UserService.Infrastructure.Extensions;
using UserService.Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;
var host = builder.Host;
var app = builder.Build();

builder.UseHttps();

services.AddApplication()
	.AddInfrastructure(configuration)
	.AddPersistence(configuration)
	.AddMapper()
	.AddRabbitMQ(configuration);

app.AddAPI(services);

app.AddCommon(services)
	.AddExceptions(services)
	.AddAuthorization(services, configuration)
	.AddSwagger(services)
	.AddHealthCheck(services)
	.AddLogging(host);
app.Run();