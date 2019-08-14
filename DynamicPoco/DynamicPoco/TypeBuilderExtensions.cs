using System;
using System.Reflection.Emit;

namespace DynamicPoco
{
    public static class TypeBuilderExtensions
    {
        public static Type CreateType(this TypeBuilder type)
        {
#if NETSTANDARD2_0
            return type.CreateTypeInfo().AsType();

#else
            return type.CreateType();

#endif
        }
    }
}