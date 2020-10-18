using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttribute : System.Attribute
	{
		public string Name;

		public ModuleAttribute(string Name) => this.Name = Name;
	}

	public class Module
	{
		private System.Type container;

		public Module(System.Type container) => this.container = container;

		public void GenerateMdDocumentation(string filepath)
		{
			var functions = DefinedFunctions.Values.ToList();
			functions.Sort(delegate(FunctionBase one, FunctionBase two)
			{
				return one.Name.CompareTo(two.Name);
			}); // sort alphabetically

			var definitions = Definitions;
			definitions.Sort(delegate (Definition one, Definition two)
			{
				return one.From.CompareTo(two.From);
			});

			int largestParamsNum = 0;
			foreach (var function in functions)
			{
				if (function.Parameters.Length > largestParamsNum)
					largestParamsNum = function.Parameters.Length;
			}

			string[] filelines = new string[functions.Count + definitions.Count + (definitions.Count > 0 ? 6 : 4)];
			if (functions.Count > 0)
			{
				filelines[0] = "## <div align=\"center\">Functions</div>";
				filelines[1] = "Type | Name";
				filelines[2] = "--- | ---";
				for (int i = 0; i < largestParamsNum; i++)
				{
					filelines[1] += " | Parameter " + i;
					filelines[2] += " | ---";
				}

				for (int i = 0; i < functions.Count; i++)
				{
					filelines[i + 3] = functions[i].ReturnType.Name + " | " + functions[i].Name;
					foreach (var parameter in functions[i].Parameters)
					{
						filelines[i + 3] += " | " + parameter.Type.Name + " " + parameter.Name;
					}
					if (functions[i].Parameters.Length == 0)
						filelines[i + 3] += " | ";
				}
			}

			if (definitions.Count > 0)
			{
				filelines[functions.Count + 2] = "## <div align=\"center\">Definitions</div>";
				filelines[functions.Count + 3] = "From | To";
				filelines[functions.Count + 4] = "--- | ---";

				for (int i = 0; i < definitions.Count; i++)
					filelines[i + functions.Count + 5] = definitions[i].From + " | " + definitions[i].To;
			}

			if (functions.Count == 0)
			{
				if (definitions.Count == 0)
					filelines = new string[0];
				else
					Array.Copy(filelines, 2, filelines, 0, definitions.Count + 3);
			}

			File.WriteAllLines(filepath, filelines);
		}

		public Functions DefinedFunctions
		{
			get
			{
				Functions functions = new Functions();

				var methods = container.GetMethods().Where(m => m.GetCustomAttributes(typeof(FunctionAttribute), false).Length > 0 || m.GetCustomAttributes(typeof(MethodAttribute), false).Length > 0); // Get all methods from the function class with the function attributes

				foreach (MethodInfo method in methods)
				{
					if (method == null)
						continue;

					FunctionAttribute functionAttr = ((FunctionAttribute[])System.Attribute.GetCustomAttributes(method, typeof(FunctionAttribute))).Where(attr => !(attr is MethodAttribute)).FirstOrDefault(); // get attribute for function
					MethodAttribute[] methodAttrs = (MethodAttribute[])System.Attribute.GetCustomAttributes(method, typeof(MethodAttribute)); // get attributes for method

					string name = method.Name;
					bool isMethod = false, staticOnly = false;
					if (methodAttrs.Length > 0)
					{
						if (methodAttrs[0].Name.Length > 0)
							name = methodAttrs[0].Name;

						isMethod = true;
						staticOnly = methodAttrs[0].StaticOnly;
					}
					else if (functionAttr != null)
					{
						if (functionAttr.Name.Length > 0)
							name = functionAttr.Name;
					}
					else
						continue; // skip method, it does not have either attribute   

					List<Parameter> parameters = new List<Parameter>();
					var methodParameters = method.GetParameters();

					if (isMethod)
					{
						if (staticOnly)
						{
							if (methodParameters.Length == 0 || (methodParameters.Length > 0 && methodParameters[0].ParameterType != typeof(Type)))
								throw new ModuleException("StaticOnly method's first parameter must be of type CowSpeak.Type");
						}
						else
						{
							if (methodParameters.Length == 0 || (methodParameters.Length > 0 && methodParameters[0].ParameterType != typeof(Variable)))
								throw new ModuleException("Method's first parameter must be of type CowSpeak.Variable");
						}
					}

					foreach (var parameter in methodParameters)
					{
						var paramType = Type.GetType(parameter.ParameterType, false);
						if (paramType == null)
							continue;

						parameters.Add(new Parameter(paramType, parameter.Name));
					}

					var returnType = Type.GetType(method.ReturnType, false);
					if (returnType == null)
						returnType = Type.Object;

					if (functions.ContainsKey(name))
						throw new ModuleException("A function by the name of '" + name + "' has already been defined in this module");

					functions.Add(name, new StaticFunction(name, method, returnType, parameters.ToArray(), isMethod, staticOnly));
					if (methodAttrs.Length > 1)
					{
						// Add the function again but with the additional MethodAttribute's name(s)
						for (int i = 1; i < methodAttrs.Length; i++)
							functions.Add(methodAttrs[i].Name, new StaticFunction(methodAttrs[i].Name, method, returnType, parameters.ToArray(), isMethod, methodAttrs[i].StaticOnly));
					}
				}

				return functions;
			}
		}

		public List<Definition> Definitions
		{
			get
			{
				List<Definition> definitions = container.GetFields().Where(x => x.FieldType == typeof(Definition) && x.IsStatic).Select(x => (Definition)x.GetValue(null)).ToList();
				foreach (var nestedEnum in container.GetNestedTypes().Where(type => type.IsEnum && type.CustomAttributes.Any(attr => attr.AttributeType == typeof(Definition.EnumAttribute))))
				{
					foreach (var name in Enum.GetNames(nestedEnum))
					{
						definitions.Add(new Definition
						{
							From = name,
							To = Convert.ChangeType(Enum.Parse(nestedEnum, name), Enum.GetUnderlyingType(nestedEnum)).ToString() // get the value from the name
						});
					}
				}
				return definitions;
			}
		}
	}
}
