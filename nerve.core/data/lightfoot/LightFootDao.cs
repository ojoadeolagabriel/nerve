using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace nerve.core.data.lightfoot
{
    /// <summary>
    /// LightFootDao Class.
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    public class LightFootDao<TDto>
        where TDto : class, IDto
    {
        #region Params
        public string DataStore { get; set; }
        public DataStoreType DataStoreTypeImpl { get; set; }

        public LightFootDao(string dataStore, DataStoreType dataStoreType = DataStoreType.MsSql)
        {
            DataStore = dataStore;
            DataStoreTypeImpl = dataStoreType;
        }

        private static T SafeReader<T>(SqlDataReader reader, string key, T errorValue = default(T), bool throwOnError = false)
        {
            try
            {
                return (T)Convert.ChangeType(reader[key], typeof(T));
            }
            catch (Exception exception)
            {
                if (!throwOnError)
                    return errorValue;
                throw new Exception(string.Format("[ErrorReadingDbField] {0}", key), exception);
            }
        }
        #endregion

        #region Trigger happy methods

        private readonly object _lock = new object();

        /// <summary>
        /// Execure Sp that returns a multiple records
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters">
        /// <remarks>do not use @ in specifying parameters, this is automatically done</remarks></param>
        /// <param name="propertiesToInclude"></param>
        /// <param name="useCache"></param>
        /// <param name="cacheIntervalInMinutes"></param>
        /// <returns></returns>
        public TDto ExecuteUniqueSp(string spName, Dictionary<string, string> parameters = null, List<String> propertiesToInclude = null, bool useCache = false, int cacheIntervalInMinutes = 10)
        {
            try
            {
                var cacheKey = string.Format("{0}_{1}", DataStore, spName);
                if (useCache)
                {
                    var cachedData = MemoryCacheHelper.GetCachedData(cacheKey);
                    if (cachedData != null)
                        return (TDto)cachedData;
                }

                var instance = Activator.CreateInstance<TDto>();
                var properties = instance.GetType().GetProperties();
                var connStr = ConfigurationManager.ConnectionStrings[DataStore].ConnectionString;

                using (var conn = new SqlConnection(connStr))
                {
                    var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
                    conn.Open();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(string.Format("@{0}", parameter.Key), parameter.Value);
                    }

                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        return null;

                    while (reader.Read())
                    {
                        instance = Activator.CreateInstance<TDto>();
                        foreach (var propertyInfo in properties)
                        {
                            try
                            {
                                if (propertyInfo.PropertyType.GetInterfaces().Contains(typeof(IDto)))
                                {
                                    var attr =
                                       propertyInfo.GetCustomAttributes(typeof(LightFootAttribute), false)
                                           .FirstOrDefault() as LightFootAttribute;

                                    if (attr == null)
                                        continue;

                                    var mapsToColumn = attr.MapsTo;
                                    var data = SafeReader<object>(reader, mapsToColumn);

                                    var lfoot = Type.GetType("TechQuest.Framework.Dao.LightFoot.LightFootDao`1");
                                    var stringArg1 = propertyInfo.PropertyType;

                                    var lFootInstanceMaker = lfoot.MakeGenericType(stringArg1);
                                    var lFootInstance = Activator.CreateInstance(lFootInstanceMaker, DataStore,
                                        DataStoreTypeImpl);

                                    var method = lFootInstance.GetType().GetMethod("ExecuteUniqueSp");
                                    var resInstance = method.Invoke(lFootInstance, new object[] { attr.MapsToSp, new Dictionary<string, string> { { attr.ForeignKeyId, data.ToString() } }, null, false, 0 });

                                    propertyInfo.SetValue(instance, resInstance, null);
                                }
                                else
                                {
                                    if (propertiesToInclude != null && propertiesToInclude.Contains(propertyInfo.Name))
                                        continue;

                                    var attr =
                                        propertyInfo.GetCustomAttributes(typeof(LightFootAttribute), false)
                                            .FirstOrDefault() as LightFootAttribute;
                                    var data = SafeReader<object>(reader,
                                        attr != null ? attr.ColumnNameOverride : propertyInfo.Name);

                                    var setDataVal = Convert.ChangeType(data, propertyInfo.PropertyType);
                                    propertyInfo.SetValue(instance, setDataVal, null);
                                }
                            }
                            catch (Exception exception)
                            {
                                var msg = exception.Message;
                            }
                        }

                        if (useCache)
                            MemoryCacheHelper.SetCachedData(cacheKey, _lock, cacheIntervalInMinutes, instance);

                        return instance;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }
            return null;
        }

        /// <summary>
        /// Execure Sp that returns a multiple records
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalarSp(string spName, Dictionary<string, string> parameters = null)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataStore].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
                    conn.Open();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(string.Format("@{0}", parameter.Key), parameter.Value);
                    }

                    var reader = cmd.ExecuteScalar();
                    return reader;
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }

            return null;
        }

        /// <summary>
        /// Execure Sp that returns a single record
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <param name="propertiesToInclude"></param>
        /// <param name="cacheIntervalInMinutes"></param>
        /// <returns></returns>
        public List<TDto> ExecuteSp(string spName, Dictionary<string, string> parameters = null, List<String> propertiesToInclude = null, int cacheIntervalInMinutes = 10, bool useCache = false)
        {
            try
            {
                var cacheKey = string.Format("{0}_{1}", DataStore, spName);
                if (useCache)
                {
                    var cachedData = MemoryCacheHelper.GetCachedData(cacheKey);

                    if (cachedData != null)
                        return (List<TDto>)cachedData;
                }

                var result = new List<TDto>();
                var instance = Activator.CreateInstance<TDto>();
                var properties = instance.GetType().GetProperties();
                var connStr = ConfigurationManager.ConnectionStrings[DataStore].ConnectionString;

                using (var conn = new SqlConnection(connStr))
                {
                    var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
                    conn.Open();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(string.Format("@{0}", parameter.Key), parameter.Value);
                    }

                    var reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        return null;

                    while (reader.Read())
                    {
                        instance = Activator.CreateInstance<TDto>();
                        foreach (var propertyInfo in properties)
                        {
                            try
                            {
                                if (propertiesToInclude != null && propertiesToInclude.Contains(propertyInfo.Name))
                                    continue;

                                var attr = propertyInfo.GetCustomAttributes(typeof(LightFootAttribute), false).FirstOrDefault() as LightFootAttribute;
                                var data = SafeReader<object>(reader, attr != null ? attr.ColumnNameOverride : propertyInfo.Name);

                                var setDataVal = Convert.ChangeType(data, propertyInfo.PropertyType);
                                propertyInfo.SetValue(instance, setDataVal, null);
                            }
                            catch (Exception exception)
                            {
                                var msg = exception.Message;
                            }
                        }
                        result.Add(instance);
                    }

                    if (useCache)
                        MemoryCacheHelper.SetCachedData(cacheKey, _lock, cacheIntervalInMinutes, instance);
                    return result;
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }
            return null;
        }

        /// <summary>
        /// Execute non query
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuerySp(string spName, Dictionary<string, string> parameters = null)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings[DataStore].ConnectionString;
                using (var conn = new SqlConnection(connStr))
                {
                    var cmd = new SqlCommand(spName, conn) { CommandType = CommandType.StoredProcedure };
                    conn.Open();

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                            cmd.Parameters.AddWithValue(string.Format("@{0}", parameter.Key), parameter.Value);
                    }

                    var result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
            catch (Exception exception)
            {
                var msg = exception.Message;
            }

            return 0;
        }
        #endregion
    }
}
