package bg.adminsoft.neispuo.model;

public enum TicketAzureSync {
    PENDING(0, "Очаква обработка"),
    IN_PROGRESS(1, "В процес на обработка"),
    FAILED(4, "Неуспешно зареждане на данните"),
    COMPLETED(5, "Успешно заредени данни"),
    STOPPED(99, "Спрян - технически проблем"),
    UNKNOWN(null, "Неизвестен"),;

    private final Integer status;
    private final String description;

    private TicketAzureSync(Integer status, String description) {
        this.status = status;
        this.description = description;
    }

    public Integer getStatus() {
        return status;
    }

    public String getDescription() {
        return description;
    }

    public static TicketAzureSync getByStatus(int status) {
        for(TicketAzureSync statusEnum : TicketAzureSync.values()){
            if( statusEnum.status == status) return statusEnum;
        }
        return null;
    }

    public static String getDescriptionByStatus(int status) {
        for(TicketAzureSync statusEnum : TicketAzureSync.values()){
            if( statusEnum.status == status) return statusEnum.description;
        }
        return "UNKNOWN";
    }
}
