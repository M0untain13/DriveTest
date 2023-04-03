using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Приложение.Classes.Models;

namespace Приложение.Classes.FactoryMethods
{
    public abstract class AbstractResultFactoryMethod
    {
        /// <summary>
        /// Коллекция известных типов для сериализации
        /// </summary>
        public static List<Type> listOfTypesFactory = new List<Type>
        {
            typeof(FactoryAbstractResultsOpen), 
            typeof(FactoryAbstractResultsSelectiveOne), 
            typeof(FactoryAbstractResultsSelectiveMultiple), 
            typeof(FactoryAbstractResultsMatchingSearch), 
            typeof(FactoryAbstractResultsDataInput)
        };
        public abstract AbstractDResultsAnswer Result();
    }

    public class FactoryAbstractResultsOpen : AbstractResultFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerOpen();
        }
    }

    public class FactoryAbstractResultsSelectiveOne : AbstractResultFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerSelectiveOne();
        }
    }
    public class FactoryAbstractResultsSelectiveMultiple : AbstractResultFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerSelectiveMultiple();
        }
    }
    public class FactoryAbstractResultsMatchingSearch : AbstractResultFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerMatchingSearch();
        }
    }
    public class FactoryAbstractResultsDataInput : AbstractResultFactoryMethod
    {
        public override AbstractDResultsAnswer Result()
        {
            return new DResultsAnswerDataInput();
        }
    }
}
