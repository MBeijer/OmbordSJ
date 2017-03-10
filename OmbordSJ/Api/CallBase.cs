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
				apiCallUrl = "https://www.ombord.info/hotspot/hotspot.cgi?realm=sj_mio_standard_55&method=classcheck&_=" + this.UnixTime;

			_request = (HttpWebRequest)WebRequest.Create ( apiCallUrl );
			_request.Timeout = 10000;
			_request.Referer = "http://ombord.sj.se/";
			_request.Host = "www.ombord.info";
			_request.Accept = "*/*";
			if ( this.Response != null )
			{

				_responseStream = this.Response.GetResponseStream ();
				_readerStream = new StreamReader ( _responseStream, Encoding.GetEncoding ( "utf-8" ) );

				string json = _readerStream.ReadToEnd ();
				_readerStream.Close ();
				_responseStream.Close ();

				json = json
					.Replace ( "(", "" )
					.Replace ( ");", "" )
					.Replace ( "\\x", "\\u00" );


				this._json = JObject.Parse ( json );
				return true;
			}
			return false;
		}

		~CallBase ()
		{
		}
	}
}
