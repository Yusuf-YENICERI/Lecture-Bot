using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DersBotu
{
    class HataYazdir
    {
        public static void Function(Exception e)
        {
            Console.WriteLine("\aHata var: \nHatayı atan uygulama veya nesne: " + e.Source);
            Console.WriteLine("Hatayı atan metod: " + e.TargetSite);
            Console.WriteLine("Hata mesajı: " + e.Message);
            Console.WriteLine("Inner Exception: " + e.InnerException);
            Console.WriteLine("Hata için yardımcı kaynak: " + e.HelpLink);
        }
    }
}
