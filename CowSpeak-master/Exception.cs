namespace CowSpeak
{
	public class Exception : System.Exception
	{
		public int ErrorLine;
		public string ErrorFile;
		public new string StackTrace = ""; // Hides System.Exception.StackTrace

		public Exception(string Message) : base(Message)
		{
			ErrorLine = CowSpeak.CurrentLine;
			ErrorFile = CowSpeak.CurrentFile;

			for (int i = 0; i < CowSpeak.StackTrace.Count; i++)
			{
				StackTrace += CowSpeak.StackTrace[i] + (i < CowSpeak.StackTrace.Count - 1 ? "->" : "");
			}
		}
	}
}