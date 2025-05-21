using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.SmartSearchSettings;
using NASDataBaseAPI.Data; // For SearchType

// --- Mocks (defined in the same file for simplicity) ---

public class MockTypeOfData_StrategiesTests : TypeOfData
{
    public Func<string, string, bool> EqualFunc { get; set; } = (s1, s2) => s1 == s2;
    public Func<string, string, bool> MoreFunc { get; set; } = (s1, s2) => String.Compare(s1, s2, StringComparison.Ordinal) > 0;
    public Func<string, string, bool> LessFunc { get; set; } = (s1, s2) => String.Compare(s1, s2, StringComparison.Ordinal) < 0;
    public Func<string, string, bool> MultipleFunc { get; set; } = (val, param) => false; // val is p.Data, param is query
    public Func<string, string, bool> ContainsFunc { get; set; } = (text, pattern) => text.Contains(pattern);
    
    private string _name = "MockType";
    public override string Name { get => _name; }
    public void SetName(string name) { _name = name; } 

    public override bool CanConvert(string value) => true; 
    public override bool Equal(string value1, string value2) => EqualFunc(value1, value2);
    public override bool More(string value1, string value2) => MoreFunc(value1, value2);
    public override bool Less(string value1, string value2) => LessFunc(value1, value2);
    public override bool Multiple(string value1, string value2) => MultipleFunc(value1, value2);
    public override bool Contains(string text, string pattern) => ContainsFunc(text, pattern);
    public override Type GetDataType() => typeof(string);
    public override TypeOfElement GetTypeOfElement() => _name == "Text" ? TypeOfElement.Text : TypeOfElement.Number;
    public override object ConvertToType(string value) => value;
    public override string ConvertToString(object value) => value?.ToString() ?? "";
}

public class MockColumn_StrategiesTests : AColumn
{
    public Func<ItemData[]> GetDatasFunc { get; set; } = () => Array.Empty<ItemData>();

    public MockColumn_StrategiesTests(string name, TypeOfData typeOfData) 
    { 
        Name = name; 
        TypeOfData = typeOfData ?? throw new ArgumentNullException(nameof(typeOfData));
    }
    
    public override ItemData[] GetDatas() => GetDatasFunc();

    public override bool SetDatas(ItemData[] datas) => throw new NotImplementedException();
    public override bool SetDataByID(ItemData newData) => throw new NotImplementedException();
    public override int FindID(string data) => throw new NotImplementedException();
    public override int[] FindIDs(string data) => throw new NotImplementedException();
    public override string FindDataByID(int id) => throw new NotImplementedException();
    public override bool Push(string data, uint CountBoxes) => throw new NotImplementedException();
    public override bool Push(string data, int ID) => throw new NotImplementedException();
    public override bool Pop(string data) => throw new NotImplementedException();
    public override bool TryPopByIDAndData(ItemData itemData) => throw new NotImplementedException();
    public override void PopByID(int id) => throw new NotImplementedException();
    public override void ChangType(TypeOfData type) => throw new NotImplementedException();
    public override void ClearBoxes() => throw new NotImplementedException();
    public override int GetCounts() => GetDatasFunc().Length;
    public override void Init(string name, TypeOfData dataType, uint Offset) { Name = name; TypeOfData = dataType; }
}

// --- Test Class ---
public class SearchStrategiesTests
{
    private readonly MockTypeOfData_StrategiesTests _mockTypeOfData;
    private readonly MockColumn_StrategiesTests _mockColumnParams; // Used to pass TypeOfData to strategies
    private readonly MockColumn_StrategiesTests _mockInColumn;     // Used to provide data for searching

    public SearchStrategiesTests()
    {
        _mockTypeOfData = new MockTypeOfData_StrategiesTests();
        // _mockColumnParams provides the TypeOfData that the strategy will use for its logic (e.g., .Equal, .More)
        _mockColumnParams = new MockColumn_StrategiesTests("paramsCol", _mockTypeOfData);
        // _mockInColumn provides the actual data items to be searched. Its TypeOfData is also set for completeness,
        // though strategies primarily use the TypeOfData from _mockColumnParams.
        _mockInColumn = new MockColumn_StrategiesTests("inCol", _mockTypeOfData); 
    }

    [Fact]
    public void Equally_SearchID_ReturnsCorrectIDs()
    {
        var strategy = new Equally();
        _mockInColumn.GetDatasFunc = () => new ItemData[]
        {
            new ItemData(1, "apple"), new ItemData(2, "banana"), new ItemData(3, "apple")
        };
        // For Equally, the first parameter to EqualFunc is the query, second is item.Data
        _mockTypeOfData.EqualFunc = (queryParam, itemDataVal) => queryParam == itemDataVal; 
        
        var searchParameters = new SearchParameters("apple", SearchType.Equally);
        var expectedIDs = new List<int> { 1, 3 };

        var actualIDs = strategy.SearchID(_mockColumnParams, _mockInColumn, searchParameters);

        Assert.Equal(expectedIDs.OrderBy(id => id), actualIDs.OrderBy(id => id));
    }

