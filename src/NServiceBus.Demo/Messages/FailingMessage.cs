using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceBus.Demo.Messages
{
    class FailingMessage : ICommand
    {
        private static Random Rand = new Random();

        public FailingMessage()
        {
            FacilityId = Rand.Next(20000, 200017);
        }

        public int UserId { get; set; } = 10;

        public int FacilityId { get; set; }
        public string TrackId { get; set; } = Guid.NewGuid().ToString("N");

    }
}
