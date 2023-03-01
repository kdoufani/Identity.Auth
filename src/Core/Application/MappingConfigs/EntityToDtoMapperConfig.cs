namespace Identity.Auth.Core.Application.MappingConfigs;

using Identity.Auth.Core.Domain.Dtos;
using Identity.Auth.Core.Domain.Entities;
using Mapster;

/// <summary>
/// Config class for all the mappings from entites to Dtos.
/// </summary>
public static class EntityToDtoMapperConfig
{
	/// <summary>
	/// Store all the mappings from entites to DTOs.
	/// </summary>
	public static void RegisterMappings()
	{
		TypeAdapterConfig<ApplicationUser, IdentityUserDto>.NewConfig()
			.Map(dest => dest.RefreshTokens, src => src.RefreshTokens.Select(r => r.Token))
			.Map(dest => dest.Roles, src => 
										src.UserRoles.Where(m => m.Role != null)
										.Select(q => q.Role!.Name));
	}
}
