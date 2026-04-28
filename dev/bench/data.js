window.BENCHMARK_DATA = {
  "lastUpdate": 1777335799423,
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
      },
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
          "id": "0046f23c49432893c67c6cc3ff3454774e3f3cab",
          "message": "Merge pull request #3 from EFNext/feat/consolidated-generated-classes\n\nConsolidate generated expression classes into partial classes",
          "timestamp": "2026-03-27T03:01:58Z",
          "tree_id": "436b7a3c77f099d8626b43dea6248ba646a57a4e",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/0046f23c49432893c67c6cc3ff3454774e3f3cab"
        },
        "date": 1774580879807,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7441.995618184407,
            "unit": "ns",
            "range": "± 53.34367322053667"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1394.9290396372478,
            "unit": "ns",
            "range": "± 5.448077193463971"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.243337581555049,
            "unit": "ns",
            "range": "± 0.004796838048350559"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 157.61576430002847,
            "unit": "ns",
            "range": "± 0.6318785317680965"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21495.650451660156,
            "unit": "ns",
            "range": "± 8245.537226032178"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1583.762092590332,
            "unit": "ns",
            "range": "± 32.24036475542114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.017772729198137,
            "unit": "ns",
            "range": "± 0.0015140653065349923"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 78.97923870881398,
            "unit": "ns",
            "range": "± 0.05743791141001386"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21477.565958658855,
            "unit": "ns",
            "range": "± 8536.102203889383"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2532.2789001464844,
            "unit": "ns",
            "range": "± 36.94916769176209"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.962483286857605,
            "unit": "ns",
            "range": "± 0.013500908130004183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 80.43834642569225,
            "unit": "ns",
            "range": "± 0.043000335776041405"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 21131.25565592448,
            "unit": "ns",
            "range": "± 7503.25963032253"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3217.1149571736655,
            "unit": "ns",
            "range": "± 448.34585470493164"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.55583497385184,
            "unit": "ns",
            "range": "± 0.013996314964073843"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 55.91248110930125,
            "unit": "ns",
            "range": "± 0.30223701365251376"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 147281.13020833334,
            "unit": "ns",
            "range": "± 27583.343516251625"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 7898.979766845703,
            "unit": "ns",
            "range": "± 253.75951040334567"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.09974718093872,
            "unit": "ns",
            "range": "± 0.017304007770271496"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 12928.405354817709,
            "unit": "ns",
            "range": "± 5708.787142882005"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80467.06217447917,
            "unit": "ns",
            "range": "± 1376.6520093890663"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.635474701722464,
            "unit": "ns",
            "range": "± 0.014119507669291941"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.725018297632536,
            "unit": "ns",
            "range": "± 0.010055949343586444"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.02655602991581,
            "unit": "ns",
            "range": "± 0.004965929060486962"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 700228.3157552084,
            "unit": "ns",
            "range": "± 104795.84048828343"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 682235.12890625,
            "unit": "ns",
            "range": "± 90313.93319778147"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1299006.9557291667,
            "unit": "ns",
            "range": "± 245120.90547145656"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1378405.3072916667,
            "unit": "ns",
            "range": "± 345322.72699958185"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2342519.3424479165,
            "unit": "ns",
            "range": "± 58222.91496287357"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 867085.455078125,
            "unit": "ns",
            "range": "± 78058.97654809248"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 893974.9661458334,
            "unit": "ns",
            "range": "± 70645.84198070999"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 41230742.102564104,
            "unit": "ns",
            "range": "± 481014.7321203797"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42919010.69444445,
            "unit": "ns",
            "range": "± 553813.3371716702"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 91385419.11111112,
            "unit": "ns",
            "range": "± 13449891.84825341"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 94714503.58333333,
            "unit": "ns",
            "range": "± 40830082.91522622"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 146540999.77777776,
            "unit": "ns",
            "range": "± 23421610.0447283"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 64202105.6,
            "unit": "ns",
            "range": "± 6512084.076953573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 75927154.58333333,
            "unit": "ns",
            "range": "± 16722114.429583391"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 442337015.6666667,
            "unit": "ns",
            "range": "± 51444773.58838317"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 426539108.3333333,
            "unit": "ns",
            "range": "± 27395611.881162304"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 421304573,
            "unit": "ns",
            "range": "± 20883488.807468235"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 445889463.3333333,
            "unit": "ns",
            "range": "± 48336848.78619619"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 439557380.6666667,
            "unit": "ns",
            "range": "± 38056805.44493306"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "6808d2d0395ee3091a5f64ec422969bafe31c340",
          "message": "Merge pull request #4 from EFNext/feat/proxied-expressives\n\nIntroduce ExpressiveFor and ExpressiveForConstructor attributes",
          "timestamp": "2026-03-27T03:15:23Z",
          "tree_id": "67ee846cd39557035ab984f8ccd4b4d478905536",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/6808d2d0395ee3091a5f64ec422969bafe31c340"
        },
        "date": 1774581709323,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7620.958457946777,
            "unit": "ns",
            "range": "± 78.24359129961304"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1651.4872233072917,
            "unit": "ns",
            "range": "± 50.43836424261635"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.546056911349297,
            "unit": "ns",
            "range": "± 0.005391156770336572"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 154.9496652285258,
            "unit": "ns",
            "range": "± 0.45915717754768043"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21050.42822265625,
            "unit": "ns",
            "range": "± 6754.520791650951"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2396.2288411458335,
            "unit": "ns",
            "range": "± 597.6971388629727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.895862991611162,
            "unit": "ns",
            "range": "± 0.020570325881583255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.9153368473053,
            "unit": "ns",
            "range": "± 0.5292657435307632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 26778.877563476562,
            "unit": "ns",
            "range": "± 8079.064867167261"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2653.8881047566733,
            "unit": "ns",
            "range": "± 15.185528493844673"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.973808114727339,
            "unit": "ns",
            "range": "± 0.008484257714312476"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.5382052262624,
            "unit": "ns",
            "range": "± 0.1952679608580868"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 19943.31396484375,
            "unit": "ns",
            "range": "± 3584.3711608951444"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3124.2361755371094,
            "unit": "ns",
            "range": "± 80.79547122424067"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 9.316427528858185,
            "unit": "ns",
            "range": "± 0.039040363881819584"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.85895679394404,
            "unit": "ns",
            "range": "± 0.09542183089039329"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 149479.83203125,
            "unit": "ns",
            "range": "± 32665.954903705413"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 11713.60752360026,
            "unit": "ns",
            "range": "± 2702.405942698624"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.057552794615427,
            "unit": "ns",
            "range": "± 0.009809746308435636"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 10612.611948649088,
            "unit": "ns",
            "range": "± 990.7604509994419"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83000.02270507812,
            "unit": "ns",
            "range": "± 2286.6254654644285"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.759176840384802,
            "unit": "ns",
            "range": "± 0.012586134625844903"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.70862186451753,
            "unit": "ns",
            "range": "± 0.05266512966115969"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.980249971151352,
            "unit": "ns",
            "range": "± 0.00790598696862428"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 1093917.3072916667,
            "unit": "ns",
            "range": "± 172222.2060592727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 685178.9583333334,
            "unit": "ns",
            "range": "± 105211.09271354672"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 2576358.8020833335,
            "unit": "ns",
            "range": "± 1010129.3031217313"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 2902373.4244791665,
            "unit": "ns",
            "range": "± 1360402.4403225353"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2459354.5182291665,
            "unit": "ns",
            "range": "± 1018179.3997990382"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 52311.46211751302,
            "unit": "ns",
            "range": "± 1435.8133014581797"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 423620.1145833333,
            "unit": "ns",
            "range": "± 53274.33259280114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42858695.44444445,
            "unit": "ns",
            "range": "± 183467.89642352206"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42080892.80555556,
            "unit": "ns",
            "range": "± 338740.50824202446"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 67327739.13333333,
            "unit": "ns",
            "range": "± 14651464.796758542"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 89832074.33333333,
            "unit": "ns",
            "range": "± 28378370.535963845"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 83552333.75,
            "unit": "ns",
            "range": "± 24657951.658990514"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 497585.7373046875,
            "unit": "ns",
            "range": "± 263.61330796491967"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 5409170.427083333,
            "unit": "ns",
            "range": "± 785381.50414373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 440344042,
            "unit": "ns",
            "range": "± 30619592.89960366"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 451104264.6666667,
            "unit": "ns",
            "range": "± 36471993.691886276"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 434334226.6666667,
            "unit": "ns",
            "range": "± 24132856.468420234"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7212313.317708333,
            "unit": "ns",
            "range": "± 8140.505806384072"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12475252.375,
            "unit": "ns",
            "range": "± 349659.53509311064"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "e82b015698a05c1cf91c454b2594b4a560a425b0",
          "message": "Merge pull request #5 from EFNext/feat/extended-rewritable-queryable-coverage\n\nAdd missing LINQ overloads for IRewritableQueryable",
          "timestamp": "2026-03-28T01:46:45Z",
          "tree_id": "c6407da0c3fe5120023418a00cf14344377faeb4",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/e82b015698a05c1cf91c454b2594b4a560a425b0"
        },
        "date": 1774662788796,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7493.565455118815,
            "unit": "ns",
            "range": "± 55.965778885542186"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1619.3521531422932,
            "unit": "ns",
            "range": "± 4.5427589540529825"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.839140807588895,
            "unit": "ns",
            "range": "± 0.03435963034977354"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 147.30821363131204,
            "unit": "ns",
            "range": "± 0.793389264594885"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21576.624755859375,
            "unit": "ns",
            "range": "± 7199.59672578625"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1533.154706319173,
            "unit": "ns",
            "range": "± 30.17200505752371"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.839695051312447,
            "unit": "ns",
            "range": "± 0.020988950379387843"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 86.83245301246643,
            "unit": "ns",
            "range": "± 0.2136333926215009"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21960.86590576172,
            "unit": "ns",
            "range": "± 6733.68265741808"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2612.8366635640464,
            "unit": "ns",
            "range": "± 11.576162302704127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.849799459179243,
            "unit": "ns",
            "range": "± 0.0010789479394258143"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.63233613967896,
            "unit": "ns",
            "range": "± 0.05188295448103872"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 20572.953572591145,
            "unit": "ns",
            "range": "± 3899.81582286824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3191.6124013264975,
            "unit": "ns",
            "range": "± 50.45195458985113"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.762333899736404,
            "unit": "ns",
            "range": "± 0.009473188271977847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 55.746972600618996,
            "unit": "ns",
            "range": "± 0.00844748550720744"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 149356.2013346354,
            "unit": "ns",
            "range": "± 30400.99945966293"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 10233.154215494791,
            "unit": "ns",
            "range": "± 1372.3648156593779"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.038683185974756,
            "unit": "ns",
            "range": "± 0.044188249347768924"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 11086.256713867188,
            "unit": "ns",
            "range": "± 2212.408925185815"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83165.6455078125,
            "unit": "ns",
            "range": "± 6489.000941813461"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.720861340562502,
            "unit": "ns",
            "range": "± 0.052933395993880396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.151906917492548,
            "unit": "ns",
            "range": "± 0.014433459192237744"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.072257479031881,
            "unit": "ns",
            "range": "± 0.02216126438377974"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 673221.03515625,
            "unit": "ns",
            "range": "± 92554.85579481377"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 695756.65234375,
            "unit": "ns",
            "range": "± 114416.98234736899"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 2676179.4427083335,
            "unit": "ns",
            "range": "± 984498.961913648"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 2145158.78125,
            "unit": "ns",
            "range": "± 86409.87492125847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2180146.9401041665,
            "unit": "ns",
            "range": "± 81142.54889103545"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53747.838704427086,
            "unit": "ns",
            "range": "± 2120.005677469838"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 419368.359375,
            "unit": "ns",
            "range": "± 54742.759355656"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42708282.583333336,
            "unit": "ns",
            "range": "± 150720.2597057808"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41157480.472222224,
            "unit": "ns",
            "range": "± 50748.2076409215"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 88461385.75,
            "unit": "ns",
            "range": "± 21390607.14683824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 116050337.66666667,
            "unit": "ns",
            "range": "± 5418383.092037912"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 100349372.41666667,
            "unit": "ns",
            "range": "± 21626095.623258412"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 503774.4417317708,
            "unit": "ns",
            "range": "± 1931.1855467056992"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 4305768.106770833,
            "unit": "ns",
            "range": "± 315862.6193577741"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 434843502,
            "unit": "ns",
            "range": "± 43739717.145348474"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 421321424.6666667,
            "unit": "ns",
            "range": "± 28178564.994129602"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 441188952.3333333,
            "unit": "ns",
            "range": "± 23831128.097465344"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7097516.518229167,
            "unit": "ns",
            "range": "± 11629.532451421064"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12668553.979166666,
            "unit": "ns",
            "range": "± 851575.4529648282"
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
          "id": "ecfbc5e56f1286bc2e49d3a7e39dd6a20d320d50",
          "message": "fix: address PR review feedback for anonymous return type and README accuracy\n\n- Fix anonymous-type branch in EmitGenericSingleLambda: derive return type\n  param from method.ReturnType via symbol comparison instead of blindly\n  using the last type argument. Fixes incorrect return type for methods\n  like ExceptBy<T,TKey> where TKey is anonymous but return is T.\n- README: \"All standard\" → \"Most common\" to accurately reflect coverage.\n\nCo-Authored-By: Claude Opus 4.6 (1M context) <noreply@anthropic.com>",
          "timestamp": "2026-03-28T01:48:56Z",
          "tree_id": "64ee2449cd06a4e25b313172f0260add2437c3aa",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/ecfbc5e56f1286bc2e49d3a7e39dd6a20d320d50"
        },
        "date": 1774663047268,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7306.596099853516,
            "unit": "ns",
            "range": "± 33.73642840554388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1617.6470743815105,
            "unit": "ns",
            "range": "± 58.476695042816495"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.962341959277789,
            "unit": "ns",
            "range": "± 0.03731418309893601"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 155.6974796851476,
            "unit": "ns",
            "range": "± 2.3004730300415255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 25270.147094726562,
            "unit": "ns",
            "range": "± 7066.006767964979"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2228.660212198893,
            "unit": "ns",
            "range": "± 1112.9733320962775"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.899670526385307,
            "unit": "ns",
            "range": "± 0.022830377216727635"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 79.53254107634227,
            "unit": "ns",
            "range": "± 0.1436496014676377"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 22196.46024576823,
            "unit": "ns",
            "range": "± 6676.912910179035"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2658.1752319335938,
            "unit": "ns",
            "range": "± 55.172724008205954"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.05713958044847,
            "unit": "ns",
            "range": "± 0.004435934531331277"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 75.56518858671188,
            "unit": "ns",
            "range": "± 0.24421940080410565"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 20563.504475911457,
            "unit": "ns",
            "range": "± 4859.569998730738"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3140.7159576416016,
            "unit": "ns",
            "range": "± 64.07807556687932"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.523738856116931,
            "unit": "ns",
            "range": "± 0.0015186277959852042"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 51.053440034389496,
            "unit": "ns",
            "range": "± 0.048306029596115345"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 150142.0050455729,
            "unit": "ns",
            "range": "± 29252.926089598386"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 10188.307556152344,
            "unit": "ns",
            "range": "± 727.0650864457251"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 11.488964358965555,
            "unit": "ns",
            "range": "± 0.2629630517033224"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 11806.106669108072,
            "unit": "ns",
            "range": "± 2948.9383397785614"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80226.11157226562,
            "unit": "ns",
            "range": "± 1923.191759251887"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.743087947368622,
            "unit": "ns",
            "range": "± 0.029917818704945188"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.673662761847178,
            "unit": "ns",
            "range": "± 0.01445406602267705"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.982862164576848,
            "unit": "ns",
            "range": "± 0.01016469802640856"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 688817.6041666666,
            "unit": "ns",
            "range": "± 114702.04053756443"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 689551.6673177084,
            "unit": "ns",
            "range": "± 103052.12017988117"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 2169291.2578125,
            "unit": "ns",
            "range": "± 92962.75239804634"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 2136743.9348958335,
            "unit": "ns",
            "range": "± 82866.55228766914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2508059.625,
            "unit": "ns",
            "range": "± 89984.88501528832"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 51440.60196940104,
            "unit": "ns",
            "range": "± 589.9350273711749"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 615830.2356770834,
            "unit": "ns",
            "range": "± 105956.2620276241"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42829457.25,
            "unit": "ns",
            "range": "± 223322.78085870645"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41754959.75,
            "unit": "ns",
            "range": "± 109189.90833065577"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 123683726.66666664,
            "unit": "ns",
            "range": "± 31047239.55195822"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 114098720.1111111,
            "unit": "ns",
            "range": "± 11565259.954403723"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 109898156.11111112,
            "unit": "ns",
            "range": "± 3367817.0831923555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 493659.6315104167,
            "unit": "ns",
            "range": "± 725.035375784576"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 4202663.296875,
            "unit": "ns",
            "range": "± 310669.68796058506"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 449301566.3333333,
            "unit": "ns",
            "range": "± 45787368.3859307"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 420252345.3333333,
            "unit": "ns",
            "range": "± 20448982.507041894"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 421649414.3333333,
            "unit": "ns",
            "range": "± 20177895.950579047"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7302492.244791667,
            "unit": "ns",
            "range": "± 26610.011437520585"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10671169.744791666,
            "unit": "ns",
            "range": "± 358374.5233719214"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "365cd8495bdac1aeb5ecb2a75f302c03b599a8b6",
          "message": "Merge pull request #10 from EFNext/feat/efcore-relational-extensions\n\nAdd support for SQL window functions in ExpressiveSharp.EntityFrameworkCore",
          "timestamp": "2026-03-28T21:59:13Z",
          "tree_id": "64006f99291ccd9ddda2dcaa880f259467e90dbb",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/365cd8495bdac1aeb5ecb2a75f302c03b599a8b6"
        },
        "date": 1774735541945,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7735.852132161458,
            "unit": "ns",
            "range": "± 26.96655588062514"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1608.5279509226482,
            "unit": "ns",
            "range": "± 19.840367359318442"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.369569276769956,
            "unit": "ns",
            "range": "± 0.01476607006474056"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 159.8014136950175,
            "unit": "ns",
            "range": "± 1.6750446079982688"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 22333.721435546875,
            "unit": "ns",
            "range": "± 7560.649690412388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2394.1306355794272,
            "unit": "ns",
            "range": "± 1459.0362784318884"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.87189615269502,
            "unit": "ns",
            "range": "± 0.02226664692766553"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 81.35373449325562,
            "unit": "ns",
            "range": "± 0.04598035911704168"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 26842.884236653645,
            "unit": "ns",
            "range": "± 7543.017452674681"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2614.168988545736,
            "unit": "ns",
            "range": "± 12.682823427473204"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.87122429907322,
            "unit": "ns",
            "range": "± 0.023571278173417608"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 79.24083332220714,
            "unit": "ns",
            "range": "± 0.3374632359994183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 20870.121419270832,
            "unit": "ns",
            "range": "± 3957.978026672656"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3162.9846954345703,
            "unit": "ns",
            "range": "± 75.8678201545973"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.652573078870773,
            "unit": "ns",
            "range": "± 0.03856662065585133"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 55.727967441082,
            "unit": "ns",
            "range": "± 0.03147441573483032"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 149649.5441080729,
            "unit": "ns",
            "range": "± 34223.7575765059"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 10811.691975911459,
            "unit": "ns",
            "range": "± 2006.8033748756536"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.070660094420115,
            "unit": "ns",
            "range": "± 0.01230999059897917"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 12212.508666992188,
            "unit": "ns",
            "range": "± 3094.767531844663"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 84595.18294270833,
            "unit": "ns",
            "range": "± 5658.000940789397"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.667995492617289,
            "unit": "ns",
            "range": "± 0.021790676653095566"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.66924578944842,
            "unit": "ns",
            "range": "± 0.05308109141678414"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.057607625921568,
            "unit": "ns",
            "range": "± 0.09336683970831372"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 685037.4700520834,
            "unit": "ns",
            "range": "± 94086.17511089107"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 685193.8352864584,
            "unit": "ns",
            "range": "± 95227.47743019758"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1423781.8033854167,
            "unit": "ns",
            "range": "± 236676.6639891141"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 2252197.56640625,
            "unit": "ns",
            "range": "± 308627.92346715886"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2878809.7200520835,
            "unit": "ns",
            "range": "± 1078197.5751778872"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 57302.515869140625,
            "unit": "ns",
            "range": "± 8234.975327975417"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 617814.7473958334,
            "unit": "ns",
            "range": "± 106159.9755694436"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 128759375.83333333,
            "unit": "ns",
            "range": "± 15630891.600768352"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42812505.69444445,
            "unit": "ns",
            "range": "± 685765.1731920883"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 99080341.08333333,
            "unit": "ns",
            "range": "± 14072686.130380973"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 128415855.55555557,
            "unit": "ns",
            "range": "± 38310033.29878322"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 90269426.88888888,
            "unit": "ns",
            "range": "± 10569877.597954214"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 495027.447265625,
            "unit": "ns",
            "range": "± 1855.0682625595437"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 4226907.192708333,
            "unit": "ns",
            "range": "± 313775.2186441469"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 434009894.6666667,
            "unit": "ns",
            "range": "± 27730832.548028816"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 443258463.3333333,
            "unit": "ns",
            "range": "± 24418852.861943416"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 449504779.3333333,
            "unit": "ns",
            "range": "± 41265850.36170968"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7769123.463541667,
            "unit": "ns",
            "range": "± 290738.0844432931"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 13614543.625,
            "unit": "ns",
            "range": "± 2331358.965733994"
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
          "id": "d98aab28f1978e98992a1f553e3f1cd13bce76c4",
          "message": "fix: handle anonymous types in PolyfillInterceptorGenerator (#8, #9)\n\nRemove the IsAnonymousType element guard that blocked interceptor\ngeneration for operators after Select into anonymous types (#9). Update\nall per-operator emitters (Where, Select, SelectMany, Ordering, GroupBy,\nGroupByMulti, Join, GenericSingleLambda) to route through the generic\ncode path when the element type is anonymous.\n\nAdd anonymous-type branch to EmitJoin following the SelectMany3 pattern,\nso Join/GroupJoin with anonymous result selectors produce valid generic\ninterceptors (#8).\n\nThread type aliases through ReflectionFieldCache and EmitLambdaBody so\nthat typeof() expressions in the generated body use generic type params\n(e.g. typeof(TElem)) instead of unnameable anonymous type FQNs.\n\nCo-Authored-By: Claude Opus 4.6 (1M context) <noreply@anthropic.com>",
          "timestamp": "2026-03-29T19:04:50Z",
          "tree_id": "5c374c3bfc2f2fc4845fa2f0cafd368a34a09789",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/d98aab28f1978e98992a1f553e3f1cd13bce76c4"
        },
        "date": 1774811559475,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7261.277814229329,
            "unit": "ns",
            "range": "± 18.09886070687625"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2113.1096954345703,
            "unit": "ns",
            "range": "± 862.1202708446851"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.3326371510823565,
            "unit": "ns",
            "range": "± 0.027796292367255632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 147.58451318740845,
            "unit": "ns",
            "range": "± 0.7556327939788282"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21714.835896809895,
            "unit": "ns",
            "range": "± 7186.903519429673"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2524.3010915120444,
            "unit": "ns",
            "range": "± 1031.358689717262"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.884123866756758,
            "unit": "ns",
            "range": "± 0.050305672405624204"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 88.07917535305023,
            "unit": "ns",
            "range": "± 3.6923927809988477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 24002.940348307293,
            "unit": "ns",
            "range": "± 11508.625814270845"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2612.57306543986,
            "unit": "ns",
            "range": "± 2.8456008587133246"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.911774491270384,
            "unit": "ns",
            "range": "± 0.14363727662914086"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 78.45748498042424,
            "unit": "ns",
            "range": "± 0.0962925849226383"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 20338.408732096355,
            "unit": "ns",
            "range": "± 4664.780760296892"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3100.705062866211,
            "unit": "ns",
            "range": "± 135.83944681684403"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.552317758401235,
            "unit": "ns",
            "range": "± 0.00775887579176118"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 52.05464172363281,
            "unit": "ns",
            "range": "± 0.036657307075623806"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 157987.923828125,
            "unit": "ns",
            "range": "± 35236.22886657477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 11567.106262207031,
            "unit": "ns",
            "range": "± 2990.017123507976"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.115502282977104,
            "unit": "ns",
            "range": "± 0.05066901573878079"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 10571.216684977213,
            "unit": "ns",
            "range": "± 1084.0936163908796"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83091.53141276042,
            "unit": "ns",
            "range": "± 5611.642571815221"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.742641766866049,
            "unit": "ns",
            "range": "± 0.02021082327263493"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.615017612775167,
            "unit": "ns",
            "range": "± 0.043706117958837346"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.022603039940199,
            "unit": "ns",
            "range": "± 0.04797972446072977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 682339.6438802084,
            "unit": "ns",
            "range": "± 89701.9224060183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 691761.1712239584,
            "unit": "ns",
            "range": "± 119941.76678805183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 2180431.7421875,
            "unit": "ns",
            "range": "± 93732.98180087651"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1504976.30078125,
            "unit": "ns",
            "range": "± 322242.2389575195"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2402602.8046875,
            "unit": "ns",
            "range": "± 56141.897174754224"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53337.147135416664,
            "unit": "ns",
            "range": "± 1438.009630109906"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 655636.859375,
            "unit": "ns",
            "range": "± 147241.00309967148"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 41254671.256410256,
            "unit": "ns",
            "range": "± 154967.89477335697"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 43429234.25,
            "unit": "ns",
            "range": "± 88848.51304338657"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 88052927.25,
            "unit": "ns",
            "range": "± 21649371.363130346"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 89645385.41666667,
            "unit": "ns",
            "range": "± 27359416.99982841"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 96330380.83333333,
            "unit": "ns",
            "range": "± 52231403.88274736"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 501847.279296875,
            "unit": "ns",
            "range": "± 848.6198473993213"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 4237965.6328125,
            "unit": "ns",
            "range": "± 269816.8621540946"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 427653540,
            "unit": "ns",
            "range": "± 51077342.79652131"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 444828039.3333333,
            "unit": "ns",
            "range": "± 42955986.739129975"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 444225165.6666667,
            "unit": "ns",
            "range": "± 38311380.27774047"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7144181.005208333,
            "unit": "ns",
            "range": "± 8951.52084061388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12334470.927083334,
            "unit": "ns",
            "range": "± 549347.5511736559"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "dc0d77d33212857c0d0109cc3784230b4379775b",
          "message": "Merge pull request #12 from EFNext/feat/docs\n\nAdded docs site",
          "timestamp": "2026-03-30T00:10:32+01:00",
          "tree_id": "99ba829c6637834abdc091ac01aaede95f8f22c7",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/dc0d77d33212857c0d0109cc3784230b4379775b"
        },
        "date": 1774826220802,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7511.388399759929,
            "unit": "ns",
            "range": "± 25.22025817854134"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1770.9227091471355,
            "unit": "ns",
            "range": "± 232.4954883072158"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.06513449549675,
            "unit": "ns",
            "range": "± 0.04685632706196003"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 158.0473646322886,
            "unit": "ns",
            "range": "± 1.5133946730455126"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21199.401000976562,
            "unit": "ns",
            "range": "± 6661.446283280108"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2857.1287638346353,
            "unit": "ns",
            "range": "± 1443.0259781083748"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.894036496678988,
            "unit": "ns",
            "range": "± 0.016551282802916845"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 167.8906147480011,
            "unit": "ns",
            "range": "± 0.2575165041823101"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21135.239705403645,
            "unit": "ns",
            "range": "± 5378.281048071639"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2774.3958485921225,
            "unit": "ns",
            "range": "± 138.11574956848366"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.9126624862353,
            "unit": "ns",
            "range": "± 0.009564016969404794"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.821186820666,
            "unit": "ns",
            "range": "± 0.18081850777409914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 21501.56612141927,
            "unit": "ns",
            "range": "± 6377.972066671533"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3092.762181599935,
            "unit": "ns",
            "range": "± 64.794412177582"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.618845696250598,
            "unit": "ns",
            "range": "± 0.00607655201268284"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.71821375687917,
            "unit": "ns",
            "range": "± 0.021646938644273143"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 152347.298828125,
            "unit": "ns",
            "range": "± 30990.763820159336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 10159.064595540365,
            "unit": "ns",
            "range": "± 849.7090114125049"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 9.030731101830801,
            "unit": "ns",
            "range": "± 0.040671434917588935"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 10675.390299479166,
            "unit": "ns",
            "range": "± 1662.215222589637"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 85025.18033854167,
            "unit": "ns",
            "range": "± 7314.065094461941"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.982804030179977,
            "unit": "ns",
            "range": "± 0.008479852645166581"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.595494712392489,
            "unit": "ns",
            "range": "± 0.015954189858699612"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.326685294508934,
            "unit": "ns",
            "range": "± 0.03661616379415264"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 682011.5709635416,
            "unit": "ns",
            "range": "± 103130.5309014321"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 686057.4742838541,
            "unit": "ns",
            "range": "± 118651.8611270766"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 2528306.7135416665,
            "unit": "ns",
            "range": "± 997989.4493665659"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 2459189.6953125,
            "unit": "ns",
            "range": "± 108004.7274287188"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 2899844.1888020835,
            "unit": "ns",
            "range": "± 1049162.556782967"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54085.659200032555,
            "unit": "ns",
            "range": "± 1271.3354594478742"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 423098.3255208333,
            "unit": "ns",
            "range": "± 52697.21519572959"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42755653.19444445,
            "unit": "ns",
            "range": "± 131425.18904125635"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 43680224.44444444,
            "unit": "ns",
            "range": "± 223729.15398214082"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 100284137.66666667,
            "unit": "ns",
            "range": "± 16634548.237383226"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 99456798.25,
            "unit": "ns",
            "range": "± 17368827.49808193"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 90394538.5,
            "unit": "ns",
            "range": "± 22293193.035217773"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 502757.2275390625,
            "unit": "ns",
            "range": "± 3181.2745161283246"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 5448494.6875,
            "unit": "ns",
            "range": "± 743258.9708338979"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 429828251.3333333,
            "unit": "ns",
            "range": "± 27402657.808894273"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 427678785,
            "unit": "ns",
            "range": "± 26256989.6496925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 437955969.6666667,
            "unit": "ns",
            "range": "± 26222528.01116056"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7353170.041666667,
            "unit": "ns",
            "range": "± 32041.015569511328"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 21636689.5,
            "unit": "ns",
            "range": "± 4909644.598184373"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "27d6bd8c8eb4f95e97a0f468be395ca20aa7ca06",
          "message": "Merge pull request #15 from EFNext/fix/benchmark-stability\n\nfix: improve benchmark stability and adjust alert threshold",
          "timestamp": "2026-03-30T02:14:06+01:00",
          "tree_id": "5abc1a35175a765d82bc509dbedffd52d40ec49b",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/27d6bd8c8eb4f95e97a0f468be395ca20aa7ca06"
        },
        "date": 1774834706229,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7649.313294022171,
            "unit": "ns",
            "range": "± 64.5346525970101"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1691.3301391601562,
            "unit": "ns",
            "range": "± 48.31180396670601"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.1922875470840015,
            "unit": "ns",
            "range": "± 0.008332262002948171"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 157.45949065685272,
            "unit": "ns",
            "range": "± 3.7045729343880573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 15506.545976911273,
            "unit": "ns",
            "range": "± 173.35684744893194"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1650.7308158874512,
            "unit": "ns",
            "range": "± 9.852393915753185"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.119602074225744,
            "unit": "ns",
            "range": "± 0.2343458609635464"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.2793030500412,
            "unit": "ns",
            "range": "± 2.4873515346008213"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 15068.914334810697,
            "unit": "ns",
            "range": "± 724.4899446376689"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2706.0827536747374,
            "unit": "ns",
            "range": "± 13.686180272225284"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.871927391352324,
            "unit": "ns",
            "range": "± 0.04674495622201055"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 75.58589859803517,
            "unit": "ns",
            "range": "± 0.6384021017303227"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 18109.970541147086,
            "unit": "ns",
            "range": "± 166.78940403441112"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3231.697236328125,
            "unit": "ns",
            "range": "± 12.173173715205492"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.527610323495335,
            "unit": "ns",
            "range": "± 0.03933834011808082"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.45997115197005,
            "unit": "ns",
            "range": "± 0.7999467219425873"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 105444.04978785022,
            "unit": "ns",
            "range": "± 430.9459800385861"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8625.512774432147,
            "unit": "ns",
            "range": "± 41.454755381469234"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.929649847848662,
            "unit": "ns",
            "range": "± 0.025802470873764135"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8715.428976876396,
            "unit": "ns",
            "range": "± 90.54116882326913"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83407.17422598379,
            "unit": "ns",
            "range": "± 258.6360007292125"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.65970496998893,
            "unit": "ns",
            "range": "± 0.11093809705398726"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.556554276881547,
            "unit": "ns",
            "range": "± 0.02143289704542595"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.02010886669159,
            "unit": "ns",
            "range": "± 0.04034217182023309"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 497924.6880387931,
            "unit": "ns",
            "range": "± 18921.638565224388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 484237.5029296875,
            "unit": "ns",
            "range": "± 4091.8308038426508"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1023927.056640625,
            "unit": "ns",
            "range": "± 95385.53351945101"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 987338.467578125,
            "unit": "ns",
            "range": "± 106172.1614594287"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 989166.8907552083,
            "unit": "ns",
            "range": "± 103658.97297617694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53561.19593641493,
            "unit": "ns",
            "range": "± 324.8379565418249"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 260216.22621372767,
            "unit": "ns",
            "range": "± 11511.01206022834"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 43243215.19252874,
            "unit": "ns",
            "range": "± 758461.8338337898"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 44427909.538720526,
            "unit": "ns",
            "range": "± 881384.1112298535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 42726645.655555554,
            "unit": "ns",
            "range": "± 5763373.629146065"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 37060254.765,
            "unit": "ns",
            "range": "± 3564419.1849871664"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 41385067.59444444,
            "unit": "ns",
            "range": "± 5438473.3131242255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 512572.3749663254,
            "unit": "ns",
            "range": "± 2653.2516079922734"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3419028.161979167,
            "unit": "ns",
            "range": "± 215182.23913089375"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 320340365.0740741,
            "unit": "ns",
            "range": "± 3106037.645850614"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 327369779.1851852,
            "unit": "ns",
            "range": "± 4834584.579647157"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 324578244.1923077,
            "unit": "ns",
            "range": "± 3400610.4264911264"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 8003557.232291667,
            "unit": "ns",
            "range": "± 175720.74391726233"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11898131.021763394,
            "unit": "ns",
            "range": "± 318640.0640721202"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "d29634bc31ed095230b8fdca5a5d1a5f53330366",
          "message": "Merge pull request #16 from EFNext/feat/relational-extensions-abstractions\n\nAdd RelationalExtensions.Abstractions for SQL window functions",
          "timestamp": "2026-03-30T02:41:24+01:00",
          "tree_id": "036d95283ccd387cb5e8857152e612d9fccfc9e1",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/d29634bc31ed095230b8fdca5a5d1a5f53330366"
        },
        "date": 1774836324576,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7586.3277693123655,
            "unit": "ns",
            "range": "± 56.24846865814639"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1556.2525090535482,
            "unit": "ns",
            "range": "± 13.421057307971063"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.2816759640971815,
            "unit": "ns",
            "range": "± 0.06484080073519195"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 169.53376069466273,
            "unit": "ns",
            "range": "± 8.588382835270343"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14507.729560546875,
            "unit": "ns",
            "range": "± 194.1040930904852"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1600.5261595589775,
            "unit": "ns",
            "range": "± 25.776627431917554"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.00895261338779,
            "unit": "ns",
            "range": "± 0.036083555801648945"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 92.29530492322198,
            "unit": "ns",
            "range": "± 9.645392025934791"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14937.471697126117,
            "unit": "ns",
            "range": "± 108.62313372054487"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2736.3706770272092,
            "unit": "ns",
            "range": "± 40.48686302748632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.921973581971793,
            "unit": "ns",
            "range": "± 0.03497739712864077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.92310507338622,
            "unit": "ns",
            "range": "± 0.4261261381491695"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17707.83575439453,
            "unit": "ns",
            "range": "± 382.09355100464154"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3135.201618739537,
            "unit": "ns",
            "range": "± 34.49396513402788"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.6224064294781,
            "unit": "ns",
            "range": "± 0.054618743248566415"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.6014944847141,
            "unit": "ns",
            "range": "± 3.6187441374693714"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 104467.25237630209,
            "unit": "ns",
            "range": "± 683.5142202799327"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8948.232640019169,
            "unit": "ns",
            "range": "± 100.85332645565246"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.975265185894637,
            "unit": "ns",
            "range": "± 0.06028578850874051"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8994.333003452846,
            "unit": "ns",
            "range": "± 127.77902499670914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83179.60130931714,
            "unit": "ns",
            "range": "± 683.8705534340756"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 11.851457484563191,
            "unit": "ns",
            "range": "± 2.2051489521635887"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.71884133949362,
            "unit": "ns",
            "range": "± 0.09368685365799265"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.112985311945279,
            "unit": "ns",
            "range": "± 0.06442934466200366"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 520525.80442708335,
            "unit": "ns",
            "range": "± 24405.69882212026"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 490535.53771033656,
            "unit": "ns",
            "range": "± 4827.796715975826"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1009055.968359375,
            "unit": "ns",
            "range": "± 106856.50315879541"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1019337.0779947917,
            "unit": "ns",
            "range": "± 93787.61645753912"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1031704.2171875,
            "unit": "ns",
            "range": "± 97538.26076216552"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54328.710510253906,
            "unit": "ns",
            "range": "± 971.3853410768378"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 254480.56098090278,
            "unit": "ns",
            "range": "± 5570.384630538838"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 43240568.68678162,
            "unit": "ns",
            "range": "± 432661.7682413914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42713957.85714286,
            "unit": "ns",
            "range": "± 137284.91830645443"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 39713955.61388888,
            "unit": "ns",
            "range": "± 4591726.298233708"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 40993831.67555556,
            "unit": "ns",
            "range": "± 6019227.327597399"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 35249629.53050398,
            "unit": "ns",
            "range": "± 4049232.5907766605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 510624.668041088,
            "unit": "ns",
            "range": "± 4156.589596854671"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3401264.1026041666,
            "unit": "ns",
            "range": "± 224857.25925182246"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 322080352.7037037,
            "unit": "ns",
            "range": "± 4634737.802661373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 325449158,
            "unit": "ns",
            "range": "± 5162563.852783212"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 330018847.8965517,
            "unit": "ns",
            "range": "± 7874126.552743011"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7467495.26953125,
            "unit": "ns",
            "range": "± 214482.09076993781"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11496833.613425925,
            "unit": "ns",
            "range": "± 347064.1607776739"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "5392b48e57afdd7c5bb380b9765ae401e048c545",
          "message": "Merge pull request #11 from EFNext/fix/removed-obsolete-cache\n\nRefactor ExpressionTreeEmitter and ReflectionFieldCache to remove field prefix and static field handling",
          "timestamp": "2026-03-30T02:42:12+01:00",
          "tree_id": "f8391addda29e428f158634cdcbed9f993c3804d",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/5392b48e57afdd7c5bb380b9765ae401e048c545"
        },
        "date": 1774836396097,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7624.644969685873,
            "unit": "ns",
            "range": "± 52.41964186275698"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1595.2332700532058,
            "unit": "ns",
            "range": "± 20.23680576866523"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.800522323165621,
            "unit": "ns",
            "range": "± 0.4009485333200131"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 153.05770785013834,
            "unit": "ns",
            "range": "± 2.2237509578900547"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14970.771244755497,
            "unit": "ns",
            "range": "± 371.7008066752359"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1597.205924987793,
            "unit": "ns",
            "range": "± 17.18735605757901"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.850340278446675,
            "unit": "ns",
            "range": "± 0.025207536022743317"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 86.44958181253502,
            "unit": "ns",
            "range": "± 2.263100379994516"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14349.086656358508,
            "unit": "ns",
            "range": "± 347.6077687130764"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2761.9584624679,
            "unit": "ns",
            "range": "± 184.43881976893434"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.851813342550706,
            "unit": "ns",
            "range": "± 0.019002676337093898"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.29981486155437,
            "unit": "ns",
            "range": "± 0.2957473180744081"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17134.833544049943,
            "unit": "ns",
            "range": "± 180.8915710608891"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3210.1783787653994,
            "unit": "ns",
            "range": "± 15.20221448064418"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.567286925124271,
            "unit": "ns",
            "range": "± 0.07554129024563093"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 51.553445774096026,
            "unit": "ns",
            "range": "± 0.47830410326061845"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 103618.11159752155,
            "unit": "ns",
            "range": "± 551.1062176005836"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8623.171362740653,
            "unit": "ns",
            "range": "± 32.13426657599759"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.88204041066078,
            "unit": "ns",
            "range": "± 0.009379068089005694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8904.637650709887,
            "unit": "ns",
            "range": "± 242.46233611906405"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 82297.43602643695,
            "unit": "ns",
            "range": "± 703.2662904775473"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.564577601850033,
            "unit": "ns",
            "range": "± 0.019543940480062506"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.660399238268534,
            "unit": "ns",
            "range": "± 0.037383665212636884"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.012850171019291,
            "unit": "ns",
            "range": "± 0.03378075855176925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 492332.2947126116,
            "unit": "ns",
            "range": "± 18442.72005288028"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 475578.47825520835,
            "unit": "ns",
            "range": "± 3593.5630172092033"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 869628.6852101294,
            "unit": "ns",
            "range": "± 123873.87999374144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1004353.8700520833,
            "unit": "ns",
            "range": "± 111017.56878841507"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1016051.7067708333,
            "unit": "ns",
            "range": "± 89392.89426280007"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 52828.15561349051,
            "unit": "ns",
            "range": "± 945.9495407480997"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 253137.44466145834,
            "unit": "ns",
            "range": "± 4050.8958425321152"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42401063.51436782,
            "unit": "ns",
            "range": "± 352039.33596169535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42512284.54761904,
            "unit": "ns",
            "range": "± 233421.1348329926"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 38191311.91944445,
            "unit": "ns",
            "range": "± 3807453.063420757"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 35378346.025000006,
            "unit": "ns",
            "range": "± 2843962.218114159"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 40066403.436111115,
            "unit": "ns",
            "range": "± 4705307.492039446"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 504578.0048828125,
            "unit": "ns",
            "range": "± 5011.3473825632245"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2972473.328425481,
            "unit": "ns",
            "range": "± 33743.7815580539"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 308728481.44,
            "unit": "ns",
            "range": "± 1933380.6123411157"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 312243216.78571427,
            "unit": "ns",
            "range": "± 5953314.37174632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 310702171.68,
            "unit": "ns",
            "range": "± 2432896.6449935543"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7184510.972098215,
            "unit": "ns",
            "range": "± 87215.88506740858"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10852296.61875,
            "unit": "ns",
            "range": "± 380449.63823521836"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "afcbb059155c88ddd83316f7883ae586ff8aa8dd",
          "message": "Merge pull request #14 from EFNext/feat/generator-cleanup\n\nRefactor code structure for improved readability and maintainability",
          "timestamp": "2026-03-30T02:52:41+01:00",
          "tree_id": "7d9107f2be743bcbbdfae85032ed749b217367bd",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/afcbb059155c88ddd83316f7883ae586ff8aa8dd"
        },
        "date": 1774837024745,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7670.5001968383785,
            "unit": "ns",
            "range": "± 88.9177663691001"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1587.5041867769683,
            "unit": "ns",
            "range": "± 11.530209553518956"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.244854489790982,
            "unit": "ns",
            "range": "± 0.042709109057609204"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 155.94306643520082,
            "unit": "ns",
            "range": "± 4.047733448882077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 15344.875766601563,
            "unit": "ns",
            "range": "± 131.22730048562778"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1612.5828699384417,
            "unit": "ns",
            "range": "± 12.798506271128339"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.855740149815878,
            "unit": "ns",
            "range": "± 0.0274018086637114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 82.41722952893802,
            "unit": "ns",
            "range": "± 0.8339895045415422"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14679.676098632812,
            "unit": "ns",
            "range": "± 159.2172284227029"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2696.168647493635,
            "unit": "ns",
            "range": "± 9.875694705241072"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.666816801936537,
            "unit": "ns",
            "range": "± 0.848516049497574"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.46179987103851,
            "unit": "ns",
            "range": "± 0.617893119241558"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17428.757161458332,
            "unit": "ns",
            "range": "± 211.1648072738951"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3210.721669514974,
            "unit": "ns",
            "range": "± 48.92103538130861"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.562048714607954,
            "unit": "ns",
            "range": "± 0.024578612869755535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 58.31352985134492,
            "unit": "ns",
            "range": "± 1.11614593149737"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 104251.14845433728,
            "unit": "ns",
            "range": "± 898.297456221734"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8881.054677327475,
            "unit": "ns",
            "range": "± 174.2435935933445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.981067797967365,
            "unit": "ns",
            "range": "± 0.05253172217583745"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8894.60408606896,
            "unit": "ns",
            "range": "± 128.37642675326248"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83126.17215670072,
            "unit": "ns",
            "range": "± 386.2938306050731"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.556623760196898,
            "unit": "ns",
            "range": "± 0.017721484631981584"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.89343040276851,
            "unit": "ns",
            "range": "± 0.24686044024641576"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.482790221941881,
            "unit": "ns",
            "range": "± 0.49722958481143487"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 504744.62359375,
            "unit": "ns",
            "range": "± 8393.551151093177"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 486367.1466238839,
            "unit": "ns",
            "range": "± 9057.63750809645"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1006908.3240885417,
            "unit": "ns",
            "range": "± 100684.90936102906"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1002616.3891927083,
            "unit": "ns",
            "range": "± 106756.43099025816"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1014317.9205729166,
            "unit": "ns",
            "range": "± 107751.14089833535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54666.35911402209,
            "unit": "ns",
            "range": "± 683.6725147411648"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 266746.6002720424,
            "unit": "ns",
            "range": "± 18046.99671686578"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 43208420.804597706,
            "unit": "ns",
            "range": "± 275272.9945756247"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41900073.04022988,
            "unit": "ns",
            "range": "± 405014.8292573951"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 36726221.925,
            "unit": "ns",
            "range": "± 2725752.299199197"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 42408722.83888888,
            "unit": "ns",
            "range": "± 4888667.081875719"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 45154271.277777776,
            "unit": "ns",
            "range": "± 5310099.091025747"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 506415.4190126616,
            "unit": "ns",
            "range": "± 3320.6739707894267"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3385958.290625,
            "unit": "ns",
            "range": "± 219415.68328807445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 315418919.84615386,
            "unit": "ns",
            "range": "± 3174248.044392507"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 317296150.65384614,
            "unit": "ns",
            "range": "± 4477868.508278026"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 318303441.1923077,
            "unit": "ns",
            "range": "± 4588466.071101313"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7325382.561383928,
            "unit": "ns",
            "range": "± 79845.68298618561"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11243783.0625,
            "unit": "ns",
            "range": "± 259299.17896406646"
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
          "id": "28bba98aa6d4efbf8ae9ceffe8b042f50da5b85f",
          "message": " add Codecov upload step to CI workflow and update README badge",
          "timestamp": "2026-03-30T23:19:12Z",
          "tree_id": "b66b8dbf7cfbc44c19d401f251936e8c0dce09c4",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/28bba98aa6d4efbf8ae9ceffe8b042f50da5b85f"
        },
        "date": 1774914264591,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7351.898215993246,
            "unit": "ns",
            "range": "± 45.19337478218162"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1562.123949404116,
            "unit": "ns",
            "range": "± 13.993859635584585"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.017824677320627,
            "unit": "ns",
            "range": "± 0.901015014122076"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 177.16490416867393,
            "unit": "ns",
            "range": "± 27.70314657023784"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14690.713453020368,
            "unit": "ns",
            "range": "± 71.17766001311321"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1567.1564070383708,
            "unit": "ns",
            "range": "± 14.296545944518066"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.831005659920198,
            "unit": "ns",
            "range": "± 0.014073823443500303"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 83.90564318497975,
            "unit": "ns",
            "range": "± 4.1111813012215075"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 13896.923858642578,
            "unit": "ns",
            "range": "± 190.75379967518424"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2588.9799402171166,
            "unit": "ns",
            "range": "± 10.72044770777869"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.86142077137317,
            "unit": "ns",
            "range": "± 0.04104906987316267"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.48564044192985,
            "unit": "ns",
            "range": "± 1.0284875506413544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17004.65894963191,
            "unit": "ns",
            "range": "± 103.08600608585373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3055.225334754357,
            "unit": "ns",
            "range": "± 66.70010732876769"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.567297534695987,
            "unit": "ns",
            "range": "± 0.024686226421641984"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 53.8279013977601,
            "unit": "ns",
            "range": "± 2.0643619490400567"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 100712.8890625,
            "unit": "ns",
            "range": "± 1799.6005941891228"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8751.749971742984,
            "unit": "ns",
            "range": "± 332.35564573492985"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.898862582114008,
            "unit": "ns",
            "range": "± 0.02374001048878964"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8681.115594046456,
            "unit": "ns",
            "range": "± 184.2467734383364"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79771.25109863281,
            "unit": "ns",
            "range": "± 342.7232643375945"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.558327879224505,
            "unit": "ns",
            "range": "± 0.017809944845343022"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.750117116447154,
            "unit": "ns",
            "range": "± 0.19198638728257292"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.155278744796911,
            "unit": "ns",
            "range": "± 0.09961524418322135"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 489688.502999442,
            "unit": "ns",
            "range": "± 10352.848636163844"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 469748.57392939815,
            "unit": "ns",
            "range": "± 7631.260642648991"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 878957.9156788794,
            "unit": "ns",
            "range": "± 130865.95086486914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 778055.7156519396,
            "unit": "ns",
            "range": "± 53694.389909775724"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 870446.9595905172,
            "unit": "ns",
            "range": "± 115592.91065775862"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 51964.539912782864,
            "unit": "ns",
            "range": "± 201.61859768093967"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 252930.84840494793,
            "unit": "ns",
            "range": "± 5328.900710513929"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42207092.94642858,
            "unit": "ns",
            "range": "± 1138423.055967405"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42628561.08035714,
            "unit": "ns",
            "range": "± 236076.63322438294"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37927110.67333333,
            "unit": "ns",
            "range": "± 5412700.550628027"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 40067884.80555555,
            "unit": "ns",
            "range": "± 4701403.975141048"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36059553.18333333,
            "unit": "ns",
            "range": "± 3009264.1397119937"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 497611.5763165509,
            "unit": "ns",
            "range": "± 2581.3508799157535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2916913.75,
            "unit": "ns",
            "range": "± 19712.22780353187"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 312377266.8214286,
            "unit": "ns",
            "range": "± 4250710.410728681"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 315869464.12,
            "unit": "ns",
            "range": "± 2147152.4204521305"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 310911084.34615386,
            "unit": "ns",
            "range": "± 2621471.9732239437"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7269252.094866072,
            "unit": "ns",
            "range": "± 90258.57165119788"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10688418.66517857,
            "unit": "ns",
            "range": "± 110266.81523868961"
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
          "id": "9c721c6233f338e05dd4123a5eb044584b526cab",
          "message": "added missing diagnostic tests",
          "timestamp": "2026-03-30T23:29:23Z",
          "tree_id": "d9c014666bc3985bdae38dbfb0e7e3f3fe633d25",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/9c721c6233f338e05dd4123a5eb044584b526cab"
        },
        "date": 1774914823192,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7550.812289533944,
            "unit": "ns",
            "range": "± 42.932627403550704"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1594.4026158196586,
            "unit": "ns",
            "range": "± 26.101095746944576"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.264820345892356,
            "unit": "ns",
            "range": "± 0.01969259957967487"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 171.14498203376243,
            "unit": "ns",
            "range": "± 6.611080250534345"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 15279.984377347506,
            "unit": "ns",
            "range": "± 185.9367970517763"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1641.724240338361,
            "unit": "ns",
            "range": "± 11.481473517912415"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.359445075576122,
            "unit": "ns",
            "range": "± 0.5572309157523624"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 82.91293832233974,
            "unit": "ns",
            "range": "± 3.380431884324618"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14715.553946358817,
            "unit": "ns",
            "range": "± 192.1658429836912"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2667.2769521077475,
            "unit": "ns",
            "range": "± 36.48009264050459"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.86567414248431,
            "unit": "ns",
            "range": "± 0.04206528446836647"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.13639806597321,
            "unit": "ns",
            "range": "± 1.1185287250839402"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17571.90207248264,
            "unit": "ns",
            "range": "± 428.1996861069226"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3131.696410002532,
            "unit": "ns",
            "range": "± 21.804339200048993"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.252869593777827,
            "unit": "ns",
            "range": "± 0.7681774656972445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.953821558218735,
            "unit": "ns",
            "range": "± 0.3173065023826111"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 107174.61853448275,
            "unit": "ns",
            "range": "± 1653.657592500469"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8690.751291128305,
            "unit": "ns",
            "range": "± 77.74545105043657"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.924000332581586,
            "unit": "ns",
            "range": "± 0.04715766495954033"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8678.319231951678,
            "unit": "ns",
            "range": "± 85.33110564357126"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 83228.90971156528,
            "unit": "ns",
            "range": "± 705.1002574653587"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.077865099310875,
            "unit": "ns",
            "range": "± 0.518316277229543"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.542168212788445,
            "unit": "ns",
            "range": "± 0.024340588285370876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.025433984809908,
            "unit": "ns",
            "range": "± 0.032475648851804075"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 484116.242115162,
            "unit": "ns",
            "range": "± 3337.8112422007125"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 478691.947193287,
            "unit": "ns",
            "range": "± 3357.036729452897"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1020214.8377604167,
            "unit": "ns",
            "range": "± 86890.49494184273"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1008815.4016927084,
            "unit": "ns",
            "range": "± 99138.36448564116"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1022569.7904947917,
            "unit": "ns",
            "range": "± 95997.10468215428"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 55053.50443070023,
            "unit": "ns",
            "range": "± 268.12298935227886"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 266172.68313802086,
            "unit": "ns",
            "range": "± 24210.810154456645"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42792197.759615384,
            "unit": "ns",
            "range": "± 148067.69617751322"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42981165.95679012,
            "unit": "ns",
            "range": "± 170553.4841858318"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 41738061.260000005,
            "unit": "ns",
            "range": "± 5470742.089913874"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 42742030.41666667,
            "unit": "ns",
            "range": "± 5630984.386146039"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 39432821.958333336,
            "unit": "ns",
            "range": "± 4286923.908745015"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 513857.8294383082,
            "unit": "ns",
            "range": "± 2850.788016335082"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3351667.671875,
            "unit": "ns",
            "range": "± 254898.38532322503"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 325573519.1034483,
            "unit": "ns",
            "range": "± 6710314.501842594"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 321967746,
            "unit": "ns",
            "range": "± 4041056.5924026924"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 323524512.2692308,
            "unit": "ns",
            "range": "± 6899686.019433281"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7662757.241629465,
            "unit": "ns",
            "range": "± 186754.3027619146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12137759.852083333,
            "unit": "ns",
            "range": "± 418799.9661058255"
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
          "id": "c1152b4d0ba5244b42ce2643774f664941389c9d",
          "message": "Add .NET9 TFM adn consolidate on C# 14",
          "timestamp": "2026-03-31T00:00:42Z",
          "tree_id": "1f62565a9203d89fc7164e54fbd194e8f992da8f",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/c1152b4d0ba5244b42ce2643774f664941389c9d"
        },
        "date": 1774916735545,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6988.747804641724,
            "unit": "ns",
            "range": "± 105.0383212216844"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1447.1068197397085,
            "unit": "ns",
            "range": "± 6.798901080761212"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 5.096731894546085,
            "unit": "ns",
            "range": "± 0.006047772256015087"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 161.69885095645643,
            "unit": "ns",
            "range": "± 2.6453289505150765"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 13240.133676034433,
            "unit": "ns",
            "range": "± 417.44218180616826"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1455.9019521077473,
            "unit": "ns",
            "range": "± 6.798410545408979"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 6.9958430012831325,
            "unit": "ns",
            "range": "± 0.005729053223085999"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 78.33387838800748,
            "unit": "ns",
            "range": "± 4.657354077014527"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 12737.068436234085,
            "unit": "ns",
            "range": "± 181.32464507949535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2612.063035964966,
            "unit": "ns",
            "range": "± 185.40728287898273"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 6.914482168596367,
            "unit": "ns",
            "range": "± 0.009304124421884072"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 65.53749501088569,
            "unit": "ns",
            "range": "± 2.133914105306916"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 15407.290115921585,
            "unit": "ns",
            "range": "± 152.25623386381645"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 2828.934157816569,
            "unit": "ns",
            "range": "± 14.177761078657849"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 5.171803427594049,
            "unit": "ns",
            "range": "± 0.006610000195316077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.877739820877714,
            "unit": "ns",
            "range": "± 0.5199630947235264"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 76785.78273228237,
            "unit": "ns",
            "range": "± 359.39376836877847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8018.762542724609,
            "unit": "ns",
            "range": "± 85.0236820861144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 5.741651356220245,
            "unit": "ns",
            "range": "± 0.5068372822557895"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 7952.539668156551,
            "unit": "ns",
            "range": "± 93.9129667273552"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 61914.557491048174,
            "unit": "ns",
            "range": "± 214.7779228498818"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 7.283363405901652,
            "unit": "ns",
            "range": "± 0.022937046337424122"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.228569907064621,
            "unit": "ns",
            "range": "± 2.083922774440859"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 5.596384523702519,
            "unit": "ns",
            "range": "± 0.00538255881624092"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 409377.096875,
            "unit": "ns",
            "range": "± 1030.0118389845804"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 396801.57463727676,
            "unit": "ns",
            "range": "± 3483.430620605287"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 858884.9379882812,
            "unit": "ns",
            "range": "± 143448.15754931525"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 947198.8071614583,
            "unit": "ns",
            "range": "± 87937.50566262477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 947544.397265625,
            "unit": "ns",
            "range": "± 79962.12805827444"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 38698.091697184245,
            "unit": "ns",
            "range": "± 324.05622861442174"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 202840.36376953125,
            "unit": "ns",
            "range": "± 915.0630652972728"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 35996039.68472907,
            "unit": "ns",
            "range": "± 276086.4358728687"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 35177539.278571434,
            "unit": "ns",
            "range": "± 73362.3315176199"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 30860696.570476186,
            "unit": "ns",
            "range": "± 2097576.1131191733"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 34203551.93333333,
            "unit": "ns",
            "range": "± 3190181.875135649"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 32267163.386666656,
            "unit": "ns",
            "range": "± 2272632.9542165794"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 469991.6984779095,
            "unit": "ns",
            "range": "± 2161.752886333823"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2947239.333984375,
            "unit": "ns",
            "range": "± 225821.31570770996"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 288293022.5769231,
            "unit": "ns",
            "range": "± 1930617.8450776872"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 291179816.875,
            "unit": "ns",
            "range": "± 1739486.9203158307"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 288915028.6,
            "unit": "ns",
            "range": "± 1767009.2998555028"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 6994575.329282408,
            "unit": "ns",
            "range": "± 200938.8271088054"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10768183.46875,
            "unit": "ns",
            "range": "± 165211.00840134503"
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
          "id": "7a9e6013e968f6010048b503e15e2cfa6cfaae67",
          "message": "Add reversed order context creation methods and tests for UseExpressives",
          "timestamp": "2026-03-31T01:01:34Z",
          "tree_id": "52723a155cbf6d9c03d359281411fbcb05eae637",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/7a9e6013e968f6010048b503e15e2cfa6cfaae67"
        },
        "date": 1774920369192,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6866.124305470785,
            "unit": "ns",
            "range": "± 198.92117383509589"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1428.4051807948522,
            "unit": "ns",
            "range": "± 56.28819651409035"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 5.121190439164638,
            "unit": "ns",
            "range": "± 0.0069417964457462135"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 158.41372148990632,
            "unit": "ns",
            "range": "± 1.3799869726356448"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 13172.533774239677,
            "unit": "ns",
            "range": "± 207.11447293772505"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1465.4427004213687,
            "unit": "ns",
            "range": "± 12.089960917995661"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 6.943064583199365,
            "unit": "ns",
            "range": "± 0.021474233563417097"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 84.39447595817703,
            "unit": "ns",
            "range": "± 2.115564226916445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 13013.304809570312,
            "unit": "ns",
            "range": "± 168.40302382565793"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2518.0045553136756,
            "unit": "ns",
            "range": "± 35.29298330446666"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 7.0598937626238225,
            "unit": "ns",
            "range": "± 0.12714703072031147"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 69.9749598290239,
            "unit": "ns",
            "range": "± 0.2300021189407739"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 15340.722706761853,
            "unit": "ns",
            "range": "± 332.5174748350694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 2800.1040998186386,
            "unit": "ns",
            "range": "± 85.03776717554192"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 6.728259875306061,
            "unit": "ns",
            "range": "± 1.5634346231578669"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 44.80603451199002,
            "unit": "ns",
            "range": "± 1.4820583751406504"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 77874.38167898996,
            "unit": "ns",
            "range": "± 359.51660560628073"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8069.201157142376,
            "unit": "ns",
            "range": "± 99.57946700853556"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 5.389631115176059,
            "unit": "ns",
            "range": "± 0.10646021985724997"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8296.456274850028,
            "unit": "ns",
            "range": "± 376.3681377261166"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 62264.811584472656,
            "unit": "ns",
            "range": "± 364.9716046266627"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 7.240720663506251,
            "unit": "ns",
            "range": "± 0.011739514471963218"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 8.362655612415281,
            "unit": "ns",
            "range": "± 1.1607527662197565"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 5.589723481581761,
            "unit": "ns",
            "range": "± 0.005047907390396857"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 413405.9102260045,
            "unit": "ns",
            "range": "± 3591.365273742822"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 401109.1302083333,
            "unit": "ns",
            "range": "± 3932.5144380293036"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 865460.2854352678,
            "unit": "ns",
            "range": "± 143373.15356220107"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 847899.8626302084,
            "unit": "ns",
            "range": "± 123908.02065518894"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 863879.5005387932,
            "unit": "ns",
            "range": "± 138717.59638353914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 38515.83760782878,
            "unit": "ns",
            "range": "± 443.6940708243263"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 207235.77276141828,
            "unit": "ns",
            "range": "± 1016.3694343675835"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 35542087.03113553,
            "unit": "ns",
            "range": "± 460531.547327053"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 35453987.05432099,
            "unit": "ns",
            "range": "± 257633.77111887996"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 35512771.475,
            "unit": "ns",
            "range": "± 3047090.641525671"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 31580557.371111117,
            "unit": "ns",
            "range": "± 2365209.544234401"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36266515.56666667,
            "unit": "ns",
            "range": "± 3141317.682914811"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 474753.1612374442,
            "unit": "ns",
            "range": "± 969.2373271786465"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2977551.9640066964,
            "unit": "ns",
            "range": "± 251445.71540233016"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 292591006.4230769,
            "unit": "ns",
            "range": "± 2117153.281226169"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 294200906,
            "unit": "ns",
            "range": "± 2554069.4382403153"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 295667677.5769231,
            "unit": "ns",
            "range": "± 3728117.153537664"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7419838.88046875,
            "unit": "ns",
            "range": "± 96360.58686991964"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11260623.096982758,
            "unit": "ns",
            "range": "± 194087.85362640582"
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
          "id": "77c9abb128d94f195efa68417ded9e5a00b367c8",
          "message": "Added a some more test coverage for ensuring that EXP0013 is raised appropriatly",
          "timestamp": "2026-03-31T02:17:08Z",
          "tree_id": "b672f788cf1ec950907636a012efc0385995b57d",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/77c9abb128d94f195efa68417ded9e5a00b367c8"
        },
        "date": 1774924926363,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7489.8537276131765,
            "unit": "ns",
            "range": "± 34.98249073605435"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1568.440094322994,
            "unit": "ns",
            "range": "± 15.815244568172254"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.70831586574686,
            "unit": "ns",
            "range": "± 0.516260257240114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 149.8348675608635,
            "unit": "ns",
            "range": "± 1.6993651275978259"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14611.9373826247,
            "unit": "ns",
            "range": "± 175.73263001064257"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1593.16352392126,
            "unit": "ns",
            "range": "± 19.991962640248683"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.822779023876556,
            "unit": "ns",
            "range": "± 0.011217822817851095"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 78.81760106129306,
            "unit": "ns",
            "range": "± 1.4390378535556287"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14765.427060546876,
            "unit": "ns",
            "range": "± 267.9446012515415"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2612.0479839324953,
            "unit": "ns",
            "range": "± 43.13495743592724"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 10.544165835848876,
            "unit": "ns",
            "range": "± 1.7542966096152366"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.4825181806529,
            "unit": "ns",
            "range": "± 1.2777996879534614"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17316.412331717354,
            "unit": "ns",
            "range": "± 169.19113724703138"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3107.8368688512733,
            "unit": "ns",
            "range": "± 18.959727798318283"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.653338552667544,
            "unit": "ns",
            "range": "± 0.10265805736444673"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 53.583184252892224,
            "unit": "ns",
            "range": "± 1.7283248228223325"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 103152.56846788194,
            "unit": "ns",
            "range": "± 552.3976910427892"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8752.406860351562,
            "unit": "ns",
            "range": "± 55.340204977436684"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.944335833743766,
            "unit": "ns",
            "range": "± 0.04233291624181585"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 9011.980104799624,
            "unit": "ns",
            "range": "± 396.34801687768106"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80011.39980643136,
            "unit": "ns",
            "range": "± 381.6083951345851"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.553095820135084,
            "unit": "ns",
            "range": "± 0.0174822536917428"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.253325680891672,
            "unit": "ns",
            "range": "± 0.364284019485689"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.036666532180138,
            "unit": "ns",
            "range": "± 0.022184228223756944"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 478753.9426457332,
            "unit": "ns",
            "range": "± 3737.0997737885327"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 474974.01404747594,
            "unit": "ns",
            "range": "± 8519.777733894738"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1005395.65078125,
            "unit": "ns",
            "range": "± 83751.7602117511"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 893845.3563802083,
            "unit": "ns",
            "range": "± 126077.38940048183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 884514.421875,
            "unit": "ns",
            "range": "± 138167.3026231727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 52495.52415729391,
            "unit": "ns",
            "range": "± 330.3180173359815"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 257189.784375,
            "unit": "ns",
            "range": "± 4757.082545569438"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 42539793.066666655,
            "unit": "ns",
            "range": "± 136117.2541636261"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 43322286.84567901,
            "unit": "ns",
            "range": "± 512078.80277792213"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 36227512.575,
            "unit": "ns",
            "range": "± 3067835.28745227"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 36771406.875,
            "unit": "ns",
            "range": "± 3116440.1789801"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 35528237.16833333,
            "unit": "ns",
            "range": "± 2867307.3941460364"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 511111.48857421876,
            "unit": "ns",
            "range": "± 5238.224405938863"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3186580.6180245536,
            "unit": "ns",
            "range": "± 233466.371040564"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 315719022.5,
            "unit": "ns",
            "range": "± 2685205.41893851"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 320005107.3333333,
            "unit": "ns",
            "range": "± 6373408.636953318"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 316691696.9230769,
            "unit": "ns",
            "range": "± 2763433.6548696794"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7431685.875520834,
            "unit": "ns",
            "range": "± 127043.77518175784"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11067144.774784483,
            "unit": "ns",
            "range": "± 393334.46739742835"
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
          "id": "a522fe71e0fef5aa0ac9fc8104dbc9ada3253059",
          "message": "renamed opt-in method",
          "timestamp": "2026-03-31T23:56:08Z",
          "tree_id": "b849de18fe8e55bb6f72cb2b6dc2ae05c82069d1",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/a522fe71e0fef5aa0ac9fc8104dbc9ada3253059"
        },
        "date": 1775002864985,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7230.544319915772,
            "unit": "ns",
            "range": "± 74.05122082437406"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1537.8619028727214,
            "unit": "ns",
            "range": "± 36.973240779252066"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.236310559100118,
            "unit": "ns",
            "range": "± 0.04159401390599436"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 148.7812694064502,
            "unit": "ns",
            "range": "± 1.8482822644154315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14506.804936161747,
            "unit": "ns",
            "range": "± 121.17725701786864"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1550.2310525512696,
            "unit": "ns",
            "range": "± 7.629872617288672"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.871949620012726,
            "unit": "ns",
            "range": "± 0.015463148067376814"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 83.78430289030075,
            "unit": "ns",
            "range": "± 0.19815221998582921"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14434.92280796596,
            "unit": "ns",
            "range": "± 661.848107794597"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2608.195358140128,
            "unit": "ns",
            "range": "± 19.34037290293764"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.822852201227631,
            "unit": "ns",
            "range": "± 0.015547851501829728"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.39990315692765,
            "unit": "ns",
            "range": "± 1.8782750599703524"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17050.62357875279,
            "unit": "ns",
            "range": "± 163.0180359328943"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3004.2683955601283,
            "unit": "ns",
            "range": "± 43.76486192597547"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.521546320273326,
            "unit": "ns",
            "range": "± 0.033681025541030724"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.58094315617173,
            "unit": "ns",
            "range": "± 2.7532093662973667"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 101212.65990369073,
            "unit": "ns",
            "range": "± 344.29436184570466"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8633.163167317709,
            "unit": "ns",
            "range": "± 88.16602939735205"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.902580883353949,
            "unit": "ns",
            "range": "± 0.020750153318774257"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8606.581078665597,
            "unit": "ns",
            "range": "± 66.32408410535588"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79764.2566550926,
            "unit": "ns",
            "range": "± 748.9232999705133"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.586646292358637,
            "unit": "ns",
            "range": "± 0.04940967445019462"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.392903178378388,
            "unit": "ns",
            "range": "± 0.9152717872001512"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.479175590972106,
            "unit": "ns",
            "range": "± 0.44272193121805536"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 473317.58440290176,
            "unit": "ns",
            "range": "± 4531.967599975621"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 465449.31785300927,
            "unit": "ns",
            "range": "± 1587.381206215023"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 992329.79453125,
            "unit": "ns",
            "range": "± 90954.4637550115"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 945613.38046875,
            "unit": "ns",
            "range": "± 89092.86402597836"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 972821.2197916667,
            "unit": "ns",
            "range": "± 95741.57938949786"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 51408.656224744074,
            "unit": "ns",
            "range": "± 364.91520455655177"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 250298.21979166666,
            "unit": "ns",
            "range": "± 5882.9146958430465"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 41894322.264880955,
            "unit": "ns",
            "range": "± 244781.85500022114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41283976.23342175,
            "unit": "ns",
            "range": "± 534013.9117969683"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 40139916.4,
            "unit": "ns",
            "range": "± 5817060.381636357"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 40152502.47777777,
            "unit": "ns",
            "range": "± 5526943.120182607"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 42455594.23333333,
            "unit": "ns",
            "range": "± 4421009.5357099185"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 490838.4850983796,
            "unit": "ns",
            "range": "± 3363.9084079074287"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2917610.762019231,
            "unit": "ns",
            "range": "± 16011.174607325223"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 306738665.72,
            "unit": "ns",
            "range": "± 2016594.0686205607"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 310247145.6923077,
            "unit": "ns",
            "range": "± 2748540.1097838217"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 308367821.4230769,
            "unit": "ns",
            "range": "± 2550580.6095405524"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7140006.138541667,
            "unit": "ns",
            "range": "± 139377.04396570273"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10644155.090517242,
            "unit": "ns",
            "range": "± 65270.38119013707"
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
          "id": "13911f052b9ac394b4cc55f42f7f88f67326f7f0",
          "message": "Add token configuration for Codecov action in CI workflow",
          "timestamp": "2026-04-01T02:05:41Z",
          "tree_id": "67f8af17e6f661aa036289ba049e0ccbb7271ab4",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/13911f052b9ac394b4cc55f42f7f88f67326f7f0"
        },
        "date": 1775010647816,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7270.994011773004,
            "unit": "ns",
            "range": "± 120.46272976845027"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1565.375980094627,
            "unit": "ns",
            "range": "± 13.875229473854137"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.342309789998191,
            "unit": "ns",
            "range": "± 0.15542246698708503"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 148.46499082675348,
            "unit": "ns",
            "range": "± 1.7450069432902917"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14564.66357421875,
            "unit": "ns",
            "range": "± 188.11578799197477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1562.974600693275,
            "unit": "ns",
            "range": "± 41.22993717124085"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.873602409447942,
            "unit": "ns",
            "range": "± 0.043029937888377785"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 83.11189360022544,
            "unit": "ns",
            "range": "± 1.2353455547962555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14225.683401254508,
            "unit": "ns",
            "range": "± 82.20553433992838"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2693.8458486703726,
            "unit": "ns",
            "range": "± 26.825587823489276"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.818040844585214,
            "unit": "ns",
            "range": "± 0.010483377525324295"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 75.77086343025339,
            "unit": "ns",
            "range": "± 0.8233108579197206"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17387.670780726843,
            "unit": "ns",
            "range": "± 365.4128423004402"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3135.0779829758862,
            "unit": "ns",
            "range": "± 26.34518177622583"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.765720381818968,
            "unit": "ns",
            "range": "± 0.3031324700741813"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 51.95996637385467,
            "unit": "ns",
            "range": "± 0.041941148850704595"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 101109.38937904095,
            "unit": "ns",
            "range": "± 467.6171698931957"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8581.255903977613,
            "unit": "ns",
            "range": "± 34.809667455871015"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.443302589335612,
            "unit": "ns",
            "range": "± 0.5289905838722722"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8773.581540621244,
            "unit": "ns",
            "range": "± 164.211880107934"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79737.99280657087,
            "unit": "ns",
            "range": "± 260.20021200700086"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.555434459889376,
            "unit": "ns",
            "range": "± 0.01786594202550274"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.579201549887657,
            "unit": "ns",
            "range": "± 0.05682275426415171"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.991334037534122,
            "unit": "ns",
            "range": "± 0.029903952244159822"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 491501.15869140625,
            "unit": "ns",
            "range": "± 4503.525009863173"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 471390.25425502233,
            "unit": "ns",
            "range": "± 2517.4247471845583"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 989430.834375,
            "unit": "ns",
            "range": "± 94840.32451513049"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 989379.472265625,
            "unit": "ns",
            "range": "± 84909.85159737234"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 878880.2540364583,
            "unit": "ns",
            "range": "± 132137.0562907132"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 51693.451391601564,
            "unit": "ns",
            "range": "± 259.5378340197858"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 248354.90035695044,
            "unit": "ns",
            "range": "± 2407.4072086132146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 41796038.778846145,
            "unit": "ns",
            "range": "± 174807.5874474294"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41734087.77011494,
            "unit": "ns",
            "range": "± 380891.8027900336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 35155008.72666667,
            "unit": "ns",
            "range": "± 3173137.585481633"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 42386997.855555564,
            "unit": "ns",
            "range": "± 4089037.533656762"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 42215716.01111111,
            "unit": "ns",
            "range": "± 4076957.6074707573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 491382.8236462823,
            "unit": "ns",
            "range": "± 1453.4761322748222"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2908596.7466947115,
            "unit": "ns",
            "range": "± 17294.1372981789"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 310934349.5185185,
            "unit": "ns",
            "range": "± 4134428.056618485"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 308925472.68,
            "unit": "ns",
            "range": "± 3485545.4873594595"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 311290507.8,
            "unit": "ns",
            "range": "± 2402787.5747311185"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7078878.946928879,
            "unit": "ns",
            "range": "± 90999.25006803956"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10608911.280208332,
            "unit": "ns",
            "range": "± 90146.01682787845"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "81a4f05e745b556a5be929fe8bc4d0fe147daabe",
          "message": "Merge pull request #17 from EFNext/feature/execute-update\n\nSupport EF Core ExecuteUpdate via IRewritableQueryable",
          "timestamp": "2026-04-04T00:35:06+01:00",
          "tree_id": "ecbfc158173db9c851af3697f197aa8d285947b6",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/81a4f05e745b556a5be929fe8bc4d0fe147daabe"
        },
        "date": 1775260785473,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7200.326881671774,
            "unit": "ns",
            "range": "± 67.80846901316991"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1546.3315370999849,
            "unit": "ns",
            "range": "± 9.79718632267806"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.191160806903133,
            "unit": "ns",
            "range": "± 0.007789776566025959"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 154.10540671433722,
            "unit": "ns",
            "range": "± 5.593204980008115"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14637.824906569262,
            "unit": "ns",
            "range": "± 94.91297050259472"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1579.649178466797,
            "unit": "ns",
            "range": "± 5.507579691881852"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.935693789963368,
            "unit": "ns",
            "range": "± 0.023885503544059977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 79.98055626239095,
            "unit": "ns",
            "range": "± 0.16642991292967146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14315.689878110532,
            "unit": "ns",
            "range": "± 124.59216261058637"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2627.659881591797,
            "unit": "ns",
            "range": "± 53.689440059920116"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.8227129014475,
            "unit": "ns",
            "range": "± 0.019625766087964833"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 84.64170887640545,
            "unit": "ns",
            "range": "± 5.027361279832879"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17106.270258976863,
            "unit": "ns",
            "range": "± 216.42710486957387"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3069.987353985126,
            "unit": "ns",
            "range": "± 53.44479037949585"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.492027887039715,
            "unit": "ns",
            "range": "± 0.01518843667154704"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 57.4525280548976,
            "unit": "ns",
            "range": "± 0.7114605297457455"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 101657.65391322544,
            "unit": "ns",
            "range": "± 241.93107251162283"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8536.691785539899,
            "unit": "ns",
            "range": "± 38.34741971780636"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.901397299149941,
            "unit": "ns",
            "range": "± 0.024403232323956733"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8538.939507378473,
            "unit": "ns",
            "range": "± 143.76918151824486"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 78722.19756835938,
            "unit": "ns",
            "range": "± 470.07244015205396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.543316586701959,
            "unit": "ns",
            "range": "± 0.006597653729187336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.89202087521553,
            "unit": "ns",
            "range": "± 0.35570268781889974"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.99161942910265,
            "unit": "ns",
            "range": "± 0.01774402807206718"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 474759.47609375,
            "unit": "ns",
            "range": "± 7407.537210915845"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 472059.187427662,
            "unit": "ns",
            "range": "± 9187.026810544572"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 996140.1,
            "unit": "ns",
            "range": "± 76627.01496955554"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 976970.3458333333,
            "unit": "ns",
            "range": "± 88827.65496224466"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 881259.8286637932,
            "unit": "ns",
            "range": "± 136646.76218481237"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53485.21081090857,
            "unit": "ns",
            "range": "± 1187.2744406176469"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 257206.0377828664,
            "unit": "ns",
            "range": "± 18437.478723399996"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 41992426.36904763,
            "unit": "ns",
            "range": "± 150512.24800295327"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41567087.31547618,
            "unit": "ns",
            "range": "± 158662.82609049065"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 40043600.788888894,
            "unit": "ns",
            "range": "± 4819914.804000499"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 41971997.11111112,
            "unit": "ns",
            "range": "± 4348118.943861303"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 42769059.02222223,
            "unit": "ns",
            "range": "± 4770398.987580989"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 498812.4413725754,
            "unit": "ns",
            "range": "± 2860.791080503562"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3156240.1124441964,
            "unit": "ns",
            "range": "± 238954.99329166452"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 309933337.4814815,
            "unit": "ns",
            "range": "± 3593343.748984485"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 313115663.7307692,
            "unit": "ns",
            "range": "± 3263711.7695920826"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 307847467.7307692,
            "unit": "ns",
            "range": "± 3332812.677460557"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7197419.72337963,
            "unit": "ns",
            "range": "± 19209.828165268613"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10577961.129310345,
            "unit": "ns",
            "range": "± 92101.40859167675"
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
          "id": "99458e3f189780cc937ccf756a8fe1e0ad8324ff",
          "message": "Fix captured variables in polyfill interceptor expression trees",
          "timestamp": "2026-04-04T00:41:21Z",
          "tree_id": "7273c95205e24914d349846d996d3aec1e347f31",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/99458e3f189780cc937ccf756a8fe1e0ad8324ff"
        },
        "date": 1775264770178,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7478.18041865031,
            "unit": "ns",
            "range": "± 78.62091996738592"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1646.5689513107825,
            "unit": "ns",
            "range": "± 101.13682475118645"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.234888682571741,
            "unit": "ns",
            "range": "± 0.04770610809449336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 152.93192393249936,
            "unit": "ns",
            "range": "± 4.986839883299307"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14497.255676269531,
            "unit": "ns",
            "range": "± 103.906236495211"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1608.8848274230957,
            "unit": "ns",
            "range": "± 19.370050372856127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.849517900634694,
            "unit": "ns",
            "range": "± 0.0325846211799464"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.3897134286386,
            "unit": "ns",
            "range": "± 1.9641454482749396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14335.907310203269,
            "unit": "ns",
            "range": "± 74.68826612550632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2628.6685276031494,
            "unit": "ns",
            "range": "± 38.021306590620384"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.980340902826615,
            "unit": "ns",
            "range": "± 0.10350177802942769"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 84.16579595516468,
            "unit": "ns",
            "range": "± 7.408566651777682"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 16997.345493164063,
            "unit": "ns",
            "range": "± 222.06652338346797"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3343.2938385009766,
            "unit": "ns",
            "range": "± 58.46809097242525"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.548111901201051,
            "unit": "ns",
            "range": "± 0.03499328983683501"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 51.91496156763147,
            "unit": "ns",
            "range": "± 0.2436501737548733"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 102823.15773292824,
            "unit": "ns",
            "range": "± 1094.9118789955946"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8593.724859167029,
            "unit": "ns",
            "range": "± 40.81451133397948"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.909206257811908,
            "unit": "ns",
            "range": "± 0.012064221424821634"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8728.06734575544,
            "unit": "ns",
            "range": "± 204.76959504945694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80389.08103027343,
            "unit": "ns",
            "range": "± 387.77141519768577"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.588479805986086,
            "unit": "ns",
            "range": "± 0.03861064882414803"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.544680195826071,
            "unit": "ns",
            "range": "± 0.022775136727359462"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.994513885676861,
            "unit": "ns",
            "range": "± 0.030912869132278253"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 495450.9677397629,
            "unit": "ns",
            "range": "± 18876.140445454555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 475379.5963541667,
            "unit": "ns",
            "range": "± 2807.953787479549"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 989850.9537760416,
            "unit": "ns",
            "range": "± 88906.06058944171"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1006994.6203125,
            "unit": "ns",
            "range": "± 106210.40322612206"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 989824.8545572917,
            "unit": "ns",
            "range": "± 88020.19009060986"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53836.836653645834,
            "unit": "ns",
            "range": "± 333.1369632437959"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 250855.84967912946,
            "unit": "ns",
            "range": "± 4718.311108632866"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 46269940.04722223,
            "unit": "ns",
            "range": "± 4476563.699334067"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 43262735.830246925,
            "unit": "ns",
            "range": "± 1196509.582980772"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 42449697.822222225,
            "unit": "ns",
            "range": "± 3805697.2735876585"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 39155248.525,
            "unit": "ns",
            "range": "± 4266229.326680081"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 35439864.068333335,
            "unit": "ns",
            "range": "± 2694515.041761499"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 504881.1328798491,
            "unit": "ns",
            "range": "± 3352.8455109408605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2936803.060763889,
            "unit": "ns",
            "range": "± 23049.537005277976"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 313439466.5,
            "unit": "ns",
            "range": "± 2474995.3635179074"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 317713546.53571427,
            "unit": "ns",
            "range": "± 5457579.914377668"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 317879389.5185185,
            "unit": "ns",
            "range": "± 4319388.3124680035"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7346430.770474138,
            "unit": "ns",
            "range": "± 150132.99260566768"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11062736.460648147,
            "unit": "ns",
            "range": "± 166949.35780717153"
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
          "id": "fe0aacb977514268e07016b3170638b59e4a786e",
          "message": "Add integration tests for Expressive expansion in EF Core queries",
          "timestamp": "2026-04-04T02:44:54Z",
          "tree_id": "e0db4ae43c81b53c8772bb040611ab5fcabfdb87",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/fe0aacb977514268e07016b3170638b59e4a786e"
        },
        "date": 1775272160962,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7296.824872334798,
            "unit": "ns",
            "range": "± 100.79732793575195"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1556.8515751279633,
            "unit": "ns",
            "range": "± 29.51611516783727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.7225236520171165,
            "unit": "ns",
            "range": "± 0.5436072968665546"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 151.1059935371081,
            "unit": "ns",
            "range": "± 1.944664926936469"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14373.084453125,
            "unit": "ns",
            "range": "± 164.91999605762737"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1605.2898878370013,
            "unit": "ns",
            "range": "± 21.783997451416518"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.833838115687724,
            "unit": "ns",
            "range": "± 0.06554427528117152"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 84.48480681180953,
            "unit": "ns",
            "range": "± 1.388046347121182"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14071.80037887008,
            "unit": "ns",
            "range": "± 330.9345514695632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2648.2000737683525,
            "unit": "ns",
            "range": "± 130.73436593936805"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.297805948058764,
            "unit": "ns",
            "range": "± 0.5326461625983941"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 74.20322021047274,
            "unit": "ns",
            "range": "± 4.649484894785956"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17431.71093329068,
            "unit": "ns",
            "range": "± 381.3377863134648"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3092.3544213431223,
            "unit": "ns",
            "range": "± 28.251156053356734"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.483871760039494,
            "unit": "ns",
            "range": "± 0.032899959593093255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.44490585923195,
            "unit": "ns",
            "range": "± 1.3759886660754266"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 100187.36953125,
            "unit": "ns",
            "range": "± 1832.0816525859411"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8530.440556117466,
            "unit": "ns",
            "range": "± 150.2511119965148"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.793442111710707,
            "unit": "ns",
            "range": "± 0.9582505400831517"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8586.968455723354,
            "unit": "ns",
            "range": "± 148.58006483316404"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79468.52264614763,
            "unit": "ns",
            "range": "± 810.3337993017012"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.091435189391005,
            "unit": "ns",
            "range": "± 0.49912419635779637"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.523494690656662,
            "unit": "ns",
            "range": "± 0.08102936158250638"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.9281181261457245,
            "unit": "ns",
            "range": "± 0.12232565358945714"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 484629.2838766164,
            "unit": "ns",
            "range": "± 17699.403623923834"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 468271.8063151042,
            "unit": "ns",
            "range": "± 11006.098949386034"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 999861.3255208334,
            "unit": "ns",
            "range": "± 79897.00412087108"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 971513.1055989583,
            "unit": "ns",
            "range": "± 90327.57108919065"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1001202.996875,
            "unit": "ns",
            "range": "± 75401.33371905547"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53116.2936354417,
            "unit": "ns",
            "range": "± 165.7200697421584"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 250176.51194661457,
            "unit": "ns",
            "range": "± 5542.652529967571"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 48064068.344827585,
            "unit": "ns",
            "range": "± 3039222.7808678444"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 41924900.4,
            "unit": "ns",
            "range": "± 401717.28148256126"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 36719523.775,
            "unit": "ns",
            "range": "± 3218366.190964498"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 41375130.11111112,
            "unit": "ns",
            "range": "± 3652528.69441203"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 39068112.877777785,
            "unit": "ns",
            "range": "± 4609509.661285677"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 482647.5228841146,
            "unit": "ns",
            "range": "± 7173.308992725001"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2895128.274594907,
            "unit": "ns",
            "range": "± 30261.396173067355"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 307584777.2692308,
            "unit": "ns",
            "range": "± 3518673.4275009898"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 311626831.12,
            "unit": "ns",
            "range": "± 2787271.5717799836"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 309798046.84615386,
            "unit": "ns",
            "range": "± 4951292.542513332"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 6927864.746875,
            "unit": "ns",
            "range": "± 113451.63755153432"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10562145.069444444,
            "unit": "ns",
            "range": "± 100102.11255164215"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "cf65ff1589193155a84430d14ee5074499c6b6ba",
          "message": "Merge pull request #22 from EFNext/feat/better-integration-tests\n\nRevamp integration tests to hit the database",
          "timestamp": "2026-04-05T15:48:44+01:00",
          "tree_id": "20f656469189a3b4ad165fbac7f7238dfcc01059",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/cf65ff1589193155a84430d14ee5074499c6b6ba"
        },
        "date": 1775401990673,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7283.57109375,
            "unit": "ns",
            "range": "± 68.56550433649548"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1578.4782460530598,
            "unit": "ns",
            "range": "± 38.7240875405989"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.190571116549628,
            "unit": "ns",
            "range": "± 0.01665174148444648"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 149.5898661338366,
            "unit": "ns",
            "range": "± 1.856104233861872"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14737.727073386863,
            "unit": "ns",
            "range": "± 310.574022665218"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1560.0571950276692,
            "unit": "ns",
            "range": "± 11.212359631464075"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.87131969737155,
            "unit": "ns",
            "range": "± 0.05256766449316225"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.67446726560593,
            "unit": "ns",
            "range": "± 2.5765385806974335"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14127.683784179688,
            "unit": "ns",
            "range": "± 225.92502459287803"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2638.1005110059464,
            "unit": "ns",
            "range": "± 17.01256843718188"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.904420648056727,
            "unit": "ns",
            "range": "± 0.08850973371418132"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 73.37651990141187,
            "unit": "ns",
            "range": "± 3.1330706114629314"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17060.711258499712,
            "unit": "ns",
            "range": "± 89.16303764251977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3124.3370387636382,
            "unit": "ns",
            "range": "± 32.894051967409396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.65153711518416,
            "unit": "ns",
            "range": "± 0.18142886257512097"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.689869315624236,
            "unit": "ns",
            "range": "± 0.45962961398105845"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 104114.20842633929,
            "unit": "ns",
            "range": "± 1257.6347958881793"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8770.745614188058,
            "unit": "ns",
            "range": "± 218.93887150036502"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 10.601387390494347,
            "unit": "ns",
            "range": "± 2.607539224094077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8676.088354928153,
            "unit": "ns",
            "range": "± 110.48878909592547"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 81265.01569475446,
            "unit": "ns",
            "range": "± 724.9629098916683"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.60058712639979,
            "unit": "ns",
            "range": "± 0.018863654325025035"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.572010030150414,
            "unit": "ns",
            "range": "± 0.04781158665276192"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.988171490707567,
            "unit": "ns",
            "range": "± 0.01813744921843889"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 493994.6286368534,
            "unit": "ns",
            "range": "± 17674.052353966144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 478557.0803125,
            "unit": "ns",
            "range": "± 6471.139323530453"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 979469.53046875,
            "unit": "ns",
            "range": "± 90303.65752037904"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 981910.67421875,
            "unit": "ns",
            "range": "± 87607.00995689977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 886130.212890625,
            "unit": "ns",
            "range": "± 115988.02074940436"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 52335.93358533136,
            "unit": "ns",
            "range": "± 578.5058954865191"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 253366.10524088543,
            "unit": "ns",
            "range": "± 4109.322447402376"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 43516875.44345238,
            "unit": "ns",
            "range": "± 413988.3016628783"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 42241609.71551724,
            "unit": "ns",
            "range": "± 165598.6281961167"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 32252828.215555556,
            "unit": "ns",
            "range": "± 2292486.8363442263"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 39935183.41944445,
            "unit": "ns",
            "range": "± 4742709.983400606"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 35548520.38333333,
            "unit": "ns",
            "range": "± 2814423.752783348"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 498792.7197591146,
            "unit": "ns",
            "range": "± 4766.085323946067"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3105713.3883928573,
            "unit": "ns",
            "range": "± 223310.69281545977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 311287857.4074074,
            "unit": "ns",
            "range": "± 3341339.357078389"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 311823889.7692308,
            "unit": "ns",
            "range": "± 2920455.0319397463"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 311334751.5769231,
            "unit": "ns",
            "range": "± 3392889.27800223"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7040313.617456896,
            "unit": "ns",
            "range": "± 27295.390079401328"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10573352.489955356,
            "unit": "ns",
            "range": "± 60700.67970590469"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a14a5dba953c7a936d912d7a5b9ea02cb8a8320f",
          "message": "Merge pull request #20 from EFNext/feat/runtime-abstractions\n\nAdd ExpressiveSharp.Abstractions package",
          "timestamp": "2026-04-06T01:37:28+01:00",
          "tree_id": "ded52884fb6c196897ac98ed8412c24ed0a6bfd0",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/a14a5dba953c7a936d912d7a5b9ea02cb8a8320f"
        },
        "date": 1775437399612,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7424.335301892511,
            "unit": "ns",
            "range": "± 120.63079849809417"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1564.1268697668004,
            "unit": "ns",
            "range": "± 6.156440976609541"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.2660958077068685,
            "unit": "ns",
            "range": "± 0.02057668357869013"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 166.1225692073504,
            "unit": "ns",
            "range": "± 7.005203341449121"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14693.95612080892,
            "unit": "ns",
            "range": "± 88.53015273757238"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1624.0124980381556,
            "unit": "ns",
            "range": "± 8.953631198599217"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.877828695944377,
            "unit": "ns",
            "range": "± 0.01720357870120045"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 79.34646345774333,
            "unit": "ns",
            "range": "± 2.4924702117893047"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14417.944605900691,
            "unit": "ns",
            "range": "± 202.03772457744975"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2664.560839028194,
            "unit": "ns",
            "range": "± 13.028724798205483"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.832993311950794,
            "unit": "ns",
            "range": "± 0.02539954697346521"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.8498267641774,
            "unit": "ns",
            "range": "± 1.095225062483399"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 17731.60574776786,
            "unit": "ns",
            "range": "± 190.39964924517355"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3200.410503246166,
            "unit": "ns",
            "range": "± 10.929254817704706"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.564549602311233,
            "unit": "ns",
            "range": "± 0.07526739227672649"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 52.14108184311125,
            "unit": "ns",
            "range": "± 0.7765497275783446"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 103032.33206612723,
            "unit": "ns",
            "range": "± 646.9336380876292"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 9188.489055926982,
            "unit": "ns",
            "range": "± 149.21456206549536"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.926054793481644,
            "unit": "ns",
            "range": "± 0.0254717616969535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8832.845878318503,
            "unit": "ns",
            "range": "± 33.296280254413034"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80969.57982556573,
            "unit": "ns",
            "range": "± 327.7775885851323"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.681510178638357,
            "unit": "ns",
            "range": "± 0.122192540544284"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.715561041544223,
            "unit": "ns",
            "range": "± 0.01984871795119056"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 7.982562755202425,
            "unit": "ns",
            "range": "± 0.022087222732012912"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 274134.4726186899,
            "unit": "ns",
            "range": "± 4184.329257625168"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 261137.33663504463,
            "unit": "ns",
            "range": "± 5496.3286408978165"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 990787.6743489583,
            "unit": "ns",
            "range": "± 96581.57264809178"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 980538.9044270833,
            "unit": "ns",
            "range": "± 97721.84716609005"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 988482.7557291667,
            "unit": "ns",
            "range": "± 86787.28767937234"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 55044.83345992477,
            "unit": "ns",
            "range": "± 1043.472379913064"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 250782.70855034722,
            "unit": "ns",
            "range": "± 890.7147523258081"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21960585.379310343,
            "unit": "ns",
            "range": "± 566824.3039109145"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21514438.237068966,
            "unit": "ns",
            "range": "± 570737.6183400397"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 36598616.53333333,
            "unit": "ns",
            "range": "± 2851285.702136605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 36452048.29333333,
            "unit": "ns",
            "range": "± 2890845.8549629147"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36975040.64666667,
            "unit": "ns",
            "range": "± 3211522.6673418623"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 511638.2896012931,
            "unit": "ns",
            "range": "± 2140.282865497176"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3273071.5627893517,
            "unit": "ns",
            "range": "± 295213.80124242144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 319043657.88461536,
            "unit": "ns",
            "range": "± 2303523.3705187943"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 315961192.3076923,
            "unit": "ns",
            "range": "± 4063854.8022471843"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 318752006.1481481,
            "unit": "ns",
            "range": "± 4259035.0438217325"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7692448.129166666,
            "unit": "ns",
            "range": "± 364280.7506542802"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11990430.513541667,
            "unit": "ns",
            "range": "± 424196.4790853682"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "b546bbdd454450bece63709294834b0daab616fa",
          "message": "Merge pull request #21 from EFNext/feature/string-interpolation-improvements\n\nImprove string interpolation: multi-arg Concat, alignment fix, docs, and transformer",
          "timestamp": "2026-04-06T02:32:23+01:00",
          "tree_id": "6e6b900c07d4e1fb489256e10a10bd98766f0b3d",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/b546bbdd454450bece63709294834b0daab616fa"
        },
        "date": 1775440689308,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7458.986046600342,
            "unit": "ns",
            "range": "± 117.74481503168568"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1587.1117738996234,
            "unit": "ns",
            "range": "± 32.091546249625075"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.673525118402073,
            "unit": "ns",
            "range": "± 0.39743943225731615"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 167.99039727052053,
            "unit": "ns",
            "range": "± 4.446484524578496"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 14882.893658673322,
            "unit": "ns",
            "range": "± 141.3405686902872"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1590.190328157865,
            "unit": "ns",
            "range": "± 21.460905034662442"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.998000990599394,
            "unit": "ns",
            "range": "± 0.06158309833232078"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 81.12641933046538,
            "unit": "ns",
            "range": "± 2.0826454729262087"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14564.332938639323,
            "unit": "ns",
            "range": "± 372.95308566130564"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2618.295731248527,
            "unit": "ns",
            "range": "± 18.52942467235097"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.669107037371603,
            "unit": "ns",
            "range": "± 0.7504261695352745"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.09627923965454,
            "unit": "ns",
            "range": "± 0.9949370929723549"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 18091.626346982757,
            "unit": "ns",
            "range": "± 292.5344794005973"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3184.611047152815,
            "unit": "ns",
            "range": "± 25.64714503244559"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.660065071923392,
            "unit": "ns",
            "range": "± 0.08036278075668124"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 51.61394438661378,
            "unit": "ns",
            "range": "± 0.36467103229961173"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 102080.83513532366,
            "unit": "ns",
            "range": "± 1027.0682264617637"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8856.902210780552,
            "unit": "ns",
            "range": "± 90.9081761332556"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.082565132280191,
            "unit": "ns",
            "range": "± 0.11462573045286678"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8496.10648236956,
            "unit": "ns",
            "range": "± 100.47610641703335"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80820.24968261718,
            "unit": "ns",
            "range": "± 538.173683861516"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.631630420684814,
            "unit": "ns",
            "range": "± 0.0634666806388071"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.632583088108472,
            "unit": "ns",
            "range": "± 0.07662759441578916"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.109303952753544,
            "unit": "ns",
            "range": "± 0.07360635030452149"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 279569.3300107759,
            "unit": "ns",
            "range": "± 6582.577728034291"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 262827.8003771552,
            "unit": "ns",
            "range": "± 3280.651370249288"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 983528.80546875,
            "unit": "ns",
            "range": "± 99289.86177722915"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 992233.2162760417,
            "unit": "ns",
            "range": "± 92673.59838982172"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1017743.3936197917,
            "unit": "ns",
            "range": "± 89062.86392304883"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54862.54476928711,
            "unit": "ns",
            "range": "± 741.1497908625519"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 264544.37994791666,
            "unit": "ns",
            "range": "± 21817.301738394093"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 22686812.658405174,
            "unit": "ns",
            "range": "± 128969.94645932133"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21633308.22544643,
            "unit": "ns",
            "range": "± 314687.87982519146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 36403791.79333334,
            "unit": "ns",
            "range": "± 2679517.0387681117"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 44572516.55555555,
            "unit": "ns",
            "range": "± 4166756.978866823"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 40122858.79333334,
            "unit": "ns",
            "range": "± 6083448.428276079"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 510828.4785481771,
            "unit": "ns",
            "range": "± 3202.112266592639"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3262058.948814655,
            "unit": "ns",
            "range": "± 197069.32252411556"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 318365670.14285713,
            "unit": "ns",
            "range": "± 5697207.428756678"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 319845021.6296296,
            "unit": "ns",
            "range": "± 3279564.8681199737"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 316442285.3333333,
            "unit": "ns",
            "range": "± 5980713.395300804"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7962997.157327586,
            "unit": "ns",
            "range": "± 204581.15751056303"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12168568.41325431,
            "unit": "ns",
            "range": "± 444664.25178514037"
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
          "id": "6e97f077c8525dda97497e5b5ed6332b5d9e27e9",
          "message": "Add Docker-in-Docker feature to development container configuration",
          "timestamp": "2026-04-06T02:53:20Z",
          "tree_id": "56e311d9e584e523713be83081a0b1ff969d01b1",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/6e97f077c8525dda97497e5b5ed6332b5d9e27e9"
        },
        "date": 1775445530194,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7015.30200458395,
            "unit": "ns",
            "range": "± 28.85175252909362"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1494.3127351958176,
            "unit": "ns",
            "range": "± 20.80902678237069"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 5.937320165868316,
            "unit": "ns",
            "range": "± 0.19939417942186582"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 162.2003341378837,
            "unit": "ns",
            "range": "± 0.9061771200153781"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 13678.937133789062,
            "unit": "ns",
            "range": "± 87.32184945914815"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1519.6599092836734,
            "unit": "ns",
            "range": "± 9.314664294860776"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 7.273979337679015,
            "unit": "ns",
            "range": "± 0.39550929039821486"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.04255334315476,
            "unit": "ns",
            "range": "± 0.562903703246267"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 13260.11107421875,
            "unit": "ns",
            "range": "± 93.11467985439114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2480.8921914277253,
            "unit": "ns",
            "range": "± 30.62383022549719"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 7.080222995808492,
            "unit": "ns",
            "range": "± 0.0473124004879143"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 70.43366277856487,
            "unit": "ns",
            "range": "± 0.8441991385172448"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 15683.479544503349,
            "unit": "ns",
            "range": "± 101.69296740879727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3039.459487915039,
            "unit": "ns",
            "range": "± 57.779933762408334"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 5.910732955272708,
            "unit": "ns",
            "range": "± 0.3138511328921907"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 48.29392945766449,
            "unit": "ns",
            "range": "± 0.36459137106697087"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 77921.26501464844,
            "unit": "ns",
            "range": "± 531.6568477865392"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8406.292283799914,
            "unit": "ns",
            "range": "± 274.4536084658372"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 5.569200292743486,
            "unit": "ns",
            "range": "± 0.007995431982525373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 7685.608378092448,
            "unit": "ns",
            "range": "± 38.16352653527283"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 62109.88468424479,
            "unit": "ns",
            "range": "± 136.64179190334426"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 7.399568166051592,
            "unit": "ns",
            "range": "± 0.016161777192999355"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 7.43550203802685,
            "unit": "ns",
            "range": "± 0.22821276191340564"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 5.624050779938698,
            "unit": "ns",
            "range": "± 0.0054523658399021095"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 224297.68788725755,
            "unit": "ns",
            "range": "± 1781.6608698062091"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 211001.9168294271,
            "unit": "ns",
            "range": "± 1968.4067021379667"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 977817.7486979166,
            "unit": "ns",
            "range": "± 86358.75328872268"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 993896.539453125,
            "unit": "ns",
            "range": "± 72279.32759579002"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 995126.3087239583,
            "unit": "ns",
            "range": "± 98839.06309096553"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 39594.172206624346,
            "unit": "ns",
            "range": "± 195.91818150504517"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 209001.7573939732,
            "unit": "ns",
            "range": "± 490.97945509673934"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17590572.89814815,
            "unit": "ns",
            "range": "± 52134.58290008644"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17574693.057112068,
            "unit": "ns",
            "range": "± 47954.03134226457"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 34048671.1,
            "unit": "ns",
            "range": "± 2708022.4224022618"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 35336088.791666664,
            "unit": "ns",
            "range": "± 3033777.17284402"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 33103676.66071428,
            "unit": "ns",
            "range": "± 3696253.514022829"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 476158.9917896412,
            "unit": "ns",
            "range": "± 2747.351398019827"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3013276.575613839,
            "unit": "ns",
            "range": "± 274499.9998351892"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 291221731.32,
            "unit": "ns",
            "range": "± 3060903.1641339776"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 299620509.51724136,
            "unit": "ns",
            "range": "± 7958407.89618254"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 298536212.6666667,
            "unit": "ns",
            "range": "± 5054449.718827758"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7252018.820851293,
            "unit": "ns",
            "range": "± 100931.80581053463"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10568582.019396551,
            "unit": "ns",
            "range": "± 96821.07228238064"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "daa5c9477d957b7f67c57a1eda5cde5be8e7ac77",
          "message": "Merge pull request #24 from EFNext/fix/renamed-interface\n\nRename IRewritableQueryable to IExpressiveQueryable",
          "timestamp": "2026-04-09T02:21:51+01:00",
          "tree_id": "f170927423a864820c9bb64133d7e06179dfa3c7",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/daa5c9477d957b7f67c57a1eda5cde5be8e7ac77"
        },
        "date": 1775699247126,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7425.419367726644,
            "unit": "ns",
            "range": "± 103.91183292157483"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1578.7974251641167,
            "unit": "ns",
            "range": "± 5.5785988852221875"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.919681888818741,
            "unit": "ns",
            "range": "± 0.40328998235621666"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 165.8549521525701,
            "unit": "ns",
            "range": "± 10.613161204168625"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 15519.852715386285,
            "unit": "ns",
            "range": "± 241.19620439080933"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1658.4692171536958,
            "unit": "ns",
            "range": "± 5.035523669844477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.072956521596227,
            "unit": "ns",
            "range": "± 0.12972465312155923"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 81.87776533694102,
            "unit": "ns",
            "range": "± 3.3907077921697244"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14787.640244704027,
            "unit": "ns",
            "range": "± 114.48582771605716"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2687.3551205226354,
            "unit": "ns",
            "range": "± 18.042954389549088"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.765508413828652,
            "unit": "ns",
            "range": "± 0.7566424105100477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 79.25049090385437,
            "unit": "ns",
            "range": "± 3.7786002420265263"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 18111.235334123885,
            "unit": "ns",
            "range": "± 310.44186331294685"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3134.9462675871673,
            "unit": "ns",
            "range": "± 25.527586719393245"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.284771911542991,
            "unit": "ns",
            "range": "± 0.7798433024341102"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.254383644232384,
            "unit": "ns",
            "range": "± 2.4516653901302576"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 107059.39222454203,
            "unit": "ns",
            "range": "± 1238.024387377257"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 9096.197295052665,
            "unit": "ns",
            "range": "± 233.6908622904058"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.993067826543536,
            "unit": "ns",
            "range": "± 0.051185654773550825"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8410.251501012732,
            "unit": "ns",
            "range": "± 22.484123041864205"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 82692.27299804687,
            "unit": "ns",
            "range": "± 542.4749623910853"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.661985151745656,
            "unit": "ns",
            "range": "± 1.1069761947462793"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.543878629803658,
            "unit": "ns",
            "range": "± 0.01882208820972073"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.019357511709476,
            "unit": "ns",
            "range": "± 0.04887937411736175"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 275071.822265625,
            "unit": "ns",
            "range": "± 3872.210431040473"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 261165.0361328125,
            "unit": "ns",
            "range": "± 1748.3961340876817"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 998105.9364583333,
            "unit": "ns",
            "range": "± 83828.88830032993"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1000634.566796875,
            "unit": "ns",
            "range": "± 83228.55313794175"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1012546.7298177084,
            "unit": "ns",
            "range": "± 93847.15667326974"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54940.427119954424,
            "unit": "ns",
            "range": "± 1790.6214370276641"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 262285.2179036458,
            "unit": "ns",
            "range": "± 2569.981706834805"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 22233725.784598213,
            "unit": "ns",
            "range": "± 350297.4238196946"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 22124726.0546875,
            "unit": "ns",
            "range": "± 264777.1957586666"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 41087599.552777775,
            "unit": "ns",
            "range": "± 4903331.268044008"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 35192186.17756411,
            "unit": "ns",
            "range": "± 4231815.665771396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 38312132.391666666,
            "unit": "ns",
            "range": "± 3024098.281330855"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 511034.58565848216,
            "unit": "ns",
            "range": "± 3667.612440154647"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3267577.2717633927,
            "unit": "ns",
            "range": "± 252182.10770836961"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 317127325.52,
            "unit": "ns",
            "range": "± 2180005.2006570636"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 326345551.14285713,
            "unit": "ns",
            "range": "± 8922319.505450767"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 322673422.8888889,
            "unit": "ns",
            "range": "± 4496467.156636208"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 8229504.213362069,
            "unit": "ns",
            "range": "± 489329.0241637365"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12650767.291666666,
            "unit": "ns",
            "range": "± 431104.5523399102"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "011c9319e8d2fdc3da0547635db6b67e93609825",
          "message": "Merge pull request #27 from EFNext/fix-rider-test-discovery\n\nFix Rider tests discovery and remove unused file",
          "timestamp": "2026-04-10T01:46:08+01:00",
          "tree_id": "45885cc185e9940c02cbece1fdf083983a731339",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/011c9319e8d2fdc3da0547635db6b67e93609825"
        },
        "date": 1775783538297,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6387.5297612157365,
            "unit": "ns",
            "range": "± 80.78605141156282"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1579.5472036089216,
            "unit": "ns",
            "range": "± 11.454314482469488"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.09453998454686,
            "unit": "ns",
            "range": "± 0.17917823028517496"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 149.01781758768806,
            "unit": "ns",
            "range": "± 5.385143014255407"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 11580.031259042245,
            "unit": "ns",
            "range": "± 48.198767470914625"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1669.823048727853,
            "unit": "ns",
            "range": "± 42.10947924525426"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 11.154651492834091,
            "unit": "ns",
            "range": "± 0.2584405195739024"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.63921541858602,
            "unit": "ns",
            "range": "± 2.5090856890317017"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 11200.457505967883,
            "unit": "ns",
            "range": "± 54.471973269688874"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2598.096294261791,
            "unit": "ns",
            "range": "± 29.804566903031752"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 10.061632208802083,
            "unit": "ns",
            "range": "± 0.12772803321550036"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.1645413850035,
            "unit": "ns",
            "range": "± 0.21139395510884357"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 13428.98321081091,
            "unit": "ns",
            "range": "± 254.20930485048922"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3024.868411028827,
            "unit": "ns",
            "range": "± 14.329150954157875"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.208597065614802,
            "unit": "ns",
            "range": "± 0.10714188972240334"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 52.95424032211304,
            "unit": "ns",
            "range": "± 2.7084986866709824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 82591.62830403647,
            "unit": "ns",
            "range": "± 894.8815187121431"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8188.653684343611,
            "unit": "ns",
            "range": "± 81.33271156670358"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.460694320499897,
            "unit": "ns",
            "range": "± 0.018608417467457363"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 7723.519134521484,
            "unit": "ns",
            "range": "± 21.690816680369167"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 58423.79151262556,
            "unit": "ns",
            "range": "± 909.8232064747463"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.542436198464461,
            "unit": "ns",
            "range": "± 0.1413243187633801"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.220890603427376,
            "unit": "ns",
            "range": "± 0.1876601179308494"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.440770773424042,
            "unit": "ns",
            "range": "± 0.1746943932891078"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 215198.13026646205,
            "unit": "ns",
            "range": "± 1041.4826058030114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 201449.0075094289,
            "unit": "ns",
            "range": "± 2379.9525350314093"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 884783.8674568966,
            "unit": "ns",
            "range": "± 131307.45402242802"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 980829.1455729167,
            "unit": "ns",
            "range": "± 88382.7208935449"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 899637.6419270834,
            "unit": "ns",
            "range": "± 137215.05762869428"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 36048.59856262207,
            "unit": "ns",
            "range": "± 244.80327187768194"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 230526.23902994793,
            "unit": "ns",
            "range": "± 2391.5884863394544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17665280.25,
            "unit": "ns",
            "range": "± 98139.97985601133"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17587243,
            "unit": "ns",
            "range": "± 76220.2381592297"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 34784817.98055556,
            "unit": "ns",
            "range": "± 3756595.350463356"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 38879973.10444444,
            "unit": "ns",
            "range": "± 6122958.518133893"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 33022471.365476187,
            "unit": "ns",
            "range": "± 4167504.423783854"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 475224.42083333334,
            "unit": "ns",
            "range": "± 5865.712763063607"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2960152.24609375,
            "unit": "ns",
            "range": "± 14315.298482161837"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 306638150.8888889,
            "unit": "ns",
            "range": "± 4447323.249868919"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 305244819.96153843,
            "unit": "ns",
            "range": "± 3437554.300165691"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 310058898.48,
            "unit": "ns",
            "range": "± 3121986.4053451093"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7801502.795797414,
            "unit": "ns",
            "range": "± 150853.02058245262"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11431564.01400862,
            "unit": "ns",
            "range": "± 195602.0722688302"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "45dc35a06221c569b30f5d28829bd2961b812e29",
          "message": "Merge pull request #26 from EFNext/fix/cosmos-tests\n\nUpdate Cosmos tests to handle unsupported scenarios with inconclusive assertions",
          "timestamp": "2026-04-11T02:21:53+01:00",
          "tree_id": "99091d52b8021cf52e61f151fe3b02fb0f87195d",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/45dc35a06221c569b30f5d28829bd2961b812e29"
        },
        "date": 1775872064499,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6865.6753753135945,
            "unit": "ns",
            "range": "± 35.32800109345136"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1467.5309459979717,
            "unit": "ns",
            "range": "± 20.44583714917601"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 5.078973303437233,
            "unit": "ns",
            "range": "± 0.007403768623168066"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 155.55612351497015,
            "unit": "ns",
            "range": "± 3.104076213516911"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 13287.352290562221,
            "unit": "ns",
            "range": "± 139.1494393144749"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1508.1739532470704,
            "unit": "ns",
            "range": "± 8.690068626245768"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 6.9183506941795345,
            "unit": "ns",
            "range": "± 0.024006600745609546"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 74.40892570785114,
            "unit": "ns",
            "range": "± 0.11982101777002221"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 13236.157321506076,
            "unit": "ns",
            "range": "± 518.3070292475501"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2503.0217553456623,
            "unit": "ns",
            "range": "± 84.81560205496838"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 7.634509126345317,
            "unit": "ns",
            "range": "± 0.7762480578271602"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 70.18941180620875,
            "unit": "ns",
            "range": "± 0.3731879743921969"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 15411.74747269242,
            "unit": "ns",
            "range": "± 74.43266826013553"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 2928.9209244339554,
            "unit": "ns",
            "range": "± 26.493983848805108"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 5.161090364610708,
            "unit": "ns",
            "range": "± 0.013176919291604373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 47.9310241651535,
            "unit": "ns",
            "range": "± 4.400444852262101"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 76553.07447916667,
            "unit": "ns",
            "range": "± 411.5909087421754"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8246.997665405273,
            "unit": "ns",
            "range": "± 117.4141988889929"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 5.5831154546252,
            "unit": "ns",
            "range": "± 0.00788805286419383"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 7678.120933795797,
            "unit": "ns",
            "range": "± 190.19715251554854"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 61903.54436203529,
            "unit": "ns",
            "range": "± 475.8989743052605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 7.422863173815939,
            "unit": "ns",
            "range": "± 0.01071674998802187"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 7.25060498714447,
            "unit": "ns",
            "range": "± 0.008028136051699756"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 6.333042944471042,
            "unit": "ns",
            "range": "± 0.5429862132134596"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 221353.23322405134,
            "unit": "ns",
            "range": "± 1614.1748693167685"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 209784.25461154513,
            "unit": "ns",
            "range": "± 2116.41082397352"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 855680.0641163794,
            "unit": "ns",
            "range": "± 142210.59041658798"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 867296.0983297414,
            "unit": "ns",
            "range": "± 151331.51777967482"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 973374.30703125,
            "unit": "ns",
            "range": "± 92085.7425589436"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 38470.48886213631,
            "unit": "ns",
            "range": "± 294.86396931186806"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 204277.4008091518,
            "unit": "ns",
            "range": "± 2709.4365863358507"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17757164.266203705,
            "unit": "ns",
            "range": "± 247112.61948426947"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17725443.092548076,
            "unit": "ns",
            "range": "± 174119.11317203467"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 32058169.438888893,
            "unit": "ns",
            "range": "± 2997452.6121460944"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 32915893.730000004,
            "unit": "ns",
            "range": "± 3103265.869808092"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 31059870.467708334,
            "unit": "ns",
            "range": "± 4233965.851671897"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 470443.6341471354,
            "unit": "ns",
            "range": "± 2755.334081469517"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2761437.2796875,
            "unit": "ns",
            "range": "± 21164.862750421544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 289055798.61538464,
            "unit": "ns",
            "range": "± 2006924.8121367795"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 288848910.2962963,
            "unit": "ns",
            "range": "± 4619073.417139705"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 292598951.1111111,
            "unit": "ns",
            "range": "± 5060804.670753824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7008943.709635417,
            "unit": "ns",
            "range": "± 51573.46996446972"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10041744.82650862,
            "unit": "ns",
            "range": "± 103804.81391360798"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "995251688aec2058aeddebd6e11860d4e2f4196a",
          "message": "Merge pull request #25 from EFNext/feat/mongo-integration\n\nMongoDB integration",
          "timestamp": "2026-04-11T16:13:00+01:00",
          "tree_id": "1a0f5e065168f676119619d733a7f0ed02100214",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/995251688aec2058aeddebd6e11860d4e2f4196a"
        },
        "date": 1775921958970,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6459.664799499512,
            "unit": "ns",
            "range": "± 94.66269476961868"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1559.325609956469,
            "unit": "ns",
            "range": "± 13.54885018751927"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.079024538140873,
            "unit": "ns",
            "range": "± 0.19194128654007026"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 167.1545433556592,
            "unit": "ns",
            "range": "± 2.0164978307731882"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 11462.077204777645,
            "unit": "ns",
            "range": "± 114.72699725611785"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1631.4147734465423,
            "unit": "ns",
            "range": "± 17.48887228055833"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.280426868902785,
            "unit": "ns",
            "range": "± 0.6051775093235312"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 81.37815139974866,
            "unit": "ns",
            "range": "± 3.8851277428475828"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 11083.590815617488,
            "unit": "ns",
            "range": "± 84.06754130125982"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2545.3566989240976,
            "unit": "ns",
            "range": "± 23.598928326277388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.830302975243992,
            "unit": "ns",
            "range": "± 0.17389640329717462"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.34815215605956,
            "unit": "ns",
            "range": "± 0.2832944737025326"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 13525.791933412906,
            "unit": "ns",
            "range": "± 150.06531970366828"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 2966.3029929851664,
            "unit": "ns",
            "range": "± 43.5432829993808"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.141152980151,
            "unit": "ns",
            "range": "± 0.18059152881495077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 52.39813690349973,
            "unit": "ns",
            "range": "± 2.674425274643149"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 83259.65315755208,
            "unit": "ns",
            "range": "± 488.47471443481606"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8199.512485351563,
            "unit": "ns",
            "range": "± 185.2696029877693"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.300197682742562,
            "unit": "ns",
            "range": "± 0.15221983274348205"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 7953.862203744741,
            "unit": "ns",
            "range": "± 30.073204714049552"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 58845.0112802011,
            "unit": "ns",
            "range": "± 702.161917295837"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.213785023876914,
            "unit": "ns",
            "range": "± 0.18070817987688284"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.383235412148329,
            "unit": "ns",
            "range": "± 0.03460238457698029"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.450715483776454,
            "unit": "ns",
            "range": "± 0.17102454482587667"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 213916.97325721153,
            "unit": "ns",
            "range": "± 2570.4590028902276"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 211791.341796875,
            "unit": "ns",
            "range": "± 5119.733101654029"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 897516.330549569,
            "unit": "ns",
            "range": "± 146913.51570707103"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 787137.782731681,
            "unit": "ns",
            "range": "± 62779.06293757705"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1003250.9372395833,
            "unit": "ns",
            "range": "± 98683.93816102688"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 36579.66475423177,
            "unit": "ns",
            "range": "± 243.55012673278097"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 231818.11614118304,
            "unit": "ns",
            "range": "± 4238.758976323711"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17587813.052083332,
            "unit": "ns",
            "range": "± 84171.24800959344"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17586934.61226852,
            "unit": "ns",
            "range": "± 207298.68073340232"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37328736.025,
            "unit": "ns",
            "range": "± 2977830.8901958005"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 35708553.21666667,
            "unit": "ns",
            "range": "± 2712043.149746424"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36036804.983333334,
            "unit": "ns",
            "range": "± 2840736.6323585575"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 486639.9400592672,
            "unit": "ns",
            "range": "± 6888.058676472811"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3322468.7328125,
            "unit": "ns",
            "range": "± 241008.87543325857"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 307491188.2222222,
            "unit": "ns",
            "range": "± 3691549.277307766"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 309264613.32,
            "unit": "ns",
            "range": "± 4195260.687493615"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 312866717.89285713,
            "unit": "ns",
            "range": "± 6103264.737584287"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7824680.459490741,
            "unit": "ns",
            "range": "± 103537.30666600679"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 12656801.734913792,
            "unit": "ns",
            "range": "± 421649.4852840754"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "a28453cdfd9748a777d85d772886134d5af2346d",
          "message": "Merge pull request #28 from EFNext/fix-i18n-issues\n\nFix i18n issues on floating point numbers in generated source code",
          "timestamp": "2026-04-11T16:18:29+01:00",
          "tree_id": "f50fd2bd5219dd596c12891f230da9949bae86b9",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/a28453cdfd9748a777d85d772886134d5af2346d"
        },
        "date": 1775922236291,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7540.75765491354,
            "unit": "ns",
            "range": "± 60.26220678140735"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 1633.7744944645808,
            "unit": "ns",
            "range": "± 22.19090086860079"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.201540210499213,
            "unit": "ns",
            "range": "± 0.018918686041216586"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 196.32919093278738,
            "unit": "ns",
            "range": "± 16.42756173647156"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 15041.485741248498,
            "unit": "ns",
            "range": "± 175.58832996855182"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 1625.9702588594878,
            "unit": "ns",
            "range": "± 11.943613554726635"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.49432132450434,
            "unit": "ns",
            "range": "± 0.5982293532341112"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.2737244780247,
            "unit": "ns",
            "range": "± 1.434609624630146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 14508.288362943209,
            "unit": "ns",
            "range": "± 71.05162152084573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 2648.0482689429973,
            "unit": "ns",
            "range": "± 55.01195303767202"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.232942397947665,
            "unit": "ns",
            "range": "± 0.34580247793357893"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 115.3108794260025,
            "unit": "ns",
            "range": "± 35.43752271750739"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 18555.07430465133,
            "unit": "ns",
            "range": "± 1253.1233333819418"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 3281.5870919063173,
            "unit": "ns",
            "range": "± 58.215268872526664"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.527220916152,
            "unit": "ns",
            "range": "± 0.02922829240549335"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.87889418464441,
            "unit": "ns",
            "range": "± 0.19692641486204368"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 103308.51424434267,
            "unit": "ns",
            "range": "± 714.4266386688322"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 8731.344099121094,
            "unit": "ns",
            "range": "± 119.78224028163278"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.972890876765762,
            "unit": "ns",
            "range": "± 0.08379935760128211"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 8687.791273328992,
            "unit": "ns",
            "range": "± 91.25863132823535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80358.54946108218,
            "unit": "ns",
            "range": "± 742.6559323397092"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.597518160939217,
            "unit": "ns",
            "range": "± 0.053397862988675944"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.557789251208305,
            "unit": "ns",
            "range": "± 0.026985360630385542"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.06186449251793,
            "unit": "ns",
            "range": "± 0.029968900339551106"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 274508.4835902623,
            "unit": "ns",
            "range": "± 1809.2485401516974"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 262571.7840750558,
            "unit": "ns",
            "range": "± 1315.35263569836"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1015065.0078125,
            "unit": "ns",
            "range": "± 84601.09576448347"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 983869.5783854167,
            "unit": "ns",
            "range": "± 88769.34998460428"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1025248.2859375,
            "unit": "ns",
            "range": "± 94924.70282862912"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53912.66656915073,
            "unit": "ns",
            "range": "± 496.34613177132707"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 256021.56090494792,
            "unit": "ns",
            "range": "± 3047.2235439362253"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21867038.36607143,
            "unit": "ns",
            "range": "± 229777.9648220145"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 20963477.863839287,
            "unit": "ns",
            "range": "± 103893.23269027275"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37420578.602222234,
            "unit": "ns",
            "range": "± 4020009.019957852"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 37382287.333333336,
            "unit": "ns",
            "range": "± 3368585.7943875105"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36783245.825,
            "unit": "ns",
            "range": "± 3320917.7923790636"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 504237.24846540176,
            "unit": "ns",
            "range": "± 2640.014783779292"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3196662.8309151786,
            "unit": "ns",
            "range": "± 253405.4926414419"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 311255212.6,
            "unit": "ns",
            "range": "± 2808619.5119812866"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 315566248.53571427,
            "unit": "ns",
            "range": "± 6244009.976397685"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 319822263.4074074,
            "unit": "ns",
            "range": "± 6706828.584969009"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 8131549.190193965,
            "unit": "ns",
            "range": "± 310219.8583601474"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11634282.840625,
            "unit": "ns",
            "range": "± 481814.1994434609"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "2a616960b52bc72c5f2dd4cb0b9633c70ba75d24",
          "message": "Merge pull request #30 from EFNext/feat/docs-facelift\n\nEnhance documentation workflow and add Blazor WASM playground support",
          "timestamp": "2026-04-13T02:48:41+01:00",
          "tree_id": "8487943b349315adb192ae5e66409736bf9fcfc6",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/2a616960b52bc72c5f2dd4cb0b9633c70ba75d24"
        },
        "date": 1776046533788,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6279.2069723195045,
            "unit": "ns",
            "range": "± 23.42819635796362"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2929.7255864461263,
            "unit": "ns",
            "range": "± 43.08131098711817"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.481702443744455,
            "unit": "ns",
            "range": "± 0.2348129813040873"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 161.16025870641073,
            "unit": "ns",
            "range": "± 4.291679551882094"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 17060.52188901548,
            "unit": "ns",
            "range": "± 112.32292314696478"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2974.1950078503837,
            "unit": "ns",
            "range": "± 28.683472160245678"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.854536602416864,
            "unit": "ns",
            "range": "± 0.17148849434054342"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 83.84542350967725,
            "unit": "ns",
            "range": "± 1.3115477026590387"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 16724.049827293115,
            "unit": "ns",
            "range": "± 95.54017186266907"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5740.781389508928,
            "unit": "ns",
            "range": "± 480.270000919716"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.795617049703232,
            "unit": "ns",
            "range": "± 0.15851414959926965"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.38323824680768,
            "unit": "ns",
            "range": "± 1.2603157587363478"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 21781.10801595052,
            "unit": "ns",
            "range": "± 140.20417988448673"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5815.244746071951,
            "unit": "ns",
            "range": "± 25.182371298612615"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.323320878935712,
            "unit": "ns",
            "range": "± 0.19961461556007404"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.84807177305222,
            "unit": "ns",
            "range": "± 0.9991840878015538"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 89190.4781788793,
            "unit": "ns",
            "range": "± 293.8522135994047"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 17897.6494035392,
            "unit": "ns",
            "range": "± 123.95646216375397"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.662993709798213,
            "unit": "ns",
            "range": "± 0.11143663875444197"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17335.95380045573,
            "unit": "ns",
            "range": "± 90.70932907348019"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 57057.39245605469,
            "unit": "ns",
            "range": "± 366.3926566599104"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.424430141846338,
            "unit": "ns",
            "range": "± 0.026295611088451964"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.192551352359631,
            "unit": "ns",
            "range": "± 0.1574885962398449"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 9.037980866329423,
            "unit": "ns",
            "range": "± 0.20007950114665954"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 216783.81178501673,
            "unit": "ns",
            "range": "± 2069.0041172226906"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 204726.76149338944,
            "unit": "ns",
            "range": "± 560.9961553419637"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1004485.8104166667,
            "unit": "ns",
            "range": "± 98778.66505121437"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 994468.48359375,
            "unit": "ns",
            "range": "± 109201.00143626252"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 885211.491244612,
            "unit": "ns",
            "range": "± 148769.2241930603"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 36524.53145625674,
            "unit": "ns",
            "range": "± 183.52456515291556"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 227547.7025101273,
            "unit": "ns",
            "range": "± 5999.228383674564"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17480267.67025862,
            "unit": "ns",
            "range": "± 30084.99535768364"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17206443.623958334,
            "unit": "ns",
            "range": "± 576090.9284477655"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 34587291.68055555,
            "unit": "ns",
            "range": "± 3937030.2993668434"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 40228473.708333336,
            "unit": "ns",
            "range": "± 5317087.372381085"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36786124.16071428,
            "unit": "ns",
            "range": "± 2747149.82906076"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 474037.7318494073,
            "unit": "ns",
            "range": "± 989.2231427656366"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3145688.6374421297,
            "unit": "ns",
            "range": "± 238685.08651471944"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 301999158.53846157,
            "unit": "ns",
            "range": "± 3332366.9913075566"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 303977812.5,
            "unit": "ns",
            "range": "± 3332092.6318486435"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 303814989,
            "unit": "ns",
            "range": "± 4629994.548728472"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7568001.892650463,
            "unit": "ns",
            "range": "± 9242.3699973985"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11089144.561458332,
            "unit": "ns",
            "range": "± 97473.55971864493"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "4f0c526bdb88a2709b803311f99bd5c56baa46ec",
          "message": "Merge pull request #32 from EFNext/fix/docs-deploy-aot\n\nFix docs deploy: disable AOT on Playground.Wasm",
          "timestamp": "2026-04-13T03:13:35+01:00",
          "tree_id": "63c0a9fe08d6872d2057e0f627db5c17affac699",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/4f0c526bdb88a2709b803311f99bd5c56baa46ec"
        },
        "date": 1776047976283,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7362.288585005135,
            "unit": "ns",
            "range": "± 121.15888964003307"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2985.3073151687095,
            "unit": "ns",
            "range": "± 40.01492803461948"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.631817918419838,
            "unit": "ns",
            "range": "± 0.3380806993826516"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 165.05614481227738,
            "unit": "ns",
            "range": "± 7.181594180121456"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21457.627239520734,
            "unit": "ns",
            "range": "± 110.5505067719965"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3154.446270120555,
            "unit": "ns",
            "range": "± 46.23595941286677"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.105673604955276,
            "unit": "ns",
            "range": "± 0.26486411545484695"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 83.75429406762123,
            "unit": "ns",
            "range": "± 1.7388992543111228"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21881.78389160156,
            "unit": "ns",
            "range": "± 594.6778601902414"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5408.591224407328,
            "unit": "ns",
            "range": "± 33.04217624724241"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.872658351505244,
            "unit": "ns",
            "range": "± 0.022745959546467992"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 77.71866196614725,
            "unit": "ns",
            "range": "± 0.8463501886284748"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27326.595983434607,
            "unit": "ns",
            "range": "± 746.0090994521103"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6026.427785600935,
            "unit": "ns",
            "range": "± 26.841055812118384"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.705268703103066,
            "unit": "ns",
            "range": "± 0.15171320094936025"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 53.170929124722115,
            "unit": "ns",
            "range": "± 2.392026522867438"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 113546.62738506611,
            "unit": "ns",
            "range": "± 374.118138247611"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18640.20811094087,
            "unit": "ns",
            "range": "± 90.48194379335922"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.938414435386658,
            "unit": "ns",
            "range": "± 0.02506566601588852"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18442.594587053572,
            "unit": "ns",
            "range": "± 203.9383593185345"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80538.53844778879,
            "unit": "ns",
            "range": "± 273.6697830382674"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.582276266482141,
            "unit": "ns",
            "range": "± 0.042738684711233876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.538163553785395,
            "unit": "ns",
            "range": "± 0.010305013438785017"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.051981409745556,
            "unit": "ns",
            "range": "± 0.05605294943507188"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 273148.6970214844,
            "unit": "ns",
            "range": "± 2216.474063710575"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 257035.58966290508,
            "unit": "ns",
            "range": "± 5857.174396593555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 991009.23515625,
            "unit": "ns",
            "range": "± 76265.70460144657"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 978016.31328125,
            "unit": "ns",
            "range": "± 94771.62938149994"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 881213.7027633102,
            "unit": "ns",
            "range": "± 120714.55775300876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 52852.16831752232,
            "unit": "ns",
            "range": "± 1664.748896295565"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 254032.37431278935,
            "unit": "ns",
            "range": "± 5963.058478069544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21530739.89732143,
            "unit": "ns",
            "range": "± 274699.0089203358"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21537711.79310345,
            "unit": "ns",
            "range": "± 353616.6283956136"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37304176.69622332,
            "unit": "ns",
            "range": "± 7694442.756690118"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 41126071.62222222,
            "unit": "ns",
            "range": "± 4101762.0308924126"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36306423.891666666,
            "unit": "ns",
            "range": "± 2757142.4774129437"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 499689.5088564116,
            "unit": "ns",
            "range": "± 4942.264816945012"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3159794.5219907407,
            "unit": "ns",
            "range": "± 221703.6603201153"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 311830704.76,
            "unit": "ns",
            "range": "± 2251166.7861640155"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 313582163.88461536,
            "unit": "ns",
            "range": "± 3175384.915333148"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 312283831.32,
            "unit": "ns",
            "range": "± 2404442.080489095"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7213693.299665178,
            "unit": "ns",
            "range": "± 131031.17253533205"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10572054.192708334,
            "unit": "ns",
            "range": "± 99137.09547166759"
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
          "id": "9acc7f3da5ec0d677055776eaa35924e7c296bde",
          "message": "Fix docs deploy: drop stale index.html rename step\n\nThe wwwroot already ships app.htm directly (BlazorMonaco removal moved\nthe entry point off index.html), so `mv .../index.html .../app.htm`\nfails with \"No such file or directory\". Also drops the companion rm of\nBlazorMonaco static assets since those no longer exist in the publish\noutput.\n\nCo-Authored-By: Claude Opus 4.6 (1M context) <noreply@anthropic.com>",
          "timestamp": "2026-04-13T02:22:22Z",
          "tree_id": "e4fcd42b627621806a9d1caa08cb382c4b06ca3c",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/9acc7f3da5ec0d677055776eaa35924e7c296bde"
        },
        "date": 1776048484747,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7508.672928873698,
            "unit": "ns",
            "range": "± 107.33655841625907"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2980.639786856515,
            "unit": "ns",
            "range": "± 12.767480506748731"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.231214458247026,
            "unit": "ns",
            "range": "± 0.039560385961536916"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 181.45845351219177,
            "unit": "ns",
            "range": "± 9.015026043241159"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21742.86760796441,
            "unit": "ns",
            "range": "± 341.8752227279729"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3053.5719404954175,
            "unit": "ns",
            "range": "± 37.381183373075615"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.882996939122677,
            "unit": "ns",
            "range": "± 0.02381077220348045"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 79.87970162354983,
            "unit": "ns",
            "range": "± 7.85676359339154"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21440.26528226412,
            "unit": "ns",
            "range": "± 394.1232100320858"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5494.606734664352,
            "unit": "ns",
            "range": "± 107.60197348810375"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.818714322788376,
            "unit": "ns",
            "range": "± 0.014939828430363714"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 79.13253792347732,
            "unit": "ns",
            "range": "± 4.96284775681626"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27492.772723858172,
            "unit": "ns",
            "range": "± 183.1233427097844"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6008.078567504883,
            "unit": "ns",
            "range": "± 36.13488269447127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.49349327440615,
            "unit": "ns",
            "range": "± 0.015002694784924444"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.073774812398135,
            "unit": "ns",
            "range": "± 0.1419026165106245"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 114111.08483886719,
            "unit": "ns",
            "range": "± 755.4988332229598"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18356.26881995568,
            "unit": "ns",
            "range": "± 57.65518146435756"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.931557321654899,
            "unit": "ns",
            "range": "± 0.027635099239375596"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18471.554025503305,
            "unit": "ns",
            "range": "± 166.32098945051538"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 82184.93170572916,
            "unit": "ns",
            "range": "± 525.6764184766247"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.56069302918582,
            "unit": "ns",
            "range": "± 0.020828567250299267"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.6924181394279,
            "unit": "ns",
            "range": "± 0.06843906466567336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.031487456074467,
            "unit": "ns",
            "range": "± 0.03754203445405733"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 277514.52007378475,
            "unit": "ns",
            "range": "± 1181.1932358805782"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 260400.87528935185,
            "unit": "ns",
            "range": "± 7199.257316084851"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1013413.8010416667,
            "unit": "ns",
            "range": "± 93888.59342107209"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1022273.44453125,
            "unit": "ns",
            "range": "± 105797.51869768761"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 995079.641015625,
            "unit": "ns",
            "range": "± 110087.15641478247"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54158.38539341518,
            "unit": "ns",
            "range": "± 917.3265544520458"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 266161.60598958336,
            "unit": "ns",
            "range": "± 24663.288555229974"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21845672.516163792,
            "unit": "ns",
            "range": "± 236482.6507790334"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21803099.164583333,
            "unit": "ns",
            "range": "± 475209.07979427255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37735347.18479223,
            "unit": "ns",
            "range": "± 7704579.402461889"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 39876246.25333334,
            "unit": "ns",
            "range": "± 5667512.429429336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 41026560.36111111,
            "unit": "ns",
            "range": "± 4231089.847409276"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 508541.988516972,
            "unit": "ns",
            "range": "± 3767.6191195706056"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3257683.827864583,
            "unit": "ns",
            "range": "± 286018.1102922523"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 324579733.6785714,
            "unit": "ns",
            "range": "± 8413429.29478538"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 329137496.5,
            "unit": "ns",
            "range": "± 7182272.827482555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 323494235.0689655,
            "unit": "ns",
            "range": "± 9262531.196110997"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 8727515.750520833,
            "unit": "ns",
            "range": "± 378573.8406860168"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 14090919.354910715,
            "unit": "ns",
            "range": "± 1540061.5052437333"
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
          "id": "c6cc58809126522eed682a17941e3803d18a58e0",
          "message": "Fix docs deploy: copy dotfiles from dist to gh-pages\n\n`cp -r dist/* .` skips `.nojekyll`, which meant underscore-prefixed\ndirectories (_playground/, _framework/, _content/) were still being\nstripped by Jekyll on gh-pages.github.io even after adding the\n.nojekyll source file. Use `cp -rT dist .` which copies the whole\ntree including dotfiles.\n\nCo-Authored-By: Claude Opus 4.6 (1M context) <noreply@anthropic.com>",
          "timestamp": "2026-04-13T02:33:01Z",
          "tree_id": "2e999a0b5119a13626695f07433b6f1b4be1e680",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/c6cc58809126522eed682a17941e3803d18a58e0"
        },
        "date": 1776049162650,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7235.834908548991,
            "unit": "ns",
            "range": "± 150.10721120038204"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2936.381565488618,
            "unit": "ns",
            "range": "± 35.542604396846926"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.219232341150443,
            "unit": "ns",
            "range": "± 0.023981057918576104"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 171.1036086493525,
            "unit": "ns",
            "range": "± 0.5816274138459736"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21367.642600730615,
            "unit": "ns",
            "range": "± 171.24941960764346"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3045.1505764552526,
            "unit": "ns",
            "range": "± 44.96598158935864"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.841884163794694,
            "unit": "ns",
            "range": "± 0.017989057484574092"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 83.51205827082906,
            "unit": "ns",
            "range": "± 1.6171692942939382"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 20834.901688187212,
            "unit": "ns",
            "range": "± 447.5187149917753"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5401.6872307913645,
            "unit": "ns",
            "range": "± 45.04792903445557"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.855117383702048,
            "unit": "ns",
            "range": "± 0.051545596043138506"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 79.91501616327851,
            "unit": "ns",
            "range": "± 2.8749419501653555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 26686.380302734375,
            "unit": "ns",
            "range": "± 263.17780014841907"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5857.039521959093,
            "unit": "ns",
            "range": "± 48.81899288761861"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.522483993035096,
            "unit": "ns",
            "range": "± 0.02876457831737161"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 52.241124453643955,
            "unit": "ns",
            "range": "± 1.3244962489581174"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 111322.28290005388,
            "unit": "ns",
            "range": "± 536.5442476225261"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18513.445393488957,
            "unit": "ns",
            "range": "± 150.3165148442227"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.933971160958553,
            "unit": "ns",
            "range": "± 0.012948288500824445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18140.1528720198,
            "unit": "ns",
            "range": "± 272.498644667711"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79574.28779820034,
            "unit": "ns",
            "range": "± 315.3220885450061"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.568678138984573,
            "unit": "ns",
            "range": "± 0.04147281536021952"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 14.732918635562614,
            "unit": "ns",
            "range": "± 5.0915356567935195"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.008718872892446,
            "unit": "ns",
            "range": "± 0.020395281742797925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 267210.37124528555,
            "unit": "ns",
            "range": "± 635.347476670461"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 251791.5162984914,
            "unit": "ns",
            "range": "± 7607.832269963535"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 886059.678515625,
            "unit": "ns",
            "range": "± 139191.79609609244"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 984625.4658854167,
            "unit": "ns",
            "range": "± 94188.9904547759"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 989765.0114583333,
            "unit": "ns",
            "range": "± 85484.60012721297"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53448.30551673626,
            "unit": "ns",
            "range": "± 342.6369181525584"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 254804.59176432292,
            "unit": "ns",
            "range": "± 3432.780289751445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21780960.862723213,
            "unit": "ns",
            "range": "± 512590.0556511158"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 20867340.477083333,
            "unit": "ns",
            "range": "± 584704.697579243"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 33744318.55944445,
            "unit": "ns",
            "range": "± 4239040.973990826"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 42756827.37777777,
            "unit": "ns",
            "range": "± 4524730.6674402505"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 38131621.580000006,
            "unit": "ns",
            "range": "± 5195645.499874387"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 496661.2222583912,
            "unit": "ns",
            "range": "± 3229.341443029373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2971709.1216947115,
            "unit": "ns",
            "range": "± 20196.88900782713"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 308981872.61538464,
            "unit": "ns",
            "range": "± 2713855.8678020476"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 309827778.6,
            "unit": "ns",
            "range": "± 2419696.5983984265"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 308838361.5769231,
            "unit": "ns",
            "range": "± 2154540.9463885375"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7113139.977120535,
            "unit": "ns",
            "range": "± 23238.753824380077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10621579.637931034,
            "unit": "ns",
            "range": "± 85265.52949145665"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "8026759c76ebf7a0448f26272d7b806441d9beb9",
          "message": "Merge pull request #31 from EFNext/feat/expressive-projectables\n\nfeat: add [Expressive(Projectable = true)] for projection middleware compatibility",
          "timestamp": "2026-04-13T21:14:55+01:00",
          "tree_id": "931dc6973979ed488a5d77d499854f6b67e1bf21",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/8026759c76ebf7a0448f26272d7b806441d9beb9"
        },
        "date": 1776112862667,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7296.3434421933935,
            "unit": "ns",
            "range": "± 89.11900120184292"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2956.8020303660423,
            "unit": "ns",
            "range": "± 28.532816609199546"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.198552562130822,
            "unit": "ns",
            "range": "± 0.015212333871785916"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 178.64524141262316,
            "unit": "ns",
            "range": "± 6.323635285111193"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21947.45431857639,
            "unit": "ns",
            "range": "± 277.67167031101303"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3007.9016391209193,
            "unit": "ns",
            "range": "± 26.728912724270238"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 14.263455026916095,
            "unit": "ns",
            "range": "± 5.5340096830792085"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 80.5525369007012,
            "unit": "ns",
            "range": "± 1.6991927798385262"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21481.131795247395,
            "unit": "ns",
            "range": "± 253.58758887894695"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5376.49906132139,
            "unit": "ns",
            "range": "± 62.200639469019144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.845788506934277,
            "unit": "ns",
            "range": "± 0.03868219356935858"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.85479891094668,
            "unit": "ns",
            "range": "± 0.3125561766406058"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27388.379917689734,
            "unit": "ns",
            "range": "± 321.9530605872407"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6065.565032958984,
            "unit": "ns",
            "range": "± 166.68384590140593"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.707925139261143,
            "unit": "ns",
            "range": "± 0.22812459115737632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.94401116282852,
            "unit": "ns",
            "range": "± 2.367864334405418"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 111354.93711274245,
            "unit": "ns",
            "range": "± 1103.1104548232627"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18384.14287909146,
            "unit": "ns",
            "range": "± 78.0545866256857"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.9431269254822,
            "unit": "ns",
            "range": "± 0.057029060910593385"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18429.85119846889,
            "unit": "ns",
            "range": "± 117.6682562851461"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80408.42715348868,
            "unit": "ns",
            "range": "± 517.191993779285"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.925284197926521,
            "unit": "ns",
            "range": "± 0.3867049435363743"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.577912094968337,
            "unit": "ns",
            "range": "± 0.016751656958485912"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.227195443378555,
            "unit": "ns",
            "range": "± 0.2152542906663423"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 272642.7904710036,
            "unit": "ns",
            "range": "± 3241.5975843884526"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 258249.2955005787,
            "unit": "ns",
            "range": "± 1385.6805889293685"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 888110.0625,
            "unit": "ns",
            "range": "± 131601.87245120315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 869592.0505118534,
            "unit": "ns",
            "range": "± 123279.92150324355"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1006566.0854166667,
            "unit": "ns",
            "range": "± 86680.18341979737"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54978.846003605766,
            "unit": "ns",
            "range": "± 686.8246067909147"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 254503.25417564655,
            "unit": "ns",
            "range": "± 4528.8794064546955"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21501264.423958335,
            "unit": "ns",
            "range": "± 223482.06434130998"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 22435645.987068966,
            "unit": "ns",
            "range": "± 129238.7308391839"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 40617772.08888889,
            "unit": "ns",
            "range": "± 5031895.545901246"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 35182875.199999996,
            "unit": "ns",
            "range": "± 3150113.179089741"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 40423828.79722222,
            "unit": "ns",
            "range": "± 5690299.873464118"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 504091.0269059806,
            "unit": "ns",
            "range": "± 3695.432429781256"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3008068.064903846,
            "unit": "ns",
            "range": "± 45109.15223524012"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 312453719.5769231,
            "unit": "ns",
            "range": "± 3884308.612067472"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 312895523.5,
            "unit": "ns",
            "range": "± 4294106.114399673"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 320598461.3214286,
            "unit": "ns",
            "range": "± 7939304.038857459"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7393716.873842592,
            "unit": "ns",
            "range": "± 165125.7141654763"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11058138.41875,
            "unit": "ns",
            "range": "± 629270.4785795169"
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
          "id": "30b18683536337f2c0299aaf3e1377f6d13208b0",
          "message": "Enhance constructor handling in ExpressiveSharp generator and fixed various issues",
          "timestamp": "2026-04-13T22:06:20Z",
          "tree_id": "b6d511fe84a67a9b423fc2038b05164cf4a07415",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/30b18683536337f2c0299aaf3e1377f6d13208b0"
        },
        "date": 1776119555836,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7435.998924820511,
            "unit": "ns",
            "range": "± 44.65946817239881"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 3047.0756061260518,
            "unit": "ns",
            "range": "± 8.11669653528217"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.294006345172723,
            "unit": "ns",
            "range": "± 0.11053467599164173"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 180.26898050705591,
            "unit": "ns",
            "range": "± 4.958460557601877"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 22292.626424153645,
            "unit": "ns",
            "range": "± 219.70589983685124"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3184.653360595703,
            "unit": "ns",
            "range": "± 20.33483267150415"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.91057842693947,
            "unit": "ns",
            "range": "± 0.02244867702316982"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 87.66749907391412,
            "unit": "ns",
            "range": "± 1.3802487893738444"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21321.307915581598,
            "unit": "ns",
            "range": "± 219.57913522718923"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5464.849008413462,
            "unit": "ns",
            "range": "± 38.44474827679111"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.863451373896428,
            "unit": "ns",
            "range": "± 0.01519911050018546"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 76.31825099609516,
            "unit": "ns",
            "range": "± 1.9520610275681343"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 28044.591417100695,
            "unit": "ns",
            "range": "± 158.16096197473072"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6022.226103515625,
            "unit": "ns",
            "range": "± 24.56685192060319"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.614012932887784,
            "unit": "ns",
            "range": "± 0.13439259997480926"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.279847576068,
            "unit": "ns",
            "range": "± 3.05697780527327"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 115576.365625,
            "unit": "ns",
            "range": "± 1308.2422299183793"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18787.315380859374,
            "unit": "ns",
            "range": "± 57.61577435879605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.952634057296174,
            "unit": "ns",
            "range": "± 0.04559187706838494"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18208.930225917273,
            "unit": "ns",
            "range": "± 74.77667167118543"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 82209.88309733073,
            "unit": "ns",
            "range": "± 396.43098249228666"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.52851971754661,
            "unit": "ns",
            "range": "± 0.010283197458083449"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.588678024452308,
            "unit": "ns",
            "range": "± 0.03760176134496652"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.029949600326605,
            "unit": "ns",
            "range": "± 0.04070000387821127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 274154.5181039664,
            "unit": "ns",
            "range": "± 4445.5025426878865"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 257100.8463429418,
            "unit": "ns",
            "range": "± 3282.7080296550544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1020794.9147135416,
            "unit": "ns",
            "range": "± 109711.18198032156"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1007011.6356770833,
            "unit": "ns",
            "range": "± 106463.43092335307"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1011006.758203125,
            "unit": "ns",
            "range": "± 88716.65188521288"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54285.41017972506,
            "unit": "ns",
            "range": "± 827.5276732735425"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 261755.85089983259,
            "unit": "ns",
            "range": "± 17032.4324811484"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21936846.78125,
            "unit": "ns",
            "range": "± 83978.11425341468"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21381774.234953705,
            "unit": "ns",
            "range": "± 118891.95002258483"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 38853788.12222223,
            "unit": "ns",
            "range": "± 3337172.0011392296"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 36532235.653333336,
            "unit": "ns",
            "range": "± 2774357.9935231223"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 43405682.311111115,
            "unit": "ns",
            "range": "± 5492800.245731172"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 469723.887190194,
            "unit": "ns",
            "range": "± 3832.458200879321"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3295955.8255208335,
            "unit": "ns",
            "range": "± 218063.1984023194"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 314599869.1666667,
            "unit": "ns",
            "range": "± 1302000.2675135192"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 316622370.5,
            "unit": "ns",
            "range": "± 3888249.6377253584"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 318091054.7692308,
            "unit": "ns",
            "range": "± 3798885.0694804685"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 6869352.359635416,
            "unit": "ns",
            "range": "± 149089.60850150185"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10883376.216666667,
            "unit": "ns",
            "range": "± 369040.8410426291"
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
          "id": "5168e3ed0ee7c26b6de7db11cb4a1066a42b35f0",
          "message": "dropped cosmosdb as a sampel target",
          "timestamp": "2026-04-13T23:15:02Z",
          "tree_id": "592516ab4a2f7270725eaa30d56a17f202a1f804",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/5168e3ed0ee7c26b6de7db11cb4a1066a42b35f0"
        },
        "date": 1776123673647,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7143.974992207119,
            "unit": "ns",
            "range": "± 78.25219457141876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2928.4757792154946,
            "unit": "ns",
            "range": "± 12.167800049654304"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.403454988621748,
            "unit": "ns",
            "range": "± 0.20003743353960277"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 171.33347481030685,
            "unit": "ns",
            "range": "± 1.978139831264564"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21326.210366385323,
            "unit": "ns",
            "range": "± 199.076762682782"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2982.498316838191,
            "unit": "ns",
            "range": "± 14.405034799689199"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.38334893297266,
            "unit": "ns",
            "range": "± 0.5574086872353307"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 81.45945148743115,
            "unit": "ns",
            "range": "± 0.38803485094485235"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21234.0245314378,
            "unit": "ns",
            "range": "± 382.1773653039976"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5473.617489950998,
            "unit": "ns",
            "range": "± 114.43975763560982"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.223492651625916,
            "unit": "ns",
            "range": "± 0.39602462591832166"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 75.50891862007288,
            "unit": "ns",
            "range": "± 1.3395153649448037"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27172.687565730168,
            "unit": "ns",
            "range": "± 399.5818713632741"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6004.624120251886,
            "unit": "ns",
            "range": "± 30.42714331845193"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.544462149696691,
            "unit": "ns",
            "range": "± 0.059473135758926175"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 56.17563409435338,
            "unit": "ns",
            "range": "± 0.06198792201550137"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 112030.202130353,
            "unit": "ns",
            "range": "± 1627.5364470349182"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18335.046361852576,
            "unit": "ns",
            "range": "± 145.20653994795833"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.192734303659407,
            "unit": "ns",
            "range": "± 0.27362323352664597"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17897.078572199895,
            "unit": "ns",
            "range": "± 133.8154288896273"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 79853.86035970053,
            "unit": "ns",
            "range": "± 706.7104718890388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.581897384647664,
            "unit": "ns",
            "range": "± 0.037328811112464465"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.603708090384801,
            "unit": "ns",
            "range": "± 0.07177887390074389"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.048934607670224,
            "unit": "ns",
            "range": "± 0.034096056791789175"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 271152.0414315683,
            "unit": "ns",
            "range": "± 3737.034890352271"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 256221.32748647837,
            "unit": "ns",
            "range": "± 3532.978511362176"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 985910.4322916666,
            "unit": "ns",
            "range": "± 87981.2926636241"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 886292.5714285715,
            "unit": "ns",
            "range": "± 139183.08416560225"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 972866.53046875,
            "unit": "ns",
            "range": "± 79699.79541468342"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 52263.87674515336,
            "unit": "ns",
            "range": "± 1480.4861043858575"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 257191.06986177884,
            "unit": "ns",
            "range": "± 1789.9367341390803"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21446363.012931034,
            "unit": "ns",
            "range": "± 273087.946388424"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21211799.240625,
            "unit": "ns",
            "range": "± 145475.26224554595"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 38487293.548888884,
            "unit": "ns",
            "range": "± 5887863.326936682"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 39670596.06111111,
            "unit": "ns",
            "range": "± 4786366.403852297"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 33615468.33541667,
            "unit": "ns",
            "range": "± 4606804.675719281"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 451181.13700810185,
            "unit": "ns",
            "range": "± 1732.4014783582452"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2845676.494591346,
            "unit": "ns",
            "range": "± 15028.350201605255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 307775789.68,
            "unit": "ns",
            "range": "± 1720893.0461802315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 309822325.36,
            "unit": "ns",
            "range": "± 2771167.1296697184"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 309264135.44,
            "unit": "ns",
            "range": "± 2124594.459169288"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 6652510.241648707,
            "unit": "ns",
            "range": "± 144112.09694215565"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10256541.2,
            "unit": "ns",
            "range": "± 106343.21092343872"
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
          "id": "5066cea4ba22093de8564710ca5836d63072256b",
          "message": "feat: add CopyPageButton component and enhance layout with expanded sample support",
          "timestamp": "2026-04-14T00:16:13Z",
          "tree_id": "0808f59de2a4225c76d2830e36134d486420b679",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/5066cea4ba22093de8564710ca5836d63072256b"
        },
        "date": 1776127317162,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7276.723601749965,
            "unit": "ns",
            "range": "± 40.2552050040437"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 3056.336129760742,
            "unit": "ns",
            "range": "± 37.80136096294284"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.205070645644747,
            "unit": "ns",
            "range": "± 0.019541785623228707"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 179.89311598087178,
            "unit": "ns",
            "range": "± 2.1135674423648863"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 21952.415087018693,
            "unit": "ns",
            "range": "± 233.5798150802638"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3073.449491712782,
            "unit": "ns",
            "range": "± 24.158603765592"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.892278107149261,
            "unit": "ns",
            "range": "± 0.024797010421398754"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 85.68470505599318,
            "unit": "ns",
            "range": "± 5.864644659145902"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 22171.494570131654,
            "unit": "ns",
            "range": "± 146.3674692917798"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5501.406770978655,
            "unit": "ns",
            "range": "± 38.10041013259793"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.974038499182669,
            "unit": "ns",
            "range": "± 0.0719916739117752"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 74.10184734142743,
            "unit": "ns",
            "range": "± 0.06600304956628304"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27821.853701171876,
            "unit": "ns",
            "range": "± 1227.7568946592694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6066.0884355817525,
            "unit": "ns",
            "range": "± 39.598035142645536"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.525265766628857,
            "unit": "ns",
            "range": "± 0.015169747365127878"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 53.910552540650734,
            "unit": "ns",
            "range": "± 3.018516493041879"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 112362.41606613685,
            "unit": "ns",
            "range": "± 828.4152556583151"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18861.361140498408,
            "unit": "ns",
            "range": "± 278.7162582790473"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.913694874993686,
            "unit": "ns",
            "range": "± 0.022409440666317216"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18303.83707838792,
            "unit": "ns",
            "range": "± 256.6234032210394"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 81357.10540140086,
            "unit": "ns",
            "range": "± 806.6655930895171"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.65828294886483,
            "unit": "ns",
            "range": "± 0.03163202814323991"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.583725735545158,
            "unit": "ns",
            "range": "± 0.014745691781567217"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.04255185353345,
            "unit": "ns",
            "range": "± 0.045280968286369745"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 272567.5270298549,
            "unit": "ns",
            "range": "± 1529.8526987790076"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 265367.71668836806,
            "unit": "ns",
            "range": "± 3663.5925016640854"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1019905.5901041667,
            "unit": "ns",
            "range": "± 114506.31857410814"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 992185.5126302083,
            "unit": "ns",
            "range": "± 94940.38181402437"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1021463.85234375,
            "unit": "ns",
            "range": "± 91441.1574745995"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 53960.232219989484,
            "unit": "ns",
            "range": "± 711.1104340154226"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 268047.614327567,
            "unit": "ns",
            "range": "± 15803.712122772777"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21890461.089439657,
            "unit": "ns",
            "range": "± 241072.87744725824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21727325.447198275,
            "unit": "ns",
            "range": "± 131692.624874034"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 45077924.877777785,
            "unit": "ns",
            "range": "± 4413201.245292412"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 45729588.85555555,
            "unit": "ns",
            "range": "± 5097551.364420474"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 40292422.93055556,
            "unit": "ns",
            "range": "± 4415372.298547726"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 467396.695783944,
            "unit": "ns",
            "range": "± 4014.0020948746305"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3454034.6625,
            "unit": "ns",
            "range": "± 298687.67666266405"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 313426916.84,
            "unit": "ns",
            "range": "± 3631082.728622377"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 325441023.7037037,
            "unit": "ns",
            "range": "± 7387732.535494442"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 325963840.10714287,
            "unit": "ns",
            "range": "± 8983104.875031631"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7708467.288541666,
            "unit": "ns",
            "range": "± 385697.93498324876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11486479.894791666,
            "unit": "ns",
            "range": "± 460931.6799953294"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "245ce8bcdb13048bf64a815ee983363c25e1e15b",
          "message": "Merge pull request #36 from EFNext/fix/issue-35\n\nSupport nullable and value type projectables",
          "timestamp": "2026-04-15T21:43:45+01:00",
          "tree_id": "ded28d482614b4ad5abed2c8ff490570b3dba92a",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/245ce8bcdb13048bf64a815ee983363c25e1e15b"
        },
        "date": 1776287439327,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7397.560116322836,
            "unit": "ns",
            "range": "± 112.37861817509358"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 3062.878204871868,
            "unit": "ns",
            "range": "± 51.17426231796911"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.549263639251391,
            "unit": "ns",
            "range": "± 0.36757034794915144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 177.6788438643728,
            "unit": "ns",
            "range": "± 2.235080598938967"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 22209.01286711516,
            "unit": "ns",
            "range": "± 281.2878399568353"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3043.8148158146787,
            "unit": "ns",
            "range": "± 12.485024384555176"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.846069200680805,
            "unit": "ns",
            "range": "± 0.02751556087685373"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 82.26743916396437,
            "unit": "ns",
            "range": "± 2.4916414043859767"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21284.734036959133,
            "unit": "ns",
            "range": "± 99.2858239054827"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5434.493427417897,
            "unit": "ns",
            "range": "± 20.932801473224515"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.874271169304848,
            "unit": "ns",
            "range": "± 1.1005934357034113"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 75.25775686458304,
            "unit": "ns",
            "range": "± 0.9699825772831533"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27541.481480189734,
            "unit": "ns",
            "range": "± 272.5218577503017"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 6104.193526695514,
            "unit": "ns",
            "range": "± 26.547520109146394"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.5017393049266605,
            "unit": "ns",
            "range": "± 0.01338183613895258"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 55.044599101461216,
            "unit": "ns",
            "range": "± 3.5059360822840064"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 111884.1846881735,
            "unit": "ns",
            "range": "± 484.051453659709"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18609.502200535364,
            "unit": "ns",
            "range": "± 83.06918045909339"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.0262183439115,
            "unit": "ns",
            "range": "± 0.10302211757102381"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18483.037299262152,
            "unit": "ns",
            "range": "± 493.4410185906802"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80708.18549215383,
            "unit": "ns",
            "range": "± 269.0499771628628"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.596519381359771,
            "unit": "ns",
            "range": "± 0.06023593957880282"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.550505297603431,
            "unit": "ns",
            "range": "± 0.010981736319227726"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.046918984750906,
            "unit": "ns",
            "range": "± 0.033039653552447514"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 266806.1748918806,
            "unit": "ns",
            "range": "± 991.7051761437808"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 259941.0118359375,
            "unit": "ns",
            "range": "± 3288.108653431124"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 979111.0891927084,
            "unit": "ns",
            "range": "± 77892.88525368494"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 979171.8385416666,
            "unit": "ns",
            "range": "± 101773.6087667879"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 888755.083203125,
            "unit": "ns",
            "range": "± 126368.76148107223"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54334.64160608362,
            "unit": "ns",
            "range": "± 320.71540561837475"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 253789.36920572916,
            "unit": "ns",
            "range": "± 4165.726908928787"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21279532.316964287,
            "unit": "ns",
            "range": "± 147043.49357237623"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21825643.26077586,
            "unit": "ns",
            "range": "± 727207.9273757329"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37580257.28412698,
            "unit": "ns",
            "range": "± 7316322.5282344185"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 33749314.11388889,
            "unit": "ns",
            "range": "± 3839905.623418921"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 33388637.381111097,
            "unit": "ns",
            "range": "± 4121514.1886904687"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 457630.2467564174,
            "unit": "ns",
            "range": "± 1983.5975453871238"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2898441.482979911,
            "unit": "ns",
            "range": "± 21435.582527449897"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 308822499.24,
            "unit": "ns",
            "range": "± 1583542.4217855486"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 310849255.1111111,
            "unit": "ns",
            "range": "± 5072321.809709725"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 312519128.61538464,
            "unit": "ns",
            "range": "± 3722144.6742104813"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 6686512.73046875,
            "unit": "ns",
            "range": "± 74050.04974757269"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10238648.052801725,
            "unit": "ns",
            "range": "± 122560.87265140968"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "ad1718a984e29f2619a73ec977388d982ce477f3",
          "message": "Merge pull request #38 from EFNext/feat/expressive-for-enhancements\n\nSimplified ExpressiveFor",
          "timestamp": "2026-04-17T02:03:09+01:00",
          "tree_id": "f40ebe8b219152af91ff1e36a96129156c23b538",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/ad1718a984e29f2619a73ec977388d982ce477f3"
        },
        "date": 1776389388827,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6439.862832431136,
            "unit": "ns",
            "range": "± 26.27317930315539"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2995.9332685799436,
            "unit": "ns",
            "range": "± 63.45705935768521"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.62816950625607,
            "unit": "ns",
            "range": "± 0.37142284401051545"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 158.66697176982618,
            "unit": "ns",
            "range": "± 0.39615123073394254"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 17112.178139648437,
            "unit": "ns",
            "range": "± 190.85389875984146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3030.6532798180215,
            "unit": "ns",
            "range": "± 19.016009929687094"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.042412940661112,
            "unit": "ns",
            "range": "± 0.02918256925357703"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 78.39650709927082,
            "unit": "ns",
            "range": "± 0.7292612573777266"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 17105.220371791296,
            "unit": "ns",
            "range": "± 112.43712940264477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5371.868860517229,
            "unit": "ns",
            "range": "± 92.84448211145472"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 10.520295137805599,
            "unit": "ns",
            "range": "± 0.9124630559499333"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 75.32677542246304,
            "unit": "ns",
            "range": "± 1.287692446137514"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 22151.325927734375,
            "unit": "ns",
            "range": "± 301.9168416669396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5787.71762295427,
            "unit": "ns",
            "range": "± 24.055239641509825"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.308391201176814,
            "unit": "ns",
            "range": "± 0.007042563473452573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.08468217345384,
            "unit": "ns",
            "range": "± 0.6188468194471097"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 90223.56142578126,
            "unit": "ns",
            "range": "± 409.00983570902383"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18060.348323115595,
            "unit": "ns",
            "range": "± 191.31657193663202"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.481817657926253,
            "unit": "ns",
            "range": "± 0.019462894410794452"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17462.53369140625,
            "unit": "ns",
            "range": "± 92.73072876536263"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 58169.19050545528,
            "unit": "ns",
            "range": "± 768.9288824693108"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.44714051015951,
            "unit": "ns",
            "range": "± 0.4203676298451697"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.217887997054136,
            "unit": "ns",
            "range": "± 0.1899695702519429"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.980984822467521,
            "unit": "ns",
            "range": "± 0.09709797481866772"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 217157.06032986112,
            "unit": "ns",
            "range": "± 2927.972105770565"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 206270.01535560345,
            "unit": "ns",
            "range": "± 1688.173604016841"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1004617.046484375,
            "unit": "ns",
            "range": "± 84990.81180684986"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1029599.0703125,
            "unit": "ns",
            "range": "± 122689.1864089084"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 892489.3470052084,
            "unit": "ns",
            "range": "± 132534.9268646408"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 35737.250479239,
            "unit": "ns",
            "range": "± 381.0304886918274"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 231796.46613420759,
            "unit": "ns",
            "range": "± 1943.4380664667844"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17483536.54129464,
            "unit": "ns",
            "range": "± 367086.2118130388"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17356029.645089287,
            "unit": "ns",
            "range": "± 341412.51291536214"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 36220676.59166667,
            "unit": "ns",
            "range": "± 3150698.0746544674"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 37173849.166666664,
            "unit": "ns",
            "range": "± 3183951.0375003563"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36456132.18333333,
            "unit": "ns",
            "range": "± 3115678.907641427"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 444492.2322535022,
            "unit": "ns",
            "range": "± 3094.565282032247"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3080175.2645474137,
            "unit": "ns",
            "range": "± 249070.767041397"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 306480121.0769231,
            "unit": "ns",
            "range": "± 2437278.767659275"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 305814785.92,
            "unit": "ns",
            "range": "± 2349881.369188381"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 306151069.037037,
            "unit": "ns",
            "range": "± 4292265.980296068"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7197222.454326923,
            "unit": "ns",
            "range": "± 42930.07158867622"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10902193.755208334,
            "unit": "ns",
            "range": "± 106144.27334640655"
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
          "id": "786c8470f800fdc810bc7f77fba7119302bdbdaf",
          "message": "fix: emit early-return block bodies as nested Condition expressions",
          "timestamp": "2026-04-19T23:31:50Z",
          "tree_id": "3b9f36f71a75a73eabfe15a43e629858d44f9daa",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/786c8470f800fdc810bc7f77fba7119302bdbdaf"
        },
        "date": 1776643160104,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6376.544802435513,
            "unit": "ns",
            "range": "± 47.245061075903266"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2941.601339612688,
            "unit": "ns",
            "range": "± 15.448894856862642"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.124901076329165,
            "unit": "ns",
            "range": "± 0.18997517250170026"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 157.81516781678567,
            "unit": "ns",
            "range": "± 1.5755402828642708"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 16958.823034215857,
            "unit": "ns",
            "range": "± 133.09364993470172"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2968.468710092398,
            "unit": "ns",
            "range": "± 17.531755100075415"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.627590308231968,
            "unit": "ns",
            "range": "± 0.5979435351415977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 54.3226949262619,
            "unit": "ns",
            "range": "± 1.6332437117153122"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 16939.746776439526,
            "unit": "ns",
            "range": "± 220.25853758139075"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5318.087430681501,
            "unit": "ns",
            "range": "± 55.987095807042095"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.842306520503302,
            "unit": "ns",
            "range": "± 0.18276622429630343"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 53.480641192623544,
            "unit": "ns",
            "range": "± 0.853502671135215"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 21966.440307617188,
            "unit": "ns",
            "range": "± 371.51297870671726"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5625.382855341984,
            "unit": "ns",
            "range": "± 28.120206106158275"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 9.232926025986671,
            "unit": "ns",
            "range": "± 0.8918199499015682"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 52.01138500869274,
            "unit": "ns",
            "range": "± 2.8280132512906095"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 90042.75863211496,
            "unit": "ns",
            "range": "± 394.4569647203955"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 17759.21863424367,
            "unit": "ns",
            "range": "± 100.41916191114015"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.278914432282802,
            "unit": "ns",
            "range": "± 0.18501072693846826"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17386.486681707975,
            "unit": "ns",
            "range": "± 112.61285276953002"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 57974.28321620396,
            "unit": "ns",
            "range": "± 501.7807277674242"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.342425320435453,
            "unit": "ns",
            "range": "± 0.057507289519468596"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.28642969244513,
            "unit": "ns",
            "range": "± 0.0878235880815611"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 9.255990273479757,
            "unit": "ns",
            "range": "± 0.6051418673081679"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 215649.79079861112,
            "unit": "ns",
            "range": "± 1001.8787243796354"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 200339.01141131367,
            "unit": "ns",
            "range": "± 2708.734644265652"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 888143.4286458333,
            "unit": "ns",
            "range": "± 137272.19352020297"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 759859.162248884,
            "unit": "ns",
            "range": "± 48641.75868860085"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 888561.7871767242,
            "unit": "ns",
            "range": "± 141239.93683927035"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 36121.111018880205,
            "unit": "ns",
            "range": "± 293.8053015497403"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 226545.31829202586,
            "unit": "ns",
            "range": "± 5228.744187290383"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 17739201.257543102,
            "unit": "ns",
            "range": "± 95189.83430559326"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 17214479.73275862,
            "unit": "ns",
            "range": "± 498220.4039323404"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 37863715.858333334,
            "unit": "ns",
            "range": "± 3239319.492516306"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 35773351.141666666,
            "unit": "ns",
            "range": "± 2630911.1902240007"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 35679090.04666666,
            "unit": "ns",
            "range": "± 3182310.005870418"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 442800.5500404095,
            "unit": "ns",
            "range": "± 3488.9442227092186"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3131002.2893415177,
            "unit": "ns",
            "range": "± 257422.71343086418"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 305050384.1851852,
            "unit": "ns",
            "range": "± 3453354.2128020767"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 304682842.48,
            "unit": "ns",
            "range": "± 2605892.2930961694"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 303713612.64,
            "unit": "ns",
            "range": "± 2372464.6438696557"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7144628.3938577585,
            "unit": "ns",
            "range": "± 81351.56466110644"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10956711.436422413,
            "unit": "ns",
            "range": "± 164302.81538920838"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "180962a67e27f05ea3f7d2fe227dd83329cfbc66",
          "message": "Merge pull request #39 from EFNext/feat/hot-reload-support\n\nImplement hot reload support with cache clearing and registry reset",
          "timestamp": "2026-04-20T01:18:20+01:00",
          "tree_id": "50169793ad0bccad92b3508588c5845053edb013",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/180962a67e27f05ea3f7d2fe227dd83329cfbc66"
        },
        "date": 1776645861292,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7391.977745310465,
            "unit": "ns",
            "range": "± 116.83103034963086"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 3049.3616191722726,
            "unit": "ns",
            "range": "± 33.05392012736029"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.246402227768192,
            "unit": "ns",
            "range": "± 0.009739716400084718"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 175.18625634908676,
            "unit": "ns",
            "range": "± 3.3790644971089763"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 22333.886673538775,
            "unit": "ns",
            "range": "± 362.98172394184985"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3050.03990818904,
            "unit": "ns",
            "range": "± 9.882085690024173"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.963353928710733,
            "unit": "ns",
            "range": "± 0.11503675169469312"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 60.06888867749108,
            "unit": "ns",
            "range": "± 2.4753123677341398"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21472.759553132233,
            "unit": "ns",
            "range": "± 153.76802045701515"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5450.709403404822,
            "unit": "ns",
            "range": "± 25.75928043971857"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.4177427302908,
            "unit": "ns",
            "range": "± 0.5612930111208566"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 59.965741229057315,
            "unit": "ns",
            "range": "± 3.9981732648752515"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27725.018418532152,
            "unit": "ns",
            "range": "± 462.1353324485605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5943.494366681134,
            "unit": "ns",
            "range": "± 89.45657850710407"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.580696004842009,
            "unit": "ns",
            "range": "± 0.039875096375023816"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 50.99288753100804,
            "unit": "ns",
            "range": "± 0.658702350560854"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 114368.47410300926,
            "unit": "ns",
            "range": "± 983.5657028012522"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18400.905705566405,
            "unit": "ns",
            "range": "± 62.53951238360515"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.910381800532341,
            "unit": "ns",
            "range": "± 0.031201240786890478"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18421.619746616907,
            "unit": "ns",
            "range": "± 133.1264776449512"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80953.83369954427,
            "unit": "ns",
            "range": "± 1131.2226115467188"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.562240019440651,
            "unit": "ns",
            "range": "± 0.023524906079279633"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 9.55070974964362,
            "unit": "ns",
            "range": "± 0.015602192225744074"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.049353593587876,
            "unit": "ns",
            "range": "± 0.04585965617698809"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 1)",
            "value": 277222.17411747685,
            "unit": "ns",
            "range": "± 2982.662618043459"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 1)",
            "value": 261329.71506076388,
            "unit": "ns",
            "range": "± 1937.0015718851926"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 984938.041487069,
            "unit": "ns",
            "range": "± 96269.97725225425"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 977723.46484375,
            "unit": "ns",
            "range": "± 107654.00116355804"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 982281.219921875,
            "unit": "ns",
            "range": "± 98387.11601223824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54503.77823747908,
            "unit": "ns",
            "range": "± 689.0422109282946"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 257609.22049386162,
            "unit": "ns",
            "range": "± 2699.7189546713616"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator(CallSiteCount: 100)",
            "value": 21881814.65625,
            "unit": "ns",
            "range": "± 186894.27966450914"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillGeneratorBenchmarks.RunGenerator_Incremental(CallSiteCount: 100)",
            "value": 21477246.7375,
            "unit": "ns",
            "range": "± 650594.716763888"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 43536054.566666655,
            "unit": "ns",
            "range": "± 5967326.170486414"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 42184296.48888888,
            "unit": "ns",
            "range": "± 3930664.904598455"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 39748017.333333336,
            "unit": "ns",
            "range": "± 5602940.2976712985"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 470033.56595052086,
            "unit": "ns",
            "range": "± 1943.4122051029847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3146033.9360532407,
            "unit": "ns",
            "range": "± 253545.3046032629"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 310394941.2307692,
            "unit": "ns",
            "range": "± 2644244.144283993"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 314999205.72,
            "unit": "ns",
            "range": "± 3140826.27303275"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 311847392.5,
            "unit": "ns",
            "range": "± 4070206.831044823"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 6893912.7858297415,
            "unit": "ns",
            "range": "± 103436.32353352192"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11079166.6125,
            "unit": "ns",
            "range": "± 363738.84344753396"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "efe0586027d7986ca66689ba6b373480bd07a92f",
          "message": "Merge pull request #41 from EFNext/optimize-polyfill-gen\n\nImprove polyfill generator by emitting a generated file for each source file, into a partial class",
          "timestamp": "2026-04-21T19:14:29+01:00",
          "tree_id": "e1feaff816184ac8ce9659fd29dadd3987ef6291",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/efe0586027d7986ca66689ba6b373480bd07a92f"
        },
        "date": 1776797917802,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6422.9723561604815,
            "unit": "ns",
            "range": "± 68.7830605951722"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2922.8524709065755,
            "unit": "ns",
            "range": "± 21.03274953954724"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.336876693048648,
            "unit": "ns",
            "range": "± 0.061464005762815285"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 163.57654777888595,
            "unit": "ns",
            "range": "± 5.145656433547491"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 17154.778198242188,
            "unit": "ns",
            "range": "± 145.06717148191382"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2978.4278057643346,
            "unit": "ns",
            "range": "± 20.06723583772244"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.545606454765355,
            "unit": "ns",
            "range": "± 0.5408170645736382"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 53.4665880214285,
            "unit": "ns",
            "range": "± 0.09620819295276407"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 16805.27695138114,
            "unit": "ns",
            "range": "± 163.13724909369856"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5448.130681920935,
            "unit": "ns",
            "range": "± 171.41848303961163"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 10.057252945808264,
            "unit": "ns",
            "range": "± 0.02946727151166847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 53.00593355510916,
            "unit": "ns",
            "range": "± 0.018273562877682045"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 21866.609971266527,
            "unit": "ns",
            "range": "± 184.9691681675481"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5634.307535807292,
            "unit": "ns",
            "range": "± 38.41372108284532"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.222431346222207,
            "unit": "ns",
            "range": "± 0.21619560426809642"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 51.80657558529465,
            "unit": "ns",
            "range": "± 2.661643496303023"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 90341.25810895648,
            "unit": "ns",
            "range": "± 787.8777162447814"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 17742.375250680107,
            "unit": "ns",
            "range": "± 120.21726917810855"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.278778377388205,
            "unit": "ns",
            "range": "± 0.18575576423455453"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17483.447257206357,
            "unit": "ns",
            "range": "± 103.92956640788398"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 59055.076637550635,
            "unit": "ns",
            "range": "± 198.35273880093246"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.814006018638612,
            "unit": "ns",
            "range": "± 0.3651505579696551"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.136438937022769,
            "unit": "ns",
            "range": "± 0.059578951952247196"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.629878539325935,
            "unit": "ns",
            "range": "± 0.01166126192007033"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 1)",
            "value": 110484.44109881365,
            "unit": "ns",
            "range": "± 1972.9200397122042"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 1)",
            "value": 11832.254571848902,
            "unit": "ns",
            "range": "± 94.81937437934059"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 1)",
            "value": 103326.98229041466,
            "unit": "ns",
            "range": "± 640.1966940922212"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 1)",
            "value": 110038.29495804397,
            "unit": "ns",
            "range": "± 617.3249996155055"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 1)",
            "value": 11766.06450511791,
            "unit": "ns",
            "range": "± 28.841797657033027"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 1)",
            "value": 104158.5416353666,
            "unit": "ns",
            "range": "± 1588.401931736061"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1005762.9850260416,
            "unit": "ns",
            "range": "± 96210.4514402596"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 875265.62890625,
            "unit": "ns",
            "range": "± 124746.94746854652"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 783067.0567057292,
            "unit": "ns",
            "range": "± 70682.15743941147"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 35294.174211173224,
            "unit": "ns",
            "range": "± 187.24846793681988"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 227738.8231074892,
            "unit": "ns",
            "range": "± 6463.448504910098"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 1)",
            "value": 429254.44581886573,
            "unit": "ns",
            "range": "± 7178.485255993455"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 1)",
            "value": 420727.69981971156,
            "unit": "ns",
            "range": "± 3630.681100255446"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 1)",
            "value": 12997.1618309021,
            "unit": "ns",
            "range": "± 72.84254853955393"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 1)",
            "value": 433420.630234375,
            "unit": "ns",
            "range": "± 1366.4148386140291"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 1)",
            "value": 430697.31084735575,
            "unit": "ns",
            "range": "± 3290.4028996134925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 1)",
            "value": 13261.755636088054,
            "unit": "ns",
            "range": "± 156.54470290555486"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 5)",
            "value": 1952286.1044921875,
            "unit": "ns",
            "range": "± 11695.185849278036"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 5)",
            "value": 433190.92255108175,
            "unit": "ns",
            "range": "± 11098.054432193321"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 5)",
            "value": 21981.76814626058,
            "unit": "ns",
            "range": "± 80.73746176514825"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 5)",
            "value": 1974564.2516927083,
            "unit": "ns",
            "range": "± 7025.467358697458"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 5)",
            "value": 435970.4112079327,
            "unit": "ns",
            "range": "± 4038.632578747008"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 5)",
            "value": 22414.04521442282,
            "unit": "ns",
            "range": "± 515.4233440828697"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 10)",
            "value": 974454.3900862068,
            "unit": "ns",
            "range": "± 145828.11945545097"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 10)",
            "value": 12016.831538609096,
            "unit": "ns",
            "range": "± 92.47569471578345"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 10)",
            "value": 804706.0050390625,
            "unit": "ns",
            "range": "± 2075.739890317524"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 10)",
            "value": 805628.1584821428,
            "unit": "ns",
            "range": "± 2240.821179493519"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 10)",
            "value": 12172.55636333597,
            "unit": "ns",
            "range": "± 191.4662708297327"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 10)",
            "value": 812299.6102818081,
            "unit": "ns",
            "range": "± 11100.166567891654"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 10)",
            "value": 3963378.580636161,
            "unit": "ns",
            "range": "± 16491.400057851948"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 10)",
            "value": 450059.4796368634,
            "unit": "ns",
            "range": "± 3316.206634860959"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 10)",
            "value": 33935.98791721889,
            "unit": "ns",
            "range": "± 590.7103395350069"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 10)",
            "value": 3967587.632254464,
            "unit": "ns",
            "range": "± 29959.638301983967"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 10)",
            "value": 448321.7384375,
            "unit": "ns",
            "range": "± 4265.096496463269"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 10)",
            "value": 34304.762655323946,
            "unit": "ns",
            "range": "± 499.51749329205074"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 100)",
            "value": 7983407.695581896,
            "unit": "ns",
            "range": "± 35533.9236113422"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 100)",
            "value": 11883.712900797525,
            "unit": "ns",
            "range": "± 169.16670674856533"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 100)",
            "value": 7785049.372685186,
            "unit": "ns",
            "range": "± 88677.18434387736"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 100)",
            "value": 7907062.501736111,
            "unit": "ns",
            "range": "± 26931.98553787324"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 100)",
            "value": 12184.384684069404,
            "unit": "ns",
            "range": "± 290.2535441356626"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 100)",
            "value": 7771447.69,
            "unit": "ns",
            "range": "± 77728.01193389478"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 32580984.69642857,
            "unit": "ns",
            "range": "± 3590312.9492748817"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 37102667.56666667,
            "unit": "ns",
            "range": "± 3359690.176552702"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36427973.21666667,
            "unit": "ns",
            "range": "± 3264471.2659332184"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 445647.50556640624,
            "unit": "ns",
            "range": "± 1642.8414507594607"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2907158.4771634615,
            "unit": "ns",
            "range": "± 13383.973873477102"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 303709742.3076923,
            "unit": "ns",
            "range": "± 2273243.598503403"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 305664817.88,
            "unit": "ns",
            "range": "± 2365870.9712665956"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 305563320,
            "unit": "ns",
            "range": "± 2727550.3650742183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7102121.465247845,
            "unit": "ns",
            "range": "± 91264.97453144658"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10835175.113541666,
            "unit": "ns",
            "range": "± 113740.88793221666"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "af72c8d5a73792e13c5c54e0f5bc403a27289344",
          "message": "Merge pull request #42 from EFNext/feat/polyfill-cold-build-optimization\n\nperf(polyfill): skip semantic binding on non-lambda invocations",
          "timestamp": "2026-04-24T16:16:44+01:00",
          "tree_id": "27c4156e70f41cc47180432998083940ccae2d14",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/af72c8d5a73792e13c5c54e0f5bc403a27289344"
        },
        "date": 1777046938747,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7440.468531115302,
            "unit": "ns",
            "range": "± 189.33809812496892"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 3080.7064999171666,
            "unit": "ns",
            "range": "± 39.86144850285545"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.246689861416817,
            "unit": "ns",
            "range": "± 0.01330446896077748"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 213.61737077236177,
            "unit": "ns",
            "range": "± 4.834040156834624"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 22222.626268659318,
            "unit": "ns",
            "range": "± 367.10487088252745"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3136.7346016212746,
            "unit": "ns",
            "range": "± 19.24854686320308"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.228307983941502,
            "unit": "ns",
            "range": "± 0.3806429875244549"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 60.783500675360365,
            "unit": "ns",
            "range": "± 2.1838367842753144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21310.455471462672,
            "unit": "ns",
            "range": "± 335.3117868853342"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5501.651454219112,
            "unit": "ns",
            "range": "± 18.79118438746977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 8.865647391676903,
            "unit": "ns",
            "range": "± 0.02780388692113425"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 59.96249506596861,
            "unit": "ns",
            "range": "± 4.487363572991385"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27760.163429542823,
            "unit": "ns",
            "range": "± 502.9762523263897"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5975.135623508029,
            "unit": "ns",
            "range": "± 43.93559316578536"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.571761826502866,
            "unit": "ns",
            "range": "± 0.032731372760744275"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 57.484117339764325,
            "unit": "ns",
            "range": "± 1.204784686731563"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 116535.30278862847,
            "unit": "ns",
            "range": "± 1027.6343230331315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18711.40228881836,
            "unit": "ns",
            "range": "± 206.25410871466272"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.9004340376015065,
            "unit": "ns",
            "range": "± 0.021060952952707056"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18322.526834422144,
            "unit": "ns",
            "range": "± 72.50934483269117"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 82166.58915201823,
            "unit": "ns",
            "range": "± 1345.8767151826708"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.567220197916031,
            "unit": "ns",
            "range": "± 0.028385012910576053"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 11.329833766015676,
            "unit": "ns",
            "range": "± 0.00905335562591853"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.321471226626429,
            "unit": "ns",
            "range": "± 0.21113577343515463"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 0)",
            "value": 6217180.148148148,
            "unit": "ns",
            "range": "± 113428.54884641887"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 0)",
            "value": 812340.9239628232,
            "unit": "ns",
            "range": "± 109091.45049048254"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 0)",
            "value": 6214378.6516702585,
            "unit": "ns",
            "range": "± 45179.99745503282"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 0)",
            "value": 818790.8761858259,
            "unit": "ns",
            "range": "± 95389.31236201515"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 1)",
            "value": 147466.53398786273,
            "unit": "ns",
            "range": "± 1996.7973517023074"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 1)",
            "value": 14406.258961995443,
            "unit": "ns",
            "range": "± 725.2474050953692"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 1)",
            "value": 140682.9053867885,
            "unit": "ns",
            "range": "± 744.5059363358653"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 1)",
            "value": 147288.68474469866,
            "unit": "ns",
            "range": "± 1395.1749732145158"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 1)",
            "value": 13817.019222683377,
            "unit": "ns",
            "range": "± 223.78852432351687"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 1)",
            "value": 141370.23067672164,
            "unit": "ns",
            "range": "± 591.2854192735472"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1015286.6548177083,
            "unit": "ns",
            "range": "± 108538.92763313449"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1008471.6533854167,
            "unit": "ns",
            "range": "± 102020.91565934256"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1030312.40078125,
            "unit": "ns",
            "range": "± 115106.47187691003"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 55602.28727774785,
            "unit": "ns",
            "range": "± 573.5851099994447"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 257656.0751591435,
            "unit": "ns",
            "range": "± 6635.3053457942615"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 1)",
            "value": 544160.4397424768,
            "unit": "ns",
            "range": "± 9542.331082342595"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 1)",
            "value": 532576.768359375,
            "unit": "ns",
            "range": "± 5566.3924825283475"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 1)",
            "value": 15717.938817342123,
            "unit": "ns",
            "range": "± 1102.6192083299586"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 1)",
            "value": 543726.9271556713,
            "unit": "ns",
            "range": "± 9115.235764168394"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 1)",
            "value": 539196.1877893518,
            "unit": "ns",
            "range": "± 8151.051949905663"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 1)",
            "value": 15669.642823317956,
            "unit": "ns",
            "range": "± 224.87421531128294"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 5)",
            "value": 2499953.3540219907,
            "unit": "ns",
            "range": "± 20892.20941520911"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 5)",
            "value": 540704.0410853794,
            "unit": "ns",
            "range": "± 4794.419170076799"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 5)",
            "value": 25336.489453125,
            "unit": "ns",
            "range": "± 290.94746749197736"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 5)",
            "value": 2467277.6799768517,
            "unit": "ns",
            "range": "± 21534.380638971936"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 5)",
            "value": 548212.6585036058,
            "unit": "ns",
            "range": "± 5125.765289444609"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 5)",
            "value": 25176.69287109375,
            "unit": "ns",
            "range": "± 571.8989794798338"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 10)",
            "value": 1028619.8161368534,
            "unit": "ns",
            "range": "± 63124.34653527811"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 10)",
            "value": 13326.49949809483,
            "unit": "ns",
            "range": "± 84.45810343279955"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 10)",
            "value": 972713.1612025669,
            "unit": "ns",
            "range": "± 7138.969523632165"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 10)",
            "value": 999740.0666316106,
            "unit": "ns",
            "range": "± 11341.341358800104"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 10)",
            "value": 13320.394493647984,
            "unit": "ns",
            "range": "± 140.3985822723383"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 10)",
            "value": 974106.4843026621,
            "unit": "ns",
            "range": "± 15634.047619054145"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 10)",
            "value": 4938895.128502155,
            "unit": "ns",
            "range": "± 60924.989965842404"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 10)",
            "value": 565247.2603044182,
            "unit": "ns",
            "range": "± 3695.293004162667"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 10)",
            "value": 38754.05278342111,
            "unit": "ns",
            "range": "± 386.44483090970266"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 10)",
            "value": 4934465.372106481,
            "unit": "ns",
            "range": "± 99782.248547473"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 10)",
            "value": 577741.029296875,
            "unit": "ns",
            "range": "± 7132.995965137105"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 10)",
            "value": 37561.380161830355,
            "unit": "ns",
            "range": "± 692.2974338559488"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 25)",
            "value": 6740776.047433035,
            "unit": "ns",
            "range": "± 80747.95105051238"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 25)",
            "value": 758547.7846875,
            "unit": "ns",
            "range": "± 13896.608200592205"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 25)",
            "value": 6971554.680226293,
            "unit": "ns",
            "range": "± 269116.86176688864"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 25)",
            "value": 758309.0244864004,
            "unit": "ns",
            "range": "± 3582.871813103594"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 100)",
            "value": 9610923.044719828,
            "unit": "ns",
            "range": "± 85072.35181104293"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 100)",
            "value": 13290.585337886105,
            "unit": "ns",
            "range": "± 125.90238177319938"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 100)",
            "value": 9677101.905691965,
            "unit": "ns",
            "range": "± 44993.41270059299"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 100)",
            "value": 9535146.8359375,
            "unit": "ns",
            "range": "± 226490.56501517605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 100)",
            "value": 13599.665944671631,
            "unit": "ns",
            "range": "± 109.22287903415335"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 100)",
            "value": 9467496.112068966,
            "unit": "ns",
            "range": "± 82113.36441053764"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 42345658.133333325,
            "unit": "ns",
            "range": "± 4245235.470214332"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 36344833.083333336,
            "unit": "ns",
            "range": "± 2867565.9651961294"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 39599436.563888885,
            "unit": "ns",
            "range": "± 4649651.773730698"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 471424.0476831897,
            "unit": "ns",
            "range": "± 1184.0604237347309"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3292097.3671875,
            "unit": "ns",
            "range": "± 211851.74836138045"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 100)",
            "value": 7646138.152488426,
            "unit": "ns",
            "range": "± 181057.23521123605"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 100)",
            "value": 796945.2650824653,
            "unit": "ns",
            "range": "± 5840.933248360879"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 100)",
            "value": 7446359.841325431,
            "unit": "ns",
            "range": "± 60216.979936385214"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 100)",
            "value": 814590.1063368055,
            "unit": "ns",
            "range": "± 9646.659129346099"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 318135113.5925926,
            "unit": "ns",
            "range": "± 4554468.6156576555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 320531906.11538464,
            "unit": "ns",
            "range": "± 4035902.357435881"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 319100692.46153843,
            "unit": "ns",
            "range": "± 2641268.56246844"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7088321.111530173,
            "unit": "ns",
            "range": "± 111680.3349239727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 13506020.226851849,
            "unit": "ns",
            "range": "± 400531.9256985074"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "cc0c34638c49316447d00ddf0ba0173de470122e",
          "message": "Merge pull request #43 from EFNext/feat/gen-implementation-source-output\n\nRefactor source output registration in IDE generator",
          "timestamp": "2026-04-26T00:35:21+01:00",
          "tree_id": "b4316b096a4740a7125174c81b9f6ff87e3425fd",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/cc0c34638c49316447d00ddf0ba0173de470122e"
        },
        "date": 1777163311351,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6355.997755686442,
            "unit": "ns",
            "range": "± 52.90718960110186"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2934.442163194929,
            "unit": "ns",
            "range": "± 19.529064598469024"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.385557443455413,
            "unit": "ns",
            "range": "± 0.0973556554893191"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 169.6090742308518,
            "unit": "ns",
            "range": "± 2.60408546443615"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 17410.562459309895,
            "unit": "ns",
            "range": "± 158.87845602086963"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3006.911486921639,
            "unit": "ns",
            "range": "± 21.446881901602133"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 10.05102585894721,
            "unit": "ns",
            "range": "± 0.02119731370989029"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 52.97142610788345,
            "unit": "ns",
            "range": "± 0.019498051771350573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 17044.949087289664,
            "unit": "ns",
            "range": "± 155.86222259924847"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5280.57154410226,
            "unit": "ns",
            "range": "± 51.99624429867211"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.832405580015019,
            "unit": "ns",
            "range": "± 0.16596284562705477"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 52.984555195484845,
            "unit": "ns",
            "range": "± 0.3493222078815508"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 22031.753096969038,
            "unit": "ns",
            "range": "± 224.16335530419073"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5670.959967251481,
            "unit": "ns",
            "range": "± 42.06895291191062"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.16587499320507,
            "unit": "ns",
            "range": "± 0.18200552110315804"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 54.95977466476375,
            "unit": "ns",
            "range": "± 0.14040270145894002"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 92091.5931943696,
            "unit": "ns",
            "range": "± 719.9531149152632"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18036.693217686243,
            "unit": "ns",
            "range": "± 259.55208888182347"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 8.467363996165139,
            "unit": "ns",
            "range": "± 0.021743345072564917"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17401.171269008093,
            "unit": "ns",
            "range": "± 134.83886888981252"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 58751.30886314655,
            "unit": "ns",
            "range": "± 595.9907548549759"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.036463543772697,
            "unit": "ns",
            "range": "± 0.030056876603126764"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.432672900649218,
            "unit": "ns",
            "range": "± 0.06489907764815478"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.63997610764844,
            "unit": "ns",
            "range": "± 0.010837896556624487"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 0)",
            "value": 4990743.776506697,
            "unit": "ns",
            "range": "± 90936.16227381543"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 0)",
            "value": 556259.1829659598,
            "unit": "ns",
            "range": "± 5035.14125297784"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 0)",
            "value": 4806456.70842634,
            "unit": "ns",
            "range": "± 10147.276107721527"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 0)",
            "value": 546942.8192608173,
            "unit": "ns",
            "range": "± 2453.2506516394756"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 1)",
            "value": 104889.24656519397,
            "unit": "ns",
            "range": "± 2868.715927167156"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 1)",
            "value": 11818.883805411202,
            "unit": "ns",
            "range": "± 189.54186932935767"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 1)",
            "value": 101303.07506872107,
            "unit": "ns",
            "range": "± 1229.255126150258"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 1)",
            "value": 105710.69890485491,
            "unit": "ns",
            "range": "± 383.11068384771824"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 1)",
            "value": 12263.877277119955,
            "unit": "ns",
            "range": "± 54.566758713219706"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 1)",
            "value": 103181.56585467303,
            "unit": "ns",
            "range": "± 1443.1701224146884"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 887127.5279947916,
            "unit": "ns",
            "range": "± 138790.95391748653"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 893748.888671875,
            "unit": "ns",
            "range": "± 145999.51710011926"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 985350.8327047414,
            "unit": "ns",
            "range": "± 87723.77847990498"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 36285.74476044754,
            "unit": "ns",
            "range": "± 142.57008342554704"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 231644.02336516205,
            "unit": "ns",
            "range": "± 2882.149803059769"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 1)",
            "value": 416537.7029441551,
            "unit": "ns",
            "range": "± 4853.457308974238"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 1)",
            "value": 413593.61747685185,
            "unit": "ns",
            "range": "± 6506.717761853852"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 1)",
            "value": 13117.91486911116,
            "unit": "ns",
            "range": "± 234.49681495253603"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 1)",
            "value": 413853.1696777344,
            "unit": "ns",
            "range": "± 2270.5321442797767"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 1)",
            "value": 415329.6954571759,
            "unit": "ns",
            "range": "± 3250.9386882382933"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 1)",
            "value": 12966.112665303548,
            "unit": "ns",
            "range": "± 52.384123646896576"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 5)",
            "value": 1897589.0268049568,
            "unit": "ns",
            "range": "± 13590.544903422367"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 5)",
            "value": 422995.8761449353,
            "unit": "ns",
            "range": "± 1747.17707701696"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 5)",
            "value": 22306.184678254303,
            "unit": "ns",
            "range": "± 71.1545242083992"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 5)",
            "value": 1889459.5472005208,
            "unit": "ns",
            "range": "± 14911.86996908671"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 5)",
            "value": 415863.7177734375,
            "unit": "ns",
            "range": "± 2812.5709133438013"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 5)",
            "value": 21892.42452457973,
            "unit": "ns",
            "range": "± 155.45512774272223"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 10)",
            "value": 768411.8399784482,
            "unit": "ns",
            "range": "± 4677.342365955061"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 10)",
            "value": 11988.17700246175,
            "unit": "ns",
            "range": "± 228.13656136971179"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 10)",
            "value": 764335.5418911638,
            "unit": "ns",
            "range": "± 4015.9520857626244"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 10)",
            "value": 784331.8501481682,
            "unit": "ns",
            "range": "± 2499.001462297786"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 10)",
            "value": 12138.207806291251,
            "unit": "ns",
            "range": "± 180.12314175021876"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 10)",
            "value": 780107.4015764509,
            "unit": "ns",
            "range": "± 5053.253076700255"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 10)",
            "value": 3850020.1358816964,
            "unit": "ns",
            "range": "± 16921.726315547494"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 10)",
            "value": 437374.61328125,
            "unit": "ns",
            "range": "± 2623.467683484567"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 10)",
            "value": 33333.12653459822,
            "unit": "ns",
            "range": "± 506.4422538032499"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 10)",
            "value": 3861006.6311383927,
            "unit": "ns",
            "range": "± 30840.296837610746"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 10)",
            "value": 439912.0978009259,
            "unit": "ns",
            "range": "± 1861.0306795804056"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 10)",
            "value": 33373.23373518319,
            "unit": "ns",
            "range": "± 289.6154008265203"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 25)",
            "value": 5362452.083533654,
            "unit": "ns",
            "range": "± 89662.44430779848"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 25)",
            "value": 604670.8565538195,
            "unit": "ns",
            "range": "± 2567.980449919964"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 25)",
            "value": 5295465.934430803,
            "unit": "ns",
            "range": "± 25361.183794887158"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 25)",
            "value": 613472.2170662716,
            "unit": "ns",
            "range": "± 15110.756345109065"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 100)",
            "value": 7581062.950161638,
            "unit": "ns",
            "range": "± 80240.11889433068"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 100)",
            "value": 12009.999867932549,
            "unit": "ns",
            "range": "± 174.32245097439858"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 100)",
            "value": 7539472.377314814,
            "unit": "ns",
            "range": "± 37560.190811812725"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 100)",
            "value": 7520992.691685268,
            "unit": "ns",
            "range": "± 16820.62063467571"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 100)",
            "value": 12419.224816385906,
            "unit": "ns",
            "range": "± 120.12755235648945"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 100)",
            "value": 7590177.487239583,
            "unit": "ns",
            "range": "± 110757.41563184577"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 35711967.416666664,
            "unit": "ns",
            "range": "± 2686797.538670611"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 36272412.39,
            "unit": "ns",
            "range": "± 3329441.7619056627"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 37717523.36666667,
            "unit": "ns",
            "range": "± 3932037.128470299"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 446871.6522135417,
            "unit": "ns",
            "range": "± 2407.1209064308377"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3274847.9932291666,
            "unit": "ns",
            "range": "± 216901.46442576713"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 100)",
            "value": 6148593.61077009,
            "unit": "ns",
            "range": "± 29755.15166927911"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 100)",
            "value": 936651.0927083333,
            "unit": "ns",
            "range": "± 106940.12467770673"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 100)",
            "value": 6176723.401909722,
            "unit": "ns",
            "range": "± 21871.477074515024"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 100)",
            "value": 921036.359375,
            "unit": "ns",
            "range": "± 109767.21914997057"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 304938012.5,
            "unit": "ns",
            "range": "± 2572364.566517293"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 307789077.0769231,
            "unit": "ns",
            "range": "± 2937657.3284395295"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 306353084.38461536,
            "unit": "ns",
            "range": "± 3267329.0022409814"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7125952.3809267245,
            "unit": "ns",
            "range": "± 18337.358812364302"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10983708.229525862,
            "unit": "ns",
            "range": "± 157544.50778013677"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "07cde84acdbadcee8cd5902a20ba1b3c72142eab",
          "message": "Merge pull request #40 from EFNext/feat/expressive-for-synthesize\n\nWrap up replacing Projectables with ExpressiveFor with synthesized properties",
          "timestamp": "2026-04-27T02:39:17+01:00",
          "tree_id": "a86630e2e62fd74f5c7e63629df664b65d564fc8",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/07cde84acdbadcee8cd5902a20ba1b3c72142eab"
        },
        "date": 1777257113457,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 7611.298556988056,
            "unit": "ns",
            "range": "± 144.69019593180045"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2991.9437171672953,
            "unit": "ns",
            "range": "± 14.437892995862196"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 7.322269805051662,
            "unit": "ns",
            "range": "± 0.02878672504300519"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 173.31706718461854,
            "unit": "ns",
            "range": "± 0.7544032074894117"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 22159.167345319474,
            "unit": "ns",
            "range": "± 324.9655324624106"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3076.5164794921875,
            "unit": "ns",
            "range": "± 44.065572573854915"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 8.886986175981852,
            "unit": "ns",
            "range": "± 0.03508406982161183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 62.825169263062655,
            "unit": "ns",
            "range": "± 6.183821807997849"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 21497.265860421317,
            "unit": "ns",
            "range": "± 193.69517453958235"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5424.821073178892,
            "unit": "ns",
            "range": "± 62.788197183505574"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.021975969826734,
            "unit": "ns",
            "range": "± 0.11539814976970296"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 62.04181593862073,
            "unit": "ns",
            "range": "± 7.319848155531449"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 27636.451782226562,
            "unit": "ns",
            "range": "± 258.0015086562867"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5964.441966552735,
            "unit": "ns",
            "range": "± 189.585120483153"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 7.628031722136906,
            "unit": "ns",
            "range": "± 0.0452890346098278"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 57.0062264757497,
            "unit": "ns",
            "range": "± 0.722652255045471"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 113429.31081627155,
            "unit": "ns",
            "range": "± 2116.0809677131"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18498.469822324554,
            "unit": "ns",
            "range": "± 165.63233689015811"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 7.919684882920522,
            "unit": "ns",
            "range": "± 0.03401594328900338"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 18040.89124665437,
            "unit": "ns",
            "range": "± 60.51413240047944"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 80398.76217447917,
            "unit": "ns",
            "range": "± 888.9962712848849"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 9.531802075986679,
            "unit": "ns",
            "range": "± 0.01783507655611865"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.017281956142849,
            "unit": "ns",
            "range": "± 0.4844198409350977"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.494813739107204,
            "unit": "ns",
            "range": "± 0.5464533110153429"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 0)",
            "value": 6552420.536368535,
            "unit": "ns",
            "range": "± 154304.16452697176"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 0)",
            "value": 726185.988734654,
            "unit": "ns",
            "range": "± 7460.5289748883315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 0)",
            "value": 6390238.118024553,
            "unit": "ns",
            "range": "± 71686.44990865224"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 0)",
            "value": 728106.7152054398,
            "unit": "ns",
            "range": "± 8523.177522606753"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 1)",
            "value": 181652.98888739225,
            "unit": "ns",
            "range": "± 2267.546672490027"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 1)",
            "value": 24353.007255817283,
            "unit": "ns",
            "range": "± 298.1326925448247"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 1)",
            "value": 164087.72458984374,
            "unit": "ns",
            "range": "± 1726.076227103802"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 1)",
            "value": 182826.11877020475,
            "unit": "ns",
            "range": "± 2725.246202658405"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 1)",
            "value": 24547.73298908102,
            "unit": "ns",
            "range": "± 693.7906359886607"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 1)",
            "value": 167984.73763020834,
            "unit": "ns",
            "range": "± 969.3794948621336"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1010533.0634114583,
            "unit": "ns",
            "range": "± 88730.22824630927"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1030959.4102864583,
            "unit": "ns",
            "range": "± 104035.50252391324"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1035168.8659752156,
            "unit": "ns",
            "range": "± 70488.52731536378"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 65612.95948137555,
            "unit": "ns",
            "range": "± 1038.2205371329237"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 281116.9541766827,
            "unit": "ns",
            "range": "± 17335.22470565575"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 1)",
            "value": 572456.1036658654,
            "unit": "ns",
            "range": "± 1438.2177457893017"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 1)",
            "value": 570695.556780134,
            "unit": "ns",
            "range": "± 5454.383886987583"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 1)",
            "value": 24568.954326923078,
            "unit": "ns",
            "range": "± 138.89282646001465"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 1)",
            "value": 566368.654765625,
            "unit": "ns",
            "range": "± 16121.377348156619"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 1)",
            "value": 568135.4121844952,
            "unit": "ns",
            "range": "± 7133.742011592132"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 1)",
            "value": 25201.65249739022,
            "unit": "ns",
            "range": "± 168.67200382458302"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 5)",
            "value": 2531438.4285714286,
            "unit": "ns",
            "range": "± 35043.44929978111"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 5)",
            "value": 581186.398577009,
            "unit": "ns",
            "range": "± 4134.474018060127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 5)",
            "value": 36534.46066401555,
            "unit": "ns",
            "range": "± 228.43528922346533"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 5)",
            "value": 2582424.0993303573,
            "unit": "ns",
            "range": "± 27375.372720952015"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 5)",
            "value": 582756.704135237,
            "unit": "ns",
            "range": "± 8728.905651928659"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 5)",
            "value": 35674.52144949777,
            "unit": "ns",
            "range": "± 133.31959252834076"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 10)",
            "value": 1070585.3545673077,
            "unit": "ns",
            "range": "± 5085.017219807871"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 10)",
            "value": 25566.529015677315,
            "unit": "ns",
            "range": "± 1091.6817583909417"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 10)",
            "value": 1030446.0583984375,
            "unit": "ns",
            "range": "± 5668.459482093368"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 10)",
            "value": 1022824.7548466435,
            "unit": "ns",
            "range": "± 2629.2702027137657"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 10)",
            "value": 24419.65268470501,
            "unit": "ns",
            "range": "± 169.3252895993953"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 10)",
            "value": 1010539.9123263889,
            "unit": "ns",
            "range": "± 6545.703364478488"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 10)",
            "value": 4984201.3859375,
            "unit": "ns",
            "range": "± 48375.33208825544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 10)",
            "value": 578880.1888521635,
            "unit": "ns",
            "range": "± 2108.1542941145967"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 10)",
            "value": 49023.41454467773,
            "unit": "ns",
            "range": "± 360.5424151600617"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 10)",
            "value": 5037934.76032366,
            "unit": "ns",
            "range": "± 79373.59193076857"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 10)",
            "value": 601458.0640914352,
            "unit": "ns",
            "range": "± 3185.962492854652"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 10)",
            "value": 51693.563248854414,
            "unit": "ns",
            "range": "± 1348.5683646918144"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 25)",
            "value": 6819843.999730604,
            "unit": "ns",
            "range": "± 93039.53842413622"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 25)",
            "value": 794146.4245256697,
            "unit": "ns",
            "range": "± 4546.776101157739"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 25)",
            "value": 7097667.588541667,
            "unit": "ns",
            "range": "± 128104.29004316391"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 25)",
            "value": 768669.598599138,
            "unit": "ns",
            "range": "± 10105.835864136225"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 100)",
            "value": 9596567.179956896,
            "unit": "ns",
            "range": "± 115381.64709198951"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 100)",
            "value": 23836.98325565883,
            "unit": "ns",
            "range": "± 209.5344607885628"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 100)",
            "value": 9605065.963020833,
            "unit": "ns",
            "range": "± 74964.94954790575"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 100)",
            "value": 9708340.42857143,
            "unit": "ns",
            "range": "± 123889.16562429313"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 100)",
            "value": 24280.676012762662,
            "unit": "ns",
            "range": "± 541.9303023747038"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 100)",
            "value": 9648212.070581896,
            "unit": "ns",
            "range": "± 55798.04181111855"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 45770822.922222234,
            "unit": "ns",
            "range": "± 5651635.195881649"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 39833812.33611111,
            "unit": "ns",
            "range": "± 3624623.87200066"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 45945375.122222215,
            "unit": "ns",
            "range": "± 4835666.699062076"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 483895.2014347957,
            "unit": "ns",
            "range": "± 1828.9776977930374"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3336276.502083333,
            "unit": "ns",
            "range": "± 220825.06851445395"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 100)",
            "value": 7524794.107872596,
            "unit": "ns",
            "range": "± 99029.35066145442"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 100)",
            "value": 842522.1388972356,
            "unit": "ns",
            "range": "± 4736.8641316526555"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 100)",
            "value": 7593531.682112069,
            "unit": "ns",
            "range": "± 82714.59518590121"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 100)",
            "value": 839461.7216796875,
            "unit": "ns",
            "range": "± 6614.4704000980855"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 320698395.5,
            "unit": "ns",
            "range": "± 6752654.260074579"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 318721156.2,
            "unit": "ns",
            "range": "± 4095733.596395055"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 322978320.61538464,
            "unit": "ns",
            "range": "± 2950413.5327236843"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7335706.364583333,
            "unit": "ns",
            "range": "± 144803.4068222594"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11223297.236458333,
            "unit": "ns",
            "range": "± 563859.0134945917"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "9b15f1d188816558d3c34d48d50145981c8a761a",
          "message": "Merge pull request #46 from EFNext/fix/comment-bloat\n\nchore: remove bloated comments across the codebase",
          "timestamp": "2026-04-27T19:04:40+01:00",
          "tree_id": "c946e9dda6e3a6d252c66589765ce6c022cc501b",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/9b15f1d188816558d3c34d48d50145981c8a761a"
        },
        "date": 1777316157614,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6977.677295139858,
            "unit": "ns",
            "range": "± 79.68248202160618"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 3040.4694992591594,
            "unit": "ns",
            "range": "± 122.09033255468506"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 5.54358612994353,
            "unit": "ns",
            "range": "± 0.39602356504472114"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 183.21165975928307,
            "unit": "ns",
            "range": "± 1.1783364212371925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 19968.800541804387,
            "unit": "ns",
            "range": "± 40.970373340098746"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 3120.7458422625505,
            "unit": "ns",
            "range": "± 98.1975278718875"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 7.606848182954958,
            "unit": "ns",
            "range": "± 0.4473361666199573"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 45.68413491972855,
            "unit": "ns",
            "range": "± 0.4229581944463068"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 19730.0096267174,
            "unit": "ns",
            "range": "± 94.86935236638126"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5534.64400024414,
            "unit": "ns",
            "range": "± 52.26724819243731"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 7.0551726669073105,
            "unit": "ns",
            "range": "± 0.08292594039195851"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 46.58123524435635,
            "unit": "ns",
            "range": "± 0.2653746952726077"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 25253.106834129052,
            "unit": "ns",
            "range": "± 272.1885194897731"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5818.820689274715,
            "unit": "ns",
            "range": "± 30.407849526108297"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 5.308315264406027,
            "unit": "ns",
            "range": "± 0.1001957059751103"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 46.38546624353954,
            "unit": "ns",
            "range": "± 3.248791142576623"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 85388.8011011584,
            "unit": "ns",
            "range": "± 370.6158853879183"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 18648.136580247145,
            "unit": "ns",
            "range": "± 21.419292367103072"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 5.3344737488244265,
            "unit": "ns",
            "range": "± 0.010650490209456924"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17888.615708007812,
            "unit": "ns",
            "range": "± 39.86317467455765"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 61265.396161760604,
            "unit": "ns",
            "range": "± 742.38020254581"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 7.660798197346074,
            "unit": "ns",
            "range": "± 0.40547710253765606"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 7.890601743702535,
            "unit": "ns",
            "range": "± 0.5899855492274213"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 6.014885824794571,
            "unit": "ns",
            "range": "± 0.3733307078350998"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 0)",
            "value": 5190644.858028017,
            "unit": "ns",
            "range": "± 42299.28971674201"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 0)",
            "value": 606337.283359375,
            "unit": "ns",
            "range": "± 2805.305348724337"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 0)",
            "value": 5214061.548958333,
            "unit": "ns",
            "range": "± 40983.4755997551"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 0)",
            "value": 609166.3820529514,
            "unit": "ns",
            "range": "± 3518.6031341638727"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 1)",
            "value": 146945.9989858774,
            "unit": "ns",
            "range": "± 884.049395108278"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 1)",
            "value": 19914.984482337688,
            "unit": "ns",
            "range": "± 94.8469286990128"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 1)",
            "value": 136778.4711350661,
            "unit": "ns",
            "range": "± 352.6611921213585"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 1)",
            "value": 148260.00978440506,
            "unit": "ns",
            "range": "± 924.0152554995127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 1)",
            "value": 20189.091567993164,
            "unit": "ns",
            "range": "± 67.69996536010628"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 1)",
            "value": 136001.14346078725,
            "unit": "ns",
            "range": "± 471.58501409646516"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1012133.2734375,
            "unit": "ns",
            "range": "± 70003.58344000624"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1022300.8356770833,
            "unit": "ns",
            "range": "± 93935.04824140089"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1021010.6037760417,
            "unit": "ns",
            "range": "± 100143.6702405391"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 46739.041146414624,
            "unit": "ns",
            "range": "± 170.07862001386368"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 223355.0378515625,
            "unit": "ns",
            "range": "± 1127.537967770322"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 1)",
            "value": 486578.906015625,
            "unit": "ns",
            "range": "± 2255.0568266785353"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 1)",
            "value": 479506.92431640625,
            "unit": "ns",
            "range": "± 2070.3675536814294"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 1)",
            "value": 21893.23891264817,
            "unit": "ns",
            "range": "± 287.5938074118735"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 1)",
            "value": 488512.3466796875,
            "unit": "ns",
            "range": "± 1361.5145216825165"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 1)",
            "value": 476523.95363136573,
            "unit": "ns",
            "range": "± 2516.77397101625"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 1)",
            "value": 20394.227349175348,
            "unit": "ns",
            "range": "± 347.29405314307127"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 5)",
            "value": 2102047.2799703665,
            "unit": "ns",
            "range": "± 18818.27198408313"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 5)",
            "value": 485753.1427801724,
            "unit": "ns",
            "range": "± 4871.98175751682"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 5)",
            "value": 29236.587722252156,
            "unit": "ns",
            "range": "± 1310.8258782099242"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 5)",
            "value": 2096727.2574869792,
            "unit": "ns",
            "range": "± 3208.7233519837364"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 5)",
            "value": 487712.5841238839,
            "unit": "ns",
            "range": "± 4291.445757959463"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 5)",
            "value": 30582.46434892927,
            "unit": "ns",
            "range": "± 78.61303474552835"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 10)",
            "value": 871651.6917550223,
            "unit": "ns",
            "range": "± 2404.457146864088"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 10)",
            "value": 20027.058864198883,
            "unit": "ns",
            "range": "± 241.47043448236315"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 10)",
            "value": 871195.0230189732,
            "unit": "ns",
            "range": "± 10087.73054866324"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 10)",
            "value": 880838.7458844866,
            "unit": "ns",
            "range": "± 9438.822966183869"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 10)",
            "value": 19506.599494934082,
            "unit": "ns",
            "range": "± 502.56680439738074"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 10)",
            "value": 866714.3018588362,
            "unit": "ns",
            "range": "± 13794.607991442925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 10)",
            "value": 4168997.5204326925,
            "unit": "ns",
            "range": "± 24404.766568924035"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 10)",
            "value": 502210.9656110491,
            "unit": "ns",
            "range": "± 1584.2712672250825"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 10)",
            "value": 39599.06348830003,
            "unit": "ns",
            "range": "± 750.6878731018226"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 10)",
            "value": 4173024.542564655,
            "unit": "ns",
            "range": "± 24434.968634764802"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 10)",
            "value": 505501.89111328125,
            "unit": "ns",
            "range": "± 11718.855605090392"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 10)",
            "value": 39577.65373173467,
            "unit": "ns",
            "range": "± 629.7120616759692"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 25)",
            "value": 5777899.211979167,
            "unit": "ns",
            "range": "± 135421.8707030068"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 25)",
            "value": 664067.9279597356,
            "unit": "ns",
            "range": "± 5753.2543466699435"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 25)",
            "value": 5697886.7828125,
            "unit": "ns",
            "range": "± 57189.084186627464"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 25)",
            "value": 653095.2099609375,
            "unit": "ns",
            "range": "± 2661.222178800613"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 100)",
            "value": 8050713.275,
            "unit": "ns",
            "range": "± 43175.457165868065"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 100)",
            "value": 18861.254664829798,
            "unit": "ns",
            "range": "± 51.28980591775216"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 100)",
            "value": 8121665.785560345,
            "unit": "ns",
            "range": "± 54048.10938530478"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 100)",
            "value": 8186745.446428572,
            "unit": "ns",
            "range": "± 78300.39659445228"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 100)",
            "value": 19309.566038908783,
            "unit": "ns",
            "range": "± 439.2997577701042"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 100)",
            "value": 8108902.645089285,
            "unit": "ns",
            "range": "± 100837.41784414375"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 33662474.16487069,
            "unit": "ns",
            "range": "± 5234274.175447869"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 34292137.02166667,
            "unit": "ns",
            "range": "± 3261265.274557087"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36885994.608333334,
            "unit": "ns",
            "range": "± 3142648.7443644265"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 454422.68603515625,
            "unit": "ns",
            "range": "± 2343.6785795266164"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 2791472.6040625,
            "unit": "ns",
            "range": "± 38223.031766733504"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 100)",
            "value": 6014034.427455357,
            "unit": "ns",
            "range": "± 20816.209412191576"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 100)",
            "value": 840984.4494280134,
            "unit": "ns",
            "range": "± 129313.3376409482"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 100)",
            "value": 6253542.419719827,
            "unit": "ns",
            "range": "± 40679.60655295396"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 100)",
            "value": 712735.9351128472,
            "unit": "ns",
            "range": "± 7313.512036142772"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 291775380.6,
            "unit": "ns",
            "range": "± 2375549.4094620687"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 292412502.28,
            "unit": "ns",
            "range": "± 3024848.1043306333"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 296296775.4,
            "unit": "ns",
            "range": "± 3946221.695222965"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7325221.88359375,
            "unit": "ns",
            "range": "± 61560.65689501138"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 11104272.89174107,
            "unit": "ns",
            "range": "± 55490.40348758534"
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
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "98d1657623eccec4d902a4a82bd76b2872f4e98c",
          "message": "Merge pull request #45 from EFNext/feat/synthesized-sources\n\nExpressiveProperties are now visible to other Expressives",
          "timestamp": "2026-04-28T00:32:43+01:00",
          "tree_id": "7087b7eed77d4f215d1b56e35571e441945192c3",
          "url": "https://github.com/EFNext/ExpressiveSharp/commit/98d1657623eccec4d902a4a82bd76b2872f4e98c"
        },
        "date": 1777335791331,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.Baseline",
            "value": 6353.319594284584,
            "unit": "ns",
            "range": "± 60.62146090053488"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Property",
            "value": 2931.7787110464915,
            "unit": "ns",
            "range": "± 47.19021949920145"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Property",
            "value": 8.776615488209895,
            "unit": "ns",
            "range": "± 0.2494066767923339"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_RemoveNullConditionalPatterns",
            "value": 161.9384745189122,
            "unit": "ns",
            "range": "± 3.389541036349162"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Property",
            "value": 17106.147061051994,
            "unit": "ns",
            "range": "± 116.07931275379684"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_Method",
            "value": 2979.268064226423,
            "unit": "ns",
            "range": "± 34.19914104525424"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Method",
            "value": 9.727014241804337,
            "unit": "ns",
            "range": "± 0.022236525575978296"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenBlockExpressions",
            "value": 53.22059182657136,
            "unit": "ns",
            "range": "± 0.20836249189897277"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_Method",
            "value": 17122.513732910156,
            "unit": "ns",
            "range": "± 108.42979657258216"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_NullConditional",
            "value": 5341.18291815396,
            "unit": "ns",
            "range": "± 45.176628159118025"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_MethodWithParams",
            "value": 9.837818384743654,
            "unit": "ns",
            "range": "± 0.1858456105008913"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_ConvertLoopsToLinq",
            "value": 52.809085245132444,
            "unit": "ns",
            "range": "± 0.5314143395972211"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.WithExpressives_NullConditional",
            "value": 22099.17987060547,
            "unit": "ns",
            "range": "± 139.14038170301893"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_BlockBody",
            "value": 5613.832045335036,
            "unit": "ns",
            "range": "± 33.91567538914297"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.Resolve_Constructor",
            "value": 8.335468101182155,
            "unit": "ns",
            "range": "± 0.013445775352111146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.Transform_FlattenTupleComparisons",
            "value": 49.46591290831566,
            "unit": "ns",
            "range": "± 0.02671691227318753"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_WithExpressives",
            "value": 89968.30869838169,
            "unit": "ns",
            "range": "± 400.5980142405623"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionReplacerBenchmarks.Replace_DeepChain",
            "value": 17748.626073376887,
            "unit": "ns",
            "range": "± 97.90453822275781"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Property",
            "value": 9.027306860243833,
            "unit": "ns",
            "range": "± 0.5982429781336884"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.TransformerBenchmarks.ExpandExpressives_FullPipeline",
            "value": 17430.16272844587,
            "unit": "ns",
            "range": "± 147.27008179697728"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.EFCoreQueryOverheadBenchmarks.ColdStart_Baseline",
            "value": 57901.35100266029,
            "unit": "ns",
            "range": "± 646.620698663342"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Method",
            "value": 10.536140949085906,
            "unit": "ns",
            "range": "± 0.11134051844880251"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_MethodWithParams",
            "value": 10.198460882529616,
            "unit": "ns",
            "range": "± 0.18567027179071344"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.ExpressionResolverBenchmarks.ResolveViaReflection_Constructor",
            "value": 8.463512218660778,
            "unit": "ns",
            "range": "± 0.173112059469305"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 0)",
            "value": 5018535.102101293,
            "unit": "ns",
            "range": "± 130327.01267948296"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 0)",
            "value": 727332.1341869213,
            "unit": "ns",
            "range": "± 149784.6604320553"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 0)",
            "value": 4907653.209821428,
            "unit": "ns",
            "range": "± 126443.80766047572"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 0)",
            "value": 587747.0398995535,
            "unit": "ns",
            "range": "± 5082.594746045831"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 1)",
            "value": 143600.52345703126,
            "unit": "ns",
            "range": "± 1704.428610487612"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 1)",
            "value": 18374.322653634208,
            "unit": "ns",
            "range": "± 47.35419999914731"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 1)",
            "value": 124034.35083912036,
            "unit": "ns",
            "range": "± 351.2657796220547"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 1)",
            "value": 143771.00718470983,
            "unit": "ns",
            "range": "± 2765.317105407714"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 1)",
            "value": 18779.712908514615,
            "unit": "ns",
            "range": "± 148.088178953883"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 1)",
            "value": 126622.74419487847,
            "unit": "ns",
            "range": "± 879.4567613544308"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1)",
            "value": 1091804.7486979167,
            "unit": "ns",
            "range": "± 91906.76789119827"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1)",
            "value": 1088754.0845052083,
            "unit": "ns",
            "range": "± 99394.92086639519"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1)",
            "value": 1055740.992578125,
            "unit": "ns",
            "range": "± 93016.01666029905"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1)",
            "value": 54488.95205583244,
            "unit": "ns",
            "range": "± 721.410025147156"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1)",
            "value": 288233.7223195043,
            "unit": "ns",
            "range": "± 41182.59243412139"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 1)",
            "value": 460114.6788411458,
            "unit": "ns",
            "range": "± 5233.149438257867"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 1)",
            "value": 440969.50563401444,
            "unit": "ns",
            "range": "± 6776.323843360431"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 1)",
            "value": 19749.041228117767,
            "unit": "ns",
            "range": "± 98.96125850903445"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 1)",
            "value": 455232.9809194711,
            "unit": "ns",
            "range": "± 7286.512950049732"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 1)",
            "value": 440072.62855747767,
            "unit": "ns",
            "range": "± 6235.017161298784"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 1)",
            "value": 19938.952833387586,
            "unit": "ns",
            "range": "± 57.59728655603713"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 5)",
            "value": 1986077.1967075893,
            "unit": "ns",
            "range": "± 19368.5372024389"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 5)",
            "value": 449603.81961495534,
            "unit": "ns",
            "range": "± 7047.8122297329965"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 5)",
            "value": 28352.12860107422,
            "unit": "ns",
            "range": "± 320.2706103483544"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 5)",
            "value": 1955256.0227213542,
            "unit": "ns",
            "range": "± 6568.823005312911"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 5)",
            "value": 449244.39024939906,
            "unit": "ns",
            "range": "± 3033.1159649876677"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 5)",
            "value": 29420.308152297446,
            "unit": "ns",
            "range": "± 240.8833289821892"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 10)",
            "value": 821531.7428152902,
            "unit": "ns",
            "range": "± 3305.5865138231784"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 10)",
            "value": 18404.555653889973,
            "unit": "ns",
            "range": "± 201.45178647236204"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 10)",
            "value": 804920.5402018229,
            "unit": "ns",
            "range": "± 3098.1979341537176"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 10)",
            "value": 828196.1658528646,
            "unit": "ns",
            "range": "± 7456.268530958609"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 10)",
            "value": 18634.67293724647,
            "unit": "ns",
            "range": "± 103.8670916575904"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 10)",
            "value": 799777.3783804086,
            "unit": "ns",
            "range": "± 10631.451225083647"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold(FileCount: 10)",
            "value": 3844242.7174030175,
            "unit": "ns",
            "range": "± 26089.953322495774"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile(FileCount: 10)",
            "value": 460495.4186759159,
            "unit": "ns",
            "range": "± 4422.250890432278"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile(FileCount: 10)",
            "value": 41203.535787648165,
            "unit": "ns",
            "range": "± 401.3642466421103"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Cold_E2E(FileCount: 10)",
            "value": 3880825.406550481,
            "unit": "ns",
            "range": "± 70332.31884785098"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditCallSiteFile_E2E(FileCount: 10)",
            "value": 464256.13317418983,
            "unit": "ns",
            "range": "± 2158.1361038856826"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillMultiFileBenchmarks.Incremental_EditNoiseFile_E2E(FileCount: 10)",
            "value": 40119.59442349138,
            "unit": "ns",
            "range": "± 210.18649315223198"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 25)",
            "value": 5256751.975520833,
            "unit": "ns",
            "range": "± 40763.09383129899"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 25)",
            "value": 631555.5152762277,
            "unit": "ns",
            "range": "± 4585.28652458435"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 25)",
            "value": 5436874.1171875,
            "unit": "ns",
            "range": "± 111414.00621840483"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 25)",
            "value": 640195.2488064236,
            "unit": "ns",
            "range": "± 3857.6879449339854"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold(CallSiteCount: 100)",
            "value": 7622682.141225962,
            "unit": "ns",
            "range": "± 62154.939346810825"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile(CallSiteCount: 100)",
            "value": 18502.140269688196,
            "unit": "ns",
            "range": "± 67.73982841484522"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile(CallSiteCount: 100)",
            "value": 7602509.412946428,
            "unit": "ns",
            "range": "± 40947.97430065499"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Cold_E2E(CallSiteCount: 100)",
            "value": 7535341.16796875,
            "unit": "ns",
            "range": "± 93287.18955532566"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditEntityFile_E2E(CallSiteCount: 100)",
            "value": 18706.406661422163,
            "unit": "ns",
            "range": "± 141.8563736735146"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillSingleFileBenchmarks.Incremental_EditQueryFile_E2E(CallSiteCount: 100)",
            "value": 7456620.803385417,
            "unit": "ns",
            "range": "± 37580.37758795454"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 100)",
            "value": 33430825.08777777,
            "unit": "ns",
            "range": "± 2323521.252667536"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 100)",
            "value": 37061995.791666664,
            "unit": "ns",
            "range": "± 3221105.523524656"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 100)",
            "value": 36924586.825,
            "unit": "ns",
            "range": "± 3455338.0751169706"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 100)",
            "value": 470297.5536063058,
            "unit": "ns",
            "range": "± 3163.924444243493"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 100)",
            "value": 3350613.371354167,
            "unit": "ns",
            "range": "± 203348.67183050193"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold(NoiseInvocationsPerFile: 100)",
            "value": 6146821.43610491,
            "unit": "ns",
            "range": "± 54249.723743005925"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile(NoiseInvocationsPerFile: 100)",
            "value": 678609.3708683894,
            "unit": "ns",
            "range": "± 3706.8045164618293"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Cold_E2E(NoiseInvocationsPerFile: 100)",
            "value": 6039366.272536058,
            "unit": "ns",
            "range": "± 71733.65947423123"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.PolyfillColdBuildWithNoiseBenchmarks.Incremental_EditCallSiteFile_E2E(NoiseInvocationsPerFile: 100)",
            "value": 697048.3690682871,
            "unit": "ns",
            "range": "± 13817.352999470038"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator(ExpressiveCount: 1000)",
            "value": 303267445.84615386,
            "unit": "ns",
            "range": "± 2415746.5395659986"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_NoiseChange(ExpressiveCount: 1000)",
            "value": 306688630.64,
            "unit": "ns",
            "range": "± 4177527.4970751526"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 307192964.92,
            "unit": "ns",
            "range": "± 2349653.491703261"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_NoiseChange(ExpressiveCount: 1000)",
            "value": 7113332.083984375,
            "unit": "ns",
            "range": "± 21874.92035795022"
          },
          {
            "name": "ExpressiveSharp.Benchmarks.GeneratorBenchmarks.RunGenerator_Incremental_ExpressiveChange(ExpressiveCount: 1000)",
            "value": 10834636.538793104,
            "unit": "ns",
            "range": "± 84682.24193360178"
          }
        ]
      }
    ]
  }
}