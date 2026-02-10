namespace MON.Report.Service
{
    using MON.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Telerik.WebReportDesigner.Services;

    /// <summary>
    /// Извлича данните за отчета от базата данни
    /// </summary>
    public class DbDefinitionStorage : DbService, IDefinitionStorage
    {
        public string BaseDir { get; set; }

        public DbDefinitionStorage(MONContext db) : base(db)
        {
            //_logger = logger;
        }


        /// <summary>
        /// Lists all report definitions.
        /// </summary>
        /// <returns>A list of all report definitions present in the storage.</returns>
        public IEnumerable<string> ListDefinitions()
        {
            // Retrieve all available reports in the database and return their unique identifiers.
            var definitions = from i in _db.PrintTemplates
                              where i.Contents != null
                              select i.Id.ToString() + ".trdp";
            return definitions.ToList();
        }

        /// <summary>
        /// Finds a report definition by its id.
        /// </summary>
        /// <param name="definitionId">The report definition identifier.</param>
        /// <returns>The bytes of the report definition.</returns>
        public byte[] GetDefinition(string definitionId)
        {
            // Retrieve the report definition bytes from the database.
            int id = Convert.ToInt32(definitionId.Replace(".trdp", ""));
            var definition = _db.PrintTemplates.FirstOrDefault(i => i.Id == id);
            return definition?.Contents;
        }

        /// <summary>
        /// Creates new or overwrites an existing report definition with the provided definition bytes.
        /// </summary>
        /// <param name="definitionId">The report definition identifier.</param>
        /// <param name="definition">The new bytes of the report definition.</param>
        public void SaveDefinition(string definitionId, byte[] definition)
        {
            // Save the report definiton bytes to the database.
            int id = Convert.ToInt32(definitionId.Replace(".trdp", ""));
            var dbDefinition = _db.PrintTemplates.FirstOrDefault(i => i.Id == id);
            dbDefinition.Contents = definition;
            dbDefinition.ModifiedBySysUserId = dbDefinition.CreatedBySysUserId;
            dbDefinition.ModifyDate = DateTime.Now;

            Save();
        }

        /// <summary>
        /// Deletes an existing report definition.
        /// </summary>
        /// <param name="definitionId">The report definition identifier.</param>
        public void DeleteDefinition(string definitionId)
        {
            // Delete the report definition from the database.
            throw new NotImplementedException();
        }

        public string GetReportDefinition(string definitionId)
        {
            int id = Convert.ToInt32(definitionId.Replace(".trdp", ""));
            var basicDocument = (from p in _db.PrintTemplates
                                 where p.Id == id
                                 select p.BasicDocument).FirstOrDefault();
            return basicDocument.ReportFormPath;
        }
    }
}
