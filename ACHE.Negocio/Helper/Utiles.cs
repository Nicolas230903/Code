using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ACHE.Negocio.Helper
{
    public static class Utiles
    {
        public static object ConvertToObjectWithoutPropertiesWithNullValues<T>(this T objectToTransform)
        {
            var type = objectToTransform.GetType();
            var returnClass = new ExpandoObject() as IDictionary<string, object>;
            foreach (var propertyInfo in type.GetProperties())
            {
                var value = propertyInfo.GetValue(objectToTransform);
                var valueIsNotAString = !(value is string && !string.IsNullOrWhiteSpace(value.ToString()));
                if (valueIsNotAString && value != null)
                {
                    returnClass.Add(propertyInfo.Name, value);
                }
            }
            return returnClass;
        }

    }
}
