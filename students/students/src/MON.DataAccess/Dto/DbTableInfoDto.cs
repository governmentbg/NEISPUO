namespace MON.DataAccess.Dto
{
    public class DbTableInfoDto
    {
        public int OrdinalPosition { get; set; }
        public string TableCatalog { get; set; }
        public string TableSchema { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string ColumnDefault { get; set; }
        public bool IsNulable { get; set; }
        public int? CharacterMaxLength { get; set; }
        public byte? NumericPrecision { get; set; }
        public short? NumericPrecisionRadix { get; set; }
        public int? NumericScale { get; set; }
        public short? DatetimePrecison { get; set; }
        public bool IsPrimaryKey { get; set; }
    }
}
