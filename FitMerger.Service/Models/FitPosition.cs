namespace FitMerger.Service.Models;

public class FitPosition
{
    public int Latitude { get; set; }
    public int Longitude { get; set; }
    public FitPosition(int latitude, int longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
    public FitPosition(RecordMesg record)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        Latitude = record.GetPositionLat().Value;
        Longitude = record.GetPositionLong().Value;
    }
    public override string ToString()
    {
        return $"[Lat = {Latitude}, Long = {Longitude}]";
    }
}
