package bg.adminsoft.neispuo.model;

import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.sql.Timestamp;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
public class TicketData {
    String ticketId;
    Long institutionId;
    Integer schoolYear;
    Integer providerId;
    TicketStatusEnum status;
    TicketTypeEnum ticketType;
    TicketSubTypeEnum ticketSubType;
    Integer sysUserId;
    String remoteIpAddress;
    String userAgent;
    Timestamp created;

    public TicketData(String ticketId) {
        this.ticketId = ticketId;
    }

    public TicketData(String ticketId, Long institutionId, Integer schoolYear) {
        this.ticketId = ticketId;
        this.institutionId = institutionId;
        this.schoolYear = schoolYear;
    }

    public TicketData(String ticketId, Long institutionId, Integer schoolYear,
                      String remoteIpAddress, String userAgent) {
        this.ticketId = ticketId;
        this.institutionId = institutionId;
        this.schoolYear = schoolYear;
        this.remoteIpAddress = remoteIpAddress;
        this.userAgent = userAgent;
    }
}
