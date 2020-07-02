using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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