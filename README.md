!Это просто школьная работа так, что не воспринимайте ее всерьёз!
Это исходный код моей библиотеки для создания своей базы данных.  
Вы можете вкратце ознакомиться со всем что находится внутри через [ДОКУМЕНТАЦИЮ.](https://docs.google.com/document/d/1ekIoIGqzT_iCoHpOobplHOc7ajQwUDz_D_OkgWtXOpo/edit?usp=sharing)
____
Пример создания базы:
```C#
  static void Main(string[] args)
  {
  
              var key = SimpleEncryptor.GenerateRandomKey(128);//Генерируем ключ кодирования 
  
              var settings = new DataBaseSettings("TestBase", "D:\\", key, 10000, 0, 2, 1, false);//Настройки базы
  
              DataBase dataBase = DataBaseManager.CreateDataBase("D\\",settings); //создаем проект с базой
                                                                                  
              dataBase.AddData(new string[] { "Tom", "11" }); //Добавляем элемент в базу {Имя} и {возраст}
  
              Console.WriteLine(dataBase.PrintBase());//Выводим базу в консоль
  }
```
____
  Пример загрузки:
```c#
  static void Main(string[] args)
  {
             
              DataBase dataBase = DataBaseManager.LoadDB("D\\TestBase", 1); //подгружаем базу ( первый кластер) 
                                                                                  
              dataBase.AddData(new string[] { "Tom2", "15" }); //Добавляем элемент в базу {Имя} и {возраст}
  
              Console.WriteLine(dataBase.PrintBase());//Выводим базу в консоль
  }
```
___
  Пример работы с индексатором базы(можно и по другому): 
```c#
  static void Main(string[] args)
  {
            //Загрузка базы и/или действия над ней...
            int id = dataBase["Name"].FindeID("Tom1"); //обращаемся к столбцу c именем Name и ищем там id по данным (Tom1)             
  }
``` 
