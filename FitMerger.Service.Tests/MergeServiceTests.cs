namespace FitMerger.Service.Tests;

[TestClass]
public class MergeServiceTests
{
    public string filesDir = @"E:\Lukasz\Downloads\";
    public string meilanTestFile = "Kolna_Meilan_Cadence_.fit";
    public string stravaTestFile = "Kolna.fit";
    [TestMethod]
    public void TestMethod1()
    {
        new MergeService($"{filesDir}{stravaTestFile}").DecodeFitFile();
    }
}
