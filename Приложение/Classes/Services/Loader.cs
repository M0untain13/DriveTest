using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Приложение.Classes.Models;

namespace Приложение.Classes.Services
{
    /// <summary>
    /// Класс в который выносится логика загрузки и сохранения файлов
    /// </summary>
    internal static class Loader
    {
        private static DataContractSerializerSettings _settings;

        private static DataContractSerializer _serializer;

        /// <summary>
        /// Сохранить тест
        /// </summary>
        /// <param name="test"> Экземпляр теста </param>
        /// <param name="path"> Путь к папке теста </param>
        public static void SaveTest(DTest test, string path)
        {
            _settings = new DataContractSerializerSettings()
            {
                KnownTypes = DTest.listOfTypes,
                PreserveObjectReferences = true,
            };
            _serializer = new DataContractSerializer(typeof(DTest), _settings);

            Directory.CreateDirectory(path);
            path += "\\Test.XML";
            var fs = new FileStream(path, FileMode.Create);
            var reader = XmlWriter.Create(fs);
            _serializer.WriteObject(reader, test);

            _serializer = null;
            _settings = null;
            reader.Close();
            fs.Close();
        }

        /// <summary>
        /// Загрузить тест
        /// </summary>
        /// <param name="path"> Путь к папке теста </param>
        /// <returns> Экземпляр теста </returns>
        /// <exception cref="System.Exception"> Загрузка null или объекта типа не DTest </exception>
        public static DTest LoadTest(string path)
        {
            _settings = new DataContractSerializerSettings()
            {
                KnownTypes = DTest.listOfTypes,
                PreserveObjectReferences = true,
            };
            _serializer = new DataContractSerializer(typeof(DTest), _settings);

            path += "\\Test.XML";
            FileStream fs = new FileStream(path, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            if (_serializer.ReadObject(reader, true) is not DTest test) throw new System.Exception("Объект не является файлом теста");

            _serializer = null;
            _settings = null;
            reader.Close();
            fs.Close();
            return test;
        }

        /// <summary>
        /// Сохранить результат
        /// </summary>
        /// <param name="result"> Объект результата </param>
        /// <param name="path"> Путь к папке результатов </param>
        /// <param name="name"> Название файла </param>
        public static void SaveResult(DResult result, string path, string name)
        {
            _settings = new DataContractSerializerSettings()
            {
                KnownTypes = DResult.listOfTypes,
                PreserveObjectReferences = true,
            };
            _serializer = new DataContractSerializer(typeof(DResult), _settings);

            Directory.CreateDirectory(path);
            path += "\\" + name + ".XML";
            var fs = new FileStream(path, FileMode.Create);
            var reader = XmlWriter.Create(fs);
            _serializer.WriteObject(reader, result);

            _serializer = null;
            _settings = null;
            reader.Close();
            fs.Close();
        }

        /// <summary>
        /// Загрузить результат
        /// </summary>
        /// <param name="path"> Путь к папке результатов </param>
        /// <param name="name"> Название файла </param>
        /// <returns>  Объект результата </returns>
        /// <exception cref="System.Exception"> Загрузка null или объекта типа не DResult </exception>
        public static DResult LoadResult(string path, string name)
        {
            _settings = new DataContractSerializerSettings()
            {
                KnownTypes = DResult.listOfTypes,
                PreserveObjectReferences = true,
            };
            _serializer = new DataContractSerializer(typeof(DResult), _settings);

            path += "\\" + name + ".XML";
            FileStream fs = new FileStream(path, FileMode.Open);
            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            if (_serializer.ReadObject(reader, true) is not DResult result) throw new System.Exception("Объект не является файлом результата");

            _serializer = null;
            _settings = null;
            reader.Close();
            fs.Close();
            return result;
        }
    }
}
