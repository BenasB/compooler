namespace Compooler.API;

public interface IMappableTo<out T>
{
    T Map();
}
