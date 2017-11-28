using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NovAtelLogReader.Publishers
{
    abstract class AbstractGenericPublisher : IPublisher
    {
        abstract public void Close();
        abstract public void Open();
        abstract public void Publish<T>(List<T> value);

        protected void PublishList<T>(IEnumerable<object> value)
        {
            Publish(value.Cast<T>().ToList());
        }

        public void Publish(Type type, IEnumerable<object> value)
        {
            GetType()
                .GetMethod("PublishList", BindingFlags.NonPublic | BindingFlags.Instance)
                .MakeGenericMethod(type)
                .Invoke(this, new object[] { value });
        }
    }
}
