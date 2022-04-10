using System;

namespace EventBus
{
    public class TransportMessage 
    {
        public Guid Id { get; set; }

        public string Payload { get; set; }

        public string Destination { get; set; }
    }
}
                                                                                      