using System.Runtime.Serialization;

namespace BillableTracking.Shared.Enums
{
    public enum CrudOperation
    {
        [EnumMember(Value = "NULL")]
        Null = 0,
        [EnumMember(Value = "Create")]
        Create = 1,
        [EnumMember(Value = "Read")]
        Read = 2,
        [EnumMember(Value = "Update")]
        Update = 3,
        [EnumMember(Value = "Delete")]
        Delete = 4,
    }
}
