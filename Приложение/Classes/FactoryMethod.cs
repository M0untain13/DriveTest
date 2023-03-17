using System.IO;
using Приложение.Windows;

namespace Приложение.Classes
{
    /// <summary>
    /// Да это же фабричный метод!!!
    /// </summary>
    public interface IFactoryMethod
    {
        /// <summary>
        /// При необходимости, устанавливаем путь к папке, где лежат тесты.
        /// </summary>
        public DirectoryInfo SetDirectory { set; }
        /// <summary>
        /// Возвращает экземпляр окна
        /// </summary>
        /// <returns></returns>
        public IDriveTestWindow Window();
    }

    public class InitCT : IFactoryMethod
    {
        public DirectoryInfo SetDirectory { set { } }
        public IDriveTestWindow Window()
        {
            return new CreateTest();
        }
    }

    public class InitEFE : IFactoryMethod
    {
        public DirectoryInfo SetDirectory { set { } }
        public IDriveTestWindow Window()
        {
            return new ExitFromEdit();
        }
    }

    public class InitOR : IFactoryMethod
    {
        private DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenResults(_directory);
        }
    }

    public class InitOT : IFactoryMethod
    {
        private DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenTest(_directory);
        }
    }

    public class InitOTFE : IFactoryMethod
    {
        private DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenTestForEdit(_directory);
        }
    }
}
