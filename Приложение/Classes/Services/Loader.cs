using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace Приложение.Classes.Services
{
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
            DTest test = _serializer.ReadObject(reader, true) as DTest;

            _serializer = null;
            _settings = null;
            reader.Close();
            fs.Close();
            return test;
        }

        public static void SaveResult(DResult result, string path)
        {

        }
    }
}
