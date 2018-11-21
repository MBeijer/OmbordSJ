namespace OmbordSJ.Api
{
	public class System : CallBase
	{
		public int Id => Json == null ? -1 : int.Parse( Json["system_id"].ToString() );

		public System() : base( "system" ) { }
	}
}