    [Fact]
    public void MoreSettings_SearchID_ReturnsCorrectIDs()
    {
        var strategy = new MoreSettings();
        _mockInColumn.GetDatasFunc = () => new ItemData[]
        {
            new ItemData(1, "10"), new ItemData(2, "20"), new ItemData(3, "5"), new ItemData(4, "15")
        };
        // For MoreSettings, first param to MoreFunc is query, second is item.Data
        _mockTypeOfData.MoreFunc = (queryParam, itemDataVal) => int.Parse(itemDataVal) > int.Parse(queryParam);

        var searchParameters = new SearchParameters("15", SearchType.More);
        var expectedIDs = new List<int> { 2 }; // Only "20" is > "15"

        var actualIDs = strategy.SearchID(_mockColumnParams, _mockInColumn, searchParameters);

        Assert.Equal(expectedIDs.OrderBy(id => id), actualIDs.OrderBy(id => id));
    }

    [Fact]
    public void ByRange_SearchID_TextType_SingleMatch_ReturnsCorrectIDs()
    {
        var strategy = new ByRange();
        _mockTypeOfData.SetName("Text"); // Set TypeOfData name to "Text" for this test
        _mockInColumn.GetDatasFunc = () => new ItemData[]
        {
            new ItemData(1, "apple"), new ItemData(2, "banana"), new ItemData(3, "cherry")
        };
        // ByRange for Text uses QueryValues.Contains(item.Data.ToString()) - no specific func from TypeOfData needed here
        // other than TypeOfData.Name == "Text"

        var searchParameters = new SearchParameters("banana", SearchType.ByRange); // QueryValues will be ["banana"]
        var expectedIDs = new List<int> { 2 };

        var actualIDs = strategy.SearchID(_mockColumnParams, _mockInColumn, searchParameters);

        Assert.Equal(expectedIDs.OrderBy(id => id), actualIDs.OrderBy(id => id));
    }

    [Fact]
    public void ByRange_SearchID_NumericType_MultipleMatches_ReturnsCorrectIDs()
    {
        var strategy = new ByRange();
        _mockTypeOfData.SetName("Int"); // Set TypeOfData name to non-"Text"
        _mockInColumn.GetDatasFunc = () => new ItemData[]
        {
            new ItemData(1, "10"), new ItemData(2, "20"), new ItemData(3, "30"), new ItemData(4, "40")
        };
        // For ByRange non-Text, first param to EqualFunc is valFromQuery, second is p.Data
        _mockTypeOfData.EqualFunc = (valFromQuery, itemDataVal) => valFromQuery == itemDataVal;

        var searchParameters = new SearchParameters("10,30,50", SearchType.ByRange); // QueryValues will be ["10", "30", "50"]
        var expectedIDs = new List<int> { 1, 3 };

        var actualIDs = strategy.SearchID(_mockColumnParams, _mockInColumn, searchParameters);

        Assert.Equal(expectedIDs.OrderBy(id => id), actualIDs.OrderBy(id => id));
    }

    [Fact]
    public void StartWith_SearchID_ReturnsCorrectIDs()
    {
        var strategy = new StartWith();
        _mockInColumn.GetDatasFunc = () => new ItemData[]
        {
            new ItemData(1, "applepie"), new ItemData(2, "apricot"), new ItemData(3, "banana")
        };
        // StartWith directly uses string.StartsWith, no TypeOfData func needed for the core logic

        var searchParameters = new SearchParameters("ap", SearchType.StartWith);
        var expectedIDs = new List<int> { 1, 2 };

        var actualIDs = strategy.SearchID(_mockColumnParams, _mockInColumn, searchParameters);

        Assert.Equal(expectedIDs.OrderBy(id => id), actualIDs.OrderBy(id => id));
    }

    [Fact]
    public void Multiple_SearchID_ReturnsCorrectIDs()
    {
        var strategy = new Multiple();
        _mockInColumn.GetDatasFunc = () => new ItemData[]
        {
            new ItemData(1, "10"), new ItemData(2, "7"), new ItemData(3, "25"), new ItemData(4, "12")
        };
        // For Multiple, first param to MultipleFunc is p.Data, second is query
        _mockTypeOfData.MultipleFunc = (itemDataVal, queryParam) => 
            int.TryParse(itemDataVal, out int val) && 
            int.TryParse(queryParam, out int param) && 
            param != 0 && val % param == 0;

        var searchParameters = new SearchParameters("5", SearchType.Multiple);
        var expectedIDs = new List<int> { 1, 3 }; // 10 and 25 are multiples of 5

        var actualIDs = strategy.SearchID(_mockColumnParams, _mockInColumn, searchParameters);

        Assert.Equal(expectedIDs.OrderBy(id => id), actualIDs.OrderBy(id => id));
    }
}
