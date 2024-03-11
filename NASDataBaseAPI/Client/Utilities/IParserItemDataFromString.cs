using NASDataBaseAPI.Server.Data;

namespace NASDataBaseAPI.Client.Utilities
{
    public interface IParserItemDataFromString
    {
        ItemData GetItemData(string text);
        string ParsItemData(ItemData itemData);      
    }
}
