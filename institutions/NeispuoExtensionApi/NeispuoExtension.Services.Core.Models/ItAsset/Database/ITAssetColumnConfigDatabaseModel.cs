namespace NeispuoExtension.Services.Core.Models.ItAsset.Database
{
    public class ITAssetColumnConfigDatabaseModel
    {
        public int Id { get; set; }

        public string FieldName { get; set; }

        public int ColumnIndex { get; set; }

        public bool IsRequired { get; set; }

        public int DataTypeId { get; set; }

        public string LookupSource { get; set; }

        public int? DependsOnColumnIndex { get; set; }
    }
}
