using System;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmbordSJ.Api
{
	public class CallBase
	{

		private HttpWebRequest _request = null;
		private HttpWebResponse _response = null;
		private Stream _responseStream = null;
		private StreamReader _readerStream = null;
		private string _apiCall = "";
		private string _kind = "";
		protected JObject _json = null;

		public int UnixTime
		{
			get { return (int)( DateTime.UtcNow.Subtract ( new DateTime ( 1970, 1, 1 ) ) ).TotalSeconds; }
		}

		private HttpWebResponse Response
		{
			get
			{
				try
				{
					_response = (HttpWebResponse)_request.GetResponse ();

					return _response;
				}
				catch ( Exception e )
				{
					return null;
				}
			}
		}

		public CallBase ( string apiCall, string kind = "api" )
		{
			this._apiCall = apiCall;
			this._kind = kind;

			this.Update ();
		}

		public bool Update ()
		{
			string apiCallUrl = "";
			if ( this._kind == "api" )
				apiCallUrl = "http://www.ombord.info/api/jsonp/" + this._apiCall + "/?_=" + this.UnixTime;
			else if ( this._kind == "system" )
				apiCallUrl = "http://services.ombord.sj.se/traffic/Trip?systemid=" + this._apiCall;
			else if ( this._kind == "login" )
				apiCallUrl = "https://www.ombord.info/hotspot/hotspot.cgi?method=login&password=&username=&callback=_jsonp_1&_=" + this.UnixTime;

			this.CreateWebRequestAndSetHeaders ( apiCallUrl );

			if ( this.Response != null )
			{

				this._responseStream = this.Response.GetResponseStream ();
				this._readerStream = new StreamReader ( _responseStream, Encoding.GetEncoding ( "utf-8" ) );

				string json = this._readerStream.ReadToEnd ();
				this._readerStream.Close ();
				this._responseStream.Close ();
				if ( this._kind != "login" )
				{
					json = json
						.Replace ( "(", "" )
						.Replace ( ");", "" )
						.Replace ( "\\x", "\\u00" );


					this._json = JObject.Parse ( json );
				}
				return true;
			}
			return false;
		}

		private void CreateWebRequestAndSetHeaders ( string apiCallUrl )
		{
			this._request = (HttpWebRequest)WebRequest.Create ( apiCallUrl );
			this._request.Timeout = 10000;
			this._request.Referer = "http://ombord.sj.se/";

			if ( this._kind == "login" )
				this._request.Host = "www.ombord.info";
			else
				this._request.Host = "services.ombord.sj.se";

			this._request.Accept = "*/*";
			this._request.Headers.Add ( "DNT", "1" );
			this._request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
		}

		~CallBase ()
		{
		}
	}
}
