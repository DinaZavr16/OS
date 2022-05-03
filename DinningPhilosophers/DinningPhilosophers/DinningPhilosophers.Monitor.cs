using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mime;
using System.Threading;

namespace DinningPhilosophers
{
	public partial class DinningPhilosophers : IDisposable
	{
		class MonitorSolution
		{
			private readonly object _lock = new object(); // Спрячем объект для Монитора от всех, чтобы без дедлоков
			private DateTime?[] _waitTimes = new DateTime?[PhilosophersAmount]; // Время ожидания потока

			public void Run(int i, CancellationToken token)
			{
				var watch = new Stopwatch();
				while (true)
				{
					watch.Restart();
					TakeForks(i);
					_waitTime[i] += watch.ElapsedMilliseconds;
					eatenFood[i] = (eatenFood[i] + 1) % (int.MaxValue - 1);
					watch.Restart();
					PutForks(i);
					_waitTime[i] += watch.ElapsedMilliseconds;
					Think(i);
					if (token.IsCancellationRequested) break;
				}
			}

			bool CanIEat(int i)
			{
				// Если есть вилки:
				if (forks[Left(i)] != 0 && forks[Right(i)] != 0)
					return false;
				var now = DateTime.Now;
				// Может, если соседи не более голодные, чем текущий
				foreach (var p in new int[] { LeftPhilosopher(i), RightPhilosopher(i) })
					if (_waitTimes[p] != null && now - _waitTimes[p] > now - _waitTimes[i])
						return false;
				return true;
			}

			void TakeForks(int i)
			{
				// Зайти в Монитор
				bool lockTaken = false;
				Monitor.Enter(_lock, ref lockTaken);
				try
				{
					_waitTimes[i] = DateTime.Now;
					// Освобождаем лок, если не выполненно сложное условие. И ждем пока кто-нибудь сделает Pulse / PulseAll
					while (!CanIEat(i))
						Monitor.Wait(_lock);
					forks[Left(i)] = i + 1;
					forks[Right(i)] = i + 1;
					_waitTimes[i] = null;
				}
				finally
				{
					if (lockTaken) Monitor.Exit(_lock);
				}
			}

			void PutForks(int i)
			{
				bool lockTaken = false;
				Monitor.Enter(_lock, ref lockTaken);
				try
				{
					forks[Left(i)] = 0;
					forks[Right(i)] = 0;
					// Освободить все потоки в очереди ПОСЛЕ вызова Monitor.Exit 
					Monitor.PulseAll(_lock);
				}
				finally
				{
					if (lockTaken) Monitor.Exit(_lock);
				}
			}
		}
	}
}
