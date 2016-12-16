using Newtonsoft.Json.Linq;

namespace OmbordSJ
{
	public class HotspotTrip : ApiCallBase
	{
		public int TrainNumber
		{
			get
			{
				if ( this._json == null )
					return -1;
				else
					return int.Parse ( this._json["AnnouncedTrainNumber"].ToString () );
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

		public HotspotTrip ( string id ) : base ( id, "system" )
		{



		}


		~HotspotTrip ()
		{

		}
	}
}
