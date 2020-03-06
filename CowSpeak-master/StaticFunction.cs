using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CowSpeak
{
	internal class StaticFunction : FunctionBase
	{
		public MethodInfo Definition;

		public StaticFunction(string Name, MethodInfo Definition, Type type, Parameter[] Parameters, bool isMethod = false) {
			this.DefinitionType = DefinitionType.Language;
			this.type = type;
			this.Definition = Definition;

			string Params = "";
			for (int i = 0; i < Parameters.Length; i++)
			{
				Params += Parameters[i].Type + " " + Parameters[i].Name;
				if (i != Parameters.Length - 1)
					Params += ", ";
			}

   			this.ProperUsage = type.Name + " " + Name + "(" + Params + ")";
			this.Name = Name;
			this.Parameters = Parameters;
			this.isMethod = isMethod;
		}

		public override Any Execute(string usage)
		{
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				throw new Exception("Invalid usage of function: '" + usage);

			string usage_temp = usage;
			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = ParseParameters(usage).ToList();
			Variable Object = null;
			if (isMethod && Parameters.Length != parameters.Count - 1)
			{
				if (usage_temp.IndexOf(".") == -1)
					throw new Exception(Name + " can only be called as a method");
				Object = CowSpeak.GetVariable(usage_temp.Substring(0, usage_temp.IndexOf(".")));
			}

			CheckParameters(parameters);

			try
			{
				CowSpeak.StackTrace.Add(Usage);

				List<object> InvocationParams = new List<object>();
				if (isMethod)
					InvocationParams.Add(Object);
				foreach (var parameter in parameters)
					InvocationParams.Add(parameter.Value);
				
				object ReturnValue = Definition.Invoke(null, InvocationParams.ToArray()); // obj is null because the function should be static
				CowSpeak.StackTrace.RemoveAt(CowSpeak.StackTrace.Count - 1);

				if (ReturnValue == null)
					return null; // Probably a void function

				Type ReturnedType = null;

				if (ReturnValue is Any)
					return (Any)ReturnValue;
				else if (ReturnValue is System.Array)
					ReturnedType = Type.GetType(((System.Array)ReturnValue).GetType());
				else
					ReturnedType = Type.GetType(ReturnValue.GetType());

				return new Any(ReturnedType, ReturnValue);
			}
			catch (System.Reflection.TargetInvocationException ex)
			{
				System.Exception baseEx = ex.GetBaseException();
				if (baseEx is Exception)
					throw baseEx as Exception;
				throw new Exception("FunctionCall '" + Name + "' returned an exception: " + baseEx.Message);
			}
		}
	}

	internal class FunctionAttr : System.Attribute
	{
		public string Name;

		public FunctionAttr(string Name)
		{
			this.Name = Name;
		}

		public static List< FunctionBase > GetFunctions()
		{
            List< FunctionBase > functions = new List< FunctionBase >();

			var methods = typeof(Functions).GetMethods().Where(m => m.GetCustomAttributes(typeof(FunctionAttr), false).Length > 0 || m.GetCustomAttributes(typeof(MethodAttr), false).Length > 0); // Get all methods from the function class with the function attributes

            foreach (MethodInfo method in methods)
			{
				if (method == null)
					continue;

				FunctionAttr functionAttr = (FunctionAttr)System.Attribute.GetCustomAttribute(method, typeof(FunctionAttr)); // get attribute for function
				MethodAttr methodAttr = (MethodAttr)System.Attribute.GetCustomAttribute(method, typeof(MethodAttr)); // get attribute for method

				string Name;
				bool isMethod = false;
				if (methodAttr != null)
				{
					Name = methodAttr.Name;
					isMethod = true;
				}
				else if (functionAttr != null)
					Name = functionAttr.Name;
				else
					continue; // skip method, it does not have either attribute   

				// Convert MethodInfo parameters to StaticFunction parameters
				List<Parameter> Parameters = new List<Parameter>();
				foreach (var _param in method.GetParameters())
				{
					var paramType = Type.GetType(_param.ParameterType, false);
					if (paramType == null)
						continue;

					Parameters.Add(new Parameter(paramType, _param.Name));
				}

				var ReturnType = Type.GetType(method.ReturnType, false);
				if (ReturnType == null)
					ReturnType = Type.Any;

                functions.Add(new StaticFunction(Name, method, ReturnType, Parameters.ToArray(), isMethod));
            }

            return functions;
        } // get a list of all methods from the Functions class that have this attribute
	}

	internal class MethodAttr : FunctionAttr
	{
		public MethodAttr(string Name) : base(Name){}
	}
}