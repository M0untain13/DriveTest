using System;
using System.Collections.Generic;
using System.Linq;

namespace Приложение.Classes
{
    public abstract class AbstractAnswerFactoryMethod
    {
        /// <summary>
        /// Коллекция известных типов для сериализации
        /// </summary>
        public static List<Type> listOfTypesFactory = new List<Type> { typeof(FactoryAnswer), typeof(FactoryAnswerOne), typeof(FactoryAnswerPair) }.Concat(AbstractAnswer.listOfTypesAnswer).ToList();
        public abstract AbstractAnswer Answer();
    }

    public class FactoryAnswer : AbstractAnswerFactoryMethod
    {
        public override AbstractAnswer Answer()
        {
            return new DAnswer();
        }
    }

    public class FactoryAnswerPair : AbstractAnswerFactoryMethod
    {
        public override AbstractAnswer Answer()
        {
            return new DAnswerPair();
        }
    }

    public class FactoryAnswerOne : AbstractAnswerFactoryMethod
    {
        public override AbstractAnswer Answer()
        {
            return new DAnswerOne();
        }
    }
}
