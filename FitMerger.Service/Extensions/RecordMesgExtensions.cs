namespace FitMerger.Service.Extensions;

public static class RecordMesgExtensions
{
    public static RecordMesg AddFieldsFrom(this RecordMesg baseMesg, RecordMesg toAdd)
    {
        foreach (var field in toAdd.Fields)
        {
            var num = field.Num;
            if (!baseMesg.HasField(num))
            {
                baseMesg.SetFieldValue(num, toAdd.GetFieldValue(num));
            }
        }
        return baseMesg;
    }
}
