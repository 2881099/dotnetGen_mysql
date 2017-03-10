using System.Linq;
using System.Reflection;

public class ModuleInfo {
	public string Name { get; set; }

	public Assembly Assembly { get; set; }

	public string ShortName {
		get {
			return Name.Split('.').Last();
		}
	}

	public string Path { get; set; }
}