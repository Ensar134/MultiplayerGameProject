using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Mirror
{
    public class Pool<T>
    {
        readonly Queue<T> objects = new Queue<T>(); // Stack<T> yerine Queue<T> kullanıldı
        readonly Func<T> objectGenerator;

        public Pool(Func<T> objectGenerator, int initialCapacity)
        {
            this.objectGenerator = objectGenerator;

            for (int i = 0; i < initialCapacity; ++i)
                objects.Enqueue(objectGenerator()); // Push() yerine Enqueue() kullanıldı
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get() => objects.Count > 0 ? objects.Dequeue() : objectGenerator(); // Pop() yerine Dequeue() kullanıldı

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(T item) => objects.Enqueue(item); // Push() yerine Enqueue() kullanıldı

        public int Count => objects.Count;
    }
}