namespace Identity.Auth.Core.Application.MappingConfigs;

/// <summary>
/// Config class for all mappings.
/// </summary>
public static class MappingConfig
{
	/// <summary>
	/// Store all mappings.
	/// </summary>
	public static void RegisterMappings()
	{
		EntityToDtoMapperConfig.RegisterMappings();
		DtoToEntityMapperConfig.RegisterMappings();
	}
}
