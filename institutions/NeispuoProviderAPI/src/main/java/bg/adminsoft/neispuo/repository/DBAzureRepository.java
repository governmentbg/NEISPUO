package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.azure.*;

import java.util.List;

public interface DBAzureRepository {

    List<AzureTeacherInstitution> getTeacherInstitutionList(TicketData ticket, Short operationType);
    List<AzureTeacherCurriculum> getTeacherCurriculumList(TicketData ticket, Short operationType);
    List<AzureCurriculumCreate> getCurriculumListCreate(TicketData ticket);
    List<AzureCurriculumUpdate> getCurriculumListUpdate(TicketData ticket);
    List<AzureCurriculumDelete> getCurriculumListDelete(TicketData ticket);
    List<AzureStudentCurriculum> getStudentCurriculumList(TicketData ticket, Short operationType);

}
