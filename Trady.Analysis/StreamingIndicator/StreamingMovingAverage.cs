using System;
using System.Collections.Generic;
using Trady.Analysis.Infrastructure;

namespace Trady.Analysis.Indicator.Streaming
{
    internal class StreamingMovingAverage : StreamingCumulativeAnalyzableBase<decimal?, decimal?>
    {
        private readonly int _initialValueIndex;
        private readonly Func<int, decimal?> _inputFunction;
        private readonly Func<int, decimal> _smoothingFactorFunction;

        public StreamingMovingAverage(int initialValueIndex,
            Func<int, decimal?> initialValueFunction,
            Func<int, decimal?> indexValueFunction,
            Func<int, decimal> smoothingFactorFunction,
            int inputCount
            ) : base(new List<decimal?>(inputCount))
        {
            _initialValueIndex = initialValueIndex;
            _inputFunction = i => i < initialValueIndex ?
                null : initialValueIndex == i ?
                    initialValueFunction(i) : indexValueFunction(i);
            _smoothingFactorFunction = smoothingFactorFunction;
        }

        public StreamingMovingAverage(Func<int, decimal?> initialValueFunction, Func<int, decimal?> indexValueFunction, Func<int, decimal> smoothingFactorFunction, int inputCount)
            : this(0, initialValueFunction, indexValueFunction, smoothingFactorFunction, inputCount)
        {
        }

        public StreamingMovingAverage(
            Func<int, decimal?> indexValueFunction,
            Func<int, decimal> smoothingFactorFunction,
            int inputCount)
            : this(0, indexValueFunction, indexValueFunction, smoothingFactorFunction, inputCount)
        {
        }

        public StreamingMovingAverage(Func<int, decimal?> indexValueFunction, decimal smoothingFactor, int inputCount)
            : this(0, indexValueFunction, indexValueFunction, i => smoothingFactor, inputCount)
        {
        }

        protected override int InitialValueIndex => _initialValueIndex;

        protected override decimal? ComputeInitialValue(IList<decimal?> mappedInputs, int index) => _inputFunction(index);

        protected override decimal? ComputeCumulativeValue(IList<decimal?> mappedInputs, int index, decimal? prevOutputToMap)
            => prevOutputToMap + (_smoothingFactorFunction(index) * (_inputFunction(index) - prevOutputToMap));
    }
}
