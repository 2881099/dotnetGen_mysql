using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

public class ModuleViewLocationExpander : IViewLocationExpander {
	private const string _moduleKey = "module";

	public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations) {
		if (context.Values.ContainsKey(_moduleKey)) {
			var module = context.Values[_moduleKey];
			if (!string.IsNullOrWhiteSpace(module)) {
				var moduleViewLocations = new string[] {
						"/Module/" + module + "/Views/{1}/{0}.cshtml",
						"/Module/" + module + "/Views/Shared/{0}.cshtml"
					};
				viewLocations = moduleViewLocations.Concat(viewLocations);
			}
		}
		return viewLocations;
	}

	public void PopulateValues(ViewLocationExpanderContext context) {
		var controller = context.ActionContext.ActionDescriptor.DisplayName;
		var moduleName = controller.Split('.')[2];
		context.Values[_moduleKey] = moduleName;
	}
}