using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class VirtualItem : Item
    {
        public VirtualItem (KnetikClient client)
            : base(client)
        {
        }

        public VirtualItem (KnetikClient client, int id)
            : base(client, id)
        {
        }
    }
}
