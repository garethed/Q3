namespace Q3Server.Interfaces
{
    public interface IObjectGetter<T>
    {
        T Get(string id);
    }
}
