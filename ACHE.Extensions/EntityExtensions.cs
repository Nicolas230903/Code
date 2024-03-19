using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data.Objects.DataClasses;

namespace ACHE.Extensions
{
    /// <summary>
    /// Extension methods for all kind of Lists implementing the IList&lt;T&gt; interface
    /// </summary>
	public static class EntityExtensions
    {

		public static T Clone<T>(this T source) {
			var dcs = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
			using (var ms = new System.IO.MemoryStream()) {
				dcs.WriteObject(ms, source);
				ms.Seek(0, System.IO.SeekOrigin.Begin);
				return (T)dcs.ReadObject(ms);
			}
		}
    }
}