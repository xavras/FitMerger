namespace FitMerger.Service.Extensions;

public static class ListRecordMesgExtensions
{
    public static IList<RecordMesg> MergeFieldsByTimestamp(this IList<RecordMesg> records)
    {
        var result = new List<RecordMesg>();
        foreach (var recordByTimestamp in records.GroupBy(
            x => x.GetTimestamp().GetTimeStamp()).ToList())
        {
            result.Add(recordByTimestamp.ToList().MergeFields());
        }
        return result;
    }
    public static RecordMesg MergeFields(this IList<RecordMesg> records)
    {
        var result = records.First();
        for (var i = 1; i < records.Count; i++)
        {
            result.AddFieldsFrom(records[i]);
        }
        return result;
    }
}
