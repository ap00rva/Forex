using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForexAPITester
{
    public class Library
    {
        public const string UpBar = "Up Bar";
        public const string DownBar = "Down Bar";

        public enum Relations
        {
            Both = 0,
            UpOnly = 1,
            DownOnly = 2
        }
    }
}
