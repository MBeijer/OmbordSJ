using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using OmbordSJ.Api;
using static System.String;

namespace OmbordSJ
{
	public class HotspotRunner
	{
		private static HotspotRunner _instance;
		private bool _reset;
		private string _eth = Empty;

		public Login Login { get; }

		public User User { get; private set; }

		public Api.System System { get; }

		public Trip Trip { get; }

		public static HotspotRunner Ins => _instance ?? ( _instance = new HotspotRunner() );

		private HotspotRunner()
		{
			User = new User();
			System = new Api.System();
			Trip = new Trip( System.Id.ToString() );
			Login = new Login();
		}

		public bool Running
		{
			get
			{
				Login.Update();

				if ( User.Update() )
				{
					Trip.Update();

					GetInterfaceByUserIp();

					PrintTravelAndDataInformation();

					if ( ( User.DataTotalLimit - User.DataTotalUsed ) >= 0 || _reset ) return true;
					/* Check https://github.com/MBeijer/ChangeMAConOSX for chgmac application */
					var newMac = GenerateNewMacAddress();

					var test = Process.Start( "/Users/marlon/bin/chgmac.app/Contents/MacOS/chgmac-exec2", ( _eth + " " + newMac ) );
					test?.WaitForExit();
					User.ResetData();
					Console.WriteLine( "Reset!" );
					_reset = true;

					return true;
				}

				_reset = false;

				return true;
			}
		}

		private void GetInterfaceByUserIp()
		{
			if ( _eth != Empty ) return;

			foreach ( var ni in NetworkInterface.GetAllNetworkInterfaces() )
			{
				foreach ( var ip in ni.GetIPProperties().UnicastAddresses )
				{
					if ( ip.Address.AddressFamily != AddressFamily.InterNetwork ) continue;
					if ( ip.Address.ToString() != User.Ip.ToString() ) continue;

					_eth = ni.Id;
				}
			}
		}

		private static string GenerateNewMacAddress()
		{
			var random = new Random();
			var randomNumber = random.Next( 0, 256 );
			var newMac = randomNumber.ToString( "X" );
			randomNumber = random.Next( 0, 256 );
			newMac += randomNumber.ToString( "X" );

			newMac = newMac.Substring( 0, 4 );

			return newMac;
		}

		private void PrintTravelAndDataInformation()
		{
			var next = Trip.Next;

			Console.Clear();

			Console.WriteLine( "System: " + System.Id + " - Tick: " + CallBase.UnixTime + " - IP: " + User.Ip + " - Mac: " + User.Mac );
			Console.WriteLine( "Company: " + Trip.Company + " - Train #: " + Trip.TrainNumber + " - Type: " + Trip.Operator );

			if ( next != null )
			{
				Console.WriteLine( "Next station: " + next["Name"] + " - Arrival: " + next["ArrivalTime"] + " - Delayed? " + ( ( bool.Parse( next["IsArrivalDelayed"].ToString() ) ) ? "Yes" : "No" ) + ( ( bool.Parse( next["IsArrivalDelayed"].ToString() ) ) ? " - New time: " + next["RealArrivalTime"] + " - Why? " + Join( ", ", next["Remarks"].Children() ) : "" ) );

				var endTime = ( ( bool.Parse( next["IsArrivalDelayed"].ToString() ) ) ? next["RealArrivalTime"].ToString() : next["ArrivalTime"].ToString() );
				var duration = DateTime.Parse( endTime ).Subtract( DateTime.Now );

				if ( endTime != Empty )
				{
					Console.WriteLine( "Time to arrival: " + duration.Hours + "h " + duration.Minutes + "m " + duration.Seconds + "s" );
				}
			}

			Console.WriteLine( "Limit: " + ( ( User.DataTotalLimit / 1024 ) / 1024 ) + "MB" + " - Total used: " + ( ( User.DataTotalUsed / 1024 ) / 1024 ) + "MB" + " - Data left: " + ( ( ( User.DataTotalLimit - User.DataTotalUsed ) / 1024 ) / 1024 ) + "MB" );
		}

		~HotspotRunner() => User = null;
	}
}