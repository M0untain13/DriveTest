using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Приложение
{
    public static class EnumIncorrectCharacters
    {
        public static List<char> characters = new()
        {
            '\\', '/', ':', '*', '?', '"', '<', '>', '|'
        };
    }
}
