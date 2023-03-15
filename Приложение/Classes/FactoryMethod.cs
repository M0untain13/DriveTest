using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Приложение
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
        DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenResults(_directory);
        }
    }

    public class InitOT : IFactoryMethod
    {
        DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenTest(_directory);
        }
    }

    public class InitOTFE : IFactoryMethod
    {
        DirectoryInfo _directory;
        public DirectoryInfo SetDirectory { set { _directory = value; } }
        public IDriveTestWindow Window()
        {
            return new OpenTestForEdit(_directory);
        }
    }
}
