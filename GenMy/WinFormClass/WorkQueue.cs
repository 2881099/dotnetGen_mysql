using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

public class WorkQueue : WorkQueue<AnonymousHandler> {
	public WorkQueue() : this(16, -1) { }
	public WorkQueue(int thread)
		: this(thread, -1) {
	}
	public WorkQueue(int thread, int capacity) {
		base.Thread = thread;
		base.Capacity = capacity;
		base.Process += delegate(AnonymousHandler ah) {
			ah();
		};
	}
}

public class WorkQueue<T> : IDisposable {
	public delegate void WorkQueueProcessHandler(T item);
	public event WorkQueueProcessHandler Process;

	private int _thread = 16;
	private int _capacity = -1;
	private int _work_index = 0;
	private Dictionary<int, WorkInfo> _works = new Dictionary<int, WorkInfo>();
	private object _works_lock = new object();
	private Queue<T> _queue = new Queue<T>();
	private object _queue_lock = new object();

	public WorkQueue() : this(16, -1) { }
	public WorkQueue(int thread)
		: this(thread, -1) {
	}
	public WorkQueue(int thread, int capacity) {
		_thread = thread;
		_capacity = capacity;
	}

	public void Enqueue(T item) {
		lock (_queue_lock) {
			if (_capacity > 0 && _queue.Count >= _capacity) return;
			_queue.Enqueue(item);
		}
		lock (_works_lock) {
			foreach (WorkInfo w in _works.Values) {
				if (w.IsWaiting) {
					w.Set();
					return;
				}
			}
		}
		if (_works.Count < _thread) {
			if (_queue.Count > 0) {
				int index = 0;
				lock (_works_lock) {
					index = _work_index++;
					_works.Add(index, new WorkInfo());
				}
				new Thread(delegate() {
					WorkInfo work = _works[index];
					while (true) {
						List<T> de = new List<T>();
						if (_queue.Count > 0) {
							lock (_queue_lock) {
								if (_queue.Count > 0) {
									de.Add(_queue.Dequeue());
								}
							}
						}

						if (de.Count > 0) {
							try {
								this.OnProcess(de[0]);
							} catch {
							}
						}

						if (_queue.Count == 0) {
							work.WaitOne(TimeSpan.FromSeconds(20));

							if (_queue.Count == 0) {
								break;
							}
						}
					}
					lock (_works_lock) {
						_works.Remove(index);
					}
					work.Dispose();
				}).Start();
			}
		}
	}

	protected virtual void OnProcess(T item) {
		if (Process != null) {
			Process(item);
		}
	}

	#region IDisposable 成员

	public void Dispose() {
		lock (_queue_lock) {
			_queue.Clear();
		}
		lock (_works_lock) {
			foreach (WorkInfo w in _works.Values) {
				w.Dispose();
			}
		}
	}

	#endregion

	public int Thread {
		get { return _thread; }
		set {
			if (_thread != value) {
				_thread = value;
			}
		}
	}
	public int Capacity {
		get { return _capacity; }
		set {
			if (_capacity != value) {
				_capacity = value;
			}
		}
	}

	public int UsedThread {
		get { return _works.Count; }
	}
	public int Queue {
		get { return _queue.Count; }
	}

	public string Statistics {
		get {
			string value = string.Format(@"线程：{0}/{1}
队列：{2}

", _works.Count, _thread, _queue.Count);
			int[] keys = new int[_works.Count];
			try {
				_works.Keys.CopyTo(keys, 0);
			} catch {
				lock (_works_lock) {
					keys = new int[_works.Count];
					_works.Keys.CopyTo(keys, 0);
				}
			}
			foreach (int k in keys) {
				WorkInfo w = null;
				if (_works.TryGetValue(k, out w)) {
					value += string.Format(@"线程{0}：{1}
", k, w.IsWaiting);
				}
			}
			return value;
		}
	}

	class WorkInfo : IDisposable {
		private ManualResetEvent _reset = new ManualResetEvent(false);
		private bool _isWaiting = false;

		public void WaitOne(TimeSpan timeout) {
			try {
				_reset.Reset();
				_isWaiting = true;
				_reset.WaitOne(timeout);
			} catch { }
		}
		public void Set() {
			try {
				_isWaiting = false;
				_reset.Set();
			} catch { }
		}

		public bool IsWaiting {
			get { return _isWaiting; }
		}

		#region IDisposable 成员

		public void Dispose() {
			this.Set();
		}

		#endregion
	}
}