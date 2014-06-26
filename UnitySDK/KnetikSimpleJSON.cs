//#define USE_SharpZipLib

/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Features / attributes:
 * - provides strongly typed node classes and lists / dictionaries
 * - provides easy access to class members / array items / data values
 * - the parser ignores data types. Each value is a string.
 * - only double quotes (") are used for quoting strings.
 * - values and names are not restricted to quoted strings. They simply add up and are trimmed.
 * - There are only 3 types: arrays(JSONArray), objects(JSONClass) and values(JSONData)
 * - provides "casting" properties to easily convert to / from those types:
 *   int / float / double / bool
 * - provides a common interface for each node so no explicit casting is required.
 * - the parser try to avoid errors, but if malformed JSON is parsed the result is undefined
 * 
 * 
 * 2012-12-17 Update:
 * - Added internal JSONLazyCreator class which simplifies the construction of a JSON tree
 *   Now you can simple reference any item that doesn't exist yet and it will return a JSONLazyCreator
 *   The class determines the required type by it's further use, creates the type and removes itself.
 * - Added binary serialization / deserialization.
 * - Added support for BZip2 zipped binary format. Requires the SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ )
 *   The usage of the SharpZipLib library can be disabled by removing or commenting out the USE_SharpZipLib define at the top
 * - The serializer uses different types when it comes to store the values. Since my data values
 *   are all of type string, the serializer will "try" which format fits best. The order is: int, float, double, bool, string.
 *   It's not the most efficient way but for a moderate amount of data it should work on all platforms.
 * 
 * * * * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KnetikSimpleJSON
{
	public enum KnetikJSONBinaryTag
	{
		Array			= 1,
		Class			= 2,
		Value			= 3,
		IntValue		= 4,
		DoubleValue		= 5,
		BoolValue		= 6,
		FloatValue		= 7,
	}

	public class KnetikJSONNode
    {
        #region common interface
		public virtual void Add(string aKey, KnetikJSONNode aItem){ }
		public virtual KnetikJSONNode this[int aIndex]   { get { return null; } set { } }
		public virtual KnetikJSONNode this[string aKey]  { get { return null; } set { } }
        public virtual string Value                { get { return "";   } set { } }
		public virtual int Count                   { get { return 0;    } }

		public virtual void Add(KnetikJSONNode aItem)
        {
            Add("", aItem);
        }

		public virtual KnetikJSONNode Remove(string aKey) { return null; }
		public virtual KnetikJSONNode Remove(int aIndex) { return null; }
		public virtual KnetikJSONNode Remove(KnetikJSONNode aNode) { return aNode; }

		public virtual IEnumerable<KnetikJSONNode> Childs { get { yield break;} }
		public IEnumerable<KnetikJSONNode> DeepChilds
        {
            get
            {
                foreach (var C in Childs)
                    foreach (var D in C.DeepChilds)
                        yield return D;
            }
        }

        public override string ToString()
        {
			return "KnetikJSONNode";
        }
        public virtual string ToString(string aPrefix)
        {
			return "KnetikJSONNode";
        }

        #endregion common interface
       #region typecasting properties
        public virtual int AsInt
        {
            get
            {
                int v = 0;
                if (int.TryParse(Value,out v))
                    return v;
                return 0;
            }
            set
            {
                Value = value.ToString();
            }
        }
        public virtual float AsFloat
        {
            get
            {
                float v = 0.0f;
                if (float.TryParse(Value,out v))
                    return v;
                return 0.0f;
            }
            set
            {
                Value = value.ToString();
            }
        }
        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value,out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString();
            }
        }
        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value,out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value)?"true":"false";
            }
        }
		public virtual KnetikJSONArray AsArray
        {
            get
            {
				return this as KnetikJSONArray;
            }
        }
		public virtual KnetikJSONClass AsObject
        {
            get
            {
				return this as KnetikJSONClass;
            }
        }


        #endregion typecasting properties

        #region operators
		public static implicit operator KnetikJSONNode(string s)
        {
			return new KnetikJSONData(s);
        }
		public static implicit operator string(KnetikJSONNode d)
        {
            return (d == null)?null:d.Value;
        }
		public static bool operator ==(KnetikJSONNode a, object b)
		{
			if (b == null && a is KnetikJSONLazyCreator)
				return true;
			return System.Object.ReferenceEquals(a,b);
		}
		
		public static bool operator !=(KnetikJSONNode a, object b)
		{
		    return !(a == b);
		}
		public override bool Equals (object obj)
		{
			return System.Object.ReferenceEquals(this, obj);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}
		
		
        #endregion operators

        internal static string Escape(string aText)
        {
            string result = "";
            foreach(char c in aText)
            {
                switch(c)
                {
                    case '\\' : result += "\\\\"; break;
                    case '\"' : result += "\\\""; break;
                    case '\n' : result += "\\n" ; break;
                    case '\r' : result += "\\r" ; break;
                    case '\t' : result += "\\t" ; break;
                    case '\b' : result += "\\b" ; break;
                    case '\f' : result += "\\f" ; break;
                    default   : result += c     ; break;
                }
            }
            return result;
        }

		public static KnetikJSONNode Parse(string aJSON)
        {
			Stack<KnetikJSONNode> stack = new Stack<KnetikJSONNode>();
			KnetikJSONNode ctx = null;
            int i = 0;
            string Token = "";
            string TokenName = "";
            bool QuoteMode = false;
            while (i < aJSON.Length)
            {
                switch (aJSON[i])
                {
                    case '{':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
						stack.Push(new KnetikJSONClass());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
							if (ctx is KnetikJSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName,stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                    break;

                    case '[':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }

						stack.Push(new KnetikJSONArray());
                        if (ctx != null)
                        {
                            TokenName = TokenName.Trim();
							if (ctx is KnetikJSONArray)
                                ctx.Add(stack.Peek());
                            else if (TokenName != "")
                                ctx.Add(TokenName,stack.Peek());
                        }
                        TokenName = "";
                        Token = "";
                        ctx = stack.Peek();
                    break;

                    case '}':
                    case ']':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (Token != "")
                        {
                            TokenName = TokenName.Trim();
							if (ctx is KnetikJSONArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName,Token);
                        }
                        TokenName = "";
                        Token = "";
                        if (stack.Count>0)
                            ctx = stack.Peek();
                    break;

                    case ':':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        TokenName = Token;
                        Token = "";
                    break;

                    case '"':
                        QuoteMode ^= true;
                    break;

                    case ',':
                        if (QuoteMode)
                        {
                            Token += aJSON[i];
                            break;
                        }
                        if (Token != "")
                        {
							if (ctx is KnetikJSONArray)
                                ctx.Add(Token);
                            else if (TokenName != "")
                                ctx.Add(TokenName, Token);
                        }
                        TokenName = "";
                        Token = "";
                    break;

                    case '\r':
                    case '\n':
                    break;

                    case ' ':
                    case '\t':
                        if (QuoteMode)
                            Token += aJSON[i];
                    break;

                    case '\\':
                        ++i;
                        if (QuoteMode)
                        {
                            char C = aJSON[i];
                            switch (C)
                            {
                                case 't' : Token += '\t'; break;
                                case 'r' : Token += '\r'; break;
                                case 'n' : Token += '\n'; break;
                                case 'b' : Token += '\b'; break;
                                case 'f' : Token += '\f'; break;
                                case 'u':
                                {
                                    string s = aJSON.Substring(i+1,4);
                                    Token += (char)int.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier);
                                    i += 4;
                                    break;
                                }
                                default  : Token += C; break;
                            }
                        }
                    break;

                    default:
                        Token += aJSON[i];
                    break;
                }
                ++i;
            }
            if (QuoteMode)
            {
				Debug.LogWarning("Knetik Labs SDK - Double check the formatting of the KnetikConfig.json file. (This may not be the cause)");
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            return ctx;
        }
		
		public virtual void Serialize(System.IO.BinaryWriter aWriter) {}

		public void SaveToStream(System.IO.Stream aData)
		{
			var W = new System.IO.BinaryWriter(aData);
			Serialize(W);
		}
		
		#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToStream(gzipOut);
				gzipOut.Close();
			}
		}

		public void SaveToCompressedFile(string aFileName)
		{
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}

        #else
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}
        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        public string SaveToCompressedBase64()
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
        #endif
		
		public void SaveToFile(string aFileName)
		{
			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToStream(F);
			}
		}
		public string SaveToBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}
		public static KnetikJSONNode Deserialize(System.IO.BinaryReader aReader)
		{
			KnetikJSONBinaryTag type = (KnetikJSONBinaryTag)aReader.ReadByte();
			switch(type)
			{
			case KnetikJSONBinaryTag.Array:
			{
				int count = aReader.ReadInt32();
				KnetikJSONArray tmp = new KnetikJSONArray();
				for(int i = 0; i < count; i++)
					tmp.Add(Deserialize(aReader));
				return tmp;
			}
			case KnetikJSONBinaryTag.Class:
			{
				int count = aReader.ReadInt32();				
				KnetikJSONClass tmp = new KnetikJSONClass();
				for(int i = 0; i < count; i++)
				{
					string key = aReader.ReadString();
					var val = Deserialize(aReader);
					tmp.Add(key, val);
				}
				return tmp;
			}
			case KnetikJSONBinaryTag.Value:
			{
				return new KnetikJSONData(aReader.ReadString());
			}
			case KnetikJSONBinaryTag.IntValue:
			{
				return new KnetikJSONData(aReader.ReadInt32());
			}
			case KnetikJSONBinaryTag.DoubleValue:
			{
				return new KnetikJSONData(aReader.ReadDouble());
			}
			case KnetikJSONBinaryTag.BoolValue:
			{
				return new KnetikJSONData(aReader.ReadBoolean());
			}
			case KnetikJSONBinaryTag.FloatValue:
			{
				return new KnetikJSONData(aReader.ReadSingle());
			}
				
			default:
			{
				throw new Exception("Error deserializing JSON. Unknown tag: " + type);
			}
			}
		}
		
		#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
        #else
		public static KnetikJSONNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
		public static KnetikJSONNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
		public static KnetikJSONNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
		#endif
		
		public static KnetikJSONNode LoadFromStream(System.IO.Stream aData)
		{
			using(var R = new System.IO.BinaryReader(aData))
			{
				return Deserialize(R);
			}
		}
		public static KnetikJSONNode LoadFromFile(string aFileName)
		{
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromStream(F);
			}
		}
		public static KnetikJSONNode LoadFromBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromStream(stream);
		}
    } // End of JSONNode

	public class KnetikJSONArray : KnetikJSONNode, IEnumerable
    {
		private List<KnetikJSONNode> m_List = new List<KnetikJSONNode>();
		public override KnetikJSONNode this[int aIndex]
        {
            get
			{
				if (aIndex<0 || aIndex >= m_List.Count)
					return new KnetikJSONLazyCreator(this);
				return m_List[aIndex];
			}
            set
            {
				if (aIndex<0 || aIndex >= m_List.Count)
                    m_List.Add(value);
                else
                    m_List[aIndex] = value;
            }
        }
		public override KnetikJSONNode this[string aKey]
		{
			get{ return new KnetikJSONLazyCreator(this);}
			set{ m_List.Add(value); }
		}
		public override int Count
		{
			get { return m_List.Count; }
		}
		public override void Add(string aKey, KnetikJSONNode aItem)
        {
            m_List.Add(aItem);
        }
		public override KnetikJSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_List.Count)
                return null;
			KnetikJSONNode tmp = m_List[aIndex];
            m_List.RemoveAt(aIndex);
            return tmp;
        }
		public override KnetikJSONNode Remove(KnetikJSONNode aNode)
        {
            m_List.Remove(aNode);
            return aNode;
        }
		public override IEnumerable<KnetikJSONNode> Childs
        {
            get
            {
				foreach(KnetikJSONNode N in m_List)
                    yield return N;
            }
        }
        public IEnumerator GetEnumerator()
        {
			foreach(KnetikJSONNode N in m_List)
                yield return N;
        }
        public override string ToString()
        {
            string result = "[ ";
			foreach (KnetikJSONNode N in m_List)
            {
                if (result.Length > 2)
                    result += ", ";
                result += N.ToString();
            }
            result += " ]";
            return result;
        }
        public override string ToString(string aPrefix)
        {
            string result = "[ ";
			foreach (KnetikJSONNode N in m_List)
            {
                if (result.Length > 3)
                    result += ", ";
				result += "\n" + aPrefix + "   ";				
                result += N.ToString(aPrefix+"   ");
            }
            result += "\n" + aPrefix + "]";
            return result;
        }
		public override void Serialize (System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)KnetikJSONBinaryTag.Array);
			aWriter.Write(m_List.Count);
			for(int i = 0; i < m_List.Count; i++)
			{
				m_List[i].Serialize(aWriter);
			}
		}
    } // End of JSONArray

	public class KnetikJSONClass : KnetikJSONNode, IEnumerable
    {
		private Dictionary<string,KnetikJSONNode> m_Dict = new Dictionary<string,KnetikJSONNode>();
		public override KnetikJSONNode this[string aKey]
        {
            get
			{
				if (m_Dict.ContainsKey(aKey))
					return m_Dict[aKey];
				else
					return new KnetikJSONLazyCreator(this, aKey);
			}
            set
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = value;
                else
                    m_Dict.Add(aKey,value);
            }
        }
		public override KnetikJSONNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
					return null;
                return m_Dict.ElementAt(aIndex).Value;
            }
            set
            {
                if (aIndex < 0 || aIndex >= m_Dict.Count)
                    return;
                string key = m_Dict.ElementAt(aIndex).Key;
                m_Dict[key] = value;
            }
        }
		public override int Count
		{
			get { return m_Dict.Count; }
		}


		public override void Add(string aKey, KnetikJSONNode aItem)
        {
            if (!string.IsNullOrEmpty(aKey))
            {
                if (m_Dict.ContainsKey(aKey))
                    m_Dict[aKey] = aItem;
                else
                    m_Dict.Add(aKey, aItem);
            }
            else
                m_Dict.Add(Guid.NewGuid().ToString(), aItem);
        }

		public override KnetikJSONNode Remove(string aKey)
        {
            if (!m_Dict.ContainsKey(aKey))
                return null;
			KnetikJSONNode tmp = m_Dict[aKey];
            m_Dict.Remove(aKey);
            return tmp;        
        }
		public override KnetikJSONNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= m_Dict.Count)
                return null;
            var item = m_Dict.ElementAt(aIndex);
            m_Dict.Remove(item.Key);
            return item.Value;
        }
		public override KnetikJSONNode Remove(KnetikJSONNode aNode)
        {
            try
            {
                var item = m_Dict.Where(k => k.Value == aNode).First();
                m_Dict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

		public override IEnumerable<KnetikJSONNode> Childs
        {
            get
            {
				foreach(KeyValuePair<string,KnetikJSONNode> N in m_Dict)
                    yield return N.Value;
            }
        }

        public IEnumerator GetEnumerator()
        {
			foreach(KeyValuePair<string, KnetikJSONNode> N in m_Dict)
                yield return N;
        }
        public override string ToString()
        {
            string result = "{";
			foreach (KeyValuePair<string, KnetikJSONNode> N in m_Dict)
            {
                if (result.Length > 2)
                    result += ", ";
                result += "\"" + Escape(N.Key) + "\":" + N.Value.ToString();
            }
            result += "}";
            return result;
        }
        public override string ToString(string aPrefix)
        {
            string result = "{ ";
			foreach (KeyValuePair<string, KnetikJSONNode> N in m_Dict)
            {
                if (result.Length > 3)
                    result += ", ";
				result += "\n" + aPrefix + "   ";
                result += "\"" + Escape(N.Key) + "\" : " + N.Value.ToString(aPrefix+"   ");
            }
            result += "\n" + aPrefix + "}";
            return result;
        }
		public override void Serialize (System.IO.BinaryWriter aWriter)
		{
			aWriter.Write((byte)KnetikJSONBinaryTag.Class);
			aWriter.Write(m_Dict.Count);
			foreach(string K in m_Dict.Keys)
			{
				aWriter.Write(K);
				m_Dict[K].Serialize(aWriter);
			}
		}
    } // End of JSONClass

	public class KnetikJSONData : KnetikJSONNode
    {
        private string m_Data;
        public override string Value
        {
            get { return m_Data; }
            set { m_Data = value; }
        }
		public KnetikJSONData(string aData)
        {
            m_Data = aData;
        }
		public KnetikJSONData(float aData)
        {
            AsFloat = aData;
        }
		public KnetikJSONData(double aData)
        {
            AsDouble = aData;
        }
		public KnetikJSONData(bool aData)
        {
            AsBool = aData;
        }
		public KnetikJSONData(int aData)
        {
            AsInt = aData;
        }
		
        public override string ToString()
        {
            return "\"" + Escape(m_Data) + "\"";
        }
        public override string ToString(string aPrefix)
        {
            return "\"" + Escape(m_Data) + "\"";
        }
		public override void Serialize (System.IO.BinaryWriter aWriter)
		{
			var tmp = new KnetikJSONData("");
			
			tmp.AsInt = AsInt;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)KnetikJSONBinaryTag.IntValue);
				aWriter.Write(AsInt);
				return;
			}
			tmp.AsFloat = AsFloat;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)KnetikJSONBinaryTag.FloatValue);
				aWriter.Write(AsFloat);
				return;
			}
			tmp.AsDouble = AsDouble;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)KnetikJSONBinaryTag.DoubleValue);
				aWriter.Write(AsDouble);
				return;
			}

			tmp.AsBool = AsBool;
			if (tmp.m_Data == this.m_Data)
			{
				aWriter.Write((byte)KnetikJSONBinaryTag.BoolValue);
				aWriter.Write(AsBool);
				return;
			}
			aWriter.Write((byte)KnetikJSONBinaryTag.Value);
			aWriter.Write(m_Data);
		}
    } // End of JSONData
	
	internal class KnetikJSONLazyCreator : KnetikJSONNode
	{
		private KnetikJSONNode m_Node = null;
		private string m_clientId = null;
		
		public KnetikJSONLazyCreator(KnetikJSONNode aNode)
		{
			m_Node = aNode;
			m_clientId  = null;
		}
		public KnetikJSONLazyCreator(KnetikJSONNode aNode, string aKey)
		{
			m_Node = aNode;
			m_clientId = aKey;
		}
		
		private void Set(KnetikJSONNode aVal)
		{
			if (m_clientId == null)
			{
				m_Node.Add(aVal);
			}
			else
			{
				m_Node.Add(m_clientId, aVal);
			}
			m_Node = null; // Be GC friendly.
		}
		
		public override KnetikJSONNode this[int aIndex]
		{
			get
			{
				return new KnetikJSONLazyCreator(this);
			}
			set
			{
				var tmp = new KnetikJSONArray();
				tmp.Add(value);
				Set(tmp);
			}
		}
			
		public override KnetikJSONNode this[string aKey]
		{
			get
			{
				return new KnetikJSONLazyCreator(this, aKey);
			}
			set
			{
				var tmp = new KnetikJSONClass();
				tmp.Add(aKey, value);
				Set(tmp);
			}
		}
		public override void Add (KnetikJSONNode aItem)
		{
			var tmp = new KnetikJSONArray();
			tmp.Add(aItem);
			Set(tmp);
		}
		public override void Add (string aKey, KnetikJSONNode aItem)
		{
			var tmp = new KnetikJSONClass();
			tmp.Add(aKey, aItem);
			Set(tmp);
		}
		public static bool operator ==(KnetikJSONLazyCreator a, object b)
		{
			if (b == null)
				return true;
			return System.Object.ReferenceEquals(a,b);
		}
		
		public static bool operator !=(KnetikJSONLazyCreator a, object b)
		{
		    return !(a == b);
		}
		public override bool Equals (object obj)
		{
			if (obj == null)
				return true;
			return System.Object.ReferenceEquals(this, obj);
		}
		public override int GetHashCode ()
		{
			return base.GetHashCode();
		}
		
        public override string ToString()
        {
            return "";
        }
        public override string ToString(string aPrefix)
        {
            return "";
        }
		
        public override int AsInt
        {
            get
            {
				KnetikJSONData tmp = new KnetikJSONData(0);
				Set(tmp);
                return 0;
            }
            set
            {
				KnetikJSONData tmp = new KnetikJSONData(value);
				Set(tmp);
            }
        }
        public override float AsFloat
        {
            get
            {
				KnetikJSONData tmp = new KnetikJSONData(0.0f);
				Set(tmp);
                return 0.0f;
            }
            set
            {
				KnetikJSONData tmp = new KnetikJSONData(value);
				Set(tmp);
            }
        }
        public override double AsDouble
        {
            get
            {
				KnetikJSONData tmp = new KnetikJSONData(0.0);
				Set(tmp);
                return 0.0;
            }
            set
            {
				KnetikJSONData tmp = new KnetikJSONData(value);
				Set(tmp);
            }
        }
        public override bool AsBool
        {
            get
            {
				KnetikJSONData tmp = new KnetikJSONData(false);
				Set(tmp);
                return false;
            }
            set
            {
				KnetikJSONData tmp = new KnetikJSONData(value);
				Set(tmp);
            }
        }
		public override KnetikJSONArray AsArray
        {
            get
            {
				KnetikJSONArray tmp = new KnetikJSONArray();
				Set(tmp);
                return tmp;
            }
        }
		public override KnetikJSONClass AsObject
        {
            get
            {
				KnetikJSONClass tmp = new KnetikJSONClass();
				Set(tmp);
                return tmp;
            }
        }
	} // End of JSONLazyCreator

	public static class KnetikJSON
    {
		public static KnetikJSONNode Parse(string aJSON)
        {
			return KnetikJSONNode.Parse(aJSON);
        }
    }
}