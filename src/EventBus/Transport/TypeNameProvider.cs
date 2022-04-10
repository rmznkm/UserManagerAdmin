namespace EventBus.Transport
{
    public class TypeNameProvider : ITypeNameProvider
    {
        public string GetTypeName(Type type)
        {
            if (type == null)
                return null;
            return type.FullName;
        }
    }
}
