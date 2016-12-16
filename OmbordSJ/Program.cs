using System;
using System.Threading;


namespace OmbordSJ
{
	class MainClass
	{
		public static void Main ( string[] args )
		{
			Console.WriteLine ( "Välkommen ombord!" );

			while ( HotspotRunner.Ins.Running )
				System.Threading.Thread.Sleep ( 1000 );
		}
	}
}
