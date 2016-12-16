using System;
using System.Diagnostics;
using OmbordSJ.Api;

namespace OmbordSJ
{
	public class HotspotRunner
	{

		private static HotspotRunner _instance = null;
		private Api.User _user = null;
		private Api.System _system = null;
		private Api.Trip _trip = null;

		public Api.User User
		{
			get
			{
				return this._user;
			}
		}

		public Api.System System
		{
			get
			{
				return this._system;
			}
		}

		public Api.Trip Trip
		{
			get
			{
				return this._trip;
			}
		}

		public static HotspotRunner Ins
		{
			get
			{
				if ( _instance == null )
					_instance = new HotspotRunner ();

				return _instance;
			}
		}


		private HotspotRunner ()
		{
			this._user = new Api.User ();
			this._system = new Api.System ();
			this._trip = new Api.Trip ( this.System.Id.ToString () );
		}

		public bool Running
		{
			get
			{
				this.User.Update ();
				this.Trip.Update ();
				var next = this.Trip.Next;
				Console.Clear ();
				Console.WriteLine ( "System: " + this.System.Id + " - Tick: " + this.User.UnixTime + " - IP: " + this.User.Ip + " - Mac: " + this.User.Mac );
				Console.WriteLine ( "Company: " + this.Trip.Company + " - Train #: " + this.Trip.TrainNumber + " - Type: " + this.Trip.Operator );
				if ( next != null )
				{
					Console.WriteLine ( "Next station: " + next["Name"].ToString () + " - Arrival: " + next["ArrivalTime"].ToString () + " - Delayed? " + ( ( bool.Parse ( next["IsArrivalDelayed"].ToString () ) ) ? "Yes" : "No" ) + ( ( bool.Parse ( next["IsArrivalDelayed"].ToString () ) ) ? " - New time: " + next["RealArrivalTime"].ToString () + " - Why? " + string.Join ( ", ", next["Remarks"].Children () ) : "" ) );

					string endTime = ( ( bool.Parse ( next["IsArrivalDelayed"].ToString () ) ) ? next["RealArrivalTime"].ToString () : next["ArrivalTime"].ToString () );

					TimeSpan duration = DateTime.Parse ( endTime ).Subtract ( DateTime.Now );
					Console.WriteLine ( "Time to arrival: " + duration.Hours + "h " + duration.Minutes + "m " + duration.Seconds + "s" );
				}
				Console.WriteLine ( "Limit: " + ( ( this.User.DataTotalLimit / 1024 ) / 1024 ) + "MB" + " - Total used: " + ( ( this.User.DataTotalUsed / 1024 ) / 1024 ) + "MB" + " - Data left: " + ( ( ( this.User.DataTotalLimit - this.User.DataTotalUsed ) / 1024 ) / 1024 ) + "MB" );

				if ( ( this.User.DataTotalLimit - this.User.DataTotalUsed ) <= 0 )
				{
					/* Check https://github.com/MBeijer/ChangeMAConOSX for chgmac application */
					Process test = Process.Start ( "/Users/marlon/bin/chgmac.app/Contents/MacOS/chgmac" );
					test.WaitForExit ();

				}
				return true;
			}
		}

		~HotspotRunner ()
		{
			this._user = null;

		}
	}
}
