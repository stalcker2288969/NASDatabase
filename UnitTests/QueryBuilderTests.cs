using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using NASDataBaseAPI.Interfaces;
using NASDataBaseAPI.SmartSearchSettings;
using NASDataBaseAPI.Data;           // For SearchType
using NASDataBaseAPI.Client;       // For QueryBuilder

// --- Mocks ---
public class MockTypeOfData_QueryBuilderTests : TypeOfData
{
    public Func<string, bool> CanConvertFunc { get; set; } = (s) => true;
    public override string Name { get; } = "MockTypeQB";
    public override bool CanConvert(string value) => CanConvertFunc(value);
    public override bool Equal(string value1, string value2) => string.Equals(value1, value2, StringComparison.Ordinal);
    public override bool More(string value1, string value2) => string.Compare(value1, value2, StringComparison.Ordinal) > 0;
    public override bool Less(string value1, string value2) => string.Compare(value1, value2, StringComparison.Ordinal) < 0;
    public override bool Multiple(string value1, string value2) => false; // Basic mock
    public override bool Contains(string text, string pattern) => text?.Contains(pattern) ?? false;
    public override Type GetDataType() => typeof(string);
    public override TypeOfElement GetTypeOfElement() => TypeOfElement.Text;
    public override object ConvertToType(string value) => value;
    public override string ConvertToString(object value) => value?.ToString() ?? "";
}

public class MockColumn_QueryBuilderTests : AColumn
{
    public MockColumn_QueryBuilderTests(string name, TypeOfData typeOfData) 
    { 
        Name = name; 
        TypeOfData = typeOfData ?? throw new ArgumentNullException(nameof(typeOfData));
    }
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
    public override int GetCounts() => 0;
    public override ItemData[] GetDatas() => Array.Empty<ItemData>();
    public override void Init(string name, TypeOfData dataType, uint Offset) { Name = name; TypeOfData = dataType; }
}

public class MockSearch : ISearch
{
    public Func<AColumn, AColumn, SearchParameters, List<int>> SearchIDFunc { get; set; }
    public List<int> SearchID(AColumn aColumnParams, AColumn In, SearchParameters searchParameters)
    {
        return SearchIDFunc?.Invoke(aColumnParams, In, searchParameters) ?? new List<int>();
    }
}

// --- Test Class ---
public class QueryBuilderTests : IDisposable
{
    private readonly MockColumn_QueryBuilderTests _mockColumnDef;
    private readonly MockColumn_QueryBuilderTests _mockColumnData;

    public QueryBuilderTests()
    {
        var mockType = new MockTypeOfData_QueryBuilderTests();
        _mockColumnDef = new MockColumn_QueryBuilderTests("TestColDef", mockType);
        _mockColumnData = new MockColumn_QueryBuilderTests("TestColData", mockType);
    }

    public void Dispose()
    {
        SearchFactory.ClearTestingStrategies();
    }

