namespace FitMerger.Service.Models;

public class FitRecord
{
    public uint Timestamp { get; set; }
    public FitPosition Position { get; set; }
    public byte? GpsAccuracy { get; set; }
    public float? Altitude { get; set; }
    public float? EnhancedAltitude { get; set; }
    public float? Distance { get; set; }
    public byte? Cadence { get; set; }
    public float? Speed { get; set; }
    public float? EnhancedSpeed { get; set; }
    public sbyte? Temperature { get; set; }
    public FitRecord(RecordMesg record)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        Timestamp = record.GetTimestamp().GetTimeStamp();
        if (record.GetPositionLat().HasValue)
        {
            Position = new FitPosition(record);
        }
        GpsAccuracy = record.GetGpsAccuracy();
        Altitude = record.GetAltitude();
        EnhancedAltitude = record.GetEnhancedAltitude();
        Distance = record.GetDistance();
        Cadence = record.GetCadence();
        Speed = record.GetSpeed();
        EnhancedSpeed = record.GetEnhancedSpeed();
        Temperature = record.GetTemperature();
    }
    public override string ToString()
    {
        return $"Time = {Timestamp}, Pos = {Position}, " +
            $"Alt = {Altitude ?? EnhancedAltitude}, Dist = {Distance}, Cad = {Cadence}, " +
            $"Speed = {Speed ?? EnhancedSpeed}, Temp = {Temperature}";
    }
}
