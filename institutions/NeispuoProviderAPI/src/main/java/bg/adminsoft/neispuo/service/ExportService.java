package bg.adminsoft.neispuo.service;

public interface ExportService {

    String exportInstitution(Long institutionId, Integer schoolYear, String mapping);
    void exportInstitutionData();

}
