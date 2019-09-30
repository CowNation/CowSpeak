using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CowSpeak{
	public class ByteArray{
		public ByteArray(){}
		public byte[] bytes;

		public static Object ByteArrayToObject(byte[] arrBytes)
		{
			using (var memStream = new MemoryStream())
			{
				var binForm = new BinaryFormatter();
				memStream.Write(arrBytes, 0, arrBytes.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				var obj = binForm.Deserialize(memStream);
				return obj;
			}
		}

		public static byte[] ObjectToByteArray(Object obj)
		{
			BinaryFormatter bf = new BinaryFormatter();
			using (var ms = new MemoryStream())
			{
				bf.Serialize(ms, obj);
				return ms.ToArray();
			}
		}

		public object Get(){
			return ByteArrayToObject(bytes);
		}

		public void Set(object to){
			bytes = ObjectToByteArray(to);
		}
	}
}