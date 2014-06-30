using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// Based on node-cookiejar (https://github.com/bmeck/node-cookiejar)

namespace KnetikHTTP
{
	public class KnetikCookieAccessInfo
	{
		public string domain = null;
		public string path = null;
		public bool secure = false;
		public bool scriptAccessible = true;

		// Non-secure
		public KnetikCookieAccessInfo( string domain, string path )
		{
			this.domain = domain;
			this.path = path;
		}

		// Secure
		public KnetikCookieAccessInfo( string domain, string path, bool secure )
		{
			this.domain = domain;
			this.path = path;
			this.secure = secure;
		}

		// Secure and Script Accessible
		public KnetikCookieAccessInfo( string domain, string path, bool secure, bool scriptAccessible )
		{
			this.domain = domain;
			this.path = path;
			this.secure = secure;
			this.scriptAccessible = scriptAccessible;
		}

		// When Cookie Information Already Exists
		public KnetikCookieAccessInfo( KnetikCookie cookie )
		{
			this.domain = cookie.domain;
			this.path = cookie.path;
			this.secure = cookie.secure;
			this.scriptAccessible = cookie.scriptAccessible;
		}
	}
	
	public class KnetikCookie
	{
		public string name = null;
		public string value = null;
		public DateTime expirationDate = DateTime.MaxValue;
		public string path = null;
		public string domain = null;
		public bool secure = false;
		public bool scriptAccessible = true;
		
		private static string cookiePattern = "\\s*([^=]+)(?:=((?:.|\\n)*))?";

		// Reads Cookie
		public KnetikCookie( string cookieString )
		{
			string[] parts = cookieString.Split( ';' );
			foreach ( string part in parts )
			{
                
				Match match = Regex.Match( part, cookiePattern );
	
				if ( !match.Success )
				{
					throw new Exception( "Could not parse cookie string: " + cookieString );
				}
				
				if ( this.name == null )
				{
					this.name = match.Groups[ 1 ].Value;
					this.value = match.Groups[ 2 ].Value;
					continue;
				}
				
				switch( match.Groups[ 1 ].Value.ToLower() )
				{
				case "httponly":
					this.scriptAccessible = false;
					break;
				case "expires":
					this.expirationDate = DateTime.Parse( match.Groups[ 2 ].Value );
					break;
				case "path":
					this.path = match.Groups[ 2 ].Value;
					break;
				case "domain":
					this.domain = match.Groups[ 2 ].Value;
					break;
				case "secure":
					this.secure = true;
					break;
				default:
					// TODO: warn of unknown cookie setting?
					break;
				}
			}
		}

		// Ensure that Cookie matches existing access information
		public bool Matches( KnetikCookieAccessInfo accessInfo )
		{
			if (    this.secure != accessInfo.secure
			     || !this.CollidesWith( accessInfo ) )
			{
				return false;
			}
			
			return true;
		}
	
		public bool CollidesWith( KnetikCookieAccessInfo accessInfo )
		{
			if ( ( this.path != null && accessInfo.path == null ) || ( this.domain != null && accessInfo.domain == null ) )
			{
				return false;
			}
			
			if ( this.path != null && accessInfo.path != null && accessInfo.path.IndexOf( this.path ) != 0 )
			{
                return false;
			}
			
			if ( this.domain == accessInfo.domain )
			{
                return true;
			}
			else if ( this.domain != null && this.domain.Length >= 1 && this.domain[ 0 ] == '.' )
			{
                int wildcard = accessInfo.domain.IndexOf( this.domain.Substring( 1 ) );
				if( wildcard == -1 || wildcard != accessInfo.domain.Length - this.domain.Length + 1 )
				{
                    return false;
				}
			}
			else if ( this.domain != null )
			{
                return false;
			}

            return true;
		}
		
		public string ToValueString()
		{
			return this.name + "=" + this.value;
		}
		
		public override string ToString()
		{
			List< string > elements = new List< string >();
			elements.Add( this.name + "=" + this.value );
			
			if( this.expirationDate != DateTime.MaxValue )
			{
				elements.Add( "expires=" + this.expirationDate.ToString() );
			}

			if( this.domain != null )
			{
				elements.Add( "domain=" + this.domain );
			}

			if( this.path != null )
			{
				elements.Add( "path=" + this.path );
			}
			
			if( this.secure )
			{
				elements.Add( "secure" );
			}
	
			if( this.scriptAccessible == false )
			{
				elements.Add( "httponly" );
			}
			
			return String.Join( "; ", elements.ToArray() );
		}
	}
	
    public delegate void ContentsChangedDelegate();    
    
	// Store Cookies in Dictionary until Expired
	public class KnetikCookieJar
	{
		private static string version = "v2";
        private object cookieJarLock = new object();

		private static KnetikCookieJar instance;
		public Dictionary< string, List< KnetikCookie > > cookies;

        public ContentsChangedDelegate ContentsChanged;
		
		public static KnetikCookieJar Instance
		{
			get
			{
				if ( instance == null )
				{
					instance = new KnetikCookieJar();
				}
				return instance;
			}
		}
		
