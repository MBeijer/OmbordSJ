using System;


namespace OmbordSJ
{
	public class HotspotSystem : ApiCallBase
	{
		public int Id
		{
			get
			{
				if ( this._json == null )
					return -1;
				else
					return int.Parse ( this._json["system"].ToString () );
			}
		}


		public HotspotSystem () : base ( "system" )
		{



		}


		~HotspotSystem ()
		{

		}
	}
}
