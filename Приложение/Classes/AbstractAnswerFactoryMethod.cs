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
        public static List<Type> ListOfTypesFactory = new List<Type> { typeof(InitA), typeof(InitAO), typeof(InitAP) }.Concat(AbstractAnswer.ListOfTypesAnswer).ToList();
        public abstract AbstractAnswer Answer();
    }

    public class InitA : AbstractAnswerFactoryMethod
    {
        public override AbstractAnswer Answer()
        {
            return new DAnswer();
        }
    }

    public class InitAP : AbstractAnswerFactoryMethod
    {
        public override AbstractAnswer Answer()
        {
            return new DAnswerPair();
        }
    }

    public class InitAO : AbstractAnswerFactoryMethod
    {
        public override AbstractAnswer Answer()
        {
            return new DAnswerOne();
        }
    }
}
