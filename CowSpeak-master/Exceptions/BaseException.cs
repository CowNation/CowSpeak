namespace CowSpeak.Exceptions
{
	public class BaseException : System.Exception
	{
		public int ErrorLine;
		public string ErrorFile;
		public new string StackTrace = ""; // Hides System.Exception.StackTrace

		public BaseException(string Message) : base((Interpreter.CurrentFile != "" || Interpreter.CurrentLine != -1 ? "(" + (Interpreter.CurrentFile != "" ? Interpreter.CurrentFile : "") + (Interpreter.CurrentFile != "" ? ", " : "") + (Interpreter.CurrentLine != -1 ? Interpreter.CurrentLine.ToString() : "") + ") " : "") + Message)
		{
			ErrorLine = Interpreter.CurrentLine;
			ErrorFile = Interpreter.CurrentFile;

			for (int i = 0; i < Interpreter.StackTrace.Count; i++)
			{
				StackTrace += Interpreter.StackTrace[i] + (i < Interpreter.StackTrace.Count - 1 ? "->" : "");
			}

			Interpreter.StackTrace.Clear(); // prevent recursion exception false positives
		}
	}
}