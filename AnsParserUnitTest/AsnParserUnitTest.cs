using AsnParser;

namespace AnsParserUnitTest;
using System;
using System.IO;
using System.Linq;
using Xunit;

public class AnsParserTest : IDisposable
{
    private const string TestFilePath = "test_data.txt";
    private const string HeaderKeyWord = "HDR";
    private const string ContentKeyWord = "LINE";

    [Fact]
    public async Task ParseAnsFrom_ValidFile_ParsesCorrectly()
    {
        var dataParser = new AsnParserService(HeaderKeyWord, ContentKeyWord);
        
        CreateTestFile(TestFilePath, "HDR  TRSP117   6874453I",
            "LINE P000001661 9781473663800  12",
            "LINE P000001661 9781473667273  2",
            "LINE P000001661 9781473665798  1");

        var boxCollection = await dataParser.ParseAnsLoadToDbFrom(TestFilePath);

        Assert.Single(boxCollection);
        Assert.Equal(3, boxCollection.First().Contents.Count);
        var box = boxCollection.First();
        var boxContent = box.Contents.First();
        Assert.Equal("TRSP117", box.SupplierIdentifier);
        Assert.Equal("6874453I", box.CartonBoxInIdentifier);
        Assert.Equal("P000001661", boxContent.PoNumber);
        Assert.Equal("9781473663800", boxContent.Isbn);
        Assert.Equal(12, boxContent.Quantity);
    }
    
    [Fact]
    public async Task ParseAnsFrom_EmptyFile_ReturnsEmptyCollection()
    {
        var dataParser = new AsnParserService(HeaderKeyWord, ContentKeyWord);
        
        CreateTestFile(TestFilePath, "");

        var boxCollection = await dataParser.ParseAnsLoadToDbFrom(TestFilePath);

        Assert.Equal(0, boxCollection.Count);
    }

    [Fact]
    public void ParseAnsHeaderLine_ValidLine_ReturnsBoxWithCorrectValues()
    {
        var dataParser = new AsnParserService(HeaderKeyWord, ContentKeyWord);
        var line = "HDR TRSP117 6874453I";

        var result = dataParser.ParseAnsHeaderLine(line);

        Assert.Equal("TRSP117", result.SupplierIdentifier);
        Assert.Equal("6874453I", result.CartonBoxInIdentifier);
    }

    [Fact]
    public void ParseContentLine_ValidLine_ReturnsContentWithCorrectValues()
    {
        var dataParser = new AsnParserService(HeaderKeyWord, ContentKeyWord);
        var line = "LINE P000001661 9781473663800 12";

        var result = dataParser.ParseContentLine(line);

        Assert.Equal("P000001661", result.PoNumber);
        Assert.Equal("9781473663800", result.Isbn);
        Assert.Equal(12, result.Quantity);
    }
    
    //we should also test Save method, but its implementation varies depending on whether or not service uses EF
    //we may also more test cases as the complexity increases

    public void Dispose()
    {
        if (File.Exists(TestFilePath))
        {
            File.Delete(TestFilePath);
        }
    }

    private static void CreateTestFile(string filePath, params string[] lines)
    {
        File.WriteAllLines(filePath, lines);
    }
}
