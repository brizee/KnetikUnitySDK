using UnityEngine;
using System;
using System.Collections;

namespace KnetikHTTP
{
	public class KnetikResponseCallbackDispatcher : MonoBehaviour
    {
		private static KnetikResponseCallbackDispatcher singleton = null;
        private static GameObject singletonGameObject = null;
        private static object singletonLock = new object();

		public static KnetikResponseCallbackDispatcher Singleton {
            get {
                return singleton;
            }
        }

        public Queue requests = Queue.Synchronized( new Queue() );

        public static void Init()
        {
            if ( singleton != null )

            {
                return;
            }

            lock( singletonLock )
            {
                if ( singleton != null )
                {
                    return;
                }

                singletonGameObject = new GameObject();
				singleton = singletonGameObject.AddComponent< KnetikResponseCallbackDispatcher >();
				singletonGameObject.name = "KnetikHTTPResponseCallbackDispatcher";
            }
        }

        public void Update()
        {
            while( requests.Count > 0 )
            {
				KnetikHTTP.KnetikRequest request = (KnetikRequest)requests.Dequeue();
                request.completedCallback( request );
            }
        }
    }
}