		public KnetikCookieJar ()
		{
            this.Clear();
		}
        
        public void Clear()
        {
            lock( cookieJarLock )
            {
				cookies = new Dictionary< string, List< KnetikCookie > >();
                if ( ContentsChanged != null )
                {
                    ContentsChanged();
                }
            }
        }

		public bool SetCookie( KnetikCookie cookie )
		{
            lock( cookieJarLock )
            {
                bool expired = cookie.expirationDate < DateTime.Now;
            
                if ( cookies.ContainsKey( cookie.name ) )
                {
                    for( int index = 0; index < cookies[ cookie.name ].Count; ++index )
                    {
						KnetikCookie collidableCookie = cookies[ cookie.name ][ index ];
						if ( collidableCookie.CollidesWith( new KnetikCookieAccessInfo( cookie ) ) )
                        {
                            if( expired )
                            {
                                cookies[ cookie.name ].RemoveAt( index );
                                if ( cookies[ cookie.name ].Count == 0 )
                                {
                                    cookies.Remove( cookie.name );
                                    if ( ContentsChanged != null )
                                    {
                                        ContentsChanged();
                                    }
                                }
                                
                                return false;
                            }
                            else
                            {
                                cookies[ cookie.name ][ index ] = cookie;
                                if ( ContentsChanged != null )
                                {
                                    ContentsChanged();
                                }
                                return true;
                            }
                        }
                    }
                    
                    if ( expired )
                    {
                        return false;
                    }
                    
                    cookies[ cookie.name ].Add( cookie );
                    if ( ContentsChanged != null )
                    {
                        ContentsChanged();
                    }
                    return true;
                }
    
                if ( expired )
                {
                    return false;
                }
    
				cookies[ cookie.name ] = new List< KnetikCookie >();
				cookies[ cookie.name ].Add( cookie );
                if ( ContentsChanged != null )
                {
                    ContentsChanged();
                }
                return true;
            }
		}
		
        // TODO: figure out a way to respect the scriptAccessible flag and supress cookies being
        //       returned that should not be.  The issue is that at some point, within this
        //       library, we need to send all the correct cookies back in the request.  Right now
        //       there's no way to add all cookies (regardless of script accessibility) to the
        //       request without exposing cookies that should not be script accessible.
        
		public KnetikCookie GetCookie( string name, KnetikCookieAccessInfo accessInfo )
		{
			if ( !cookies.ContainsKey( name ) )
			{
                return null;
			}
			
			for ( int index = 0; index < cookies[ name ].Count; ++index )
			{
				KnetikCookie cookie = cookies[ name ][ index ];
				if ( cookie.expirationDate > DateTime.Now && cookie.Matches( accessInfo ) )
				{
                    return cookie;
				}
			}
			
            return null;
		}
		
		public List< KnetikCookie > GetCookies( KnetikCookieAccessInfo accessInfo )
		{
			List< KnetikCookie > result = new List< KnetikCookie >();
			foreach ( string cookieName in cookies.Keys )
			{
				KnetikCookie cookie = this.GetCookie( cookieName, accessInfo );
				if ( cookie != null )
				{
                    result.Add( cookie );
				}
			}
			
			return result;
		}
		
		public void SetCookies( KnetikCookie[] cookieObjects )
		{
			for ( var index = 0; index < cookieObjects.Length; ++index )
			{
				this.SetCookie( cookieObjects[ index ] );
			}
		}
		
		private static string cookiesStringPattern = "[:](?=\\s*[a-zA-Z0-9_\\-]+\\s*[=])";

		public void SetCookies( string cookiesString )
		{
			
			Match match = Regex.Match( cookiesString, cookiesStringPattern );

			if ( !match.Success )
			{
				throw new Exception( "Could not parse cookies string: " + cookiesString );
			}
			
			for ( int index = 0; index < match.Groups.Count; ++index )
			{
				this.SetCookie( new KnetikCookie( match.Groups[ index ].Value ) );
			}
		}

        private static string boundary = "\n!!::!!\n";

        public string Serialize()
        {
            string result = version + boundary;

            lock( cookieJarLock )
            {
                foreach ( string key in cookies.Keys )
                {
                    for ( int index = 0; index < cookies[ key ].Count; ++index )
                    {
                        result += cookies[ key ][ index ].ToString() + boundary;
                    }
                }
            }
                
            return result;
        }
        
        public void Deserialize( string cookieJarString, bool clear )
        {
            if ( clear )
            {
                this.Clear();
            }

            Regex regex = new Regex( boundary );
            string[] cookieStrings = regex.Split( cookieJarString );
			bool readVersion = false;
            foreach ( string cookieString in cookieStrings )
            {
				if ( !readVersion )
				{
					if ( cookieString.IndexOf( version ) != 0 )
					{
						return;
					}
					readVersion = true;
					continue;
				}
				
                if ( cookieString.Length > 0 )
                {
					this.SetCookie( new KnetikCookie( cookieString ) );
                }
            }
        }
	}
}
