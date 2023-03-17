using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Приложение.Classes;

namespace Приложение.Classes
{
    public interface IAnswerFactoryMethod
    {
        public IAnswer Answer();
    }

    public class InitA : IAnswerFactoryMethod
    {
        public IAnswer Answer()
        {
            return new DAnswer();
        }
    }

    public class InitAP : IAnswerFactoryMethod
    {
        public IAnswer Answer()
        {
            return new DAnswerPair();
        }
    }

    public class InitAO : IAnswerFactoryMethod
    {
        public IAnswer Answer()
        {
            return new DAnswerOne();
        }
    }
}
