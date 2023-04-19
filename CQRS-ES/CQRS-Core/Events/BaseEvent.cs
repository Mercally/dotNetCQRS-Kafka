using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Messages;

namespace CQRS.Core.Events;

public class BaseEvent : Message
{
    protected BaseEvent(string type)
    {
        Type = type;
    }

    public int Version { get; set; }
    public string Type { get; set; } = string.Empty;

    public static readonly EmptyEvent Empty = new EmptyEvent();

    public class EmptyEvent : BaseEvent
    {
        public EmptyEvent() : base("None")
        {
            Version = -1;
        }
    }
}


