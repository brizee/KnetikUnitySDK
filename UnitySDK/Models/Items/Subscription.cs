using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class Subscription : Item
    {
        public Subscription (KnetikClient client)
            : base(client)
        {
        }

        public Subscription (KnetikClient client, int id)
            : base(client, id)
        {
        }
    }
}
