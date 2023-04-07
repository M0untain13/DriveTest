using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Приложение.Classes.Models;

namespace Приложение.Classes.FactoryMethods
{
    public abstract class AbstractFactoryMethod
    {
        /// <summary>
        /// Коллекция известных типов для сериализации
        /// </summary>
        public static List<Type> listOfTypesFactory = new List<Type>
        {
            typeof(FactoryOpen), 
            typeof(FactorySelectiveOne), 
            typeof(FactorySelectiveMultiple), 
            typeof(FactoryMatchingSearch), 
            typeof(FactoryDataInput)
        }.Concat(AbstractAnswer.listOfTypesAnswer).ToList();
        public abstract AbstractDResultsAnswer Result();
        public abstract AbstractAnswer Answer();
    }

    public class FactoryOpen : AbstractFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerOpen();
        }
        public override AbstractAnswer Answer()
        {
            return new DAnswer();
        }
    }

    public class FactorySelectiveOne : AbstractFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerSelectiveOne();
        }
        public override AbstractAnswer Answer()
        {
            return new DAnswerOne();
        }
    }
    public class FactorySelectiveMultiple : AbstractFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerSelectiveMultiple();
        }
        public override AbstractAnswer Answer()
        {
            return new DAnswer();
        }
    }
    public class FactoryMatchingSearch : AbstractFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerMatchingSearch();
        }
        public override AbstractAnswer Answer()
        {
            return new DAnswerPair();
        }
    }
    public class FactoryDataInput : AbstractFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerDataInput();
        }
        public override AbstractAnswer Answer()
        {
            return new DAnswer();
        }
    }
}
