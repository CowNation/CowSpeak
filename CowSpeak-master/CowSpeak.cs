using System.Collections.Generic;

/*
* Contains constants relating to the functionality of CowSpeak
* TODOS (In no particular order):
* Parameters for static functions & replace some keywords with static functions (run, print)
* Add types for Variables
* Add user-defined functions
* move some Utils to CowSpeak class so lineIndex isn't passed every FATAL_ERROR call
*/

namespace CowSpeak{
	public class CowSpeak{
		public static List< Function > staticFX = new List< Function >{
			new Function("VOID_exit()", Functions.VOID_exit),
			new Function("VOID_pause()", Functions.VOID_pause),
			new Function("VOID_clrConsole()", Functions.VOID_clrConsole)
		};

		public bool shouldDebug = false;

		public List< Variable > Vars = new List< Variable >();

		public CowSpeak(bool shouldDebug = false){
			this.shouldDebug = shouldDebug;
		}

		public void Exec(string fileName){
			new FileLexer(this, new CowConfig.readConfig(fileName).GetLines(), shouldDebug);
		}
	}
}