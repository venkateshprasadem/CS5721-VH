using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace VaccineHub.Service.Concurrency
{
    internal sealed class AsyncSimulatedLock
    {
        private sealed class RefCounted<T>
        {
            public RefCounted(T value)
            {
                RefCount = 1;
                Value = value;
            }

            // We don't need it as such but it will give count of threads waiting to acquire the lock
            public int RefCount { get; set; }
            public T Value { get; [UsedImplicitly] private set; }
        }

        private static readonly Dictionary<string, RefCounted<SemaphoreSlim>> SemaphoreSlims
            = new();

        private static SemaphoreSlim GetOrCreate(string key)
        {
            RefCounted<SemaphoreSlim> item;
            lock (SemaphoreSlims)
            {
                if (SemaphoreSlims.TryGetValue(key, out item))
                {
                    ++item.RefCount;
                }
                else
                {
                    item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    SemaphoreSlims[key] = item;
                }
            }
            return item.Value;
        }

        public static async Task<IDisposable> LockAsync(string key, CancellationToken cancellationToken)
        {
            await GetOrCreate(key).WaitAsync(cancellationToken).ConfigureAwait(false);
            return new Releaser { Key = key };
        }

        private sealed class Releaser : IDisposable
        {
            public string Key { get; set; }

            public void Dispose()
            {
                RefCounted<SemaphoreSlim> item;

                lock (SemaphoreSlims)
                {
                    item = SemaphoreSlims[Key];
                    --item.RefCount;
                    if (item.RefCount == 0)
                        SemaphoreSlims.Remove(Key);
                }

                item.Value.Release();
            }
        }
    }
}