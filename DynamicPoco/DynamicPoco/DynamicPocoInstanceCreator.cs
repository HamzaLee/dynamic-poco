using System.Collections.Generic;

namespace DynamicPoco
{
    public class DynamicPocoInstanceCreator
    {
        private readonly DynamicPocoType _dynamicPocoType;
        private readonly Dictionary<string, object> _propertiesValues;
        public DynamicPocoInstanceCreator(string assemblyName, string moduleName, string typeName)
        {
            _dynamicPocoType = new DynamicPocoType(assemblyName, moduleName, typeName);
            _propertiesValues = new Dictionary<string, object>();
        }

        public void AddProperty<T>(string propertyName, T propertyValue)
        {
            _dynamicPocoType.AddProperty<T>(propertyName);
            _propertiesValues.Add(propertyName, propertyValue);
        }

        public object CreateInstance()
        {
            _dynamicPocoType.Build();
            var instance = _dynamicPocoType.CreateInstance();
            foreach (var pair in _propertiesValues)
            {
                _dynamicPocoType.SetProperty(instance, pair.Key, pair.Value);
            }

            return instance;
        }
    }
}