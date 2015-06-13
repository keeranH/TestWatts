using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECSChange
{
    public interface ITaskScheduler
    {
        string Name { get; }
        void Run();
        void Stop();
    }
}
