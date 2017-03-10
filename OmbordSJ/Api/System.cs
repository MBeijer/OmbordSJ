using System;


namespace OmbordSJ.Api
{
	public class System : CallBase
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


		public System () : base ( "system" )
		{



		}


		~System ()
		{

		}
	}
}