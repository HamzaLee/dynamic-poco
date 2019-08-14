using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicPoco
{
    public class DynamicPocoType
    {
        private readonly string _assemblyName;
        private readonly string _moduleName;
        private readonly string _typeName;
        private readonly List<Tuple<string, string, Type>> _properties;
        private TypeBuilder _typeBuilder;
        private Type _type;

        public DynamicPocoType(string assemblyName, string moduleName, string typeName)
        {
            _assemblyName = assemblyName;
            _moduleName = moduleName;
            _typeName = typeName;
            _properties = new List<Tuple<string, string, Type>>();
        }

        private void CreateTypeBuilder()
        {
            var name = new AssemblyName(_assemblyName);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(_moduleName);
            _typeBuilder = moduleBuilder.DefineType(_typeName, TypeAttributes.Public);

        }

        public void AddProperty(string fieldName, string propertyName, Type propertyType)
        {
            _properties.Add(new Tuple<string, string, Type>(fieldName, propertyName, propertyType));
        }
        public void AddProperty<T>(string fieldName, string propertyName)
        {
            AddProperty(fieldName, propertyName, typeof(T));
        }

        public void AddProperty(string propertyName, Type propertyType)
        {
            AddProperty("_" + propertyName.ToLower(), propertyName, propertyType);
        }

        public void AddProperty<T>(string propertyName)
        {
            AddProperty(propertyName, typeof(T));
        }
        public void Build()
        {
            CreateTypeBuilder();

            foreach (var tuple in _properties)
            {
                var fieldName = tuple.Item1;
                var propertyName = tuple.Item2;
                var propertyType = tuple.Item3;
                var fieldBuilder = _typeBuilder.DefineField(fieldName, propertyType, FieldAttributes.Private);
                var propertyBuilder = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);


                const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;


                SetPropertyGetter(propertyName, propertyType, methodAttributes, fieldBuilder, propertyBuilder);
                SetPropertySetter(propertyName, propertyType, methodAttributes, fieldBuilder, propertyBuilder);
            }

            _type = _typeBuilder.CreateType();
        }


        private void SetPropertyGetter(string propertyName, Type propertyType, MethodAttributes methodAttributes,
            FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            var propertyGetMethodBuilder = _typeBuilder.DefineMethod("get_" + propertyName,
                methodAttributes,
                propertyType,
                Type.EmptyTypes);

            var propertyGetMethodIlGenerator = propertyGetMethodBuilder.GetILGenerator();

            propertyGetMethodIlGenerator.Emit(OpCodes.Ldarg_0);
            propertyGetMethodIlGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
            propertyGetMethodIlGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(propertyGetMethodBuilder);
        }
        private void SetPropertySetter(string propertyName, Type propertyType, MethodAttributes methodAttributes,
            FieldBuilder fieldBuilder, PropertyBuilder propertyBuilder)
        {
            var propertySetMethodBuilder =
                _typeBuilder.DefineMethod("set_" + propertyName,
                    methodAttributes,
                    null,
                    new[] { propertyType });

            var propertySetMethodIlGenerator = propertySetMethodBuilder.GetILGenerator();

            propertySetMethodIlGenerator.Emit(OpCodes.Ldarg_0);
            propertySetMethodIlGenerator.Emit(OpCodes.Ldarg_1);
            propertySetMethodIlGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            propertySetMethodIlGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(propertySetMethodBuilder);
        }


        public void SetProperty(object instance, string propertyName, object value)
        {
            if (_type == null)
            {
                //TODO improve exception message
                throw new ArgumentNullException("Build before");
            }
            _type.InvokeMember(propertyName, BindingFlags.SetProperty,
                null, instance, new[] { value });
        }

        public object CreateInstance()
        {
            if (_type == null)
            {
                //TODO improve exception message
                throw new ArgumentNullException("Build before");
            }
            return Activator.CreateInstance(_type);
        }

    }
}
