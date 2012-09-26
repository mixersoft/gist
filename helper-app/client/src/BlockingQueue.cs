using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Snaphappi
{
	public class BlockingQueue<T> : IEnumerable<T>
	{
		Queue<T>  queue     = new Queue<T>();
		Semaphore semaphore = new Semaphore(0, Int32.MaxValue);

		public void Enqueue(T item)
		{
			lock (queue)
				queue.Enqueue(item);
			semaphore.Release();
		}

		public T Dequeue()
		{
			semaphore.WaitOne();
			lock (queue)
				return queue.Dequeue();
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (;;)
				yield return Dequeue();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
