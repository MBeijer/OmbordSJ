using System.Linq;
using Newtonsoft.Json.Linq;

namespace OmbordSJ.Api
{
	public class Trip : CallBase
	{
		public int TrainNumber => Json == null ? -1 : ( Json["AnnouncedTrainNumber"].ToString() != string.Empty ) ? int.Parse( Json["AnnouncedTrainNumber"].ToString() ) : -1;

		public string Operator => Json == null ? "" : Json["Operator"].ToString();

		public string Company => Json == null ? "" : Json["OperatorCompany"].ToString();

		public JToken Stations => Json == null ? "" : Json["Stations"];

		public JToken Next => Json?["Stations"].FirstOrDefault( s => !bool.Parse( s["HasArrived"].ToString() ) );

		public Trip( string id ) : base( id, "system" ) { }
	}
}