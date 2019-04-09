using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Diagnostics;

namespace ProjectHelper.Classes
{
    class ProjectStopWatch
    {
        Stopwatch stopWatch = new Stopwatch();
        TimeSpan elapsed;

        public ProjectStopWatch(string elapsedTime)
        {
            elapsed = TimeSpan.Parse(elapsedTime);

            stopWatch.Start();
        }

        //Vrací uběhnutý čas
        public string GetCurrentTime()
        {
            TimeSpan timeSpan = stopWatch.Elapsed + elapsed;
            return string.Format("{0:00}:{1:00}:{2:00}:{3:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        public void StartStopWatch()
        {
            stopWatch.Start();
        }

        public void StopStopWatch()
        {
            stopWatch.Stop();
        }
    }
}
