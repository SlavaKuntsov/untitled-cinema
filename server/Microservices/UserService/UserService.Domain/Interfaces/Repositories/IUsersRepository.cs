using UserService.Domain.Enums;
using UserService.Domain.Models.Auth;
using UserService.Domain.Models.Users;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUsersRepository
{
	Task<Guid> Create(UserModel user, RefreshTokenModel refreshTokenModel, CancellationToken cancellationToken);
	Task Delete(UserModel model, CancellationToken cancellationToken);
	Task<UserModel?> Get(Guid id, CancellationToken cancellationToken);
	Task<UserModel?> Get(string email, CancellationToken cancellationToken);
	Task<IList<UserModel>> Get(CancellationToken cancellationToken);
	Task<Role?> GetRoleById(Guid id, CancellationToken cancellationToken);
	Task<UserModel> Update(UserModel model, CancellationToken cancellationToken);
}
