using Newtonsoft.Json.Linq;

namespace OmbordSJ.Api
{
	public class Trip : CallBase
	{
		public int TrainNumber
		{
			get
			{
				if ( this._json == null )
					return -1;
				else
					return ( this._json["AnnouncedTrainNumber"].ToString () != string.Empty ) ? int.Parse ( this._json["AnnouncedTrainNumber"].ToString () ) : -1;
			}
		}

		public string Operator
		{
			get
			{
				if ( this._json == null )
					return "";
				else
					return this._json["Operator"].ToString ();
			}
		}

		public string Company
		{
			get
			{
				if ( this._json == null )
					return "";
				else
					return this._json["OperatorCompany"].ToString ();
			}
		}

		public JToken Stations
		{
			get
			{
				if ( this._json == null )
					return "";
				else
					return this._json["Stations"];
			}
		}

		public JToken Next
		{
			get
			{
				if ( this._json == null )
					return null;
				else
					foreach ( JToken s in this._json["Stations"] )
					{
						if ( bool.Parse ( s["HasArrived"].ToString () ) == true )
							continue;
						else
							return s;

					}
				return null;
			}
		}

		public Trip ( string id ) : base ( id, "system" )
		{



		}


		~Trip ()
		{

		}
	}
}
