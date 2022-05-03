using System;

namespace DinningPhilosophers
{
    class Philisophers
    {
        static void Main(string[] args)
        {
			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				Console.WriteLine("unhandled exception");
			};
			using (var dinner = new DinningPhilosophers())
			{
				if (args.Length == 0 || !Enum.TryParse(args[0], out DinningPhilosophers.EMethods method))
					method = DinningPhilosophers.EMethods.SpinLock;
				dinner.Run(method);
			}
		}
    }
}
