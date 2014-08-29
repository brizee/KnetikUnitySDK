using System;
using System.Collections.Generic;
using KnetikSimpleJSON;

namespace Knetik
{
	public class Wallet : KnetikModel
	{
        public int ID {
            get;
            set;
        }

        public double Balance {
            get;
            set;
        }

        public string CurrencyName {
            get;
            set;
        }

		public Wallet (KnetikClient client)
			: base(client)
		{
		}

        public override void Deserialize (KnetikJSONNode json)
        {
            ID = json ["id"].AsInt;
            Balance = json ["balance"].AsDouble;
            CurrencyName = json ["currency_name"].ToString ();
        }
	}
}

