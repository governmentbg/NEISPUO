package bg.adminsoft.neispuo.repository;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.azure.*;
import lombok.*;
import lombok.extern.slf4j.Slf4j;
import org.springframework.dao.DataAccessException;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

@Component
@Slf4j
@RequiredArgsConstructor
public class DBAzureRepositoryImpl implements DBAzureRepository {

    private final DatabaseConfig databaseConfig;
    private final JdbcTemplate jdbcTemplate;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Override
    public List<AzureTeacherInstitution> getTeacherInstitutionList(TicketData ticket, Short operationType) {
        try {
            String teacherSql = "SELECT DISTINCT e.PersonID, e.InstitutionID " +
                    "  FROM " + databaseConfig.getStagingSchema() + ".EducationalState e, " +
                    "       core.Person p " +
                    " WHERE e.PersonID = p.PersonID " +
                    "   AND e.PositionID = 2 " +
                    "   AND p.AzureID IS NOT NULL" +
                    "   AND e.TicketID = ? " +
                    "   AND e.OperationType = ? ";
            return jdbcTemplate.query(
                    teacherSql,
                    (rs, rowNum) ->
                            new AzureTeacherInstitution(
                                    rs.getLong("PersonID"),
                                    rs.getLong("InstitutionID")),
                    ticket.getTicketId(), operationType
            );
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Извличане на данни за синхронизация с Azure. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public List<AzureTeacherCurriculum> getTeacherCurriculumList(TicketData ticket, Short operationType) {
        try {
            String teacherSql = "SELECT DISTINCT s.PersonID, c.CurriculumID " +
                    "  FROM " + databaseConfig.getStagingSchema() + ".CurriculumTeacher c, " +
                    "       " + databaseConfig.getStagingSchema() + ".StaffPosition s, " +
                    "       core.Person p " +
                    " WHERE c.StaffPositionID = s.StaffPositionID " +
                    "   AND s.PersonID = p.PersonID" +
                    "   AND p.AzureID IS NOT NULL" +
                    "   AND c.TicketID = s.TicketID " +
                    "   AND c.TicketID = ? " +
                    "   AND c.OperationType = ? " +
                    "  ORDER BY s.PersonID, c.CurriculumID";
            List<Pair> pairs = jdbcTemplate.query(
                    teacherSql,
                    (rs, rowNum) -> new Pair(rs.getLong("PersonID"),
                                             rs.getLong("CurriculumID")),
                    ticket.getTicketId(), operationType
            );
            List<AzureTeacherCurriculum> result = new ArrayList<>();
            AzureTeacherCurriculum teacher = null;
            for (Pair pair : pairs) {
                if (teacher == null || !teacher.getPersonId().equals(pair.getMainId())) {
                    if (teacher != null) {
                        result.add(teacher);
                    }
                    teacher = new AzureTeacherCurriculum(pair.getMainId(), new ArrayList<>());
                }
                teacher.getCurriculumIds().add(pair.getChildId());
            }
            result.add(teacher);
            return result;
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Извличане на данни за синхронизация с Azure. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);
        }
    }

    @Override
    public List<AzureCurriculumCreate> getCurriculumListCreate(TicketData ticket) {
        try {
            List<AzureCurriculum> curriculums = getCurriculumList(ticket, (short) 1);
            List<AzureCurriculumCreate> result = new ArrayList<>();
            for (AzureCurriculum item : curriculums) {
                List<Long> teachers = getTeacherList(ticket.getTicketId(), item.getCurriculumId(), AzureOperationEnum.create);
                List<Long> students = getStudentList(ticket.getTicketId(), item.getCurriculumId(), AzureOperationEnum.create);
                AzureCurriculumCreate curriculum = new AzureCurriculumCreate(item.getCurriculumId(),
                        Stream.concat(teachers.stream(), students.stream()).collect(Collectors.toList()),
                        item.getInstitutionId());
                result.add(curriculum);
            }
            return result;
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Извличане на данни за синхронизация с Azure. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);

        }
    }

