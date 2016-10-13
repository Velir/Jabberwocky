using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Jabberwocky.Core.Utils
{
	public static class LockingUtil
	{
		#region Object.GetLock()

		/// <summary>
		/// Gets a lock for an object (discriminated by this object's ToString()), which can be shared by other objects that call this extension method.
		/// To release the lock, the caller must call Dispose on the returned object.
		/// </summary>
		/// <remarks>
		/// The lock context returned is guaranteed to be the same reference if two or more objects (with equivalent ToString() keys) 
		/// call this method 'at the same time'. The lock context is not guaranteed to be the same between calls if requests to GetLock are executed 
		/// serially with no concurrent calls with the same key.
		/// Does not lock on 'this'.
		/// </remarks>
		/// <param name="obj">The object to get a lock for</param>
		/// <typeparam name="T">The type of this object</typeparam>
		/// <returns>A lock unique to this object's ToString() value</returns>
		public static LockObject GetLock<T>(this T obj)
		{
			return GetLock(obj.ToString());
		}

		/// <summary>
		/// Gets a lock for an object (discriminated by this object's ToString()), which can be shared by other objects that call this extension method.
		/// To release the lock, the caller must call Dispose on the returned object.
		/// </summary>
		/// <remarks>
		/// The lock context returned is guaranteed to be the same reference if two or more objects (with equivalent ToString() keys) 
		/// call this method 'at the same time'. The lock context is not guaranteed to be the same between calls if requests to GetLock are executed 
		/// serially with no concurrent calls with the same key.
		/// Does not lock on 'this'.
		/// </remarks>
		/// <param name="obj">The object to get a lock for</param>
		/// <param name="token">Cancellation token</param>
		/// <typeparam name="T">The type of this object</typeparam>
		/// <returns>A lock unique to this object's ToString() value</returns>
		public static Task<LockObject> GetLockAsync<T>(this T obj, CancellationToken token = default(CancellationToken))
		{
			return GetLockAsync(obj.ToString(), token);
		}

		/// <summary>
		/// Gets a lock for an object (discriminated by the provided 'key' param), which can be shared by other objects that call this extension method.
		/// To release the lock, the caller must call Dispose on the returned object.
		/// </summary>
		/// <remarks>
		/// The lock context returned is guaranteed to be the same reference if two or more objects (with equivalent keys) 
		/// call this method 'at the same time'. The lock context is not guaranteed to be the same between calls if requests to GetLock are executed 
		/// serially with no concurrent calls with the same key.
		/// Does not lock on the key.
		/// </remarks>
		/// <param name="key">A unique key to create a lock for</param>
		/// <returns>A lock unique to the specified key value</returns>
		public static LockObject GetLock(string key)
		{
			LockObject @lock = default(LockObject);
			do
			{
				@lock.Dispose();

				@lock = new LockObject(key, LockManager.Locks, LockManager.Locks.GetOrAdd(key, _ => new LockState()));
				@lock.Acquire();
			} while (!@lock.IsValid);
			return @lock;
		}

		/// <summary>
		/// Gets a lock for an object (discriminated by the provided 'key' param), which can be shared by other objects that call this extension method.
		/// To release the lock, the caller must call Dispose on the returned object.
		/// </summary>
		/// <remarks>
		/// The lock context returned is guaranteed to be the same reference if two or more objects (with equivalent keys) 
		/// call this method 'at the same time'. The lock context is not guaranteed to be the same between calls if requests to GetLock are executed 
		/// serially with no concurrent calls with the same key.
		/// Does not lock on the key.
		/// </remarks>
		/// <param name="key">A unique key to create a lock for</param>
		/// <param name="token">A cancellation token</param>
		/// <returns>A lock unique to the specified key value</returns>
		public static async Task<LockObject> GetLockAsync(string key, CancellationToken token = default(CancellationToken))
		{
			LockObject @lock = default(LockObject);
			do
			{
				@lock.Dispose();

				@lock = new LockObject(key, LockManager.Locks, LockManager.Locks.GetOrAdd(key, _ => new LockState()));
				await @lock.AcquireAsync(token).ConfigureAwait(false);
			} while (!@lock.IsValid);
			return @lock;
		}

		private static class LockManager
		{
			private static readonly Lazy<ConcurrentDictionary<string, LockState>> LazyLocks = new Lazy<ConcurrentDictionary<string, LockState>>();
			internal static ConcurrentDictionary<string, LockState> Locks => LazyLocks.Value;
		}

		public struct LockObject : IDisposable
		{
			private readonly ConcurrentDictionary<string, LockState> _container;
			private readonly LockState _lockState;
			private readonly string _key;

			internal bool IsValid => _lockState.IsValid;

			internal LockObject(string key, ConcurrentDictionary<string, LockState> container, LockState lockState)
			{
				_key = key;
				_container = container;
				_lockState = lockState;
				Interlocked.Increment(ref _lockState.RefCount);
			}

			internal void Acquire()
			{
				_lockState.Semaphore.Wait();
			}

			internal Task AcquireAsync(CancellationToken token = default(CancellationToken))
			{
				return _lockState.Semaphore.WaitAsync(token);
			}

			public void Dispose()
			{
				if (_lockState == null) return;

				try
				{
					if (Interlocked.Decrement(ref _lockState.RefCount) <= 0)
					{
						if (!_lockState.IsValid) return;

						_lockState.IsValid = false;
						LockState val;
						_container.TryRemove(_key, out val);
					}
				}
				finally
				{
					_lockState.Semaphore.Release();
				}
			}
		}

		internal sealed class LockState
		{
			internal int RefCount;
			// Note that we no longer dispose of this resource; we will now rely on non-deterministic finalization cleanup
			// Justification: http://source.roslyn.codeplex.com/#Microsoft.CodeAnalysis.Workspaces/Utilities/AsyncSemaphore.cs,91c6ef418f692fa1,references
			internal readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);
			internal volatile bool IsValid = true;
		}

		#endregion
	}
}
