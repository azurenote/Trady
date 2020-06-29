using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;
using Trady.Core.Infrastructure;

namespace Trady.Analysis.Indicator.Streaming
{
    public class ExponentialMovingAverage<TInput, TOutput> : StreamingNumericAnalyzableBase<TInput, decimal?, TOutput>
    {
        private readonly StreamingMovingAverage _ema;

        public ExponentialMovingAverage(IList<TInput> inputs, Func<TInput, decimal?> inputMapper, int periodCount)
            : base(inputs, inputMapper)
        {
            _ema = new StreamingMovingAverage(
                i => _mappedInputs.ElementAt(i),
                Smoothing.Ema(periodCount),
                inputs.Count());

            PeriodCount = periodCount;
        }

        public int PeriodCount { get; }

        protected override decimal? ComputeByIndexImpl(IList<decimal?> mappedInputs, int index) => _ema[index];
    }

    public class ExponentialMovingAverageByTuple : ExponentialMovingAverage<decimal?, decimal?>
    {
        public ExponentialMovingAverageByTuple(IList<decimal?> inputs, int periodCount)
            : base(inputs, i => i, periodCount)
        {
        }

        public ExponentialMovingAverageByTuple(IList<decimal> inputs, int periodCount)
            : this(inputs.Cast<decimal?>().ToList(), periodCount)
        {
        }
    }

    public class ExponentialMovingAverage : ExponentialMovingAverage<IOhlcv, AnalyzableTick<decimal?>>
    {
        public ExponentialMovingAverage(IList<IOhlcv> inputs, int periodCount)
            : base(inputs, i => i.Close, periodCount)
        {
        }
    }
}
