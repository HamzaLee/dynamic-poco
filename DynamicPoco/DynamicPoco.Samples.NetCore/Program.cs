using System;

namespace DynamicPoco.Samples.NetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var dynamicPocoInstanceCreator = new DynamicPocoInstanceCreator("MyDynamicAssembly", "MyDynamicModule", "DynamicPerson");
            dynamicPocoInstanceCreator.AddProperty("Name", "John Doe");
            dynamicPocoInstanceCreator.AddProperty("Age", 33);
            var instance = dynamicPocoInstanceCreator.CreateInstance();
            Console.ReadKey(true);
        }
    }
}
