using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicPoco.Samples.NetFramework
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
