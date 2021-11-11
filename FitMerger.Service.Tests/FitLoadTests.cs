namespace FitMerger.Service.Tests;

[TestClass]
public class FitLoadTests
{
    public string filesDir = @"E:\Lukasz\Downloads\";
    public string meilanTestFile = "Kolna_Meilan_Cadence_.fit";
    public string stravaTestFile = "Kolna.fit";
    [TestMethod]
    public void MesgsFromFileWhenFilepathReturnMesgsList()
    {
        var mesgs = FitLoad.MesgsFromFile($"{filesDir}{meilanTestFile}");
        Assert.IsTrue(mesgs.Count > 0);
    }
    [TestMethod]
    public void RecordMesgsFromFileWhenFilepathReturnMesgsList()
    {
        var records = FitLoad.RecordMesgsFromFile($"{filesDir}{stravaTestFile}");
        Assert.IsTrue(records.Count > 0);
    }
    [TestMethod]
    public void RecordMesgsFromFileByTimestampWhenFilepathReturnMesgsList()
    {
        var recordsByTimestamp = FitLoad.RecordMesgsFromFileByTimestamp(
            $"{filesDir}{stravaTestFile}");
        Assert.IsTrue(recordsByTimestamp.Count > 0);
    }
}
