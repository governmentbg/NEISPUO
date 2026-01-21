package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.config.DatabaseConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.TicketAzureSync;
import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.model.azure.*;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBAzureRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.List;
import java.util.concurrent.Semaphore;

@Service
@Slf4j
@RequiredArgsConstructor
public class AzureServiceImpl extends BaseServiceImpl implements AzureService {

    private static final SimpleDateFormat dateFormat = new SimpleDateFormat("MM/dd/yyyy HH:mm:ss");

    public static final Semaphore semaphore = new Semaphore(1);

    private final DatabaseConfig databaseConfig;
    private final DBTicketRepository dbTicketRepository;
    private final DBAzureRepository dbAzureRepository;
    private final DBAuditLogRepository dbAuditLogRepository;

    @Value("${com.adminsoft.neispuo.config.azure.url}")
    private String azureBaseUrl;

    @Value("${com.adminsoft.neispuo.config.azure.api.key}")
    private String azureApiKey;

    @Value("${com.adminsoft.neispuo.config.azure.recall.message}")
    private String recallMessage;

    private static final String azurePrefix = ""; // ""/v1/azure-integrations";

    private final RestTemplate restTemplateAzure;

    @Override
    public void azureSync() {
        try {
            TicketData ticket;
            while (true) {
                try {
                    semaphore.acquire();
                    ticket = dbTicketRepository.getTicketForAzureSync();
                    if (ticket != null && ticket.getTicketId() != null) {
                        dbTicketRepository.setTicketAzureSync(ticket, TicketAzureSync.IN_PROGRESS);
                    }
                    semaphore.release();
                } catch (InternalServerErrorException e) {
                    semaphore.release();
                    Thread.sleep(60 * 1000L);
                    continue;
                }
                try {
                    if (ticket != null && ticket.getTicketId() != null) {
                        log.info("Begin processing ticket (azure sync) " + ticket.getTicketId() + " ...");
                        try {
                            teacherInstitutionAzureSync(ticket);
                            curriculumAzureSync(ticket);
                            //teacherCurriculumSync(ticket);
                            //studentCurriculumSync(ticket);
                            dbTicketRepository.setTicketAzureSync(ticket, TicketAzureSync.COMPLETED);
                        } catch (Exception e) {
                            log.error("Ticket " + ticket.getTicketId() + " failed (azure sync)");
                            dbTicketRepository.setTicketAzureSync(ticket, TicketAzureSync.FAILED);
                        }
                    } else {
                        Thread.sleep(5 * 1000L); //t o d o
                    }
                } catch (InternalServerErrorException e) {
                    Thread.sleep(5 * 1000L); //t o d o
                }
            }
        } catch (InterruptedException e) {
            // t o d o
            String message = "Azure task InterruptedException: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.AZURE, message);
        } catch (Exception e) {
            // t o d o
            String message = "Azure task Exception: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.AZURE, message);
        }
    }

    /*@Scheduled(cron = "${com.adminsoft.neispuo.config.azure.recall.cron}")
    public void azureRecallSync() {
        log.info("Azure Recall at " + dateFormat.format(new Date()));
        try {
            Timestamp startTime = getRecallTime(-1);
            Timestamp endTime = getRecallTime(0);
            Long minId = 0L;
            while (startTime.before(endTime)) {
                List<AzureError> errors = dbAuditLogRepository.getAzureErrors(startTime, minId, recallMessage);
                if (errors.size() > 0) {
                    for (AzureError error : errors) {
                        startTime = error.getErrorDateTime();
                        minId = error.getId();
                        if (startTime.before(endTime)) {
                            AzureResult result = recallSync(error.getAddress(), error.getJson());
                        } else {
                            break;
                        }
                    }
                } else {
                    startTime = endTime;
                }
            }
        } catch (Exception e) {
            // t o d o
            String message = "Azure recall task Exception: (" + e.getMessage() + ")";
            log.error(message);
            dbAuditLogRepository.logDBError("", ApiOperationEnum.AZURE, message);
        }
    }*/

