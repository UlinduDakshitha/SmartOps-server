using SmartOps.Domain.Entities;

namespace SmartOps.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);

    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);

    Task AddAsync(RefreshToken refreshToken);

    Task UpdateAsync(RefreshToken refreshToken);
}