using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using OmbordSJ.Api;

namespace OmbordSJ
{
	public class HotspotRunner
	{

		private static HotspotRunner _instance = null;
		private Api.User _user = null;
		private Api.System _system = null;
		private Api.Trip _trip = null;
		private Api.Login _login = null;
		private bool reset = false;
		private string _eth = string.Empty;

		public Api.Login Login
		{
			get
			{
				return this._login;
			}
		}

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
			this._login = new Api.Login ();
		}

		public bool Running
		{
			get
			{
				this.Login.Update ();
				if ( !this.User.Update () )
				{

					this.reset = false;
					return true;
				}

				this.Trip.Update ();
				if ( this._eth == string.Empty )
				{
					foreach ( NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces () )
					{
						foreach ( UnicastIPAddressInformation ip in ni.GetIPProperties ().UnicastAddresses )
						{

							if ( ip.Address.AddressFamily == AddressFamily.InterNetwork )
							{
								if ( ip.Address.ToString () == this.User.Ip.ToString () )
								{
									//Console.WriteLine ( ni.Id + "-" + ip.Address.ToString () );
									this._eth = ni.Id;
								}
							}
						}
					}
				}

				Console.Clear ();


				var next = this.Trip.Next;

				Console.WriteLine ( "System: " + this.System.Id + " - Tick: " + this.User.UnixTime + " - IP: " + this.User.Ip.ToString () + " - Mac: " + this.User.Mac );
				Console.WriteLine ( "Company: " + this.Trip.Company + " - Train #: " + this.Trip.TrainNumber + " - Type: " + this.Trip.Operator );
				if ( next != null )
				{
					Console.WriteLine ( "Next station: " + next["Name"].ToString () + " - Arrival: " + next["ArrivalTime"].ToString () + " - Delayed? " + ( ( bool.Parse ( next["IsArrivalDelayed"].ToString () ) ) ? "Yes" : "No" ) + ( ( bool.Parse ( next["IsArrivalDelayed"].ToString () ) ) ? " - New time: " + next["RealArrivalTime"].ToString () + " - Why? " + string.Join ( ", ", next["Remarks"].Children () ) : "" ) );

					string endTime = ( ( bool.Parse ( next["IsArrivalDelayed"].ToString () ) ) ? next["RealArrivalTime"].ToString () : next["ArrivalTime"].ToString () );
					if ( endTime != String.Empty )
					{
						TimeSpan duration = DateTime.Parse ( endTime ).Subtract ( DateTime.Now );
						Console.WriteLine ( "Time to arrival: " + duration.Hours + "h " + duration.Minutes + "m " + duration.Seconds + "s" );
					}
				}
				Console.WriteLine ( "Limit: " + ( ( this.User.DataTotalLimit / 1024 ) / 1024 ) + "MB" + " - Total used: " + ( ( this.User.DataTotalUsed / 1024 ) / 1024 ) + "MB" + " - Data left: " + ( ( ( this.User.DataTotalLimit - this.User.DataTotalUsed ) / 1024 ) / 1024 ) + "MB" );


				if ( ( this.User.DataTotalLimit - this.User.DataTotalUsed ) < 0 && !this.reset )
				{
					/* Check https://github.com/MBeijer/ChangeMAConOSX for chgmac application */
					Random random = new Random ();
					int randomNumber = random.Next ( 0, 256 );
					string newMac = randomNumber.ToString ( "X" );
					randomNumber = random.Next ( 0, 256 );
					newMac += randomNumber.ToString ( "X" );

					newMac = newMac.Substring ( 0, 4 );

					Process test = Process.Start ( "/Users/marlon/bin/chgmac.app/Contents/MacOS/chgmac-exec2", ( this._eth + " " + newMac ).ToString () );
					test.WaitForExit ();
					this.User.ResetData ();
					Console.WriteLine ( "Reset!" );
					this.reset = true;

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
