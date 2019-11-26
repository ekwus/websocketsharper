using System;

namespace TestSupport
{
    public abstract class BaseSynchronised
    {
        protected SafeLock SyncRoot { get; private set; }

        protected BaseSynchronised()
        {
            SyncRoot = new SafeLock();
        }
    }
}
