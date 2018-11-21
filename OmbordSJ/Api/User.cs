using System.Net;

namespace OmbordSJ.Api
{
	public class User : CallBase
	{
		public int DataTotalLimit => Json == null ? 1 : int.Parse( Json["data_total_limit"].ToString() );

		public int DataTotalUsed => Json == null ? 0 : int.Parse( Json["data_total_used"].ToString() );

		public int DataDownloadUsed => Json == null ? -1 : int.Parse( Json["data_download_used"].ToString() );

		public int DataUploadUsed => Json == null ? -1 : int.Parse( Json["data_upload_used"].ToString() );

		public IPAddress Ip => Json == null ? null : IPAddress.Parse( Json["ip"].ToString() );

		public string Mac => Json == null ? "" : Json["mac"].ToString();

		internal void ResetData() => Json = null;

		public User() : base( "user" ) { }
	}
}