// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using MicroBenchmarks;

namespace System.Text.RegularExpressions.Tests
{
    /// <summary>
    /// Performance tests for dual-anchor optimization (Issue #118489)
    /// Tests patterns with both leading and trailing anchors for fixed-length matches
    /// </summary>
    [BenchmarkCategory(Categories.Libraries, Categories.Regex)]
    public class Perf_Regex_DualAnchorOptimization
    {
        private Regex _fixedLengthWithAnchors;
        private Regex _fixedLengthWithAnchorsEndZ;
        private Regex _alternationWithAnchors;
        
        // Test inputs of different lengths to verify fast failure optimization
        private string _exactMatch = "1234";
        private string _tooLong = "12345678901234567890"; // Much longer to test fast failure
        private string _tooShort = "12";

        [Params(RegexOptions.None, RegexOptions.Compiled)]
        public RegexOptions Options { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            // These patterns should benefit from the dual-anchor optimization
            _fixedLengthWithAnchors = new Regex(@"^1234$", Options);
            _fixedLengthWithAnchorsEndZ = new Regex(@"^1234\z", Options);
            _alternationWithAnchors = new Regex(@"^(abcd|efgh)$", Options); // Fixed-length alternation
        }

        [Benchmark]
        [MemoryRandomization]
        public bool DualAnchor_ExactMatch() => _fixedLengthWithAnchors.IsMatch(_exactMatch);

        [Benchmark]
        [MemoryRandomization]
        public bool DualAnchor_TooLong() => _fixedLengthWithAnchors.IsMatch(_tooLong);

        [Benchmark]
        [MemoryRandomization]
        public bool DualAnchor_TooShort() => _fixedLengthWithAnchors.IsMatch(_tooShort);

        [Benchmark]
        [MemoryRandomization]
        public bool DualAnchorEndZ_TooLong() => _fixedLengthWithAnchorsEndZ.IsMatch(_tooLong);

        [Benchmark]
        [MemoryRandomization]
        public bool DualAnchor_Alternation_TooLong() => _alternationWithAnchors.IsMatch(_tooLong);
    }
}