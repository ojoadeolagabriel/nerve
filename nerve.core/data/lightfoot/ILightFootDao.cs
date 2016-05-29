using System.Collections.Generic;

namespace nerve.core.data.lightfoot
{
    public interface ILightFootDao<TIiDto>
    {
        /// <summary>
        /// Execute Update Sp
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        object ExecuteScalarSp(string spName, Dictionary<string, string> parameters = null);

        string DataStore { get; set; }
        DataStoreType DataStoreTypeImpl { get; set; }

        /// <summary>
        /// Execure Sp that returns a multiple records
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <param name="propertiesToInclude"></param>
        /// <param name="useCache"></param>
        /// <param name="cacheIntervalInMinutes"></param>
        /// <returns></returns>
        TIiDto ExecuteUniqueSp(string spName, Dictionary<string, string> parameters = null, List<string> propertiesToInclude = null, bool useCache = false, int cacheIntervalInMinutes = 10);

        /// <summary>
        /// Execure Sp that returns a single record
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <param name="propertiesToInclude"></param>
        /// <param name="cacheIntervalInMinutes"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        List<TIiDto> ExecuteSp(string spName, Dictionary<string, string> parameters = null, List<string> propertiesToInclude = null, int cacheIntervalInMinutes = 10, bool useCache = false);

        /// <summary>
        /// Execute non query
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        int ExecuteNonQuerySp(string spName, Dictionary<string, string> parameters = null);
    }
}