using System;
using System.Net;


namespace OmbordSJ.Api
{
	public class User : CallBase
	{
		public int DataTotalLimit
		{
			get
			{
				if ( this._json == null )
					return 1;
				else
					return int.Parse ( this._json["data_total_limit"].ToString () );
			}
		}
		public int DataTotalUsed
		{
			get
			{
				if ( this._json == null )
					return 0;
				else
					return int.Parse ( this._json["data_total_used"].ToString () );
			}
		}

		public int DataDownloadUsed
		{
			get
			{
				if ( this._json == null )
					return -1;
				else
					return int.Parse ( this._json["data_download_used"].ToString () );
			}
		}

		public int DataUploadUsed
		{
			get
			{
				if ( this._json == null )
					return -1;
				else
					return int.Parse ( this._json["data_upload_used"].ToString () );
			}
		}

		public IPAddress Ip
		{
			get
			{
				if ( this._json == null )
					return null;
				else
					return IPAddress.Parse ( this._json["ip"].ToString () );
			}
		}

		public string Mac
		{
			get
			{
				if ( this._json == null )
					return "";
				else
					return this._json["mac"].ToString ();
			}
		}

		internal void ResetData ()
		{
			this._json = null;
		}

		public User () : base ( "user" )
		{



		}


		~User ()
		{

		}

	}
}
