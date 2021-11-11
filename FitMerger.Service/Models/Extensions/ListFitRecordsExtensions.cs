namespace FitMerger.Service.Models.Extensions;

public static class ListFitRecordsExtensions
{
    public static IList<FitRecord> MergeRecordsByTimestamp(
        this IList<FitRecord> sourceRecords)
    {
        var result = new List<FitRecord>();
        foreach (var record in sourceRecords)
        {
            if (result.Count > 0 && result.Last().Timestamp == record.Timestamp)
            {
                var last = result.Last();
                if (record.Position != null)
                {
                    last.Position = record.Position;
                }
                if (record.GpsAccuracy != null)
                {
                    last.GpsAccuracy = record.GpsAccuracy;
                }
                if (record.Altitude != null)
                {
                    last.Altitude = record.Altitude;
                }
                if (record.EnhancedAltitude != null)
                {
                    last.EnhancedAltitude = record.EnhancedAltitude;
                }
                if (record.Distance != null)
                {
                    last.Distance = record.Distance;
                }
                if (record.Cadence != null)
                {
                    last.Cadence = record.Cadence;
                }
                if (record.Speed != null)
                {
                    last.Speed = record.Speed;
                }
                if (record.EnhancedSpeed != null)
                {
                    last.EnhancedSpeed = record.EnhancedSpeed;
                }
                if (record.Temperature != null)
                {
                    last.Temperature = record.Temperature;
                }
            }
            else
            {
                result.Add(record);
            }
        }
        return result;
    }
}
