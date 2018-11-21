using System;
using System.Threading;

namespace OmbordSJ
{
	internal static class MainClass
	{
		public static void Main ( string[] args )
		{
			Console.WriteLine ( "Välkommen ombord!" );

			while ( HotspotRunner.Ins.Running )
				Thread.Sleep ( 1000 );
		}
	}
}
