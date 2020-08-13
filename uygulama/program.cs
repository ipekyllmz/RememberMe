using uygulama.Models.Context;

namespace uygulama
{
    public class program
    {
        static void Main(string[] args)
        {
            DatabaseContext Veri = new DatabaseContext();
            Veri.Database.Create();
        }
    }
}