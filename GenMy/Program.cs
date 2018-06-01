using System;
using System.Threading;

namespace GenMy
{
    class Program
    {
        static void Main(string[] args)
        {
			if (args != null && args.Length == 0) args = new[] { "?" };
			ManualResetEvent wait = new ManualResetEvent(false);
			new Thread(() => {
				Thread.CurrentThread.Join(TimeSpan.FromSeconds(1));
				ConsoleApp app = new ConsoleApp(args, wait);
			}).Start();
			wait.WaitOne();
			return;
		}
    }
}
