using System.Collections.Generic;

namespace Приложение.Classes
{
    public static class EnumIncorrectCharacters
    {
        public static List<char> characters = new()
        {
            '\\', '/', ':', '*', '?', '"', '<', '>', '|'
        };
    }
}
