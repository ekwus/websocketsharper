using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace TestSupport
{
    public class TestGate<T> : BaseDisposable where T : Enum
    {
        private Dictionary<T,AutoResetEvent> m_signals;

        public TestGate()
        {
            m_signals = new Dictionary<T, AutoResetEvent>();
        }

        public void Set(T token)
        {
            using (SyncRoot.Enter())
            {
                var signal = GetSignal(token);
                signal.Set();
            }
        }

        public bool WaitFor(T token, Duration timeout)
        {
            return WaitFor(token, (int)timeout.TotalMilliseconds);
        }

        public bool WaitFor(T token, int milliseconds)
        {
            AutoResetEvent signal = GetSignal(token);
            return signal.WaitOne(milliseconds);
        }

        public void AssertWaitFor(T token, Duration timeout)
        {
            AssertWaitFor(token, (int)timeout.TotalMilliseconds);
        }

        public void AssertWaitFor(T token, int milliseconds)
        {
            Assert.True(WaitFor(token, milliseconds), token.ToString());
        }

        private AutoResetEvent GetSignal(T token)
        {
            using (SyncRoot.Enter())
            {
                if (!m_signals.ContainsKey(token))
                {
                    m_signals[token] = new AutoResetEvent(false);
                }

                return m_signals[token];
            }
        }

        protected override void Dispose(bool disposing)
        {
            foreach(var signal in m_signals.Values)
            {
                signal?.Dispose();
            }
        }
    }
}
