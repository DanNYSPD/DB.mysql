using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarenisoft.DB.Mysql
{
    public static class DataRecordExtensions
    {
        /*
        public static bool HasColumn(this MySqlDataReader dr, string columnName) {
           DataTable dataTable= dr.GetSchemaTable();
            return dataTable.Columns.Contains(columnName);
        }
        */

        public static bool HasColumn(this IDataRecord drecord, string columnName)
        {
            for (int i = 0; i < drecord.FieldCount; i++)
            {
                if (drecord.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool IsDBNull(this IDataRecord drecord, string columnName)
        {
            for (int i = 0; i < drecord.FieldCount; i++)
            {
                if (drecord.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return drecord.IsDBNull(i);
            }
            //si no se encuentra dar true
            return true;
        }


        
    }
}
