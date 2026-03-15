using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

namespace SalesAndDistributionSystem.Services.Common
{
    public static class Extensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        public static T ToObject<T>(this DataRow row) where T: new()
        {
            return CreateItemFromRow<T>(row, typeof(T).GetProperties());
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {
                    if(row.Table.Columns.Contains(property.Name))
                    {
                        if (row[property.Name] == DBNull.Value)
                            property.SetValue(item, null, null);
                        else
                        {
                            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                            {
                                //nullable
                                object convertedValue = null;
                                try
                                {
                                    convertedValue = System.Convert.ChangeType(row[property.Name], Nullable.GetUnderlyingType(property.PropertyType));
                                }
                                catch (Exception ex)
                                {
                                }
                                property.SetValue(item, convertedValue, null);
                            }
                            else
                            {
                                var val = Convert.ChangeType(row[property.Name], property.PropertyType);
                                property.SetValue(item, val, null);
                            }
                        }
                    }
                }
            }
            return item;
        }
    }
}
