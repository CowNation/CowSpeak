using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CowSpeak.Exceptions;

namespace CowSpeak
{
	internal class StaticFunction : FunctionBase
	{
		public MethodInfo Definition;
		// The method may only be called statically (only Type.Method and not Instance.Method)
		public bool StaticOnly;

		public StaticFunction(string Name, MethodInfo Definition, Type type, Parameter[] Parameters, bool IsMethod = false, bool StaticOnly = false) {
			DefinitionType = DefinitionType.Static;
			this.ReturnType = type;
			this.Definition = Definition;

			string Params = "";
			for (int i = 0; i < Parameters.Length; i++)
			{
				Params += Parameters[i].Type + " " + Parameters[i].Name;
				if (i != Parameters.Length - 1)
					Params += ", ";
			}

			this.Name = Name;
			this.Parameters = Parameters;
			this.IsMethod = IsMethod;
			this.StaticOnly = StaticOnly;
		}

		public override Any Execute(string usage)
		{
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				throw new BaseException("Invalid usage of function: " + usage);

			string usage_temp = usage;
			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = ParseParameters(usage).ToList();

			Variable caller = null;
			Type callerType = null;
			if (IsMethod && Parameters.Length != parameters.Count - 1)
			{
				if (usage_temp.IndexOf(".") == -1)
					throw new BaseException(Name + " can only be called as a method");

				if (!StaticOnly)
					caller = Interpreter.Vars[usage_temp.Substring(0, usage_temp.IndexOf("."))];
				else
					callerType = Type.GetType(usage_temp.Substring(0, usage_temp.IndexOf(".")));
			}

			CheckParameters(parameters);

			try
			{
				Interpreter.StackTrace.Add(Usage);

				List<object> InvocationParams = new List<object>();

				if (IsMethod)
				{
					if (StaticOnly)
						InvocationParams.Add(callerType);
					else
						InvocationParams.Add(caller);
				}

				var definitionParams = Definition.GetParameters().ToList();

				if (IsMethod) // remove first object/type information parameter
					definitionParams.RemoveAt(0);

				for (int i = 0; i < parameters.Count; i++)
					InvocationParams.Add(parameters[i].GetValue(definitionParams[i].ParameterType));
				
				object returnValue = Definition.Invoke(null, InvocationParams.ToArray()); // obj is null because the function should be static
				Interpreter.StackTrace.RemoveAt(Interpreter.StackTrace.Count - 1);

				if (returnValue == null)
					return null; // Probably a void function

				Type returnedType = null;

				if (returnValue is Any)
					return (Any)returnValue;
				else if (returnValue is Array)
					returnedType = Type.GetType(((Array)returnValue).GetType());
				else
					returnedType = Type.GetType(returnValue.GetType());

				return new Any(returnedType, returnValue);
			}
			catch (TargetInvocationException ex)
			{
				Exception baseEx = ex.GetBaseException();
				if (baseEx is BaseException)
					throw baseEx as BaseException;
					
				throw baseEx;
			}
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class FunctionAttribute : Attribute
	{
		public string Name;

		public FunctionAttribute(string Name)
		{
			this.Name = Name;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class MethodAttribute : FunctionAttribute
	{
		// The method may only be called statically (only Type.Method and not Instance.Method)
		public bool StaticOnly;

		public MethodAttribute(string Name, bool StaticOnly = false) : base(Name)
		{
			this.StaticOnly = StaticOnly;
		}
	}
}