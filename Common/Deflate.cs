using System;
using System.IO;
using System.IO.Compression;
using System.Text;

public static class Deflate {

	public static string cs_head = string.Empty;

	public static byte[] Decompress(Stream stream) {
		try {
			stream.Position = 0;
			using (MemoryStream ms = new MemoryStream()) {
				using (DeflateStream def = new DeflateStream(stream, CompressionMode.Decompress)) {
					byte[] data = new byte[1024];
					int size = 0;

					while ((size = def.Read(data, 0, data.Length)) > 0) {
						ms.Write(data, 0, size);
					}
				}
				return ms.ToArray();
			}
		} catch { return (stream as MemoryStream).ToArray(); };
	}
	public static byte[] Decompress(byte[] bt) {
		return Decompress(new MemoryStream(bt));
	}

	public static byte[] Compress(string text) {
		if (text.Trim().StartsWith("using ")) {
			text = Deflate.cs_head + text;
		}
		return Compress(Encoding.UTF8.GetBytes(text));
	}
	public static byte[] Compress(byte[] bt) {
		return Compress(bt, 0, bt.Length);
	}
	public static byte[] Compress(byte[] bt, int startIndex, int length) {
		using (MemoryStream ms = new MemoryStream()) {
			using (DeflateStream def = new DeflateStream(ms, CompressionMode.Compress)) {
				def.Write(bt, startIndex, length);
			}
			return ms.ToArray();
		}
	}
}
