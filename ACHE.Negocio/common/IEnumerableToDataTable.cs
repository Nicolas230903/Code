using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Reflection;
using System.ComponentModel;

//TODO: PASAR A EXTENSIONS
namespace ACHE.Model {
	public static class IEnumerableToDataTable {
		public static DataTable CreateDataTableForPropertiesOfType<T>() {
			DataTable dt = new DataTable();
			PropertyInfo[] piT = typeof(T).GetProperties();

			foreach (PropertyInfo pi in piT) {
				Type propertyType = null;
				if (pi.PropertyType.IsGenericType) {
					propertyType = pi.PropertyType.GetGenericArguments()[0];
				}
				else {
					propertyType = pi.PropertyType;
				}
				DataColumn dc = new DataColumn(pi.Name, propertyType);

				if (pi.CanRead) {
					dt.Columns.Add(dc);
				}
			}

			return dt;
		}

		public static DataTable ToDataTable<T>(IEnumerable<T> items) {
			var table = CreateDataTableForPropertiesOfType<T>();
			PropertyInfo[] piT = typeof(T).GetProperties();

			foreach (var item in items) {
				var dr = table.NewRow();

				for (int property = 0; property < table.Columns.Count; property++) {
					if (piT[property].CanRead) {
						dr[property] = piT[property].GetValue(item, null);
					}
				}

				table.Rows.Add(dr);
			}
			return table;
		}

        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor property = properties[i];
                dt.Columns.Add(property.Name, property.PropertyType);
            }
            object[] values = new object[properties.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = properties[i].GetValue(item);
                }
                dt.Rows.Add(values);
            }
            return dt;
        }
	}
}