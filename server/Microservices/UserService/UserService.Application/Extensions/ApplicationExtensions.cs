using Microsoft.Extensions.DependencyInjection;

using UserService.Application.Handlers.Commands.Tokens;
using UserService.Application.Handlers.Commands.Users;
using UserService.Application.Handlers.Queries.Users;

namespace UserService.Application.Extensions;

public static class ApplicationExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		// Регистрация MediatR и обработчиков
		services.AddMediatR(cfg =>
		{
			cfg.RegisterServicesFromAssemblyContaining<UserRegistrationCommand.UserRegistrationCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<GenerateAndUpdateTokensCommand.GenerateAndUpdateTokensCommandHandler>();
			cfg.RegisterServicesFromAssemblyContaining<LoginQuery.LoginQUeryHandler>();
			cfg.RegisterServicesFromAssemblyContaining<GetUserQuery.GetUserQueryHandler>();
		});


		//services.AddTransient<IRequestHandler<DeleteUserCommand<ParticipantModel>>, DeleteUserCommandHandler<ParticipantModel>>();
		//services.AddTransient<IRequestHandler<DeleteUserCommand<AdminModel>>, DeleteUserCommandHandler<AdminModel>>();

		//services.AddTransient<IRequestHandler<GetOrAuthorizeUserQuery<ParticipantModel>, ParticipantModel?>, GetOrAuthorizeUserQueryHandler<ParticipantModel>>();
		//services.AddTransient<IRequestHandler<GetOrAuthorizeUserQuery<AdminModel>, AdminModel?>, GetOrAuthorizeUserQueryHandler<AdminModel>>();

		//services.AddTransient<IRequestHandler<GetUserByFilterQuery<ParticipantModel>, ParticipantModel?>, GetUserByFilterQueryHandler<ParticipantModel>>();
		//services.AddTransient<IRequestHandler<GetUserByFilterQuery<AdminModel>, AdminModel?>, GetUserByFilterQueryHandler<AdminModel>>();

		//services.AddTransient<IRequestHandler<GetUsersQuery<ParticipantModel>, IList<ParticipantModel>>, GetUsersQueryHandler<ParticipantModel>>();
		//services.AddTransient<IRequestHandler<GetUsersQuery<AdminModel>, IList<AdminModel>>, GetUsersQueryHandler<AdminModel>>();

		//services.AddTransient<IRequestHandler<LoginUserQuery<ParticipantModel>>, LoginUserCommandHandler<ParticipantModel>>();
		//services.AddTransient<IRequestHandler<LoginUserQuery<AdminModel>>, LoginUserCommandHandler<AdminModel>>();

		//services.AddTransient<IRequestHandler<UpdateParticipantCommand, ParticipantDto>, UpdateParticipantCommandHandler>();

		//services.AddTransient<IRequestHandler<UserRegistrationCommand<ParticipantModel>, AuthDto>, UserRegistrationCommandHandler<ParticipantModel>>();
		//services.AddTransient<IRequestHandler<UserRegistrationCommand<AdminModel>, AuthDto>, UserRegistrationCommandHandler<AdminModel>>();


		return services;
	}
}