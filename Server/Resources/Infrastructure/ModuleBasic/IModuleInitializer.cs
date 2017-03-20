using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public interface IModuleInitializer {
	void Init(IApplicationBuilder services);
}