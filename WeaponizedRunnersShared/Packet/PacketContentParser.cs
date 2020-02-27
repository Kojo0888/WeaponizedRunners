using System;
using System.Collections.Generic;
using System.Text;

namespace WeaponizedRunnersShared.PacketContents
{
    public static class PacketContentParser
    {
        public static byte[] ToBytes<Type>(Type data)
        {
            if (typeof(Type) == typeof(string))
                return Encoding.UTF8.GetBytes(data.ToString());

            switch (data)
            {
                case int dataAsType:
                    return BitConverter.GetBytes(dataAsType);
                case float dataAsType:
                    return BitConverter.GetBytes(dataAsType);
                default:
                    throw new Exception("Data format not supported: " + typeof(Type));
            }
        }

        public static Type ToType<Type>(byte[] bytes)
        {
            if (typeof(Type) == typeof(string))
            {
                var message = Encoding.UTF8.GetString(bytes);
                return (Type)Convert.ChangeType(message, typeof(Type));
            }

            switch (typeof(Type).Name)
            {
                case "int":
                    return GetFuckingType<Type>(BitConverter.ToInt32(bytes, 0));
                case "float":
                    return GetFuckingType<Type>(BitConverter.ToSingle(bytes, 0));
                default:
                    throw new Exception("Data format not supported: " + typeof(Type).Name);
            }
        }

        private static Type GetFuckingType<Type>(object value)
        {
            return (Type)Convert.ChangeType(value, typeof(Type));
        }
    }
}