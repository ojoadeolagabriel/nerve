using System;

namespace nerve.core.data.lightfoot
{
    public class LightFootAttribute : Attribute
    {
        public string TableName { get; set; }
        public string ColumnNameOverride { get; set; }
        public string PrimaryKeyIdentifier { get; set; }

        public string MapsTo { get; set; }

        public string MapsToSp { get; set; }

        public string ForeignKeyId { get; set; }
    }
}