using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jabberwocky.Core.Testing;
using Jabberwocky.Core.Utils;
using NUnit.Framework;

namespace Jabberwocky.Core.Tests.Utils
{
	[TestFixture]
	public class LockUtilTests
	{
		const int NumIterations = 100000;
		const int ContentionRatio = 10; // 1 in 10 (10%)

		[Test]
		public void GetLockHighConcurrencyTest()
		{
			Parallel.For(0, NumIterations, i => (i % ContentionRatio).GetLock().Dispose());
		}

		[Test]
		public void GetLockAsyncHighConcurrencyTest()
		{
			var bag = new ConcurrentBag<Task>();
			Parallel.For(0, NumIterations, i => bag.Add(Task.Run(async () => (await (i % ContentionRatio).GetLockAsync()).Dispose())));

			Task.WaitAll(bag.ToArray());
		}

		[Test]
		public void GetLock()
		{
			// Arrange

			const string key1 = "1";

			IDisposable lock1 = null;
			IDisposable lock2 = null;

			var mreSync1 = new ManualResetEventSlim(false);
			var mreSync2 = new ManualResetEventSlim(false);
			var taskSync = new ManualResetEventSlim(false);
			var taskSync2 = new ManualResetEventSlim(false);

			var mre1 = new ManualResetEventSlim(false);
			var mre2 = new ManualResetEventSlim(false);

			var task1 = new Thread(() =>
				{
					taskSync2.Wait();   // Wait for Task2 to actually start
					using (lock1 = key1.GetLock())
					{
						taskSync.Set();	// guarantee that Task1 executes first
						mreSync1.Set();
						mre1.Wait();
					}
					mreSync1.Set();	// T1P2
					mre1.Wait();
				});

			var task2 = new Thread(() =>
			{
				taskSync2.Set();
				taskSync.Wait(); // wait for task1 to enter lock first
				using (lock2 = key1.GetLock())
				{
					mreSync2.Set();	// T2P2
					mre2.Wait();
				}
			});

			task1.Start();
			task2.Start();

			try
			{
				// Wait for both tasks to start (waiting on Task1 implicitly waits on Task2 (due to taskSync2)
				mreSync1.Wait();

				Assert.IsNotNull(lock1);
				Assert.IsNull(lock2);

				// Continue Task1 (verify that task2 threadstate is 'blocked')
				if (!SpinWait.SpinUntil(() => task2.ThreadState == ThreadState.WaitSleepJoin, TimeSpan.FromMilliseconds(100)))
					Assert.Fail("Timeout expired trying to validate state of task2.");
				Assert.AreEqual(ThreadState.WaitSleepJoin, task2.ThreadState);
				mre1.Set();

				// Wait now for tasks to get into position T1P2 and T2P2 respectively
				mreSync1.Wait();
				mreSync2.Wait();

				// Task 1 is out of lock, Task 2 should be IN lock (assignment should have occurred)
				Assert.IsNotNull(lock2);
				dynamic dLock1 = DynamicWrapper.For((object)lock1);
				dynamic dLock2 = DynamicWrapper.For((object)lock2);

				// Assert that the actual lock objects are the same reference
				Assert.AreSame(dLock1._lockState, dLock2._lockState);
				Assert.AreEqual(1, ((ICollection)dLock1._container).Count);

				// Release the locks
				mre1.Set();
				mre2.Set();

				//Task.WaitAll(task1, task2);
				Assert.IsTrue(new[] { task1, task2 }.All(t => t.Join(100)), "At least one thread did not complete in a timely manner!");

				// Now show that another lock on the same key (with no other re-entrant threads grabbing the lock) will be different
				using (var lock3 = key1.GetLock())
				{
					dynamic dLock3 = DynamicWrapper.For((object)lock3);
					Assert.AreNotSame(dLock3._lockState, dLock2._lockState);
					Assert.AreEqual(1, ((ICollection)dLock1._container).Count);
				}

				// Show that the 'elastic' dictionary is empty
				Assert.AreEqual(0, ((ICollection)dLock1._container).Count);
			}
			finally
			{
				// Cleanup
				mreSync1.Dispose();
				mreSync2.Dispose();
				mre1.Dispose();
				mre2.Dispose();
				taskSync.Dispose();
				taskSync2.Dispose();
			}
		}

