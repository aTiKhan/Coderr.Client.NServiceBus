using System.Collections.Generic;
using System.Linq;

namespace Coderr.Client.NServiceBus
{
    public class DuplicateDetector
    {
        public static DuplicateDetector Instance = new DuplicateDetector();
        private readonly LinkedList<string> _messageIds = new LinkedList<string>();


        public bool Validate(string messageId)
        {
            if (_messageIds.Any(x => x == messageId))
                return false;


            _messageIds.AddLast(messageId);
            while (_messageIds.Count > 1000)
                _messageIds.RemoveFirst();

            return true;
        }
    }
}