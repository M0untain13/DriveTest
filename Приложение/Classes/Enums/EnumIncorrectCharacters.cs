using System.Collections.Generic;

namespace Приложение.Classes.Enums
{
    public static class EnumIncorrectCharacters
    {
        public static List<char> characters = new()
        {
            '\\', '/', ':', '*', '?', '"', '<', '>', '|'
        };
    }
}