		[Test]
		public void GetLockAsync()
		{
			// Arrange

			const string key1 = "1";

			IDisposable lock1 = null;
			IDisposable lock2 = null;

			var mreSync1 = new ManualResetEventSlim(false);
			var mreSync2 = new ManualResetEventSlim(false);
			var taskSync = new ManualResetEventSlim(false);
			var taskSync2 = new ManualResetEventSlim(false);

			var mre1 = new ManualResetEventSlim(false);
			var mre2 = new ManualResetEventSlim(false);

			var task1 = Task.Run(async () =>
				{
					taskSync2.Wait();	// Wait for Task2 to actually start
					using (lock1 = await key1.GetLockAsync())
					{
						taskSync.Set();	// guarantee that Task1 executes first
						mreSync1.Set();
						mre1.Wait();
					}
					mreSync1.Set();	// T1P2
					mre1.Wait();
				});

			var task2 = Task.Run(async () =>
			{
				taskSync2.Set();
				taskSync.Wait(); // wait for task1 to enter lock first
				using (lock2 = await key1.GetLockAsync())
				{
					mreSync2.Set();	// T2P2
					mre2.Wait();
				}
			});

			try
			{
				// Wait for both tasks to start (waiting on Task1 implicitly waits on Task2 (due to taskSync2)
				mreSync1.Wait();

				Assert.IsNotNull(lock1);
				Assert.IsNull(lock2);

				// Continue Task1 (verify that task2 threadstate is 'blocked')
				if (!SpinWait.SpinUntil(() => task2.Status == TaskStatus.WaitingForActivation, TimeSpan.FromMilliseconds(100)))
					Assert.Fail("Timeout expired trying to validate state of task2.");
				Assert.AreEqual(TaskStatus.WaitingForActivation, task2.Status);
				mre1.Set();

				// Wait now for tasks to get into position T1P2 and T2P2 respectively
				mreSync1.Wait();
				mreSync2.Wait();

				// Task 1 is out of lock, Task 2 should be IN lock (assignment should have occurred)
				Assert.IsNotNull(lock2);
				dynamic dLock1 = DynamicWrapper.For((object)lock1);
				dynamic dLock2 = DynamicWrapper.For((object)lock2);

				// Assert that the actual lock objects are the same reference
				Assert.AreSame(dLock1._lockState, dLock2._lockState);
				Assert.AreEqual(1, ((ICollection)dLock1._container).Count);

				// Release the locks
				mre1.Set();
				mre2.Set();

				Task.WaitAll(new[] { task1, task2 }, TimeSpan.FromMilliseconds(100));
				Assert.IsTrue(new[] { task1, task2 }.All(t => t.IsCompleted), "At least one task did not complete in a timely manner!");

				// Now show that another lock on the same key (with no other re-entrant threads grabbing the lock) will be different
				// AND: this is a SYNC lock, not ASYNC!
				using (var lock3 = key1.GetLock())
				{
					dynamic dLock3 = DynamicWrapper.For((object)lock3);
					Assert.AreNotSame(dLock3._lockState, dLock2._lockState);
					Assert.AreEqual(1, ((ICollection)dLock1._container).Count);
				}

				// Show that the 'elastic' dictionary is empty
				Assert.AreEqual(0, ((ICollection)dLock1._container).Count);
			}
			finally
			{
				// Cleanup
				mreSync1.Dispose();
				mreSync2.Dispose();
				mre1.Dispose();
				mre2.Dispose();
				taskSync.Dispose();
				taskSync2.Dispose();
			}
		}
	}
}
