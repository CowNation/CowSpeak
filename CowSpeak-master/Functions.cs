using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace CowSpeak
{
	static class Functions
	{
		#region ALL_METHODS
		[FunctionAttr("Any.ToString", Syntax.Types.String, "", true)]
		public static Any ToString(Any[] parameters)
		{
			return new Any(Type.String, parameters[0].Get().ToString());
		}
		#endregion

		#region STRING_METHODS
		[FunctionAttr(Syntax.Types.String + ".OccurrencesOf", Syntax.Types.Integer, Syntax.Types.String + " counter", true)]
		public static Any OccurrencesOf(Any[] parameters)
		{
			return new Any(Type.Integer, Utils.OccurrencesOf(parameters[0].Get().ToString(), parameters[1].Get().ToString()));
		}

		[FunctionAttr(Syntax.Types.String + ".Sub" + Syntax.Types.c_String, Syntax.Types.String, Syntax.Types.Integer + " startIndex, " + Syntax.Types.Integer + " endIndex", true)]
		public static Any SubString(Any[] parameters)
		{
			string str = parameters[0].Get().ToString();
			int startIndex = (int)parameters[1].Get();
			int endIndex = (int)parameters[2].Get();

			return new Any(Type.String, str.Substring(startIndex, endIndex - startIndex));
		}

		[FunctionAttr(Syntax.Types.String + "." + Syntax.Types.c_Character + "At", Syntax.Types.Character, Syntax.Types.Integer + " index", true)]
		public static Any CharacterAt(Any[] parameters)
		{
			return new Any(Type.Character, parameters[0].Get().ToString()[(int)parameters[1].Get()]);
		}

		[FunctionAttr(Syntax.Types.String + ".Length", Syntax.Types.Integer, "", true)]
		public static Any Length(Any[] parameters)
		{
			return new Any(Type.Integer, parameters[0].Get().ToString().Length);
		}

		[FunctionAttr(Syntax.Types.String + ".Remove", Syntax.Types.String, Syntax.Types.Integer + " startIndex, " + Syntax.Types.Integer + " endIndex", true)]
		public static Any Remove(Any[] parameters)
		{
			int startIndex = (int)parameters[1].Get();
			int endIndex = (int)parameters[2].Get();
			return new Any(Type.String, parameters[0].Get().ToString().Remove(startIndex, endIndex - startIndex));
		}

		[FunctionAttr(Syntax.Types.String + ".Insert", Syntax.Types.String, Syntax.Types.Integer + " startIndex, " + Syntax.Types.String + " value", true)]
		public static Any Insert(Any[] parameters)
		{
			int index = (int)parameters[1].Get();
			string toPut = parameters[2].Get().ToString();
			return new Any(Type.String, parameters[0].Get().ToString().Insert(index, toPut));
		}

		[FunctionAttr(Syntax.Types.String + ".IndexOf", Syntax.Types.Integer, Syntax.Types.String + " value", true)]
		public static Any IndexOf(Any[] parameters)
		{
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(Type.String, parameters[0].Get().ToString().IndexOf(toSearchFor));
		}

		[FunctionAttr(Syntax.Types.String + ".LastIndexOf", Syntax.Types.Integer, Syntax.Types.String + " value", true)]
		public static Any LastIndexOf(Any[] parameters)
		{
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(Type.String, parameters[0].Get().ToString().LastIndexOf(toSearchFor));
		}

		[FunctionAttr(Syntax.Types.String + ".To" + Syntax.Types.c_Integer, Syntax.Types.Integer, "", true)]
		public static Any ToInteger(Any[] parameters)
		{
			int o;
			string str = parameters[0].Get().ToString();

			if (Utils.IsHexadecimal(str))
				return new Any(Type.Integer, int.Parse(str.Substring(2), System.Globalization.NumberStyles.HexNumber));

			if (System.Int32.TryParse(str, out o))
				return new Any(Type.Integer, o);
			else
				throw new Exception("Could not convert " + Syntax.Types.String + " to an " + Syntax.Types.Integer);
		}

		[FunctionAttr(Syntax.Types.String + ".To" + Syntax.Types.c_Decimal, Syntax.Types.Decimal, "", true)]
		public static Any ToDecimal(Any[] parameters)
		{
			double o;
			string str = parameters[0].Get().ToString();
			if (System.Double.TryParse(str, out o))
				return new Any(Type.Decimal, o);
			else
				throw new Exception("Could not convert " + Syntax.Types.String + " to an " + Syntax.Types.Decimal);
		}
		#endregion

		#region CHARACTER_METHODS
		[FunctionAttr(Syntax.Types.Character + ".ToUpper", Syntax.Types.Character, "", true)]
		public static Any ToUpper(Any[] parameters)
		{
			return new Any(Type.Character, System.Char.ToUpper(parameters[0].Get().ToString()[0]));
		}
		[FunctionAttr(Syntax.Types.Character + ".ToLower", Syntax.Types.Character, "", true)]
		public static Any ToLower(Any[] parameters)
		{
			return new Any(Type.Character, System.Char.ToLower(parameters[0].Get().ToString()[0]));
		}
		[FunctionAttr(Syntax.Types.Character + ".ToInteger", Syntax.Types.Integer, "", true)]
		public static Any _ToInteger(Any[] parameters)
		{
			return new Any(Type.Integer, (int)parameters[0].Get().ToString()[0]);
		}
		#endregion

		#region INTEGER_METHODS
		[FunctionAttr(Syntax.Types.Integer + ".ToHexadecimal", Syntax.Types.String, "", true)]
		public static Any ToHexadecimal(Any[] parameters)
		{
			return new Any(Type.String, ((int)parameters[0].Get()).ToString("X"));
		}
		[FunctionAttr(Syntax.Types.Integer + ".ToCharacter", Syntax.Types.Character, "", true)]
		public static Any ToCharacter(Any[] parameters)
		{
			return new Any(Type.Character, (char)(int)parameters[0].Get());
		}
		#endregion

		#region INTEGER64_METHODS
		[FunctionAttr(Syntax.Types.Integer64 + ".ToHexadecimal", Syntax.Types.String, "", true)]
		public static Any ToHexadecimal64(Any[] parameters) => new Any(Type.String, ((long)parameters[0].Get()).ToString("X"));
		[FunctionAttr(Syntax.Types.Integer64 + ".ToCharacter", Syntax.Types.Character, "", true)]
		public static Any ToCharacter64(Any[] parameters) => new Any(Type.Character, (char)(long)parameters[0].Get());
		#endregion

		[FunctionAttr("Sin", Syntax.Types.Decimal, Syntax.Types.Decimal + " a")]
		public static Any Sin(Any[] parameters)
		{
			return new Any(Type.Decimal, System.Math.Sin((double)parameters[0].Get()));
		}

		[FunctionAttr("Cos", Syntax.Types.Decimal, Syntax.Types.Decimal + " d")]
		public static Any Cos(Any[] parameters)
		{
			return new Any(Type.Decimal, System.Math.Cos((double)parameters[0].Get()));
		}

		[FunctionAttr("Tan", Syntax.Types.Decimal, Syntax.Types.Decimal + " a")]
		public static Any Tan(Any[] parameters)
		{
			return new Any(Type.Decimal, System.Math.Tan((double)parameters[0].Get()));
		}

		[FunctionAttr("Import", Syntax.Types.Void, Syntax.Types.String + " modulePath")]
		public static Any Import(Any[] parameters)
		{
			string modulePath = parameters[0].Get().ToString();

			// make modulePath relative to CurrentFile as long as modulePath is relative
			if (!Path.IsPathRooted(modulePath))
			{
				if (CowSpeak.CurrentFile.IndexOf("/") != -1)
					modulePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("/") + 1) + modulePath;
				if (CowSpeak.CurrentFile.IndexOf("\\") != -1)
					modulePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("\\") + 1) + modulePath;
			}

			CowSpeak.Exec(modulePath);
			return null;
		}

		[FunctionAttr("GetHtmlFromUrl", Syntax.Types.String, Syntax.Types.String + " url")]
		public static Any GetHtmlFromUrl(Any[] parameters)
		{
			string urlAddress = parameters[0].Get().ToString();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream receiveStream = response.GetResponseStream();
				StreamReader readStream = null;

				if (string.IsNullOrWhiteSpace(response.CharacterSet))
					readStream = new StreamReader(receiveStream);
				else
					readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

				string data = readStream.ReadToEnd();

				response.Close();
				readStream.Close();
				return new Any(Type.String, data);
			}

			throw new Exception("Cannot get a HttpWebResponse from '" + urlAddress + "'");
		}

		[FunctionAttr("Abs", Syntax.Types.Decimal, Syntax.Types.Decimal + " value")]
		public static Any Abs(Any[] parameters)
		{
			return new Any(Type.Decimal, System.Math.Abs((double)parameters[0].Get()));
		}

		[FunctionAttr("Round", Syntax.Types.Integer, Syntax.Types.Decimal + " value")]
		public static Any Round(Any[] parameters)
		{
			return new Any(Type.Integer, System.Math.Round((double)parameters[0].Get()));
		}

		[FunctionAttr("Sqrt", Syntax.Types.Decimal, Syntax.Types.Decimal + " value")]
		public static Any Sqrt(Any[] parameters)
		{
			return new Any(Type.Integer, System.Math.Sqrt((double)parameters[0].Get()));
		}

		[FunctionAttr("Pow", Syntax.Types.Decimal, Syntax.Types.Decimal + " x, " + Syntax.Types.Decimal + " y")]
		public static Any Pow(Any[] parameters)
		{
			return new Any(Type.Decimal, System.Math.Pow((double)parameters[0].Get(), (double)parameters[1].Get()));
		}

		[FunctionAttr("Sleep", Syntax.Types.Void, Syntax.Types.Integer + " ms")]
		public static Any Sleep(Any[] parameters)
		{
			Thread.Sleep((int)parameters[0].Get());
			return null;
		}

		[FunctionAttr("Read" + Syntax.Types.c_Character, Syntax.Types.Character, "")]
		public static Any ReadCharacter(Any[] parameters)
		{
			return new Any(Type.Character, (char)System.Console.ReadKey().KeyChar);
		}

		[FunctionAttr("ClearConsole", Syntax.Types.Void, "")]
		public static Any ClearConsole(Any[] parameters)
		{
			System.Console.Clear();
			return null;
		}

		[FunctionAttr("Define", Syntax.Types.Void, Syntax.Types.String + " from, " + Syntax.Types.String + " to")]
		public static Any Define(Any[] parameters) 
		{
			CowSpeak.Definitions.Add(new Definition
			{
				from = parameters[0].Get().ToString(),
				to = parameters[1].Get().ToString(),
				DefinitionType = DefinitionType.User
			});
			return null;
		}

		[FunctionAttr("Evaluate", Syntax.Types.Decimal, Syntax.Types.String + " toExec")]
		public static Any _Evaluate(Any[] parameters) // evaluates an expression
		{
			Any evaluatedValue = new Any();
			evaluatedValue.vType = Type.Decimal;
			evaluatedValue.Set(Evaluate.EvaluateExpression(parameters[0].Get().ToString()));
			if (((double)evaluatedValue.Get()).ToString().IndexOf(".") == -1)
				evaluatedValue.vType = Type.Integer; // decimal not found, we can convert to int

			return evaluatedValue;
		}

		private static string toStr(Any toPrep)
		{
			return Utils.FixBoolean(toPrep.Get().ToString());
		}

		[FunctionAttr("Random" + Syntax.Types.c_Integer, Syntax.Types.Integer, Syntax.Types.Integer + " minimum, " + Syntax.Types.Integer + " maximum")]
		public static Any RandomInteger(Any[] parameters)
		{
			int minimum = (int)parameters[0].Get();
			int maximum = (int)parameters[1].Get() + 1;

			if (minimum > maximum)
				throw new Exception("Minimum must be less than the maximum");

			return new Any(Type.Integer, Utils.rand.Next(minimum, maximum));
		}

		[FunctionAttr("Print", Syntax.Types.Void, Syntax.Types.Any + " text")]
		public static Any Print(Any[] parameters)
		{
			System.Console.Write(parameters[0].Get().ToString());
			return null;
		}

		[FunctionAttr("Exit", Syntax.Types.Void, Syntax.Types.Integer + " exitCode")]
		public static Any Exit(Any[] parameters)
		{
			System.Environment.Exit((int)parameters[0].Get());
			return null;
		}

		[FunctionAttr("ThrowError", Syntax.Types.Void, Syntax.Types.String + " errorText")]
		public static Any ThrowError(Any[] parameters)
		{
			throw new Exception(parameters[0].Get().ToString());
		}

		[FunctionAttr("Input" + Syntax.Types.c_String, Syntax.Types.String, "")]
		public static Any InputString(Any[] parameters)
		{
			return new Any(Type.String, System.Console.ReadLine());	
		}

		[FunctionAttr("Input" + Syntax.Types.c_Integer, Syntax.Types.Integer, "")]
		public static Any InputInteger(Any[] parameters)
		{
			string built = "";
			System.ConsoleKeyInfo key = new System.ConsoleKeyInfo();
			while (key.Key != System.ConsoleKey.Enter)
			{
				Thread.Sleep(100);

				key = System.Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf("-") == -1 && key.KeyChar == '-'))
					built += key.KeyChar;
				else if (key.Key == System.ConsoleKey.Backspace)
					built = built.Remove(built.Length - 1, 1);
				else
					System.Console.Write("\b \b");
			}
			int _out = -1;
			System.Int32.TryParse(built, out _out);
			return new Any(Type.Integer, _out);
		}

		[FunctionAttr("Input" + Syntax.Types.c_Character, Syntax.Types.Character, "")]
		public static Any InputCharacter(Any[] parameters)
		{
			return new Any(Type.Character, System.Console.ReadKey().KeyChar);
		}

		[FunctionAttr("Input" + Syntax.Types.c_Decimal, Syntax.Types.Decimal, "")]
		public static Any InputDecimal(Any[] parameters)
		{
			string built = "";
			System.ConsoleKeyInfo key = new System.ConsoleKeyInfo();
			while (key.Key != System.ConsoleKey.Enter)
			{
				Thread.Sleep(50);

				key = System.Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf(".") == -1 && key.KeyChar == '.') || (built.IndexOf("-") == -1 && key.KeyChar == '-'))
					built += key.KeyChar;
				else if (key.Key == System.ConsoleKey.Backspace)
					built = built.Remove(built.Length - 1, 1);
				else
					System.Console.Write("\b \b");
			}
			float _out = -1;
			System.Single.TryParse(built, out _out);
			return new Any(Type.Decimal, _out);
		}
	}
}