using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Principal;

namespace Repository
{
    public abstract class RepositoryBase<T> where T : class, new()
    {

        public SqlConnection GetConnection()
        {
            var sqlStringBuilder = new SqlConnectionStringBuilder();
            sqlStringBuilder["Server"] = ".\\sqlexpress";
            sqlStringBuilder["Database"] = "AdoTest";
            sqlStringBuilder["Integrated Security"] = true;
            var sqlStringConnection = sqlStringBuilder.ToString();
            var connection = new SqlConnection(sqlStringConnection);
            return connection;
        }

        #region GetAsync
        //public async Task<T?> GetAsync<T>(string sql, string propertyName, string value) where T : new()
        //{
        //    T result = new();
        //    using(SqlConnection connection = GetConnection())
        //    { 
        //        await connection.OpenAsync();
        //        var adapter = new SqlDataAdapter();
        //        SqlCommand command = new SqlCommand(sql, connection);
        //        if(!string.IsNullOrEmpty(value)) {
        //            command.Parameters.AddWithValue($"@{propertyName}", value);
        //        }
        //        else
        //        {
        //            return default(T);
        //        }

        //        using (SqlDataReader reader = await command.ExecuteReaderAsync())
        //        {
        //            if(await reader.ReadAsync())
        //            {
        //                var properties = typeof(T).GetProperties();
        //                foreach (var property in properties)
        //                {
        //                    property.SetValue(result, reader[property.Name].ToString());
        //                }
        //                return result;
        //            }
        //            else
        //            {
        //                return default(T);
        //            }
        //        }
        //    }
        //}
        #endregion

        public async Task<List<T>> GetAllUsersAsync(string roleName, string methodName)
        {
            List<T> userList = new List<T>();

            string sql = "SELECT * FROM Users  " +
                "JOIN  Roles ON Users.RoleId = Roles.RoleId " +
                "JOIN Methods ON Users.MethodId = Methods.MethodId WHERE Roles.RoleName = @roleName" +
                "AND Methods.MethodName = @methodName";

            using (SqlConnection connection = GetConnection())
            {
                try { 
                await connection.OpenAsync();
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                        command.Parameters.AddWithValue("@roleName", roleName);
                        command.Parameters.AddWithValue("@methodName", methodName);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                            
                            if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                T user = new T();
                                var properties = typeof(T).GetProperties();
                                foreach (var property in properties)
                                {
                                    if (reader[property.Name] != DBNull.Value)
                                    {
                                        property.SetValue(user, reader[property.Name].ToString());
                                    }
                                }
                                userList.Add(user);
                            }
                        }
                    }
                }
                return userList;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<T> GetUserByIdAsync(string Id)    
        {
            T user = null;

            string sql = "SELECT * FROM Users WHERE Id = @Id";

            using (SqlConnection connection = GetConnection())
            {
                try { 
                await connection.OpenAsync();

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", Id);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new T();
                            var properties = typeof(T).GetProperties();
                            foreach (var property in properties)
                            {
                                if (reader[property.Name] != DBNull.Value)
                                {
                                    property.SetValue(user, reader[property.Name].ToString());
                                }
                            }
                        }
                    }
                }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }

            return user;
        }

        public async Task<Int32> AddAsync(T entity)
        {
            using (SqlConnection connection = GetConnection())
            {
                try { 
                await connection.OpenAsync();
                var properties = typeof(T).GetProperties().Where(prop => prop.CanWrite && prop.Name != "Id");
                string columns = string.Join(", ", properties.Select(prop => prop.Name));
                string parameters = string.Join(", ", properties.Select(prop => $"@{prop.Name}"));
                string sql = $"INSERT INTO Users ({columns}) VALUES ({parameters}); SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(sql, connection);
                foreach (var property in properties)
                {
                    command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(entity));
                }
                var newId = await command.ExecuteScalarAsync();
                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty != null)
                {
                    idProperty.SetValue(entity, Convert.ChangeType(newId, idProperty.PropertyType));
                }
                return await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<Int32> DeleteAysnc(string id)
        {
            using (SqlConnection connection = GetConnection())
            {
                try { 
                await connection.OpenAsync();
                string sql = $"DELETE FROM Users WHERE Id = @Id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
               return await command.ExecuteNonQueryAsync();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
           
        }

        public async Task<Int32> UpdateAsync(T entity)
        {
            using (SqlConnection connection = GetConnection())
            {
                await connection.OpenAsync();
                try
                {
                    var properties = typeof(T).GetProperties().Where(prop => prop.CanWrite && prop.Name != "Id");
                    var idProperty = typeof(T).GetProperty("Id");
                    if (idProperty == null)
                    {
                        throw new InvalidOperationException("Entity does not have an ID property.");
                    }

                    var idValue = idProperty.GetValue(entity);
                    if (idValue == null)
                    {
                        throw new InvalidOperationException("Entity's ID property is null.");
                    }

                    string columns = string.Join(", ", properties.Select(prop => $"{prop.Name} = @{prop.Name}"));
                    string sql = $"UPDATE Users SET {columns} WHERE {idProperty.Name} = @{idProperty.Name};";
                    SqlCommand command = new SqlCommand(sql, connection);
                    foreach (var property in properties)
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(entity));
                    }
                    command.Parameters.AddWithValue($"@{idProperty.Name}", idValue);
                    await command.ExecuteNonQueryAsync();

                    return await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public async Task<int> ActivateUsersAsync(List<T> entities) 
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Type T does not contain a property named 'Id'.");

            using (SqlConnection connection = GetConnection())
            {
                await connection.OpenAsync();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    foreach (T item in entities)
                    {
                        object idValue = idProperty.GetValue(item);
                        if (idValue == null)
                            throw new InvalidOperationException("Id value cannot be null.");

                        string updateCommandText = $"UPDATE Users SET StatusId = 1 WHERE ID = @Id"; 
                        SqlCommand command = new SqlCommand(updateCommandText, connection, transaction);
                        command.Parameters.AddWithValue("@Id", idValue);

                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();

                    return entities.Count; 
                }
                catch (Exception ex)
                {
                    
                    transaction.Rollback();
                    throw ex; 
                }
            }
        }

        public async Task<int> DeActivateUsersAsync(List<T> entities)
        {
            PropertyInfo idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Type T does not contain a property named 'Id'.");

            using (SqlConnection connection = GetConnection())
            {
                await connection.OpenAsync();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    foreach (T item in entities)
                    {
                        object idValue = idProperty.GetValue(item);
                        if (idValue == null)
                            throw new InvalidOperationException("Id value cannot be null.");

                        string updateCommandText = $"UPDATE Users SET StatusId = 2 WHERE ID = @Id";
                        SqlCommand command = new SqlCommand(updateCommandText, connection, transaction);
                        command.Parameters.AddWithValue("@Id", idValue);

                        await command.ExecuteNonQueryAsync();
                    }
                    transaction.Commit();

                    return entities.Count;
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
