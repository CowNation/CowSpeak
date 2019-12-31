using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;

namespace CowSpeak{
	public class ByteArray{
		public ByteArray(){}
		public byte[] bytes;

		public string GetAddress(){
			GCHandle handle = GCHandle.Alloc(bytes[0], GCHandleType.WeakTrackResurrection);
			int address = GCHandle.ToIntPtr(handle).ToInt32();
			return address.ToString();
		}

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

		public object Get(){
			return ByteArrayToObject(bytes);
		}

		public void Set(object to){
			bytes = ObjectToByteArray(to);
		}
	}
}