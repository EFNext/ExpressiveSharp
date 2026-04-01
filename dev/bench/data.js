window.BENCHMARK_DATA = {
  "lastUpdate": 1775010648572,
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
      }
    ]
  }
}