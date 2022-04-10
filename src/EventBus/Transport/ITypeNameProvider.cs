using System;

namespace EventBus
{
    public interface ITypeNameProvider
    {
        string GetTypeName(Type type);
    }
}
