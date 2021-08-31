using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using CowSpeak.Exceptions;
using FastDelegate.Net;

namespace CowSpeak
{
	internal class StaticFunction : BaseFunction
	{
		public MethodInfo Definition;
		public Func<object, object[], object> BoundDelegate;

		// The method may only be called statically (only Type.Method and not Instance.Method)
		public bool StaticOnly;

		public StaticFunction(string name, MethodInfo definition, Type type, Parameter[] parameters, bool isMethod = false, bool staticOnly = false) {
			DefinitionType = DefinitionType.Static;
			this.ReturnType = type;
			this.Definition = definition;
			this.BoundDelegate = definition.Bind();

			string Params = "";
			for (int i = 0; i < parameters.Length; i++)
			{
				Params += parameters[i].Type + " " + parameters[i].Name;
				if (i != parameters.Length - 1)
					Params += ", ";
			}

			this.Name = name;
			this.Parameters = parameters;
			this.IsMethod = isMethod;
			this.StaticOnly = staticOnly;
		}

		public override Any Invoke(string usage)
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

				List<object> invocationParams = new List<object>();

				if (IsMethod)
				{
					if (StaticOnly)
						invocationParams.Add(callerType);
					else
					{
						if (caller.obj == null)
							throw new BaseException("Cannot call a method on a null object");

						invocationParams.Add(caller);
					}
				}

				var definitionParams = Definition.GetParameters().ToList();

				if (IsMethod) // remove first object/type information parameter
					definitionParams.RemoveAt(0);

				for (int i = 0; i < parameters.Count; i++)
				{
					invocationParams.Add(parameters[i].ConvertValue(definitionParams[i].ParameterType));
				}

				object returnValue = BoundDelegate.Invoke(null, invocationParams.ToArray()); // obj is null because the function should be static
				Interpreter.StackTrace.RemoveAt(Interpreter.StackTrace.Count - 1);

				Type returnedType = null;

				if (ReturnType == Types.Void)
					return null;
				else if (returnValue == null)
					returnedType = Types.Object;
				else if (returnValue is Any)
					return (Any)returnValue;
				else if (returnValue is Array)
					returnedType = Type.GetType(((Array)returnValue).GetType());
				else if (Type.GetType(returnValue.GetType(), false) != null)
					returnedType = Type.GetType(returnValue.GetType());
				else
					returnedType = Types.Object;

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
		public string Name = "";

		public FunctionAttribute()
		{

		}

		public FunctionAttribute(string name)
		{
			this.Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class MethodAttribute : FunctionAttribute
	{
		// The method may only be called statically (only Type.Method and not Instance.Method)
		public bool StaticOnly;

		public MethodAttribute(string name, bool staticOnly = false) : base(name)
		{
			this.StaticOnly = staticOnly;
		}
	}
}