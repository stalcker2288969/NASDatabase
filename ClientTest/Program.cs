using NASDataBaseAPI.Client.Utilities;
using NASDataBaseAPI.Client;


Console.Write("Name: ");

Client client = new Client(Console.ReadLine(), "NAS");
client.ConnectTo<ClientCommandsWorker>("127.0.0.1", 6666);

for(; ;)
{
    Console.WriteLine("---------------------------");
    Console.Write("name >>");
    var d1 = Console.ReadLine();
    Console.Write("old >>");
    var d2 = Console.ReadLine();

    try
    {
        client.AddData(d1, d2);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
}


Console.ReadLine();