using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace CowSpeak
{
	internal class ByteArray
	{
		public ByteArray()
		{

		}

		public byte[] bytes;

		public static object ByteArrayToObject(byte[] arrBytes)
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

		protected object Get()
		{
			if (bytes == null)
				throw new Exception("Object not initialized");

			return ByteArrayToObject(bytes);
		}

		protected void Set(object to) => bytes = ObjectToByteArray(to);
	}
}