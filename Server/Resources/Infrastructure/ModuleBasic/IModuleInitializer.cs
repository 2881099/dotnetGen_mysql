using Microsoft.Extensions.DependencyInjection;

public interface IModuleInitializer {
	void Init(IServiceCollection serviceCollection);
}