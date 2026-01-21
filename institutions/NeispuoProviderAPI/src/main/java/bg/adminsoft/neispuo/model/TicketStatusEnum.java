package bg.adminsoft.neispuo.model;

public enum TicketStatusEnum {
    // todo ????
    PENDING(0, "Очаква обработка"),
    IN_PROGRESS(1, "В процес на обработка"),
    TECHNICAL_VALIDATION_FAILED(2, "Неуспешна техническа валидация на данните"),
    LOGICAL_VALIDATION_FAILED(3, "Неуспешна логическа валидация на данните"),
    LOAD_DATA_FAILED(4, "Неуспешно зареждане на данните"),
    COMPLETED(5, "Успешно заредени данни"),
    STOPPED(99, "Спрян - технически проблем"),
    UNKNOWN(null, "Неизвестен"),;

    private final Integer status;
    private final String description;

    private TicketStatusEnum(Integer status, String description) {
        this.status = status;
        this.description = description;
    }

    public Integer getStatus() {
        return status;
    }

    public String getDescription() {
        return description;
    }

    public static TicketStatusEnum getByStatus(int status) {
        for(TicketStatusEnum statusEnum : TicketStatusEnum.values()){
            if( statusEnum.status == status) return statusEnum;
        }
        return null;
    }

    public static String getDescriptionByStatus(int status) {
        for(TicketStatusEnum statusEnum : TicketStatusEnum.values()){
            if( statusEnum.status == status) return statusEnum.description;
        }
        return "UNKNOWN";
    }
}
