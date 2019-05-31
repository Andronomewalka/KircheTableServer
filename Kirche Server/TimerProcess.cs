using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kirche_Server
{
    //public class TimerProcess
    //{
    //    int interval;
    //    static object _locker = new object();
    //    Action timerCallback;
    //    static Timer _timer;
    //
    //    public TimerProcess(Action timerCallback, int interval)
    //    {
    //        this.timerCallback = timerCallback;
    //        this.interval = interval;
    //    }
    //
    //    public void Start()
    //    {
    //        _timer = new Timer(Callback, null, 0, interval);
    //    }
    //
    //    public void Stop()
    //    {
    //        _timer.Dispose();
    //    }
    //
    //    private void Callback(object state)
    //    {
    //        var hasLock = false;
    //
    //        try
    //        {
    //            Monitor.TryEnter(_locker, ref hasLock);
    //            if (!hasLock)
    //                return;
    //
    //            timerCallback?.Invoke();
    //        }
    //        finally
    //        {
    //            if (hasLock)
    //                Monitor.Exit(_locker);
    //        }
    //    }
    //}
}
