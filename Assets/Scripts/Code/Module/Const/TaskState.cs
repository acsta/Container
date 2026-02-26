using System;

namespace TaoTie
{
    [Flags]
    public enum TaskState
    {
        Running = 1,
        NoStart = 2,
        Over = 4,
        All = 7
    }
}