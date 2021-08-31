using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
* This module is for those who dislike how CowSpeak's defult type names are so long
* The existing CowSpeak type names will still work as well
* Function names will still refer to the types as their defualt CowSpeak type names (Ex: myInt.ToCharacter())
* Exceptions will still also refer to the standard CowSpeak type names
* This will probably cause several bugs whenever using this module's short type names
*/

namespace CowSpeak.Modules
{
    [ModuleAttribute.AutoImport]
    [Module("Shorter Type Names")]
    public static class ShorterTypeNames
    {
        // Types
        public static Definition Integer = new Definition
        {
            From = "int",
            To = "integer"
        };
        public static Definition Integer64 = new Definition
        {
            From = "int64",
            To = "integer64"
        };
        public static Definition Boolean = new Definition
        {
            From = "bool",
            To = "boolean"
        };
        public static Definition Character = new Definition
        {
            From = "char",
            To = "character"
        };

        // Array Types
        public static Definition IntegerArray = new Definition
        {
            From = "IntArray",
            To = "IntegerArray"
        };
        public static Definition BooleanArray = new Definition
        {
            From = "BoolArray",
            To = "BooleanArray"
        };
        public static Definition CharacterArray = new Definition
        {
            From = "CharArray",
            To = "CharacterArray"
        };
    }
}
