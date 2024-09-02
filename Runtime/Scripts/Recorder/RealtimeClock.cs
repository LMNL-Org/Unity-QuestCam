using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace QuestCam
{
    public class RealtimeClock
    {
        private readonly Stopwatch stopwatch;
        
        public long Timestamp {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                var time = stopwatch.Elapsed.Ticks * 100L;
                if (!stopwatch.IsRunning)
                    stopwatch.Start();
                return time;
            }
        }
        
        public bool Paused {
            [MethodImpl(MethodImplOptions.Synchronized)] get => !stopwatch.IsRunning;
            [MethodImpl(MethodImplOptions.Synchronized)] set => (value ? (Action)stopwatch.Stop : stopwatch.Start)();
        }
        
        public RealtimeClock () => this.stopwatch = new Stopwatch();
    }
}