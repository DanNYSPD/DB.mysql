﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarenisoft.DB.Mysql
{
    /// <summary>
    /// Author: Daniel J Hernandez Fco
    /// 
    /// This Class is created to increase the development productivity when EntityFramwork is not available.
    /// 
    /// </summary>
    public class DAO
    {
        private string Update(string tableName, List<MySqlParameter> setParamaters, List<MySqlParameter> whereParameters)
        {
            var sql = String.Format("UPDATE {0} SET  ", tableName);
            setParamaters.ForEach(p => {
                string name = "";
                if (p.ParameterName.StartsWith("@"))
                {
                    name = p.ParameterName.Substring(1);
                }
                else if (p.ParameterName.StartsWith("?"))
                {
                    name = p.ParameterName.Substring(1);
                }
                //ya que tengo el nombre uedo generar los set
                sql += " " + name + "=" + p.ParameterName;
            });
            sql += "where 1=1";
            whereParameters.ForEach(p =>
            {
                string name = "";
                if (p.ParameterName.StartsWith("@"))
                {
                    name = p.ParameterName.Substring(1);
                }
                else if (p.ParameterName.StartsWith("?"))
                {
                    name = p.ParameterName.Substring(1);
                }
                sql += "and " + name + "=" + p.ParameterName;
            });

            return sql;
        }


        
        public string Insert(string tableName, List<MySqlParameter> insertParams)
        {
            var sql = String.Format("INSERT INTO {0} SET  (", tableName);
            insertParams.ForEach(p => {
                string name = "";
                if (p.ParameterName.StartsWith("@"))
                {
                    name = p.ParameterName.Substring(1);
                }
                else if (p.ParameterName.StartsWith("?"))
                {
                    name = p.ParameterName.Substring(1);
                }
                //ya que tengo el nombre uedo generar los set
                sql += ", " + name;
            });
            sql += ") values(";
            insertParams.ConvertAll(p =>
            {
                string name = "";
                if (p.ParameterName.StartsWith("@"))
                {
                    name = p.ParameterName.Substring(1);
                }
                else if (p.ParameterName.StartsWith("?"))
                {
                    name = p.ParameterName.Substring(1);
                }
                return name;
            });
            sql += ");";
            return sql;
        }

        public List<T> query<T>(string sql, List<MySqlParameter> parameters, Func<MySqlDataReader, T> actionWithResult)
        {
            using (var conn = this.Connect())
            {

                MySqlCommand command = new MySqlCommand(sql, conn);
                //I add all the passed paramteers
                parameters.ForEach(p => command.Parameters.Add(p));


                MySqlDataReader rdr = command.ExecuteReader();

                List<T> lst = new List<T>();
                while (rdr.Read())
                {
                    lst.Add(actionWithResult(rdr));
                }
                rdr.Close();
                return lst;
            }
        }

        public T querySingle<T>(string sql, Func<MySqlDataReader, T> actionWithResult, List<MySqlParameter> parameters)
        {
            using (var conn = this.Connect())
            {

                MySqlCommand command = new MySqlCommand(sql, conn);
                //I add all the passed paramteers
                parameters.ForEach(p => command.Parameters.Add(p));
                MySqlDataReader rdr = command.ExecuteReader();
                T single = default(T);
                while (rdr.Read())
                {
                    single = (actionWithResult(rdr));
                }
                rdr.Close();
                return single;
            }
        }
        public bool exist<T>(string sql, List<MySqlParameter> parameters=null)
        {
            using (var conn = this.Connect())
            {

                MySqlCommand command = new MySqlCommand(sql, conn);
                //I add all the passed paramteers
                if (parameters != null) { 
                    parameters.ForEach(p => command.Parameters.Add(p));
                }
                MySqlDataReader rdr = command.ExecuteReader();
                
                bool res = false;
                while (rdr.Read())
                {
                    res= rdr.HasRows;
                }
                rdr.Close();
                return res;
            }
        }
        public T querySingle<T>(string sql, List<MySqlParameter> parameters)
        {
            using (var conn = this.Connect())
            {

                MySqlCommand command = new MySqlCommand(sql, conn);
                //I add all the passed paramteers
                parameters.ForEach(p => command.Parameters.Add(p));
                MySqlDataReader rd = command.ExecuteReader();
                T single = default(T);
                //single= CreateInstance(typeof(T));}
                single = (T)Activator.CreateInstance(typeof(T), new object[] { });
                Type type = typeof(T);
                var properties = type.GetProperties().ToList();

                while (rd.Read())
                {

                    foreach (var p in properties)
                    {
                        //var propertyType=p.GetType();
                        var propertyType = p.PropertyType;
                        if (!rd.HasColumn(p.Name))
                        {
                            continue;
                        }
                        if (rd.IsDBNull(p.Name))
                        {
                            continue;
                        }
                        switch (propertyType.FullName)
                        {
                            case "System.Int32":
                                p.SetValue(single, rd.GetUInt32(p.Name), null);
                                break;
                            case "System.String":
                                p.SetValue(single, rd.GetString(p.Name));
                                break;
                            case "System.Decimal":
                                p.SetValue(single, rd.GetDecimal(p.Name), null);
                                break;
                            case "System.DateTime":
                                p.SetValue(single, rd.GetDateTime(p.Name));
                                break;
                            default:
                                break;
                        }

                    }
                }
                rd.Close();
                return single;
            }
        }

        public void Insert<T>(T obj)
        {
            using (var conn = this.Connect())
            {
                
                Type type = typeof(T);
                var properties = type.GetProperties().ToList();

                var sql = string.Format("INSERT INTO {0}",type.Name);
                var s=properties.Select(x => x.Name);
                sql+="("+String.Join(",", s)+")";
                sql += "values (@"+String.Join(",@",s)+");";

                Console.WriteLine(sql);

                MySqlCommand command = new MySqlCommand(sql, conn);


                foreach (var p in properties)
                {
                    command.Parameters.Add(new MySqlParameter("@"+p.Name,MySqlDbType.));
                }
            }
        }
        private MySqlDbType resolveType() {

        }
        public MySqlConnection Connect()
        {
            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=root;" +
                "pwd=123456;database=poe_app_piloto";

            try
            {
                conn = new MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
