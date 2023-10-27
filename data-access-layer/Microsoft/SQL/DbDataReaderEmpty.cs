using System.Collections;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace data_access_layer.Microsoft.SQL
{
    internal class DbDataReaderEmpty : DbDataReader
    {
        public override object this[int ordinal]
        {
            get
            {
                return new object();
            }
        }

        public override object this[string name]
        {
            get
            {
                return new object();
            }
        }

        public override int Depth => 0;

        public override int FieldCount => 0;

        public override bool HasRows => false;

        public override bool IsClosed => true;

        public override int RecordsAffected => 0;

        public override bool GetBoolean(int ordinal)
        {
            return default;
        }

        public override byte GetByte(int ordinal)
        {
            return default;
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            return default;
        }

        public override char GetChar(int ordinal)
        {
            return default;
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            return default;
        }

        public override string GetDataTypeName(int ordinal)
        {
            return "";
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return default;
        }

        public override decimal GetDecimal(int ordinal)
        {
            return default;
        }

        public override double GetDouble(int ordinal)
        {
            return default;
        }

        public override IEnumerator GetEnumerator()
        {
            yield break;
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
        public override Type GetFieldType(int ordinal)
        {
            return typeof(object);
        }

        public override float GetFloat(int ordinal)
        {
            return default;
        }

        public override Guid GetGuid(int ordinal)
        {
            return default;
        }

        public override short GetInt16(int ordinal)
        {
            return default;
        }

        public override int GetInt32(int ordinal)
        {
            return default;
        }

        public override long GetInt64(int ordinal)
        {
            return default;
        }

        public override string GetName(int ordinal)
        {
            return "";
        }

        public override int GetOrdinal(string name)
        {
            return default;
        }

        public override string GetString(int ordinal)
        {
            return "";
        }

        public override object GetValue(int ordinal)
        {
            return new object();
        }

            public override int GetValues(object[] values)
        {
            return 0;
        }

        public override bool IsDBNull(int ordinal)
        {
            return default;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            return false;
        }
    }
}
