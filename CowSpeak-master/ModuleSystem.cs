using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak
{
	public class ModuleSystem
	{
		public ModuleSystem()
		{
			ImportStandardModules();
		}

		public Dictionary<string, Module> LoadedModules = new Dictionary<string, Module>();

		// Modules for the specific running OS will be imported at runtime
		private bool osSpecificModules = true;
		public bool OSSpecificModules
		{
			get => osSpecificModules;
			set
			{
				osSpecificModules = value;
				var osModule = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? typeof(Modules.Windows) : (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? typeof(Modules.Linux) : null);
				var osModuleAttr = osModule != null ? (ModuleAttribute)Attribute.GetCustomAttribute(osModule, typeof(ModuleAttribute)) : null;

				if (osModule == null || osModuleAttr == null)
					return;

				if (value)
				{
					if (!LoadedModules.ContainsKey(osModuleAttr.Name))
						Import(osModule);
				}
				else
				{
					if (LoadedModules.ContainsKey(osModuleAttr.Name))
						Remove(osModule);
				}
			}
		}

		public void Import(System.Type container)
		{
			ModuleAttribute moduleAttribute = (ModuleAttribute)Attribute.GetCustomAttribute(container, typeof(ModuleAttribute));
			if (moduleAttribute == null)
				throw new ModuleException("Cannot import module, container class is missing ModuleAttribute");

			// under the hood, a static class is simply sealed and abstract. we can only import static module types
			if (!container.IsStatic())
				throw new ModuleException("Cannot import module, container class isn't static");

			if (LoadedModules.ContainsKey(moduleAttribute.Name))
				throw new ModuleException("Module '" + moduleAttribute.Name + "' has already been imported");

			var module = new Module(container);

			LoadedModules.Add(moduleAttribute.Name, module);

			var definedFunctions = module.DefinedFunctions;
			foreach (var function in definedFunctions)
			{
				if (Interpreter.Functions.FunctionExists(function.Key))
					throw new ModuleException("A function by the name of '" + function.Key + "' has already been defined");

				Interpreter.Functions.Add(function.Key, function.Value);
			}

			var definitions = module.Definitions;
			foreach (var definition in definitions)
			{
				var tempDefinition = definition;
				tempDefinition.DefinitionType = DefinitionType.Static;

				if (Interpreter.Definitions.ContainsKey(tempDefinition.From))
					throw new ModuleException("A definition already exists for '" + tempDefinition.From + "'");

				Interpreter.Definitions.Add(tempDefinition.From, tempDefinition);
			}
		}
		
		public void Remove(System.Type container)
		{
			ModuleAttribute moduleAttribute = (ModuleAttribute)Attribute.GetCustomAttribute(container, typeof(ModuleAttribute));
			if (moduleAttribute == null)
				throw new ModuleException("Cannot remove module, container class is missing ModuleAttribute");

			if (!LoadedModules.ContainsKey(moduleAttribute.Name))
				throw new ModuleException("Cannot remove module, it hasn't been loaded");

			Module module = LoadedModules[moduleAttribute.Name];

			var definedFunctions = module.DefinedFunctions;
			foreach (var function in definedFunctions)
			{
				if (!Interpreter.Functions.FunctionExists(function.Key))
					throw new ModuleException("A function by the name of '" + function.Key + "' doesn't exist");

				Interpreter.Functions.Remove(function.Key);
			}

			module.Definitions.ForEach(x =>
			{
				if (!Interpreter.Definitions.ContainsKey(x.From))
					throw new ModuleException("A definition doesn't exist for '" + x.From + "'");

				Interpreter.Definitions.Remove(x.From);
			});

			LoadedModules.Remove(moduleAttribute.Name);
		}

		private void ImportStandardModules()
		{
			// an array of all classes in the assembly with the ModuleAttribute and the ModuleAttribute.AutoImportAttribute
			var standardModules = typeof(Modules.Main).Assembly.GetTypes().Where(x => x.IsStatic() &&
				x.GetCustomAttributes(typeof(ModuleAttribute), false).Length > 0 &&
				x.GetCustomAttributes(typeof(ModuleAttribute.AutoImportAttribute), false).Length > 0).ToArray();

			// usually this will import the necessary modules like main, the method modules, ect
			foreach (var module in standardModules)
				Import(module);

			if (OSSpecificModules)
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
					Import(typeof(Modules.Windows));
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
					Import(typeof(Modules.Linux));
			}
		}
	}
}
