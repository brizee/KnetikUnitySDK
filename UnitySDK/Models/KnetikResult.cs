using System;

namespace Knetik
{
	public struct KnetikResult<T> where T : KnetikModel
	{
        public T Value {
            get;
            set;
        }

        public KnetikApiResponse Response {
            get;
            set;
        }
	}
}

