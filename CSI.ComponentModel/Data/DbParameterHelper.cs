namespace CSI.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    public class DbParameterHelper
    {
        public static T CreateInputParameter<T, TDbType>(string name, TDbType dbType, object value) where T: DbParameter where TDbType: struct
        {
            return CreateInputParameter<T, TDbType>(name, dbType, value, null);
        }

        public static T CreateInputParameter<T, TDbType>(string name, TDbType dbType, object value, object defaultValue) where T: DbParameter
        {
            T local = Activator.CreateInstance(typeof(T), new object[] { name, dbType }) as T;
            if (value == null)
            {
                value = (defaultValue == null) ? DBNull.Value : defaultValue;
            }
            local.Direction = ParameterDirection.Input;
            local.Value = value;
            return local;
        }

        public static T CreateOutParameter<T, TDbType>(string name, TDbType dbType) where T: DbParameter
        {
            T local = Activator.CreateInstance(typeof(T), new object[] { name, dbType }) as T;
            local.Direction = ParameterDirection.Output;
            return local;
        }

        public static T CreateInputOutputParameter<T, TDbType>(string name, TDbType dbType, object value) where T : DbParameter
        {
            T local = Activator.CreateInstance(typeof(T), new object[] { name, dbType }) as T;
            local.Direction = ParameterDirection.InputOutput;
            local.Value = value;
            return local;
        }

        public static T CreateReturnValueParameter<T, TDbType>(TDbType dbType) where T : DbParameter
        {
            T local = Activator.CreateInstance(typeof(T), new object[] { "@ReturnValue", dbType }) as T;
            local.Direction = ParameterDirection.ReturnValue;
            return local;
        }

        public static T CreateReturnValueParameter<T, TDbType>(string name, TDbType dbType) where T: DbParameter
        {
            T local = Activator.CreateInstance(typeof(T), new object[] { name, dbType }) as T;
            local.Direction = ParameterDirection.ReturnValue;
            return local;
        }

        public static int GetReturnValue(DbCommand cmd)
        {
            foreach (DbParameter parameter in cmd.Parameters)
            {
                if (((parameter.Direction == ParameterDirection.ReturnValue) && (parameter.Value != null)) && (parameter.Value is int))
                {
                    return (int) parameter.Value;
                }
            }
            return -1;
        }
    }
}

