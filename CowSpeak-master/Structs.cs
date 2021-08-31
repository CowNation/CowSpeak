using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak
{
    public class Structures : Dictionary<string, FastStruct>
    {
        public Structures() : base()
        {

        }

		public new void Add(string key, FastStruct value)
		{
			if (ContainsKey(key))
				throw new BaseException("Structure '" + key + "' has already been defined");

			base.Add(key, value);
		}

		public new FastStruct this[string key]
		{
			get
			{
				if (!ContainsKey(key))
					throw new BaseException("Could not find structure: " + key);

				return base[key];
			}
			set
			{
				this[key] = value;
			}
		}

		public FastStruct Create(FastStruct @struct)
		{
			if (ContainsKey(@struct.Name))
				throw new BaseException("Structure '" + @struct.Name + "' has already been defined");

			Add(@struct.Name, @struct);

			return @struct;
		}
	}
}
