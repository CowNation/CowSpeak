namespace CowSpeak{
	class Exception : System.Exception {
		public int ErrorLine;
		public string ErrorFile;

		public Exception(string Message) : base(Message) {
			ErrorLine = CowSpeak.CurrentLine;
			ErrorFile = CowSpeak.CurrentFile;
		}
	};
}