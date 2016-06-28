using System.Collections.Generic;

namespace Greet.DataStructureV4.Entities
{
    public interface INeedPayload
    {
        Dictionary<int, MaterialTransportedPayload> Payload { get; }
    }
}
