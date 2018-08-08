﻿using System;
using BenchmarkDotNet.Running;

namespace Chronological.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<QueryCreation>();
            Console.ReadKey();
        }
    }
}
