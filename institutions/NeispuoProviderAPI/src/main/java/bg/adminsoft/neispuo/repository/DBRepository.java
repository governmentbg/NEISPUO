package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.*;

import java.sql.Connection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

public interface DBRepository {

    StagingTableKey getJsonPrimaryKey(JsonMapping table, LinkedHashMap row, String ticket);
    int validateStaging(TicketData ticket);
    StagingTableKey insertJsonIntoStaging(JsonMapping table, LinkedHashMap row, String ticket,
                                          StagingTableKey parentKey, JsonDefaultData defaultData,
                                          Connection connection);

    List<StagingTableData> getStagingTableData(String tableName, DBMapping table,
                                               TicketData ticket, String parentKey, Connection connection);
    Long insertStagingIntoSchema(StagingTableData stagingData, String tableName,
                                 DBMapping table, Long parentKey, Long parentParentKey,
                                 DBLinkMapping[] linkMapping, Map<String, Map<String, Long>> tableIdentifiers,
                                 TicketData ticket, Connection connection);
    void updateStagingIntoSchema(StagingTableData stagingData,
                                 String tableName, DBMapping table,
                                 TicketData ticket, Connection connection);
    void updateStagingExternalIdIntoSchema(StagingTableData stagingData,
                                           String tableName, DBMapping table,
                                           TicketData ticket, Connection connection);
    void deleteSchema(StagingTableData stagingData, String tableName, DBMapping table,
                      TicketData ticket, Connection connection);


    List<SchemaTableData> getSchemaTableData(ExportMapping mapping, Long parentKey,
                                             TicketData ticket, Connection connection);

    void incrementInstitutionVersionData(TicketData ticket, Connection connection);
}
