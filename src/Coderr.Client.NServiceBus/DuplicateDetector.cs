using System.Collections.Generic;
using System.Linq;

namespace Coderr.Client.NServiceBus
{
    public class DuplicateDetector
    {
        public static DuplicateDetector Instance = new DuplicateDetector();
        private readonly LinkedList<string> _messageIds = new LinkedList<string>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns><c>true</c> if it's NOT a duplicate; otherwise <c>false</c></returns>
        public bool Validate(string messageId)
        {
            return true;

            if (_messageIds.Any(x => x == messageId))
                return false;


            _messageIds.AddLast(messageId);
            while (_messageIds.Count > 1000)
                _messageIds.RemoveFirst();

            return true;
        }
    }
}