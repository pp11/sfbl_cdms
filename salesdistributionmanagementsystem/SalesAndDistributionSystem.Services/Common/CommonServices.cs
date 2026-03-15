using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using SalesAndDistributionSystem.Domain.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


//using System.Text.Json;

namespace SalesAndDistributionSystem.Services.Common
{
    public class CommonServices : ICommonServices
    {
        public QueryPattern AddQuery(string query, Dictionary<string, string> parametes)
        {
            QueryPattern queryPattern = new QueryPattern
            {
                Query = query
            };
            queryPattern.Parametes.Add(parametes);
            return queryPattern;
        }
        public string DataTableToJSON(DataTable table)
        {
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return JsonConvert.SerializeObject(parentRow);
        }
        public string DataSetToJSON(DataSet ds)
        {
            ArrayList root = new ArrayList();
            List<Dictionary<string, object>> table;
            Dictionary<string, object> data;

            foreach (DataTable dt in ds.Tables)
            {
                table = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    data = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        data.Add(col.ColumnName, dr[col]);
                    }
                    table.Add(data);
                }
                root.Add(table);
            }

            return JsonConvert.SerializeObject(root);
        }
        public DataRow GetDataRow(string conString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                using OracleConnection obcon = new OracleConnection(conString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                using OracleDataAdapter dataAdapter = new OracleDataAdapter(query, obcon);
                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(item.Key, item.Value);
                    }
                }
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                DataRow row = dataTable.Rows[0];
                return row;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetDataSet(string connString, string query, Dictionary<string, string> param)
        {
            try
            {
                using OracleConnection obcon = new OracleConnection(connString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                using OracleDataAdapter dataAdapter = new OracleDataAdapter(query, obcon);
                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(item.Key, item.Value);
                    }
                }
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetDataTable(string conString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                using OracleConnection obcon = new OracleConnection(conString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                using OracleDataAdapter dataAdapter = new OracleDataAdapter(query, obcon);

                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(item.Key, item.Value);
                    }
                }
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T GetMaxNumNonParaQuery<T>(string conneString, string query)
        {
            try
            {
                var data = "";
                using OracleConnection obcon = new OracleConnection(conneString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                obcon.Open();
                using OracleCommand cmd = new OracleCommand(query, obcon);
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = Convert.ToString(reader[0]);
                    }
                    reader.Close();
                    obcon.Close();
                }
                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T GetMaximumNumber<T>(string conneString, string query, Dictionary<string, string> param)
        {
            try
            {
                var data = "";
                using OracleConnection conn = new OracleConnection(conneString);
                conn.UseHourOffsetForUnsupportedTimezone = true;

                conn.Open();
                using OracleDataAdapter da = new OracleDataAdapter();
                using OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.CommandTimeout = 0;

                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        cmd.Parameters.Add(item.Key, item.Value);
                    }
                }
                OracleDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = Convert.ToString(reader[0]);
                    }
                    reader.Close();
                    conn.Close();
                }
                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(ex.Message, typeof(T));
            }
        }
        public bool SaveChanges(string conString, List<QueryPattern> queryPatterns)
        {
            try
            {
                using (OracleConnection obcon = new OracleConnection(conString))
                {
                    obcon.UseHourOffsetForUnsupportedTimezone = true;

                    obcon.Open();
                    OracleTransaction transaction;
                    // Start a local transaction.
                    //System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted;
                    transaction = obcon.BeginTransaction();
                    try
                    {
                        //foreach (var data)
                        using OracleCommand cmd = obcon.CreateCommand();
                        cmd.Transaction = transaction;
                        foreach (var data in queryPatterns)
                        {
                            cmd.CommandText = data.Query;
                            //string query = cmd.CommandText;
                            if (data.Parametes.Count > 0 && data.Parametes != null)
                            {
                                cmd.Parameters.Clear();
                                foreach (var parameter in data.Parametes)
                                {
                                    foreach (var item in parameter)
                                    {
                                        cmd.Parameters.Add(item.Key, item.Value);
                                    }
                                }
                            }
                            //foreach (SqlParameter p in cmd.Parameters)
                            //{
                            //    query = query.Replace(p.ParameterName, p.Value.ToString());
                            //}
                            cmd.ExecuteNonQuery();
                        }
                        // Attempt to commit the transaction.
                        transaction.Commit();
                        obcon.Close();
                    }
                    catch (Exception ex)
                    {
                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                        catch (Exception ex2)
                        {
                            throw ex2;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<string, string> AddParameter(string[] values)
        {
            var parameter = new Dictionary<string, string>();
            int i = 1;
            if (values.Length > 0)
            {
                foreach (var data in values)
                {
                    parameter.Add($":param{i}", data);
                    i++;
                }
            }
            return parameter;
        }
        public async Task<T> ProcedureCallAsyn<T>(string connString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                List<int> outputParam = new List<int>();
                List<string> result = new List<string>();
                using OracleConnection conn = new OracleConnection(connString);
                conn.UseHourOffsetForUnsupportedTimezone = true;

                conn.Open();
                OracleTransaction transaction;

                transaction = conn.BeginTransaction();
                try
                {
                    using OracleDataAdapter da = new OracleDataAdapter();
                    using OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = query;
                    cmd.CommandTimeout = 0;
                    if (param != null && param.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in param)
                        {
                            if (item.Value == "RefCursor")
                            {
                                cmd.Parameters.Add(item.Key, OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue; ;
                            }
                            else if (item.Value == "Int32")
                            {
                                cmd.Parameters.Add(item.Key, OracleDbType.Int32);
                                cmd.Parameters[i].Direction = ParameterDirection.ReturnValue;
                                outputParam.Add(i);
                            }
                            else if (item.Value == "Varchar2")
                            {
                                cmd.Parameters.Add(item.Key, OracleDbType.Varchar2, 32767).Direction = ParameterDirection.ReturnValue;
                                outputParam.Add(i);
                            }
                            else
                            {
                                cmd.Parameters.Add(item.Key, item.Value);
                            }
                            i++;
                        }
                    }
                    await Task.Run(() => cmd.ExecuteNonQuery());

                    foreach (var item in outputParam)
                    {
                        if (item > 0 && cmd.Parameters[item].Direction == ParameterDirection.ReturnValue)
                        {
                            result.Add(cmd.Parameters[item].Value.ToString());
                        }
                    }
                    if (result.Count == 0)
                    {
                        result.Add("1");
                    }
                    transaction.Commit();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                return (T)Convert.ChangeType(System.Text.Json.JsonSerializer.Serialize(result), typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(ex.Message, typeof(T));
            }
        }
        public async Task<T> ProcedureCallOptimizeAsync<T>(string connString, string query, Dictionary<string, string> param = null)
        {
            List<string> result = new List<string>();
            int rowsAffected = 0;
            using (OracleConnection conn = new OracleConnection(connString))
            {
                conn.UseHourOffsetForUnsupportedTimezone = true;
                await conn.OpenAsync();
                using (OracleTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (OracleCommand cmd = new OracleCommand(query, conn))
                        {
                            cmd.CommandTimeout = 0;

                            if (param != null && param.Count > 0)
                            {
                                foreach (var item in param)
                                {
                                    OracleParameter oracleParameter;

                                    if (item.Value == "RefCursor")
                                    {
                                        oracleParameter = cmd.Parameters.Add(item.Key, OracleDbType.RefCursor);
                                        oracleParameter.Direction = ParameterDirection.ReturnValue;
                                    }
                                    else if (item.Value == "Int32" || item.Value == "Varchar2")
                                    {
                                        OracleDbType oracleDbType = item.Value == "Int32" ? OracleDbType.Int32 : OracleDbType.Varchar2;
                                        int size = item.Value == "Varchar2" ? 32767 : 0;

                                        oracleParameter = cmd.Parameters.Add(item.Key, oracleDbType, size);
                                        oracleParameter.Direction = ParameterDirection.ReturnValue;
                                    }
                                    else
                                    {
                                        oracleParameter = cmd.Parameters.Add(item.Key, item.Value);
                                    }
                                }
                            }
                            rowsAffected = await cmd.ExecuteNonQueryAsync();
                            foreach (OracleParameter oracleParameter in cmd.Parameters)
                            {
                                if (oracleParameter.Direction == ParameterDirection.ReturnValue)
                                {
                                    result.Add(oracleParameter.Value.ToString());
                                }
                            }
                        }
                        await transaction.CommitAsync();
                    }
                    catch (OracleException ex)
                    {
                        await transaction.RollbackAsync();
                        result.Add(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        result.Add(ex.Message);
                    }
                }
            }
            return (T)Convert.ChangeType(result.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(result) : rowsAffected.ToString(), typeof(T));
            //return (T)Convert.ChangeType(System.Text.Json.JsonSerializer.Serialize(result), typeof(T));

        }
        public async Task<T> PreExecuteProcedureCallAsyn<T>(string connString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                List<int> outputParam = new List<int>();
                List<string> result = new List<string>();
                using OracleConnection conn = new OracleConnection(connString);
                conn.UseHourOffsetForUnsupportedTimezone = true;

                conn.Open();
                OracleTransaction transaction;

                transaction = conn.BeginTransaction();
                try
                {
                    using OracleDataAdapter da = new OracleDataAdapter();
                    using OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = query;
                    cmd.CommandTimeout = 0;
                    if (param != null && param.Count > 0)
                    {
                        int i = 0;
                        foreach (var item in param)
                        {
                            if (item.Value == "RefCursor")
                            {
                                cmd.Parameters.Add(item.Key, OracleDbType.RefCursor).Direction = ParameterDirection.ReturnValue; ;
                            }
                            else if (item.Value == "Int32")
                            {
                                cmd.Parameters.Add(item.Key, OracleDbType.Int32);
                                cmd.Parameters[i].Direction = ParameterDirection.ReturnValue;
                                outputParam.Add(i);
                            }
                            else if (item.Value == "Varchar2")
                            {
                                cmd.Parameters.Add(item.Key, OracleDbType.Varchar2, 32767).Direction = ParameterDirection.ReturnValue;
                                outputParam.Add(i);
                            }
                            else
                            {
                                cmd.Parameters.Add(item.Key, item.Value);
                            }
                            i++;
                        }
                    }

                    await Task.Run(() => cmd.ExecuteNonQuery());

                    foreach (var item in outputParam)
                    {
                        if (item > 0 && cmd.Parameters[item].Direction == ParameterDirection.ReturnValue)
                        {
                            result.Add(cmd.Parameters[item].Value.ToString());
                        }
                    }
                    if (result.Count == 0)
                    {
                        result.Add("1");
                    }
                    transaction.Commit();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

                return (T)Convert.ChangeType(System.Text.Json.JsonSerializer.Serialize(result), typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(ex.Message, typeof(T));
            }
        }
        public async Task<DataTable> GetDataTableAsyn(string connString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                OracleConnection obcon = new OracleConnection(connString)
                {
                    UseHourOffsetForUnsupportedTimezone = true
                };

                OracleDataAdapter dataAdapter = new OracleDataAdapter(query, obcon);
                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        if (item.Value == "RefCursor")
                        {
                            dataAdapter.SelectCommand.Parameters.Add(":param1", OracleDbType.RefCursor);
                            dataAdapter.SelectCommand.Parameters[0].Direction = ParameterDirection.ReturnValue;
                        }
                        else
                        {
                            dataAdapter.SelectCommand.Parameters.Add(item.Key, item.Value);
                        }
                    }
                }
                DataTable dt = new DataTable();
                await Task.Run(() => dataAdapter.Fill(dt));
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DataSet> GetDataSetAsyn(string connString, string query, Dictionary<string, string> param)
        {
            try
            {
                using OracleConnection obcon = new OracleConnection(connString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                using OracleDataAdapter dataAdapter = new OracleDataAdapter(query, obcon);
                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(item.Key, item.Value);
                    }
                }
                DataSet dataSet = new DataSet();
                await Task.Run(() => dataAdapter.Fill(dataSet));
                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<DataRow> GetDataRowAsyn(string connString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                using OracleConnection obcon = new OracleConnection(connString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                using OracleDataAdapter dataAdapter = new OracleDataAdapter(query, obcon);
                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        dataAdapter.SelectCommand.Parameters.Add(item.Key, item.Value);
                    }
                }
                DataTable dataTable = new DataTable();
                await Task.Run(() => dataAdapter.Fill(dataTable));

                DataRow row = dataTable.Rows[0];
                return row;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> SaveChangesAsyn(string connString, List<QueryPattern> queryPatterns)
        {
            try
            {
                using (OracleConnection obcon = new OracleConnection(connString))
                {
                    obcon.UseHourOffsetForUnsupportedTimezone = true;

                    obcon.Open();
                    OracleTransaction transaction;
                    // Start a local transaction.
                    //System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted;

                    transaction = obcon.BeginTransaction();
                    try
                    {
                        //foreach (var data)
                        using OracleCommand cmd = obcon.CreateCommand();
                        cmd.Transaction = transaction;
                        cmd.BindByName = true;
                        foreach (var data in queryPatterns)
                        {
                            cmd.CommandText = data.Query;
                            if (data.Parametes.Count > 0 && data.Parametes != null)
                            {
                                cmd.Parameters.Clear();
                                foreach (var parameter in data.Parametes)
                                {
                                    foreach (var item in parameter)
                                    {
                                        cmd.Parameters.Add(item.Key, item.Value);
                                    }
                                }
                            }
                            await Task.Run(() => cmd.ExecuteNonQuery());
                        }
                        // Attempt to commit the transaction.
                        transaction.Commit();
                        obcon.Close();
                    }
                    catch (Exception ex)
                    {
                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                            throw ex;
                        }
                        catch (Exception ex2)
                        {
                            throw ex2;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> SaveChangesOptimizeAsync(string connString, List<QueryPattern> queryPatterns)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connString))
                {
                    await connection.OpenAsync();

                    using (OracleTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            int totalRowsAffected = 0;

                            foreach (var data in queryPatterns)
                            {
                                using (OracleCommand cmd = connection.CreateCommand())
                                {
                                    cmd.Transaction = transaction;
                                    cmd.BindByName = true;
                                    cmd.CommandText = data.Query;

                                    if (data.Parametes.Count > 0)
                                    {
                                        cmd.Parameters.Clear();

                                        foreach (var parameter in data.Parametes.SelectMany(p => p))
                                        {
                                            cmd.Parameters.Add(parameter.Key, parameter.Value);
                                        }
                                    }

                                    totalRowsAffected += await cmd.ExecuteNonQueryAsync();
                                }
                            }

                            await transaction.CommitAsync();

                            return totalRowsAffected;
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> GetMaxNumNonParaQueryAsyn<T>(string connString, string query)
        {
            try
            {
                var data = "";
                using OracleConnection obcon = new OracleConnection(connString);
                obcon.UseHourOffsetForUnsupportedTimezone = true;

                obcon.Open();
                using OracleCommand cmd = new OracleCommand(query, obcon);
                OracleDataReader reader = await Task.Run(() => cmd.ExecuteReader());
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = Convert.ToString(reader[0]);
                    }
                    reader.Close();
                    obcon.Close();
                }
                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<T> GetMaximumNumberAsyn<T>(string connString, string query, Dictionary<string, string> param)
        {
            try
            {
                var data = "";
                using OracleConnection conn = new OracleConnection(connString);
                conn.UseHourOffsetForUnsupportedTimezone = true;

                conn.Open();
                using OracleDataAdapter da = new OracleDataAdapter();
                using OracleCommand cmd = conn.CreateCommand();
                cmd.CommandText = query;
                cmd.CommandTimeout = 0;
                if (param != null && param.Count > 0)
                {
                    foreach (var item in param)
                    {
                        cmd.Parameters.Add(item.Key, item.Value);
                    }
                }
                OracleDataReader reader = await Task.Run(() => cmd.ExecuteReader());

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        data = Convert.ToString(reader[0]);
                    }
                    reader.Close();
                    conn.Close();
                }
                return (T)Convert.ChangeType(data, typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(ex.Message, typeof(T));
            }
        }
        public string ReturnExtension(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".txt":
                    return "text/plain";

                case ".doc":
                    return "application/msword";

                case ".pdf":
                    return "application/pdf";

                case ".xls":
                    return "application/vnd.ms-excel";

                case ".gif":
                    return "image/gif";

                case ".png":
                    return "image/png";

                case ".jpg":
                case "jpeg":
                    return "image/jpeg";

                case ".bmp":
                    return "image/bmp";

                case ".wav":
                    return "audio/wav";

                case ".ppt":
                    return "application/vnd.ms-powerpoint";

                case ".dwg":
                    return "image/vnd.dwg";

                default:
                    return "application/octet-stream";
            }
        }
        public bool DataSave(string connString, string query, Dictionary<string, string> param = null)
        {
            try
            {
                using (OracleConnection obcon = new OracleConnection(connString))
                {
                    obcon.UseHourOffsetForUnsupportedTimezone = true;

                    obcon.Open();
                    using OracleCommand cmd = obcon.CreateCommand();
                    cmd.CommandText = query;
                    if (param.Count > 0 && param != null)
                    {
                        foreach (var item in param)
                        {
                            cmd.Parameters.Add(item.Key, item.Value);
                        }
                    }
                    cmd.ExecuteNonQuery();
                    obcon.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string Encrypt(string pstrText)
        {
            string pstrEncrKey = "SquareIformatixLtd";

            return Encrypt(pstrText, pstrEncrKey);
        }
        public string OptimizeEncrypt(string pstrText)
        {
            string pstrEncrKey = "SquareIformatixLtd";

            return OptimizeEncrypt(pstrText, pstrEncrKey);
        }
        public string Decrypt(string pstrText)
        {
            pstrText = pstrText.Replace(" ", "+");
            string pstrDecrKey = "SquareIformatixLtd";

            return Decrypt(pstrText, pstrDecrKey);
        }
        public string DecryptNew(string pstrText, int id)
        {
            try
            {
                pstrText = pstrText.Replace(" ", "+");
                string pstrDecrKey = "SquareIformatixLtd";
                return Decrypt(pstrText, pstrDecrKey);
            }
            catch (Exception)
            {

                return id.ToString();
            }

        }
        public string Encrypt(string plainText, string passPhrase)
        {
            int DerivationIterations = 1000;
            int Keysize = 128;
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }
        public string Decrypt(string cipherText, string passPhrase)
        {
            int DerivationIterations = 1000;
            int Keysize = 128;
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }
        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
        public string OptimizeEncrypt(string plainText, string passPhrase)
        {
            int DerivationIterations = 1000;
            int Keysize = 128;

            // Salt and IV are randomly generated each time, but are prepended to encrypted cipher text
            // so that the same Salt and IV values can be used when decrypting.
            var saltStringBytes = OptimizeGenerateRandomBytes(16); // 16 Bytes for salt
            var ivStringBytes = OptimizeGenerateRandomBytes(16); // 16 Bytes for IV
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);

                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 128;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                            }

                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes, and the cipher bytes.
                            var cipherTextBytes = saltStringBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();

                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }

        private static byte[] OptimizeGenerateRandomBytes(int length)
        {
            var randomBytes = new byte[length];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public List<T> ConvertDataTableToList<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
        public DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        public string GenerateRequisitionCode(string conString, string fn_name, string companyId, string unitId)
        {
            var query = @"SELECT " + fn_name + "(:param1, :param2) REF_NO FROM DUAL";

            var dt = GetDataTable(conString, query, AddParameter(new string[] { companyId, unitId }));

            return Convert.ToString(dt.Rows[0]["REF_NO"]);
        }
        public string GenerateRequisitionCode(string conString, string fn_name, string companyId, string unitId, string customerId)
        {
            var query = @"SELECT " + fn_name + "(:param1, :param2, :param3) REF_NO FROM DUAL";

            var dt = GetDataTable(conString, query, AddParameter(new string[] { companyId, unitId, customerId }));

            return Convert.ToString(dt.Rows[0]["REF_NO"]);
        }
        private string GetFormattedValue(PropertyInfo property, object obj)
        {
            string propertyName = property.Name;
            object propertyValue = property.GetValue(obj);

            if (propertyName.IndexOf("_date", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // Handle special formatting for properties containing "date" in any case
                return $"TO_DATE(:{propertyName}, 'DD/MM/YYYY HH:MI:SS AM')";
            }

            // Default formatting for other properties
            return $":{propertyName}";
        }
        private bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive || type.IsValueType || type == typeof(string) || type == typeof(decimal);
        }
        private string GetMemberName<T>(Expression<Func<T, object>> expression)
        {
            var memberExpression = expression.Body as MemberExpression ??
                                   ((UnaryExpression)expression.Body)?.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Expressions must be MemberExpressions");
            }

            return memberExpression.Member.Name;
        }
        private string GetWhereCondition<T>(Expression<Func<T, object>> expression, PropertyInfo[] properties, T obj)
        {
            var memberExpression = expression.Body as MemberExpression ??
                                   ((UnaryExpression)expression.Body)?.Operand as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("Expressions in whereProperties must be MemberExpressions");
            }

            var propertyName = memberExpression.Member.Name;
            return $"{propertyName} = {GetFormattedValue(properties.Single(p => p.Name == propertyName), obj)}";
        }
        public string InsertQuery<T>(T obj, string tableName)
        {
            var properties = obj.GetType().GetProperties();
            var validProperties = properties.Where(prop => prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && IsPrimitiveType(prop.PropertyType));
            var columns = string.Join(", ", validProperties.Select(p => p.Name));
            var values = string.Join(", ", validProperties.Select(p => GetFormattedValue(p, obj)));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }
        public string InsertQuerySkipProperties<T>(T obj, string tableName, params Expression<Func<T, object>>[] skipProperties)
        {
            var properties = obj.GetType().GetProperties();
            var validProperties = properties.Where(prop => !skipProperties.Any(expression => ((MemberExpression)expression.Body).Member.Name == prop.Name) && IsPrimitiveType(prop.PropertyType) && prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0);

            var columns = string.Join(", ", validProperties.Select(p => p.Name));
            var values = string.Join(", ", validProperties.Select(p => GetFormattedValue(p, obj)));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }
        public string InsertQueryIncludedProperties<T>(T obj, string tableName, params Expression<Func<T, object>>[] includedProperties)
        {
            var properties = obj.GetType().GetProperties();
            var validProperties = properties.Where(prop => includedProperties.Any(expression => ((MemberExpression)expression.Body).Member.Name == prop.Name) && IsPrimitiveType(prop.PropertyType));
            var columns = string.Join(", ", validProperties.Select(p => p.Name));
            var values = string.Join(", ", validProperties.Select(p => GetFormattedValue(p, obj)));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }
        public Dictionary<string, string> InsertParameter(object obj)
        {
            var parameter = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                // Skip properties marked with [NotMapped] and non PrimitiveType value
                if (prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && IsPrimitiveType(prop.PropertyType))
                {
                    parameter.Add($":{prop.Name}", prop.GetValue(obj)?.ToString() ?? "");
                }
            }

            return parameter;
        }
        public Dictionary<string, string> InsertParameterSkipProperties<T>(T obj, params Expression<Func<T, object>>[] skipProperties)
        {
            var parameter = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (!skipProperties.Any(expression => ((MemberExpression)expression.Body).Member.Name == prop.Name) && IsPrimitiveType(prop.PropertyType) && prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0)
                {
                    parameter.Add($":{prop.Name}", prop.GetValue(obj)?.ToString() ?? "");
                }
            }

            return parameter;
        }
        public Dictionary<string, string> InsertParameterIncludedProperties<T>(T obj, params Expression<Func<T, object>>[] includedProperties)
        {
            var parameter = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();

            foreach (var prop in properties)
            {
                if (includedProperties.Any(expression => ((MemberExpression)expression.Body).Member.Name == prop.Name) && IsPrimitiveType(prop.PropertyType) && prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0)
                {
                    parameter.Add($":{prop.Name}", prop.GetValue(obj)?.ToString() ?? "");
                }
            }

            return parameter;
        }




        //public string GenerateUpdateQuery<T>(T obj, string tableName, params Expression<Func<T, object>>[] whereProperties)
        //{
        //    var properties = obj.GetType().GetProperties();
        //    var validProperties = properties
        //        .Where(prop => prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && IsPrimitiveType(prop.PropertyType));

        //    var setClause = string.Join(", ", validProperties
        //        .Select(p => $"{p.Name} = {GetFormattedValue(p, obj)}"));

        //    var whereClause = string.Join(" AND ", whereProperties
        //        .Select(expression => GetWhereCondition(expression, properties, obj)));

        //    return $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        //}


        //public Dictionary<string, string> GenerateUpdateParameter<T>(T obj, params Expression<Func<T, object>>[] whereProperties)
        //{
        //    var parameter = new Dictionary<string, string>();
        //    var properties = obj.GetType().GetProperties();

        //    // Add properties specified in whereProperties first
        //    foreach (var whereProperty in whereProperties)
        //    {
        //        var memberExpression = whereProperty.Body as MemberExpression;
        //        if (memberExpression == null)
        //        {
        //            var unaryExpression = whereProperty.Body as UnaryExpression;
        //            memberExpression = unaryExpression?.Operand as MemberExpression;
        //        }

        //        if (memberExpression == null)
        //        {
        //            throw new ArgumentException("Expressions in whereProperties must be MemberExpressions");
        //        }

        //        var propertyName = memberExpression.Member.Name;
        //        if (!parameter.ContainsKey($":{propertyName}"))
        //        {
        //            parameter.Add($":{propertyName}", properties.Single(p => p.Name == propertyName).GetValue(obj)?.ToString() ?? "");
        //        }
        //    }

        //    // Skip properties marked with [NotMapped] and non-PrimitiveType value
        //    foreach (var prop in properties.Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && IsPrimitiveType(p.PropertyType)))
        //    {
        //        // Skip properties already added in the first loop
        //        if (!parameter.ContainsKey($":{prop.Name}"))
        //        {
        //            parameter.Add($":{prop.Name}", prop.GetValue(obj)?.ToString() ?? "");
        //        }
        //    }

        //    return parameter;
        //}

        //private string GetWhereCondition<T>(Expression<Func<T, object>> expression, PropertyInfo[] properties, T obj)
        //{
        //    var memberExpression = expression.Body as MemberExpression ??
        //                           ((UnaryExpression)expression.Body)?.Operand as MemberExpression;

        //    if (memberExpression == null)
        //    {
        //        throw new ArgumentException("Expressions in whereProperties must be MemberExpressions");
        //    }

        //    var propertyName = memberExpression.Member.Name;
        //    return $"{propertyName} = {GetFormattedValue(properties.Single(p => p.Name == propertyName), obj)}";
        //}



        public string UpdateQuery<T>(T obj, string tableName, params Expression<Func<T, object>>[] whereProperties)
        {
            var properties = obj.GetType().GetProperties();

            // Properties to be updated (excluding whereProperties)
            var updateProperties = properties
                .Where(prop => prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 &&
                               IsPrimitiveType(prop.PropertyType) &&
                               !whereProperties.Any(w => GetMemberName(w) == prop.Name));

            var setClause = string.Join(", ", updateProperties
                .Select(p => $"{p.Name} = {GetFormattedValue(p, obj)}"));

            var whereClause = string.Join(" AND ", whereProperties
                .Select(expression => GetWhereCondition(expression, properties, obj)));

            return $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        }
        public string UpdateQueryIncludedProperties<T>(T obj, string tableName, Expression<Func<T, object>>[] updateProperties, params Expression<Func<T, object>>[] whereProperties)
        {
            var properties = obj.GetType().GetProperties();

            // Properties to be updated (excluding whereProperties and properties in updateProperties)
            var propertiesToBeUpdated = properties
                .Where(prop => prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 &&
                               IsPrimitiveType(prop.PropertyType) &&
                               !whereProperties.Any(w => GetMemberName(w) == prop.Name) &&
                               updateProperties.Any(u => GetMemberName(u) == prop.Name));

            var setClause = string.Join(", ", propertiesToBeUpdated
                .Select(p => $"{p.Name} = {GetFormattedValue(p, obj)}"));

            var whereClause = string.Join(" AND ", whereProperties
                .Select(expression => GetWhereCondition(expression, properties, obj)));

            return $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        }

        public string UpdateQueryExludedProperties<T>(T obj, string tableName, Expression<Func<T, object>>[] excludedProperties, params Expression<Func<T, object>>[] whereProperties)
        {
            var properties = obj.GetType().GetProperties();

            // Properties to be updated (excluding whereProperties and properties in updateProperties)
            var propertiesToBeUpdated = properties
                .Where(prop => prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 &&
                               IsPrimitiveType(prop.PropertyType) &&
                               !whereProperties.Any(w => GetMemberName(w) == prop.Name) &&
                               !excludedProperties.Any(u => GetMemberName(u) == prop.Name));

            var setClause = string.Join(", ", propertiesToBeUpdated
                .Select(p => $"{p.Name} = {GetFormattedValue(p, obj)}"));

            var whereClause = string.Join(" AND ", whereProperties
                .Select(expression => GetWhereCondition(expression, properties, obj)));

            return $"UPDATE {tableName} SET {setClause} WHERE {whereClause}";
        }

        public Dictionary<string, string> UpdateParameter<T>(T obj, params Expression<Func<T, object>>[] whereProperties)
        {
            var parameter = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();
            foreach (var prop in properties.Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && IsPrimitiveType(p.PropertyType) && !whereProperties.Any(w => GetMemberName(w) == p.Name)))
            {
                parameter.Add($":{prop.Name}", prop.GetValue(obj)?.ToString() ?? "");
            }
            foreach (var whereProperty in whereProperties)
            {
                var propertyName = GetMemberName(whereProperty);
                if (!parameter.ContainsKey($":{propertyName}"))
                {
                    parameter.Add($":{propertyName}", properties.Single(p => p.Name == propertyName).GetValue(obj)?.ToString() ?? "");
                }
            }

            return parameter;
        }
        public Dictionary<string, string> UpdateParameterIncludedProperties<T>(T obj, Expression<Func<T, object>>[] updateProperties, params Expression<Func<T, object>>[] whereProperties)
        {
            var parameter = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();
            // Add properties specified in updateProperties first
            foreach (var updateProperty in updateProperties)
            {
                var propertyName = GetMemberName(updateProperty);
                if (!parameter.ContainsKey($":{propertyName}"))
                {
                    parameter.Add($":{propertyName}", properties.Single(p => p.Name == propertyName).GetValue(obj)?.ToString() ?? "");
                }
            }

            foreach (var whereProperty in whereProperties)
            {
                var propertyName = GetMemberName(whereProperty);
                if (!parameter.ContainsKey($":{propertyName}"))
                {
                    parameter.Add($":{propertyName}", properties.Single(p => p.Name == propertyName).GetValue(obj)?.ToString() ?? "");
                }
            }

            return parameter;
        }

        public Dictionary<string, string> UpdateParameterExcludedProperties<T>(T obj, Expression<Func<T, object>>[] excludeProperties, params Expression<Func<T, object>>[] whereProperties)
        {
            var parameter = new Dictionary<string, string>();
            var properties = obj.GetType().GetProperties();
            // Add properties specified in updateProperties first
            foreach (var prop in properties.Where(p => p.GetCustomAttributes(typeof(NotMappedAttribute), false).Length == 0 && IsPrimitiveType(p.PropertyType) && !whereProperties.Any(w => GetMemberName(w) == p.Name) && !excludeProperties.Any(w => GetMemberName(w) == p.Name)))
            {
                parameter.Add($":{prop.Name}", prop.GetValue(obj)?.ToString() ?? "");
            }

            foreach (var whereProperty in whereProperties)
            {
                var propertyName = GetMemberName(whereProperty);
                if (!parameter.ContainsKey($":{propertyName}"))
                {
                    parameter.Add($":{propertyName}", properties.Single(p => p.Name == propertyName).GetValue(obj)?.ToString() ?? "");
                }
            }

            return parameter;
        }

        public async Task<DataSet> GetDataSetForMultiQueryAsync(string connString, string[] queries)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connString))
                {
                    await connection.OpenAsync();

                    DataSet dataSet = new DataSet();

                    var tasks = queries.Select(async query =>
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            using (OracleDataAdapter dataAdapter = new OracleDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable); // Synchronous fill
                                dataSet.Tables.Add(dataTable);
                            }
                        }
                    });

                    await Task.WhenAll(tasks); // Wait for all queries to complete

                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions according to your application's requirements
                throw ex;
            }
        }
        public async Task<DataSet> GetDataSetForMultiQueryWithParamAsync(string connString, List<string> queries, List<Dictionary<string, string>> parametersList)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(connString))
                {
                    await connection.OpenAsync();

                    DataSet dataSet = new DataSet();

                    var tasks = queries.Select(async (query, index) =>
                    {
                        using (OracleCommand command = new OracleCommand(query, connection))
                        {
                            Dictionary<string, string> parameters = parametersList[index];

                            if (parameters != null && parameters.Count > 0)
                            {
                                foreach (var parameter in parameters)
                                {
                                    command.Parameters.Add(new OracleParameter(parameter.Key, parameter.Value));
                                }
                            }

                            using (OracleDataAdapter dataAdapter = new OracleDataAdapter(command))
                            {
                                DataTable dataTable = new DataTable();
                                dataAdapter.Fill(dataTable); // Synchronous fill
                                dataSet.Tables.Add(dataTable);
                            }
                        }
                    });

                    await Task.WhenAll(tasks); // Wait for all queries to complete

                    return dataSet;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions according to your application's requirements
                throw ex;
            }
        }
        public string SelectQuery<T>(string tableName, params Expression<Func<T, bool>>[] whereProperties) where T : class
        {
            var properties = typeof(T).GetProperties();

            var selectedProperties = properties
                .Where(prop => !prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Any() &&
                               IsPrimitiveType(prop.PropertyType))
                .Select(p => p.PropertyType == typeof(string) && p.Name.IndexOf("_date", StringComparison.OrdinalIgnoreCase) >= 0
                    ? $"TO_CHAR({p.Name}, 'DD/MM/YYYY') {p.Name}"
                    : p.Name);

            var whereClause = whereProperties.Length > 0
                ? $" WHERE {string.Join(" AND ", whereProperties.Select(expr => BuildWhereClause<T>(properties)(expr)))}"
                : "";

            return $"SELECT {string.Join(", ", selectedProperties)} FROM {tableName}{whereClause}";
        }
        public string SelectQueryComplex<T>(string tableName, Expression<Func<T, bool>>[] whereProperties = null, Expression<Func<T, object>>[] skipProperties = null, Expression<Func<T, object>>[] includedProperties = null) where T : class
        {
            var properties = typeof(T).GetProperties();

            var selectedProperties = properties
                .Where(prop => !prop.GetCustomAttributes(typeof(NotMappedAttribute), false).Any() && IsPrimitiveType(prop.PropertyType))
                .Select(p => p.PropertyType == typeof(string) && p.Name.IndexOf("_date", StringComparison.OrdinalIgnoreCase) >= 0
                    ? $"TO_CHAR({p.Name}, 'DD/MM/YYYY') {p.Name}"
                    : p.Name);

            if (includedProperties != null && includedProperties.Any())
            {
                selectedProperties = selectedProperties.Intersect(includedProperties.Select(p => GetMemberName(p)));
            }

            if (skipProperties != null && skipProperties.Any())
            {
                selectedProperties = selectedProperties.Except(skipProperties.Select(p => GetMemberName(p)));
            }

            var selectClause = $"SELECT {string.Join(", ", selectedProperties)} FROM {tableName}";

            if (whereProperties != null && whereProperties.Any())
            {
                var buildWhereClause = BuildWhereClause<T>(properties);
                var whereClause = $" WHERE {string.Join(" AND ", whereProperties.Select(buildWhereClause))}";
                return $"{selectClause}{whereClause}";
            }

            return selectClause;
        }
        private Func<Expression<Func<T, bool>>, string> BuildWhereClause<T>(PropertyInfo[] properties) where T : class
        {
            return expression =>
            {
                if (!(expression.Body is BinaryExpression binaryExpression))
                {
                    throw new ArgumentException("Expressions in whereProperties must be BinaryExpressions");
                }

                if (!(binaryExpression.Left is MemberExpression leftExpression) || !(binaryExpression.Right is ConstantExpression rightExpression))
                {
                    throw new ArgumentException("Expressions in whereProperties must be of the form 'property == value'");
                }

                var propertyName = leftExpression.Member.Name;
                var propertyValue = rightExpression.Value;

                return $"{propertyName} = {GetFormattedValue(properties.Single(p => p.Name == propertyName), propertyValue)}";
            };
        }


        public async Task<T> Ridwan<T>(string connString, List<QueryPattern> queryPatterns)
        {
            try
            {
                List<string> result = new List<string>();
                int totalRowsAffected = 0;

                using (OracleConnection conn = new OracleConnection(connString))
                {
                    conn.UseHourOffsetForUnsupportedTimezone = true;
                    await conn.OpenAsync();

                    using (OracleTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var queryPattern in queryPatterns)
                            {
                                using (OracleCommand cmd = new OracleCommand(queryPattern.Query, conn))
                                {
                                    cmd.CommandTimeout = 0;

                                    if (queryPattern.Parametes.Count > 0)
                                    {
                                        foreach (var parameter in queryPattern.Parametes.SelectMany(p => p))
                                        {
                                            OracleParameter oracleParameter;

                                            if (parameter.Value == "RefCursor")
                                            {
                                                oracleParameter = cmd.Parameters.Add(parameter.Key, OracleDbType.RefCursor);
                                                oracleParameter.Direction = ParameterDirection.ReturnValue;
                                            }
                                            else if (parameter.Value == "Int32" || parameter.Value == "Varchar2")
                                            {
                                                OracleDbType oracleDbType = parameter.Value == "Int32" ? OracleDbType.Int32 : OracleDbType.Varchar2;
                                                int size = parameter.Value == "Varchar2" ? 32767 : 0;

                                                oracleParameter = cmd.Parameters.Add(parameter.Key, oracleDbType, size);
                                                oracleParameter.Direction = ParameterDirection.ReturnValue;
                                            }
                                            else
                                            {
                                                oracleParameter = cmd.Parameters.Add(parameter.Key, parameter.Value);
                                            }
                                        }
                                    }

                                    totalRowsAffected += await cmd.ExecuteNonQueryAsync();

                                    foreach (OracleParameter oracleParameter in cmd.Parameters)
                                    {
                                        if (oracleParameter.Direction == ParameterDirection.ReturnValue)
                                        {
                                            result.Add(oracleParameter.Value.ToString());
                                        }
                                    }
                                }
                            }
                            if (result.Count > 0)
                            {
                                if (result[0] != "SUCCESS")
                                {
                                    await transaction.RollbackAsync();
                                }
                                else
                                {
                                    await transaction.CommitAsync();
                                }
                            }
                            else
                            {
                                await transaction.CommitAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            result.Add(ex.Message);

                        }
                    }
                }

                return (T)Convert.ChangeType(result.Count > 0 ? System.Text.Json.JsonSerializer.Serialize(result) : totalRowsAffected.ToString(), typeof(T));
                //return (T)Convert.ChangeType(System.Text.Json.JsonSerializer.Serialize(result), typeof(T));
            }
            catch (Exception ex)
            {
                return (T)Convert.ChangeType(ex.Message, typeof(T));
            }
        }

        public List<T> ConvertToList<T>(DataTable dataTable) where T : class, new()
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
                return new List<T>(); // Return an empty list if DataTable is null or empty

            List<T> resultList = new List<T>();

            var propertyNames = typeof(T).GetProperties();

            foreach (DataRow row in dataTable.Rows)
            {
                T obj = new T();

                foreach (var prop in propertyNames)
                {
                    if (dataTable.Columns.Contains(prop.Name) && row[prop.Name] != DBNull.Value)
                    {
                        // Convert the value from DataRow to the property type
                        object value = Convert.ChangeType(row[prop.Name], prop.PropertyType);
                        prop.SetValue(obj, value);
                    }
                }

                resultList.Add(obj);
            }

            return resultList;
        }

    }
}