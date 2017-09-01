using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonXaml
{
    internal class TimerStatusChangeEvent : EventArgs
        {
            public TimerState Status { get; private set; }

            private TimerStatusChangeEvent() { }

            public TimerStatusChangeEvent(TimerState status)
            {
                Status = status;
            }
        }
}
