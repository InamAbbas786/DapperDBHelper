using System;
using System.Collections.Generic;
using DapperDBHelper;
namespace AppTest
{
    class Program
    {
       static DBHelpers helpers = new DBHelpers("Data Source=DESKTOP-E6KVH5F;Initial Catalog=SIGS_PMS_Test;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=123"); 
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var mapItems = new List<MapItem>()
        {
            new MapItem(typeof(object), DataRetriveTypeEnum.FirstOrDefault, "Object1"),
            new MapItem(typeof(object), DataRetriveTypeEnum.List, "Object2")
        };

            var response =  helpers.ExecuteQueryMultiple("Select TOp 1 * from [dbo].[CATEGORY] ; Select * from[dbo].[CATEGORY]",null, mapItems);

            var object1 = response.Data.Object1;
            var listOfObject2 = ((List<dynamic>)response.Data.Object2);
        }
    }
}