    [Fact]
    public void Constructor_NullColumnDefinition_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("columnDefinition", () => new QueryBuilder(null, _mockColumnData));
    }

    [Fact]
    public void Constructor_NullColumnToSearchInData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("columnToSearchInData", () => new QueryBuilder(_mockColumnDef, null));
    }

    [Fact]
    public void Search_NoConditionSet_ThrowsInvalidOperationException()
    {
        var queryBuilder = new QueryBuilder(_mockColumnDef, _mockColumnData);
        Assert.Throws<InvalidOperationException>(() => queryBuilder.Search());
    }

    [Fact]
    public void FluentMethod_CallingSecondFluentMethod_ThrowsInvalidOperationException()
    {
        var queryBuilder = new QueryBuilder(_mockColumnDef, _mockColumnData);
        queryBuilder.IsEqualTo("value1");
        Assert.Throws<InvalidOperationException>(() => queryBuilder.IsGreaterThan("value2"));
    }

    // --- Tests for individual fluent methods ---

    private delegate QueryBuilder FluentMethodAction(QueryBuilder qb, string value);
    private delegate QueryBuilder FluentMethodActionEnumerable(QueryBuilder qb, IEnumerable<string> values);
     private delegate QueryBuilder FluentMethodActionStringList(QueryBuilder qb, string commaSeparatedValues);

    public static IEnumerable<object[]> SingleValueFluentMethodsTestData()
    {
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsEqualTo(val)), "testValEQ", SearchType.Equally };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsNotEqualTo(val)), "testValNEQ", SearchType.NotEqually };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsGreaterThan(val)), "testValGT", SearchType.More };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsGreaterThanOrEqualTo(val)), "testValGTE", SearchType.MoreOrEqually };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsLessThan(val)), "testValLT", SearchType.Less };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsLessThanOrEqualTo(val)), "testValLTE", SearchType.LessOrEqually };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.StartsWith(val)), "testValSW", SearchType.StartWith };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.EndsWith(val)), "testValEW", SearchType.StopWith };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsMultipleOf(val)), "testValMult", SearchType.Multiple };
        yield return new object[] { (FluentMethodAction)((qb, val) => qb.IsNotMultipleOf(val)), "testValNMult", SearchType.NotMultiple };
        // Contains is special (uses ByRange) - tested separately
    }

    [Theory]
    [MemberData(nameof(SingleValueFluentMethodsTestData))]
    public void SingleValueFluentMethods_And_Search_CallsSearcherWithCorrectParameters(FluentMethodAction action, string queryValue, SearchType expectedSearchType)
    {
        var queryBuilder = new QueryBuilder(_mockColumnDef, _mockColumnData);
        var mockSearch = new MockSearch();
        var expectedResult = new List<int> { 1, 2, 3 };
        bool searcherCalled = false;

        mockSearch.SearchIDFunc = (colDef, colData, searchParams) =>
        {
            searcherCalled = true;
            Assert.Same(_mockColumnDef, colDef);
            Assert.Same(_mockColumnData, colData);
            Assert.Equal(queryValue, searchParams.Query);
            Assert.Equal(expectedSearchType, searchParams.SearchType);
            Assert.Single(searchParams.QueryValues);
            Assert.Equal(queryValue, searchParams.QueryValues[0]);
            return expectedResult;
        };
        SearchFactory.RegisterSearchStrategyForTesting(expectedSearchType, () => mockSearch);

        action(queryBuilder, queryValue); // Call the specific fluent method
        var actualResult = queryBuilder.Search();

        Assert.True(searcherCalled, $"SmartSearcher was not called for {expectedSearchType}.");
        Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public void Contains_And_Search_CallsSearcherWithCorrectParameters()
    {
        var queryBuilder = new QueryBuilder(_mockColumnDef, _mockColumnData);
        var mockSearch = new MockSearch();
        var expectedResult = new List<int> { 8, 9 };
        string queryValue = "substring";
        bool searcherCalled = false;

        mockSearch.SearchIDFunc = (colDef, colData, searchParams) =>
        {
            searcherCalled = true;
            Assert.Equal(queryValue, searchParams.Query);
            Assert.Equal(SearchType.ByRange, searchParams.SearchType); // Contains maps to ByRange
            Assert.Single(searchParams.QueryValues);
            Assert.Equal(queryValue, searchParams.QueryValues[0]);
            return expectedResult;
        };
        SearchFactory.RegisterSearchStrategyForTesting(SearchType.ByRange, () => mockSearch);
        
        queryBuilder.Contains(queryValue);
        var actualResult = queryBuilder.Search();

        Assert.True(searcherCalled, "SmartSearcher was not called for Contains.");
        Assert.Equal(expectedResult, actualResult);
    }
    
    public static IEnumerable<object[]> ListValueFluentMethodsTestData()
    {
        yield return new object[] { (FluentMethodActionEnumerable)((qb, val) => qb.IsIn(val)), new List<string> { "val1", "val2" }, "val1,val2", SearchType.ByRange };
        yield return new object[] { (FluentMethodActionEnumerable)((qb, val) => qb.IsNotIn(val)), new List<string> { "valX", "valY" }, "valX,valY", SearchType.NotInRange };
    }

    [Theory]
    [MemberData(nameof(ListValueFluentMethodsTestData))]
    public void ListValueFluentMethods_IEnumerable_And_Search_CallsSearcherWithCorrectParameters(
        FluentMethodActionEnumerable action, List<string> queryValuesList, string expectedFullQuery, SearchType expectedSearchType)
    {
        var queryBuilder = new QueryBuilder(_mockColumnDef, _mockColumnData);
        var mockSearch = new MockSearch();
        var expectedResult = new List<int> { 6, 7 };
        bool searcherCalled = false;

        mockSearch.SearchIDFunc = (colDef, colData, searchParams) =>
        {
            searcherCalled = true;
            Assert.Equal(expectedFullQuery, searchParams.Query);
            Assert.Equal(expectedSearchType, searchParams.SearchType);
            Assert.Equal(queryValuesList.Count, searchParams.QueryValues.Count);
            for(int i=0; i< queryValuesList.Count; i++)
            {
                Assert.Equal(queryValuesList[i], searchParams.QueryValues[i]);
            }
            return expectedResult;
        };
        SearchFactory.RegisterSearchStrategyForTesting(expectedSearchType, () => mockSearch);

        action(queryBuilder, queryValuesList);
        var actualResult = queryBuilder.Search();

        Assert.True(searcherCalled, $"SmartSearcher was not called for {expectedSearchType} with IEnumerable.");
        Assert.Equal(expectedResult, actualResult);
    }

    public static IEnumerable<object[]> StringListValueFluentMethodsTestData()
    {
        yield return new object[] { (FluentMethodActionStringList)((qb, val) => qb.IsIn(val)), "val1,val2,val3", new List<string> { "val1", "val2", "val3" }, SearchType.ByRange };
        yield return new object[] { (FluentMethodActionStringList)((qb, val) => qb.IsNotIn(val)), "valX,valY,valZ", new List<string> { "valX", "valY", "valZ" }, SearchType.NotInRange };
    }

    [Theory]
    [MemberData(nameof(StringListValueFluentMethodsTestData))]
    public void StringListValueFluentMethods_String_And_Search_CallsSearcherWithCorrectParameters(
        FluentMethodActionStringList action, string queryValue, List<string> expectedQueryValues, SearchType expectedSearchType)
    {
        var queryBuilder = new QueryBuilder(_mockColumnDef, _mockColumnData);
        var mockSearch = new MockSearch();
        var expectedResult = new List<int> { 4, 5 };
        bool searcherCalled = false;

        mockSearch.SearchIDFunc = (colDef, colData, searchParams) =>
        {
            searcherCalled = true;
            Assert.Equal(queryValue, searchParams.Query);
            Assert.Equal(expectedSearchType, searchParams.SearchType);
            Assert.Equal(expectedQueryValues.Count, searchParams.QueryValues.Count);
            for(int i=0; i< expectedQueryValues.Count; i++)
            {
                Assert.Equal(expectedQueryValues[i], searchParams.QueryValues[i]);
            }
            return expectedResult;
        };
        SearchFactory.RegisterSearchStrategyForTesting(expectedSearchType, () => mockSearch);
        
        action(queryBuilder, queryValue);
        var actualResult = queryBuilder.Search();

        Assert.True(searcherCalled, $"SmartSearcher was not called for {expectedSearchType} with string list.");
        Assert.Equal(expectedResult, actualResult);
    }
}
