namespace CowSpeak
{
	public class Exception : System.Exception
	{
		public int ErrorLine;
		public string ErrorFile;
		public new string StackTrace = ""; // Hides System.Exception.StackTrace

		public Exception(string Message) : base((CowSpeak.CurrentFile != "" || CowSpeak.CurrentLine != -1 ? "(" + (CowSpeak.CurrentFile != "" ? CowSpeak.CurrentFile : "") + (CowSpeak.CurrentFile != "" ? ", " : "") + (CowSpeak.CurrentLine != -1 ? CowSpeak.CurrentLine.ToString() : "") + ") " : "") + Message)
		{
			ErrorLine = CowSpeak.CurrentLine;
			ErrorFile = CowSpeak.CurrentFile;

			for (int i = 0; i < CowSpeak.StackTrace.Count; i++)
			{
				StackTrace += CowSpeak.StackTrace[i] + (i < CowSpeak.StackTrace.Count - 1 ? "->" : "");
			}

			CowSpeak.StackTrace.Clear(); // prevent recursion exception false positives
		}
	}
}