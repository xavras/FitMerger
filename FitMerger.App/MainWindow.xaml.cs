namespace FitMerger.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    private IList<Mesg> MeilanMesgs;
    private uint _SecondsToAddToTimestamps = 0;
    public uint SecondsToAddToTimestamps
    {
        get => _SecondsToAddToTimestamps;
        set
        {
            _SecondsToAddToTimestamps = value;
            NotifyPropertyChanged();
        }
    }
    public MainWindow()
    {
        InitializeComponent();
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private ICommand _LoadMeilanFitFileCommand;
    public ICommand LoadMeilanFitFileCommand
    {
        get
        {
            return _LoadMeilanFitFileCommand
                ??= new DelegateCommand(() => LoadMeilanMesgsFromFile());
        }
    }
    private ICommand _AddStravaFitFileCommand;
    public ICommand AddStravaFitFileCommand
    {
        get
        {
            return _AddStravaFitFileCommand
                ??= new DelegateCommand(() => AddStravaFields());
        }
    }
    private ICommand _AddSecondsToTimestampsCommand;
    public ICommand AddSecondsToTimestampsCommand
    {
        get
        {
            return _AddSecondsToTimestampsCommand
                ??= new DelegateCommand(() => AddSecondsToTimestamps());
        }
    }
    public void LoadMeilanMesgsFromFile()
    {
        MeilanMesgs = FitLoad.MesgsFromFile(GetFitFilePathFromDialog());
    }
    private static string GetFitFilePathFromDialog()
    {
        var openFileDialog = new OpenFileDialog()
        {
            InitialDirectory = "E:\\Lukasz\\Downloads",
            Filter = "Fit files (*.fit)|*.fit|All files (*.*)|*.*",
            FilterIndex = 1,
            RestoreDirectory = true
        };
        return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
    }
    public void AddStravaFields()
    {
        var stravaRecords = FitLoad.RecordMesgsFromFileByTimestamp(
            GetFitFilePathFromDialog());
        for (var i = 0; i < MeilanMesgs.Count; i++)
        {
            var meilanMesg = MeilanMesgs[i];
            if (meilanMesg.Name == "Record")
            {
                var meilanRecord = new RecordMesg(meilanMesg);
                if (stravaRecords.ContainsKey(meilanRecord.GetTimestamp().GetTimeStamp()))
                {
                    var stravaRecord =
                        stravaRecords[meilanRecord.GetTimestamp().GetTimeStamp()];
                    meilanRecord.SetPositionLat(stravaRecord.GetPositionLat());
                    meilanRecord.SetPositionLong(stravaRecord.GetPositionLong());
                    meilanRecord.SetAltitude(stravaRecord.GetAltitude());
                    meilanRecord.SetGpsAccuracy(stravaRecord.GetGpsAccuracy());
                }
                MeilanMesgs[i] = meilanRecord;
            }
            if (meilanMesg.Name == "Lap")
            {
                meilanMesg.RemoveField(meilanMesg.GetField(
                    LapMesg.FieldDefNum.TotalCalories));
            }
        }

        var encode = new Encode(ProtocolVersion.V20);
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Fit file|*.fit",
            Title = "Save merged fit file"
        };
        saveFileDialog.ShowDialog();
        if (!string.IsNullOrEmpty(saveFileDialog.FileName))
        {
            var fitDest = new FileStream(saveFileDialog.FileName, FileMode.Create,
                FileAccess.ReadWrite, FileShare.Read);
            encode.Open(fitDest);
            encode.Write(MeilanMesgs);
            encode.Close();
            fitDest.Close();
        }
    }
    public void AddSecondsToTimestamps()
    {
        MeilanMesgs.AddToTimestamps(SecondsToAddToTimestamps);
    }
}
