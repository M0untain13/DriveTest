using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Приложение.Classes;

namespace Приложение.Classes
{
    public abstract class AbstractAnswerFactoryMethod
    {
        public static List<Type> ListOfTypesFactory = new List<Type> { typeof(FactoryAnswer), typeof(FactoryAnswerOne), typeof(FactoryAnswerPair) }.Concat(AbstractAnswer.ListOfTypesAnswer).ToList();
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
