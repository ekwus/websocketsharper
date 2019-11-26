using System;
using System.Collections.Generic;
using System.Text;

namespace TestSupport
{
    public abstract class BaseDisposable : BaseSynchronised, IDisposable
    {
        public bool IsDisposed { get; private set; }
        public bool IsDisposing { get; private set; }

        public void Dispose()
        {
            (this as IDisposable).Dispose();
        }

        protected abstract void Dispose(bool disposing);

        void IDisposable.Dispose()
        {
            using (SyncRoot.Enter())
            {
                if (IsDisposed || IsDisposing)
                {
                    return;
                }

                IsDisposing = true;
            }

            try
            {
                Dispose(true);
            }
            catch (Exception)
            {
                // Disposing so not much we can do with this exception
            }
            finally
            {
                IsDisposed = true;
            }
        }
    }
}
