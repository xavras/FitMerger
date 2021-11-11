namespace FitMerger.Service;

public static class FitLoad
{
    public static void FromFile(string fitFilePath,
        Action<object, MesgEventArgs> onMesg = null,
        Action<object, MesgEventArgs> onRecordMesg = null)
    {
        var msgBroadcaster = new MesgBroadcaster();
        var decoder = new Decode();
        decoder.MesgEvent += msgBroadcaster.OnMesg;
        decoder.MesgDefinitionEvent += msgBroadcaster.OnMesgDefinition;
        if (onMesg != null)
        {
            msgBroadcaster.MesgEvent += new MesgEventHandler(onMesg);
        }
        if (onRecordMesg != null)
        {
            msgBroadcaster.RecordMesgEvent += new MesgEventHandler(onRecordMesg);
        }

        using var fileStream = System.IO.File.OpenRead(fitFilePath);
        var status = decoder.IsFIT(fileStream);
        status &= decoder.CheckIntegrity(fileStream);
        if (status)
        {
            try
            {
                decoder.Read(fileStream);
            }
            catch (FitException ex)
            {
                Console.WriteLine("A FitException occurred when trying to decode " +
                    "the FIT file. Message: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred when trying to decode " +
                    "the FIT file. Message: " + ex.Message);
            }
        }
        else
        {
            try
            {
                Console.WriteLine("Integrity Check Failed");
                if (decoder.InvalidDataSize)
                {
                    Console.WriteLine("Invalid Size Detected, Attempting to decode...");
                    decoder.Read(fileStream);
                }
                else
                {
                    Console.WriteLine("Attempting to decode by skipping the header...");
                    decoder.Read(fileStream, DecodeMode.InvalidHeader);
                }
            }
            catch (FitException ex)
            {
                Console.WriteLine("DecodeDemo caught FitException: " + ex.Message);
            }
        }
        fileStream.Close();
    }

    public static IList<Mesg> MesgsFromFile(string fitFilePath)
    {
        var result = new List<Mesg>();
        void mesgHandler(object sender, MesgEventArgs e)
        {
            result.Add(e.mesg);
        }
        FromFile(fitFilePath, mesgHandler);
        return result;
    }
    public static IList<RecordMesg> RecordMesgsFromFile(string fitFilePath)
    {
        var result = new List<RecordMesg>();
        void recordMesgHandler(object sender, MesgEventArgs e)
        {
            result.Add((RecordMesg)e.mesg);
        }
        FromFile(fitFilePath, onRecordMesg: recordMesgHandler);
        return result;
    }
    public static IDictionary<uint, RecordMesg> RecordMesgsFromFileByTimestamp(
        string fitFilePath)
    {
        return RecordMesgsFromFile(fitFilePath).MergeFieldsByTimestamp()
            .ToDictionary(x => x.GetTimestamp().GetTimeStamp(), x => x);
    }
}
