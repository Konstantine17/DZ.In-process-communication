using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

        int[] sizes = { 100000, 1000000, 10000000 };
        foreach (var size in sizes)
        {
            int[] array = Enumerable.Range(1, size).ToArray();

            Console.WriteLine($"Array size: {size}");

            // Обычное вычисление суммы
            Stopwatch sw = Stopwatch.StartNew();
            long sum = 0;
            foreach (var item in array)
            {
                sum += item;
            }
            sw.Stop();
            Console.WriteLine($"Sequential sum: {sum}, Time: {sw.ElapsedMilliseconds} ms");

            // Параллельное вычисление суммы с использованием Thread
            sw.Restart();
            int numThreads = Environment.ProcessorCount;
            int chunkSize = size / numThreads;
            long[] partialSums = new long[numThreads];
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * chunkSize;
                int end = (i == numThreads - 1) ? size : start + chunkSize;
                int threadIndex = i;
                threads[i] = new Thread(() =>
                {
                    long partialSum = 0;
                    for (int j = start; j < end; j++)
                    {
                        partialSum += array[j];
                    }
                    partialSums[threadIndex] = partialSum;
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            long parallelSum = partialSums.Sum();
            sw.Stop();
            Console.WriteLine($"Parallel sum with Thread: {parallelSum}, Time: {sw.ElapsedMilliseconds} ms");

            //PLINQ
            sw.Restart();
            long linqsum = array.AsParallel().Sum(x => (long)x);
            sw.Stop();
            Console.WriteLine($"Parallel sum with LINQ: {linqsum}, Time: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine();
        }
