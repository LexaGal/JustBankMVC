using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;

namespace BankModel.Serialization
{
    public class Serializer
    {
        public static bool LoadDataTable<T>(List<T> list, string file) where T : class
        {
            string ext = Path.GetExtension(file);

            if (ext == ".xml")
            {
                GetXml(list, file);
                return true;
            }
            if (ext == ".json")
            {
                GetJson(list, file);
                return true;
            }
            return false;
        }
        
        public static bool GetXml<T>(List<T> list, string file) where T : class
        {
            if (String.IsNullOrEmpty(file))
            {
                file = @"B:/" + typeof(T).Name + DateTime.Now.ToFileTime() + ".xml";
            }
            
            XmlSerializer ser = new XmlSerializer(typeof (List<T>));
            try
            {
                TextWriter writer = new StreamWriter(file);
                ser.Serialize(writer, list);
                writer.Close();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public static bool GetJson<T>(List<T> list, string file) where T : class
        {
            if (String.IsNullOrEmpty(file))
            {
                file = @"B:/" + typeof(T).Name + DateTime.Now.ToFileTime() + ".json";
            }

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof (List<T>));
            try
            {
                StreamWriter writer = new StreamWriter(file);
                ser.WriteObject(writer.BaseStream, list);
                writer.Close();
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        //    private Dictionary<string, Dictionary<string, object>> DataTableToDictionary(DataTable table, string id)
        //    {
        //        IEnumerable<DataColumn> columns = table.Columns.Cast<DataColumn>().Where(c => c.ColumnName != id);

        //        Dictionary<string, Dictionary<string, object>> dictionary =
        //            new Dictionary<string, Dictionary<string, object>>();

        //        foreach (DataRow row in table.Rows)
        //        {
        //            IList<DataColumn> dataColumns = columns as IList<DataColumn> ?? columns.ToList();

        //            dictionary.Add(row[id].ToString(), dataColumns.ToDictionary(c => c.ColumnName, c => row[c.ColumnName]));
        //        }
        //        return dictionary;
        //    }

        //    private static DataTable ListToDataTable<TU>(IList<TU> data)
        //    {
        //        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof (TU));

        //        DataTable table = new DataTable();

        //        foreach (PropertyDescriptor prop in properties)
        //        {
        //            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //        }

        //        foreach (TU item in data)
        //        {
        //            DataRow row = table.NewRow();

        //            foreach (PropertyDescriptor property in properties)
        //            {
        //                row[property.Name] = property.GetValue(item) ?? DBNull.Value;
        //            }

        //            table.Rows.Add(row);
        //        }
        //        return table;
        //    }
        //}
    }
}