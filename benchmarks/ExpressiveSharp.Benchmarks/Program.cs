using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using ExpressiveSharp.Benchmarks;

BenchmarkSwitcher
    .FromAssembly(typeof(GeneratorBenchmarks).Assembly)
    .Run(args, DefaultConfig.Instance
        .WithOption(ConfigOptions.DisableOptimizationsValidator, true));
