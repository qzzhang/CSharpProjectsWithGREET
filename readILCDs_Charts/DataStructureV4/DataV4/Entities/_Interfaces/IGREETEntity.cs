using System;

namespace Greet.DataStructureV4.Entities
{
    public interface IGREETEntity
    {
        bool Discarded { get; set; }
        string DiscardedReason { get; set; }
        DateTime DiscardedOn { get; set; }
        string DiscarededBy { get; set; }
    }
}