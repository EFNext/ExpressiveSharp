window.BENCHMARK_DATA = {
  "lastUpdate": 1774577268864,
  "repoUrl": "https://github.com/EFNext/ExpressiveSharp",
  "entries": {
    "ExpressiveSharp Benchmarks": [
      {
        "commit": {
          "author": {
            "email": "koen@linker.io",
            "name": "Koen",
            "username": "koenbeuk"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a9440785e2368b66b88376e7af8d16030c0e9080",
          "message": "Merge pull request #1 from EFNext/feat/benchmarks\n\nAdd BenchmarkDotNet benchmarks and manual workflow trigger",
          "timestamp": "2026-03-26T02:41:07Z",
          "tree_id": "526868cfed670be224baa02c9752e084f2c095ea",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/a9440785e2368b66b88376e7af8d16030c0e9080"
        },
        "date": 1774493228187,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7449.431231180827,
            "unit": "ns",
            "range": "± 32.17607456219591"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1431.3472785949707,
            "unit": "ns",
            "range": "± 8.359365398343579"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.264405578374863,
            "unit": "ns",
            "range": "± 0.03564736767975162"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 156.06185166041055,
            "unit": "ns",
            "range": "± 0.6354592603704473"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 23257.474731445312,
            "unit": "ns",
            "range": "± 8868.792740024131"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1457.099199930827,
            "unit": "ns",
            "range": "± 12.70238708804063"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.071030596892038,
            "unit": "ns",
            "range": "± 0.008586112900647626"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 79.6187004049619,
            "unit": "ns",
            "range": "± 0.0623814909935875"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21373.67755126953,
            "unit": "ns",
            "range": "± 7361.186582722646"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2640.909123738607,
            "unit": "ns",
            "range": "± 451.36903462745744"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.018926819165548,
            "unit": "ns",
            "range": "± 0.048183241797869196"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.35463358958562,
            "unit": "ns",
            "range": "± 0.07126683209334768"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 23245.353474934895,
            "unit": "ns",
            "range": "± 6010.6834205835075"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3229.1914723714194,
            "unit": "ns",
            "range": "± 598.7514220193137"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.658914928634961,
            "unit": "ns",
            "range": "± 0.017282446279907562"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.06920741001765,
            "unit": "ns",
            "range": "± 0.08257621094868814"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 143199.29622395834,
            "unit": "ns",
            "range": "± 31468.948682512677"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 7669.970245361328,
            "unit": "ns",
            "range": "± 188.8991716430791"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.98447684943676,
            "unit": "ns",
            "range": "± 0.023811105732101934"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 7716.534535725911,
            "unit": "ns",
            "range": "± 196.44370128311223"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 81624.36832682292,
            "unit": "ns",
            "range": "± 4771.394110840469"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.60315499206384,
            "unit": "ns",
            "range": "± 0.028605368195234834"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.790227914849917,
            "unit": "ns",
            "range": "± 0.06820961457855054"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.036056146025658,
            "unit": "ns",
            "range": "± 0.017347238191653295"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 672670.5071614584,
            "unit": "ns",
            "range": "± 101156.1883795923"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 690870.6087239584,
            "unit": "ns",
            "range": "± 107406.98383956829"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1935933.0729166667,
            "unit": "ns",
            "range": "± 318306.0797608759"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1301776.9869791667,
            "unit": "ns",
            "range": "± 211884.62758293742"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1862741.08984375,
            "unit": "ns",
            "range": "± 356906.5469636124"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 856392.9342447916,
            "unit": "ns",
            "range": "± 71028.88161069974"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 910163.3170572916,
            "unit": "ns",
            "range": "± 58371.300269983905"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 41470554.5,
            "unit": "ns",
            "range": "± 186161.27428830584"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42143900.72222223,
            "unit": "ns",
            "range": "± 21606.87645595286"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 120080312.88888888,
            "unit": "ns",
            "range": "± 12130812.87551315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 81626142.25,
            "unit": "ns",
            "range": "± 21623059.369146433"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 75125876.16666667,
            "unit": "ns",
            "range": "± 15156860.232778925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 115138433.33333333,
            "unit": "ns",
            "range": "± 20729953.19214322"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 72219961.58333333,
            "unit": "ns",
            "range": "± 22393867.14749421"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 431167248,
            "unit": "ns",
            "range": "± 47140937.36931881"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 447992485.3333333,
            "unit": "ns",
            "range": "± 31015785.96797847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 420761243.6666667,
            "unit": "ns",
            "range": "± 26783950.688367285"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 415225933,
            "unit": "ns",
            "range": "± 46511000.65145334"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 428612235.3333333,
            "unit": "ns",
            "range": "± 40942803.227333695"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "koen@linker.io",
            "name": "Koen",
            "username": "koenbeuk"
          },
          "committer": {
            "email": "koen@linker.io",
            "name": "Koen",
            "username": "koenbeuk"
          },
          "distinct": true,
          "id": "4baf56025b74fee6eba46ad099035494a84bfa97",
          "message": "fix: Refactor method call handling for static and instance methods in ExpressionTreeEmitter",
          "timestamp": "2026-03-27T02:01:36Z",
          "tree_id": "b0a9a8551bcbe9a7123acadf47101f6925eab583",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/4baf56025b74fee6eba46ad099035494a84bfa97"
        },
        "date": 1774577268574,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7441.059305826823,
            "unit": "ns",
            "range": "± 32.0050710126471"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1473.2399406433105,
            "unit": "ns",
            "range": "± 3.2285439471767203"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.341028084357579,
            "unit": "ns",
            "range": "± 0.012686965851345199"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 159.14128653208414,
            "unit": "ns",
            "range": "± 1.2999310579199854"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21617.61024983724,
            "unit": "ns",
            "range": "± 8068.4251625973475"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1482.7219772338867,
            "unit": "ns",
            "range": "± 32.15710243432876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.049133623639742,
            "unit": "ns",
            "range": "± 0.052317416365397225"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 88.0809498031934,
            "unit": "ns",
            "range": "± 0.566595196723453"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 22181.477091471355,
            "unit": "ns",
            "range": "± 8058.600339671902"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2542.290827433268,
            "unit": "ns",
            "range": "± 95.06105306665015"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.992615501085917,
            "unit": "ns",
            "range": "± 0.026303467029649603"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.56920643647511,
            "unit": "ns",
            "range": "± 0.09260781133451418"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 21908.642008463543,
            "unit": "ns",
            "range": "± 6297.565248236394"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3037.452547709147,
            "unit": "ns",
            "range": "± 108.38036285798607"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.677397256096204,
            "unit": "ns",
            "range": "± 0.12816781877471195"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.11124565203985,
            "unit": "ns",
            "range": "± 0.10162459443285382"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 147727.16796875,
            "unit": "ns",
            "range": "± 20165.26314470694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8015.039352416992,
            "unit": "ns",
            "range": "± 559.6747508158047"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.105825439095497,
            "unit": "ns",
            "range": "± 0.03482090756773965"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8102.771881103516,
            "unit": "ns",
            "range": "± 201.41206292411348"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79415.27864583333,
            "unit": "ns",
            "range": "± 1084.7815458473356"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.58901341756185,
            "unit": "ns",
            "range": "± 0.022397246394744885"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.95463447769483,
            "unit": "ns",
            "range": "± 0.04451918271811882"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.13165533542633,
            "unit": "ns",
            "range": "± 0.014283045859176112"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 688018.5299479166,
            "unit": "ns",
            "range": "± 86451.78269441966"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 691155.2135416666,
            "unit": "ns",
            "range": "± 109608.39131792379"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 934107.2679036459,
            "unit": "ns",
            "range": "± 90659.51793884508"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1876945.7799479167,
            "unit": "ns",
            "range": "± 152359.06827196968"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1881677.0651041667,
            "unit": "ns",
            "range": "± 154838.60837872582"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 854310.6380208334,
            "unit": "ns",
            "range": "± 66357.29460430666"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 871439.2298177084,
            "unit": "ns",
            "range": "± 54927.40442447315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 43186571.5,
            "unit": "ns",
            "range": "± 87129.19729850824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41401725.97222222,
            "unit": "ns",
            "range": "± 287222.32681907393"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 71178709.5,
            "unit": "ns",
            "range": "± 1356732.741957425"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 86781378,
            "unit": "ns",
            "range": "± 31996287.809535738"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 81733867.5,
            "unit": "ns",
            "range": "± 18643073.455465138"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 79093733.58333333,
            "unit": "ns",
            "range": "± 19483853.610039737"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 79640457.58333333,
            "unit": "ns",
            "range": "± 13073268.121748274"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 424586870.3333333,
            "unit": "ns",
            "range": "± 39437031.90194503"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 415918085.3333333,
            "unit": "ns",
            "range": "± 27033241.581714284"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 439954391.6666667,
            "unit": "ns",
            "range": "± 39624444.61917146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 448417190.6666667,
            "unit": "ns",
            "range": "± 48233049.530011944"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 450475196.6666667,
            "unit": "ns",
            "range": "± 62353309.074029826"
          }
        ]
      }
    ]
  }
}