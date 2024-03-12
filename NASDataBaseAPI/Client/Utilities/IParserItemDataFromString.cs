using NASDatabase.Server.Data;

namespace NASDatabase.Client.Utilities
{
    public interface IParserItemDataFromString
    {
        ItemData GetItemData(string text);
        string ParsItemData(ItemData itemData);      
    }
}
