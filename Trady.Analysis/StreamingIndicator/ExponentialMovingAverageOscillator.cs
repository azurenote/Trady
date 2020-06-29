using System;
using System.Collections.Generic;
using System.Linq;
using Trady.Analysis.Infrastructure;
using Trady.Core;
using Trady.Core.Infrastructure;

namespace Trady.Analysis.Indicator.Streaming
{
    public class ExponentialMovingAverageOscillator<TInput, TOutput> : StreamingNumericAnalyzableBase<TInput, decimal, TOutput>
    {
        private readonly ExponentialMovingAverageByTuple _ema2;
        private readonly ExponentialMovingAverageByTuple _ema1;

        public ExponentialMovingAverageOscillator(IList<TInput> inputs, Func<TInput, decimal> inputMapper, int periodCount1, int periodCount2)
            : base(inputs, inputMapper)
        {
            _ema1 = new ExponentialMovingAverageByTuple(inputs.Select(inputMapper).ToList(), periodCount1);
            _ema2 = new ExponentialMovingAverageByTuple(inputs.Select(inputMapper).ToList(), periodCount2);

            PeriodCount1 = periodCount1;
            PeriodCount2 = periodCount2;
        }

        public override void Add(TInput input)
        {
            base.Add(input);
            _ema1.Add(_inputMapper(input));
            _ema2.Add(_inputMapper(input));
        }

        public int PeriodCount1 { get; }

        public int PeriodCount2 { get; }

        protected override decimal? ComputeByIndexImpl(IList<decimal> mappedInputs, int index) => _ema1[index] - _ema2[index];
    }

    public class ExponentialMovingAverageOscillatorByTuple : ExponentialMovingAverageOscillator<decimal, decimal?>
    {
        public ExponentialMovingAverageOscillatorByTuple(IList<decimal> inputs, int periodCount1, int periodCount2)
            : base(inputs, i => i, periodCount1, periodCount2)
        {
        }
    }

    public class ExponentialMovingAverageOscillator : ExponentialMovingAverageOscillator<IOhlcv, AnalyzableTick<decimal?>>
    {
        public ExponentialMovingAverageOscillator(IList<IOhlcv> inputs, int periodCount1, int periodCount2)
            : base(inputs, i => i.Close, periodCount1, periodCount2)
        {
        }
    }
}
