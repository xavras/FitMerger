namespace FitMerger.Service;

public class MergeService
{
    public IList<FitRecord> Records = new List<FitRecord>();
    public IList<Mesg> Mesgs = new List<Mesg>();
    public string FilePath;
    public MergeService(string filePath)
    {
        FilePath = filePath;
    }
    public IList<FitRecord> DecodeFitFile()
    {
        var msgBroadcaster = new MesgBroadcaster();
        var decoder = new Decode();

        decoder.MesgEvent += msgBroadcaster.OnMesg;
        decoder.MesgDefinitionEvent += msgBroadcaster.OnMesgDefinition;
        decoder.DeveloperFieldDescriptionEvent += OnDeveloperFieldDescriptionEvent;

        msgBroadcaster.MesgDefinitionEvent += OnMesgDefn;
        msgBroadcaster.MesgEvent += OnMesg;
        msgBroadcaster.FileIdMesgEvent += OnFileIDMesg;
        msgBroadcaster.UserProfileMesgEvent += OnUserProfileMesg;
        msgBroadcaster.MonitoringMesgEvent += OnMonitoringMessage;
        msgBroadcaster.DeviceInfoMesgEvent += OnDeviceInfoMessage;
        msgBroadcaster.RecordMesgEvent += OnRecordMessage;

        using var fileStream = System.IO.File.OpenRead(FilePath);
        var status = decoder.IsFIT(fileStream);
        status &= decoder.CheckIntegrity(fileStream);
        if (status)
        {
            try
            {
                Console.WriteLine("####################################### START ######");
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
        return Records;
    }
    private void OnDeveloperFieldDescriptionEvent(object sender,
        DeveloperFieldDescriptionEventArgs args)
    {
        Console.WriteLine("New Developer Field Description");
        Console.WriteLine("   App Id: {0}", args.Description.ApplicationId);
        Console.WriteLine("   App Version: {0}", args.Description.ApplicationVersion);
        Console.WriteLine("   Field Number: {0}", args.Description.FieldDefinitionNumber);
    }
    private void OnRecordMessage(object sender, MesgEventArgs e)
    {
        Console.WriteLine($"Record Handler: Received {e.mesg.Num} Mesg, " +
            $"it has global ID#{e.mesg.Name}");
        var record = new FitRecord((RecordMesg)e.mesg);
        Records.Add(record);
        Console.WriteLine(record);
    }
    static void OnFileIDMesg(object sender, MesgEventArgs e)
    {
        Console.WriteLine("FileIdHandler: Received {1} Mesg with global ID#{0}", e.mesg.Num, e.mesg.Name);
        FileIdMesg myFileId = (FileIdMesg)e.mesg;
        try
        {
            Console.WriteLine("\tType: {0}", myFileId.GetType());
            Console.WriteLine("\tManufacturer: {0}", myFileId.GetManufacturer());
            Console.WriteLine("\tProduct: {0}", myFileId.GetProduct());
            Console.WriteLine("\tSerialNumber {0}", myFileId.GetSerialNumber());
            Console.WriteLine("\tNumber {0}", myFileId.GetNumber());
            Console.WriteLine("\tTimeCreated {0}", myFileId.GetTimeCreated());

            //Make sure properties with sub properties arent null before trying to create objects based on them
            if (myFileId.GetTimeCreated() != null)
            {
                Dynastream.Fit.DateTime dtTime = new(myFileId.GetTimeCreated()
                    .GetTimeStamp());
            }
        }
        catch (FitException exception)
        {
            Console.WriteLine("\tOnFileIDMesg Error {0}", exception.Message);
            Console.WriteLine("\t{0}", exception.InnerException);
        }
    }
    static void OnUserProfileMesg(object sender, MesgEventArgs e)
    {
        Console.WriteLine("UserProfileHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
        UserProfileMesg myUserProfile = (UserProfileMesg)e.mesg;
        string friendlyName;
        try
        {
            try
            {
                friendlyName = myUserProfile.GetFriendlyNameAsString();
            }
            catch (ArgumentNullException)
            {
                //There is no FriendlyName property
                friendlyName = "";
            }
            Console.WriteLine("\tFriendlyName \"{0}\"", friendlyName);
            Console.WriteLine("\tGender {0}", myUserProfile.GetGender().ToString());
            Console.WriteLine("\tAge {0}", myUserProfile.GetAge());
            Console.WriteLine("\tWeight  {0}", myUserProfile.GetWeight());
        }
        catch (FitException exception)
        {
            Console.WriteLine("\tOnUserProfileMesg Error {0}", exception.Message);
            Console.WriteLine("\t{0}", exception.InnerException);
        }
    }

    static void OnDeviceInfoMessage(object sender, MesgEventArgs e)
    {
        Console.WriteLine("DeviceInfoHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
        DeviceInfoMesg myDeviceInfoMessage = (DeviceInfoMesg)e.mesg;
        try
        {
            Console.WriteLine("\tTimestamp  {0}", myDeviceInfoMessage.GetTimestamp());
            Console.WriteLine("\tBattery Status{0}", myDeviceInfoMessage.GetBatteryStatus());
        }
        catch (FitException exception)
        {
            Console.WriteLine("\tOnDeviceInfoMesg Error {0}", exception.Message);
            Console.WriteLine("\t{0}", exception.InnerException);
        }
    }
    private void OnMesg(object sender, MesgEventArgs e)
    {
        Console.WriteLine("OnMesg: Received Mesg with global ID#{0}, its name is {1}", e.mesg.Num, e.mesg.Name);

        int i = 0;
        foreach (Field field in e.mesg.Fields)
        {
            for (int j = 0; j < field.GetNumValues(); j++)
            {
                Console.WriteLine("\tField{0} Index{1} (\"{2}\" Field#{4}) Value: {3} (raw value {5})",
                    i,
                    j,
                    field.GetName(),
                    field.GetValue(j),
                    field.Num,
                    field.GetRawValue(j));
            }

            i++;
        }

        foreach (var devField in e.mesg.DeveloperFields)
        {
            for (int j = 0; j < devField.GetNumValues(); j++)
            {
                Console.WriteLine("\tDeveloper{0} Field#{1} Index{2} (\"{3}\") Value: {4} (raw value {5})",
                    devField.DeveloperDataIndex,
                    devField.Num,
                    j,
                    devField.Name,
                    devField.GetValue(j),
                    devField.GetRawValue(j));
            }
        }
        Mesgs.Add(e.mesg);
    }
    static void OnMonitoringMessage(object sender, MesgEventArgs e)
    {
        Console.WriteLine("MonitoringHandler: Received {1} Mesg, it has global ID#{0}", e.mesg.Num, e.mesg.Name);
        MonitoringMesg myMonitoringMessage = (MonitoringMesg)e.mesg;
        try
        {
            Console.WriteLine("\tTimestamp  {0}", myMonitoringMessage.GetTimestamp());
            Console.WriteLine("\tActivityType {0}", myMonitoringMessage.GetActivityType());
            switch (myMonitoringMessage.GetActivityType()) // Cycles is a dynamic field
            {
                case ActivityType.Walking:
                case ActivityType.Running:
                    Console.WriteLine("\tSteps {0}", myMonitoringMessage.GetSteps());
                    break;
                case ActivityType.Cycling:
                case ActivityType.Swimming:
                    Console.WriteLine("\tStrokes {0}", myMonitoringMessage.GetStrokes());
                    break;
                default:
                    Console.WriteLine("\tCycles {0}", myMonitoringMessage.GetCycles());
                    break;
            }
        }
        catch (FitException exception)
        {
            Console.WriteLine("\tOnDeviceInfoMesg Error {0}", exception.Message);
            Console.WriteLine("\t{0}", exception.InnerException);
        }
    }
    static void OnMesgDefn(object sender, MesgDefinitionEventArgs e)
    {
        Console.WriteLine("OnMesgDef: Received Defn for local message #{0}, global num {1}", e.mesgDef.LocalMesgNum, e.mesgDef.GlobalMesgNum);
        Console.WriteLine("\tIt has {0} fields {1} developer fields and is {2} bytes long",
            e.mesgDef.NumFields,
            e.mesgDef.NumDevFields,
            e.mesgDef.GetMesgSize());
    }
}
