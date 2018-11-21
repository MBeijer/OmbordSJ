using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OmbordSJ.Api
{
	public class CallBase
	{
		private HttpWebRequest _request;
		private HttpWebResponse _response;
		private Stream _responseStream;
		private StreamReader _readerStream;
		private readonly string _apiCall = "";
		private readonly string _kind = "";
		protected JObject Json;

		public static int UnixTime => (int) ( DateTime.UtcNow.Subtract( new DateTime( 1970, 1, 1 ) ) ).TotalSeconds;

		private HttpWebResponse Response
		{
			get
			{
				try
				{
					_response = (HttpWebResponse) _request.GetResponse();

					return _response;
				}
				catch ( Exception )
				{
					return null;
				}
			}
		}

		protected CallBase( string apiCall, string kind = "api" )
		{
			_apiCall = apiCall;
			_kind = kind;

			Update();
		}

		public bool Update()
		{
			var apiCallUrl = "";

			switch ( _kind )
			{
				case "api":
					apiCallUrl = "http://www.ombord.info/api/jsonp/" + _apiCall + "/?_=" + UnixTime;

					break;
				case "system":
					apiCallUrl = "http://services.ombord.sj.se/traffic/Trip?systemid=" + _apiCall;

					break;
				case "login":

					apiCallUrl =
						"https://www.ombord.info/hotspot/hotspot.cgi?method=login&password=&username=&callback=_jsonp_1&_=" +
						UnixTime;

					break;
				default:

					throw new Exception( "Missing api type" );
			}

			CreateWebRequestAndSetHeaders( apiCallUrl );

			if ( Response == null ) return false;
			_responseStream = Response.GetResponseStream();
			_readerStream = new StreamReader( _responseStream ?? throw new Exception(), encoding: Encoding.GetEncoding( "utf-8" ) );

			var json = _readerStream.ReadToEnd();
			_readerStream.Close();
			_responseStream.Close();

			if ( _kind == "login" ) return true;

			json = json
				.Replace( "(", "" )
				.Replace( ");", "" )
				.Replace( "\\x", "\\u00" );

			Json = JObject.Parse( json );

			return true;
		}

		private void CreateWebRequestAndSetHeaders( string apiCallUrl )
		{
			_request = (HttpWebRequest) WebRequest.Create( apiCallUrl );
			_request.Timeout = 10000;
			_request.Referer = "http://ombord.sj.se/";

			_request.Host = _kind == "login" ? "www.ombord.info" : "services.ombord.sj.se";

			_request.Accept = "*/*";
			_request.Headers.Add( "DNT", "1" );
			_request.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
		}
	}
}