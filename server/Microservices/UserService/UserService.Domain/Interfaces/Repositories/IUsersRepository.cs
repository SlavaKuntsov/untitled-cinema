using UserService.Domain.Enums;
using UserService.Domain.Models;

namespace UserService.Domain.Interfaces.Repositories;

public interface IUsersRepository
{
	Task<Guid> CreateAsync(UserModel user, RefreshTokenModel refreshTokenModel, CancellationToken cancellationToken);
	Task DeleteAsync(UserModel model, CancellationToken cancellationToken);
	Task<UserModel?> GetAsync(Guid id, CancellationToken cancellationToken);
	Task<UserModel?> GetAsync(string email, CancellationToken cancellationToken);
	Task<IList<UserModel>> GetAsync(CancellationToken cancellationToken);
	Task<Role?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<UserModel> UpdateAsync(UserModel model, CancellationToken cancellationToken);
}
