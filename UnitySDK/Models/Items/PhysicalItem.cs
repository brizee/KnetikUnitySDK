using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
    public class PhysicalItem : Item
    {
        public PhysicalItem (KnetikClient client)
            : base(client)
        {
        }

        public PhysicalItem (KnetikClient client, int id)
            : base(client, id)
        {
        }
    }
}
