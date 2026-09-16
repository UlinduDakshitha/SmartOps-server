using SmartOps.Application.DTOs.SLA;

namespace SmartOps.Application.Interfaces.Services;

public interface ISLAPolicyService
{
    Task<SLAPolicyResponse> GetByIdAsync(Guid id);

    Task<List<SLAPolicyResponse>> GetAllAsync();

    Task<SLAPolicyResponse> CreateAsync(
        CreateSLAPolicyRequest request);

    Task<SLAPolicyResponse> UpdateAsync(
        Guid id,
        UpdateSLAPolicyRequest request);
}