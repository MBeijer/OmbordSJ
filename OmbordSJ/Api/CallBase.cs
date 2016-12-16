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

		public void Update ()
		{
			string apiCallUrl = "";
			if ( this._kind == "api" )
				apiCallUrl = "http://www.ombord.info/api/jsonp/" + this._apiCall + "/?_=" + this.UnixTime;
			else
				apiCallUrl = "http://services.ombord.sj.se/traffic/Trip?systemid=" + this._apiCall;

			_request = (HttpWebRequest)WebRequest.Create ( apiCallUrl );

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
			}

		}

		~CallBase ()
		{
		}
	}
}
