using System.IO;
using Приложение.Windows.InterWindows;

namespace Приложение.Classes.FactoryMethods
{
    /// <summary>
    /// Да это же фабричный метод!!!
    /// </summary>
    public interface IWindowFactoryMethod
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

    public class FactoryExitFromTest : IWindowFactoryMethod
    {
        public DirectoryInfo SetDirectory { set { } }
        public IDriveTestWindow Window()
        {
            return new ExitFromTest();
        }
    }

    public class FactoryCreateTest : IWindowFactoryMethod
    {
        public DirectoryInfo SetDirectory { set { } }
        public IDriveTestWindow Window()
        {
            return new CreateTest();
        }
    }

    public class FactoryExitFromEdit : IWindowFactoryMethod
    {
        public DirectoryInfo SetDirectory { set { } }
        public IDriveTestWindow Window()
        {
            return new ExitFromEdit();
        }
    }

    public class FactoryOpenResults : IWindowFactoryMethod
    {
        private DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenResults(_directory);
        }
    }

    public class FactoryOpenTest : IWindowFactoryMethod
    {
        private DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenTest(_directory);
        }
    }

    public class FactoryOpenTestForEdit : IWindowFactoryMethod
    {
        private DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenTestForEdit(_directory);
        }
    }
}