    private void teacherInstitutionAzureSync(TicketData ticket) {
        try {
            List<AzureTeacherInstitution> createList = dbAzureRepository.getTeacherInstitutionList(ticket, (short) 1);
            for (AzureTeacherInstitution teacher : createList) {
                AzureResult result = teacherInstitutionSync(teacher, AzureOperationEnum.create);
            }
            List<AzureTeacherInstitution> deleteList = dbAzureRepository.getTeacherInstitutionList(ticket, (short) 3);
            for (AzureTeacherInstitution teacher : deleteList) {
                AzureResult result = teacherInstitutionSync(teacher, AzureOperationEnum.delete);
            }
        } catch (Exception e) {
            //log.error(e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }
    }

    private void curriculumAzureSync(TicketData ticket) {
        try {
            List<AzureCurriculumCreate> createList = dbAzureRepository.getCurriculumListCreate(ticket);
            for (AzureCurriculumCreate curriculum : createList) {
                AzureResult result = curriculumSync(curriculum, AzureOperationEnum.create);
            }
            List<AzureCurriculumUpdate> updateList = dbAzureRepository.getCurriculumListUpdate(ticket);
            for (AzureCurriculumUpdate curriculum : updateList) {
                if (curriculum.getPersonIDsToCreate() != null && curriculum.getPersonIDsToCreate().size() > 0 ||
                        curriculum.getPersonIDsToDelete() != null && curriculum.getPersonIDsToDelete().size() > 0) {
                    AzureResult result = curriculumSync(curriculum, AzureOperationEnum.update);
                }
            }
            List<AzureCurriculumDelete> deleteList = dbAzureRepository.getCurriculumListDelete(ticket);
            for (AzureCurriculumDelete curriculum : deleteList) {
                AzureResult result = curriculumSync(curriculum, AzureOperationEnum.delete);
            }
        } catch (Exception e) {
            //log.error(e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }
    }

    /*private void teacherCurriculumSync(TicketData ticket) {
        try {
            List<AzureTeacherCurriculum> createList = dbAzureRepository.getTeacherCurriculumList(ticket, (short) 1);
            for (AzureTeacherCurriculum teacher : createList) {
                AzureResult result = teacherCurriculumSync(teacher, AzureOperationEnum.create);
            }
            List<AzureTeacherCurriculum> deleteList = dbAzureRepository.getTeacherCurriculumList(ticket, (short) 3);
            for (AzureTeacherCurriculum teacher : deleteList) {
                AzureResult result = teacherCurriculumSync(teacher, AzureOperationEnum.delete);
            }
        } catch (Exception e) {
            //log.error(e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }
    }*/

    /*private void studentCurriculumSync(TicketData ticket) {
        try {
            List<AzureStudentCurriculum> createList = dbAzureRepository.getStudentCurriculumList(ticket, (short) 1);
            for (AzureStudentCurriculum student : createList) {
                AzureResult result = studentCurriculumSync(student, AzureOperationEnum.create);
            }
            List<AzureStudentCurriculum> deleteList = dbAzureRepository.getStudentCurriculumList(ticket, (short) 3);
            for (AzureStudentCurriculum student : deleteList) {
                AzureResult result = studentCurriculumSync(student, AzureOperationEnum.delete);
            }
        } catch (Exception e) {
            //log.error(e.getMessage());
            throw new InternalServerErrorException(e.getMessage());
        }
    }*/

    private AzureResult curriculumSync(Object curriculumRequest,
                                       AzureOperationEnum operation) throws JsonProcessingException {

        try {
            ResponseEntity<AzureResult> response =
                    restTemplateAzure.postForEntity(azureBaseUrl + azurePrefix +
                                    "/class/" + operation.name(),
                            new HttpEntity<>(curriculumRequest, getAzureHeaders()),
                            AzureResult.class);

            return response.getBody();
        } catch (Exception e) {
            String address = azureBaseUrl + azurePrefix + "/class/" + operation.name();
            String data = new ObjectMapper().writeValueAsString(curriculumRequest);
            dbAuditLogRepository.logAzureDBError(ApiOperationEnum.AZURE, address, e.getMessage(), data);
            log.error(e.getMessage());
        }
        return null;
    }

    private AzureResult teacherInstitutionSync(AzureTeacherInstitution teacherInstitutionRequest,
                                          AzureOperationEnum operation) throws JsonProcessingException {
        try {
            ResponseEntity<AzureResult> response =
                    restTemplateAzure.postForEntity(azureBaseUrl + azurePrefix +
                                    "/teacher/enrollment-school-" + operation.name(),
                            new HttpEntity<>(teacherInstitutionRequest, getAzureHeaders()),
                            AzureResult.class);

            return response.getBody();
        } catch (Exception e) {
            String address = azureBaseUrl + azurePrefix + "/teacher/enrollment-school-" + operation.name();
            String data = new ObjectMapper().writeValueAsString(teacherInstitutionRequest);
            dbAuditLogRepository.logAzureDBError(ApiOperationEnum.AZURE, address, e.getMessage(), data);
            log.error(e.getMessage());
        }
        return null;
    }

    private AzureResult recallSync(String address, String data) throws JsonProcessingException {
        try {
            ResponseEntity<AzureResult> response =
                    restTemplateAzure.postForEntity(address,
                            new HttpEntity<>(data, getAzureHeaders()),
                            AzureResult.class);

            return response.getBody();
        } catch (Exception e) {
            dbAuditLogRepository.logAzureDBError(ApiOperationEnum.AZURE_RECALL, address, e.getMessage(), data);
            log.error(e.getMessage());
        }
        return null;
    }

    /*private AzureResult teacherCurriculumSync(AzureTeacherCurriculum teacherCurriculumRequest,
                                         AzureOperationEnum operation) {
        try {
            ResponseEntity<AzureResult> response =
                    restTemplateAzure.postForEntity(azureBaseUrl + azurePrefix +
                                    "/teacher/enrollment-class-" + operation.name(),
                            new HttpEntity<>(teacherCurriculumRequest, getAzureHeaders()),
                            AzureResult.class);

            return response.getBody();
        } catch (Exception ignored) { }
        return null;
    }*/


    /*private AzureResult studentCurriculumSync(AzureStudentCurriculum studentCurriculumRequest,
                                         AzureOperationEnum operation) {
        try {
            ResponseEntity<AzureResult> response =
                    restTemplateAzure.postForEntity(azureBaseUrl + azurePrefix +
                                    "/student/enrollment-class-" + operation.name(),
                            new HttpEntity<>(studentCurriculumRequest, getAzureHeaders()),
                            AzureResult.class);

            return response.getBody();
        } catch (Exception ignored) { }
        return null;
    }*/

    private HttpHeaders getAzureHeaders() {
        HttpHeaders headers = new HttpHeaders();
        //headers.set("Content-Type", "application/json");
        headers.set("X-API-KEY", azureApiKey);
        return headers;
    }

    private Timestamp getRecallTime(int offset) {
        Calendar calendar = Calendar.getInstance();
        calendar.add(Calendar.DATE, offset);
        calendar.set(Calendar.HOUR_OF_DAY, 0);
        calendar.set(Calendar.MINUTE, 0);
        calendar.set(Calendar.SECOND, 0);
        calendar.set(Calendar.MILLISECOND, 0);
        return new Timestamp(calendar.getTime().getTime());
    }
}