    @Override
    public List<AzureCurriculumUpdate> getCurriculumListUpdate(TicketData ticket) {
        try {
            List<AzureCurriculum> curriculums = getCurriculumList(ticket, (short) 2);
            List<AzureCurriculumUpdate> result = new ArrayList<>();
            for (AzureCurriculum item : curriculums) {
                List<Long> teachersCreated = getTeacherList(ticket.getTicketId(), item.getCurriculumId(), AzureOperationEnum.create);
                List<Long> studentsCreated = getStudentList(ticket.getTicketId(), item.getCurriculumId(), AzureOperationEnum.create);
                List<Long> teachersDeleted = getTeacherList(ticket.getTicketId(), item.getCurriculumId(), AzureOperationEnum.delete);
                List<Long> studentsDeleted = getStudentList(ticket.getTicketId(), item.getCurriculumId(), AzureOperationEnum.delete);
                AzureCurriculumUpdate curriculum = new AzureCurriculumUpdate(item.getCurriculumId(),
                        Stream.concat(teachersCreated.stream(), studentsCreated.stream()).collect(Collectors.toList()),
                        Stream.concat(teachersDeleted.stream(), studentsDeleted.stream()).collect(Collectors.toList()),
                        item.getInstitutionId());
                result.add(curriculum);
            }
            return result;
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Извличане на данни за синхронизация с Azure. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);

        }
    }

    @Override
    public List<AzureCurriculumDelete> getCurriculumListDelete(TicketData ticket) {
        try {
            List<AzureCurriculum> curriculums = getCurriculumList(ticket, (short) 3);
            List<AzureCurriculumDelete> result = new ArrayList<>();
            for (AzureCurriculum item : curriculums) {
                AzureCurriculumDelete curriculum = new AzureCurriculumDelete(item.getCurriculumId());
                result.add(curriculum);
            }
            return result;
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Извличане на данни за синхронизация с Azure. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);

        }
    }

    @Override
    public List<AzureStudentCurriculum> getStudentCurriculumList(TicketData ticket, Short operationType) {
        try {
            String studentSql = "SELECT DISTINCT c.CurriculumID, c.PersonID " +
                    "  FROM " + databaseConfig.getStagingSchema() + ".CurriculumStudent c, " +
                    "       core.Person p " +
                    " WHERE c.PersonID = p.PersonID " +
                    "   AND p.AzureID IS NOT NULL" +
                    "   AND c.TicketID = ? " +
                    "   AND c.OperationType = ? " +
                    "  ORDER BY c.CurriculumID, c.PersonID";
            List<Pair> pairs = jdbcTemplate.query(
                    studentSql,
                    (rs, rowNum) -> new Pair(rs.getLong("CurriculumID"),
                            rs.getLong("PersonID")),
                    ticket.getTicketId(), operationType
            );
            List<AzureStudentCurriculum> result = new ArrayList<>();
            AzureStudentCurriculum student = null;
            for (Pair pair : pairs) {
                if (student == null || !student.getCurriculumId().equals(pair.getMainId())) {
                    if (student != null) {
                        result.add(student);
                    }
                    student = new AzureStudentCurriculum(pair.getMainId(), new ArrayList<>(), ticket.getInstitutionId());
                }
                student.getPersonIds().add(pair.getChildId());
            }
            result.add(student);
            return result;
        } catch (DataAccessException e) {
            String message = ticket.getTicketId() + ": Извличане на данни за синхронизация с Azure. (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError(ticket, ApiOperationEnum.AZURE, message);
            throw new InternalServerErrorException(message);

        }
    }

    private List<AzureCurriculum> getCurriculumList(TicketData ticket, Short operationType) {
        String curriculumSql = "SELECT CurriculumID, InstitutionID " +
                " FROM " + databaseConfig.getStagingSchema() + ".Curriculum " +
                "WHERE TicketID = ? " +
                "  AND OperationType = ? ";
        return jdbcTemplate.query(
                curriculumSql,
                (rs, rowNum) ->
                        new AzureCurriculum(
                                rs.getLong("CurriculumID"),
                                rs.getLong("InstitutionID")),
                ticket.getTicketId(), operationType
        );
    }

    private List<Long> getTeacherList(String ticket, Long curriculumId, AzureOperationEnum operation) {
        String teacherSql = "SELECT s.PersonID " +
                " FROM " + databaseConfig.getStagingSchema() + ".CurriculumTeacher c, " +
                "      " + databaseConfig.getStagingSchema() + ".StaffPosition s, " +
                "       core.Person p " +
                " WHERE s.PersonID = p.PersonID " +
                "   AND p.AzureID IS NOT NULL" +
                "   AND c.StaffPositionID = s.StaffPositionID " +
                "   AND c.TicketID = s.TicketID " +
                "   AND c.TicketID = ? " +
                "   AND c.OperationType = ? " +
                "   AND c.CurriculumID = ? ";
        return jdbcTemplate.query(
                teacherSql,
                (rs, rowNum) -> rs.getLong("PersonID"),
                ticket, operation.getOperationType(), curriculumId
        );
    }

    private List<Long> getStudentList(String ticket, Long curriculumId, AzureOperationEnum operation) {
        String teacherSql = "SELECT s.PersonID " +
                " FROM " + databaseConfig.getStagingSchema() + ".CurriculumStudent s, " +
                "       core.Person p " +
                " WHERE s.PersonID = p.PersonID " +
                "   AND p.AzureID IS NOT NULL" +
                "   AND s.TicketID = ? " +
                "   AND s.OperationType = ? " +
                "   AND s.CurriculumID = ? ";
        return jdbcTemplate.query(
                teacherSql,
                (rs, rowNum) -> rs.getLong("PersonID"),
                ticket, operation.getOperationType(), curriculumId
        );
    }

    @Getter
    @Setter
    @NoArgsConstructor
    @AllArgsConstructor
    public static class Pair {
        private Long mainId;
        private Long childId;
    }
}
