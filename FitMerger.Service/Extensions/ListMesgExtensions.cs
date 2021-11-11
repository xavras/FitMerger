namespace FitMerger.Service.Extensions;

public static class ListMesgExtensions
{
    public static IList<Mesg> AddToTimestamps(this IList<Mesg> mesgs, uint seconds)
    {
        foreach (var mesg in mesgs)
        {
            var oldTimestamp = mesg.GetFieldValue(RecordMesg.FieldDefNum.Timestamp);
            if (oldTimestamp != null)
            {
                mesg.SetFieldValue(RecordMesg.FieldDefNum.Timestamp,
                    (uint)oldTimestamp + seconds);
            }
        }
        return mesgs;
    }
}
