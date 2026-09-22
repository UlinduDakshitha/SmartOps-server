using SmartOps.Application.DTOs.SLA;
using SmartOps.Application.Interfaces.Repositories;
using SmartOps.Application.Interfaces.Services;
using SmartOps.Domain.Entities;

namespace SmartOps.Application.Services;

public class SLAPolicyService : ISLAPolicyService
{
    private readonly ISLAPolicyRepository _slaPolicyRepository;

    public SLAPolicyService(
        ISLAPolicyRepository slaPolicyRepository)
    {
        _slaPolicyRepository = slaPolicyRepository;
    }

    public async Task<SLAPolicyResponse> GetByIdAsync(Guid id)
    {
        var policy = await _slaPolicyRepository.GetByIdAsync(id);

        if (policy is null)
            throw new KeyNotFoundException("SLA policy not found.");

        return MapToResponse(policy);
    }

    public async Task<List<SLAPolicyResponse>> GetAllAsync()
    {
        var policies = await _slaPolicyRepository.GetAllAsync();

        return policies
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<SLAPolicyResponse> CreateAsync(
        CreateSLAPolicyRequest request)
    {
        var existingPolicy =
            await _slaPolicyRepository.GetByPriorityAsync(
                request.Priority);

        if (existingPolicy is not null)
        {
            throw new InvalidOperationException(
                "An SLA policy already exists for this priority.");
        }

        var policy = new SLAPolicy(
            request.Name.Trim(),
            request.Priority,
            request.ResponseTimeMinutes,
            request.ResolutionTimeMinutes);

        await _slaPolicyRepository.AddAsync(policy);

        return MapToResponse(policy);
    }

    public async Task<SLAPolicyResponse> UpdateAsync(
        Guid id,
        UpdateSLAPolicyRequest request)
    {
        var policy = await _slaPolicyRepository.GetByIdAsync(id);

        if (policy is null)
            throw new KeyNotFoundException("SLA policy not found.");

        var existingPolicy =
            await _slaPolicyRepository.GetByPriorityAsync(
                request.Priority);

        if (existingPolicy is not null &&
            existingPolicy.Id != policy.Id)
        {
            throw new InvalidOperationException(
                "An SLA policy already exists for this priority.");
        }

        policy.Update(
            request.Name.Trim(),
            request.Priority,
            request.ResponseTimeMinutes,
            request.ResolutionTimeMinutes);

        await _slaPolicyRepository.UpdateAsync(policy);

        return MapToResponse(policy);
    }

    private static SLAPolicyResponse MapToResponse(
        SLAPolicy policy)
    {
        return new SLAPolicyResponse
        {
            Id = policy.Id,
            Name = policy.Name,
            Priority = policy.Priority,
            ResponseTimeMinutes = policy.ResponseTimeMinutes,
            ResolutionTimeMinutes = policy.ResolutionTimeMinutes,
            CreatedAt = policy.CreatedAt,
            UpdatedAt = policy.UpdatedAt
        };
    }
}