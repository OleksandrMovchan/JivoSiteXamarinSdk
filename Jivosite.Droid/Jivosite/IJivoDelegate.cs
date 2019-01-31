namespace Jivosite.Jivosite
{
    public interface IJivoDelegate
    {
        void OnEvent(string name, string data);
    }
}