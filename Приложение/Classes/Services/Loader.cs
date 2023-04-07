using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Navigation;
using System.Xml;
using Приложение.Classes.Enums;
using Приложение.Classes.Models;

namespace Приложение.Classes.Services
{
    /// <summary>
    /// Класс, в который выносится логика загрузки и сохранения файлов
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
            if (!CheckTest(test, out var message)) throw new Exception($"Тест не пригоден для сохранения! Причина: {message}");

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
        /// Проверка теста на ограничения
        /// </summary>
        /// <param name="test"></param>
        /// <param name="message"></param>
        /// <returns> Возможно ли сохранение теста </returns>
        public static bool CheckTest(DTest test, out string message)
        {
            if (test == null)
            {
                message = "Ссылка не указывает на экземпляр";
                return false;
            }
            if (test.name == string.Empty)
            {
                message = "Название теста не может быть пустым";
                return false;
            }

            if (test.quests.Count == 0)
            {
                message = "Тест должен содержать хотя бы один вопрос";
                return false;
            }
            foreach (var quest in test.quests)
            {
                var knownAnswers = new HashSet<string>();

                if (quest.name == string.Empty)
                {
                    message = "Какой-то вопрос не содержит описания";
                    return false;
                }

                if (quest.Type is not (EnumTypeQuestion.OPEN_ANSWER or EnumTypeQuestion.DATA_INPUT) && quest.Answers.Count < 2)
                {
                    message = "Какой-то вопрос c выбором ответов или соответствием содержит менее двух ответов";
                    return false;
                }

                var correctCheck = false; //TODO: нужно ещё сделать проверку на повторы

                if (quest.Type != EnumTypeQuestion.DATA_INPUT) foreach (var answer in quest.Answers)
                {
                    if(answer.IsCorrect || answer is DAnswerPair) correctCheck = true;
                    switch (answer)
                    {
                        case DAnswer concreteAnswer:
                            if (concreteAnswer.Answer1 == string.Empty)
                            {
                                message = "Какой-то ответ не содержит описания";
                                return false;
                            }
                            if (knownAnswers.Contains(answer.Answer1))
                            {
                                message = "Какой-то ответ дублируется";
                                return false;
                            }
                            knownAnswers.Add(answer.Answer1);
                            break;
                        case DAnswerOne concreteAnswer:
                            if (concreteAnswer.Answer1 == string.Empty)
                            {
                                message = "Какой-то ответ не содержит описания";
                                return false;
                            }
                            if (knownAnswers.Contains(answer.Answer1))
                            {
                                message = "Какой-то ответ дублируется";
                                return false;
                            }
                            knownAnswers.Add(answer.Answer1);
                            break;
                        case DAnswerPair concreteAnswer:
                            if (concreteAnswer.Answer1 == string.Empty || concreteAnswer.Answer2 == string.Empty)
                            {
                                message = "Какое-то соответствие не содержит описания";
                                return false;
                            }
                            if (knownAnswers.Contains(answer.Answer1))
                            {
                                message = "Какой-то ответ дублируется";
                                return false;
                            }
                            knownAnswers.Add(answer.Answer1);
                            if (knownAnswers.Contains(answer.Answer2))
                            {
                                message = "Какой-то ответ дублируется";
                                return false;
                            }
                            knownAnswers.Add(answer.Answer2);
                            break;
                    }
                }
                else
                {
                    correctCheck = true;
                }

                if (!correctCheck)
                {
                    message = "В каком-то вопросе отсутствует пометка правильного ответа";
                    return false;
                }

            }
            message = "Тест можно сохранить";
            return true;
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
