using System;

namespace Model {

	[Serializable]
	public class BuildInfo {

		private string _path;
		private byte[] _data;

		public BuildInfo() { }
		public BuildInfo(string path, byte[] data) {
			_path = path;
			_data = data;
		}

		public string Path {
			get { return _path; }
		}

		public byte[] Data {
			get { return _data; }
		}
	}
}
