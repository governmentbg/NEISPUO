package bg.adminsoft.neispuo.model.azure;

public enum AzureOperationEnum {
    create(1), update(2), delete(3);

    private final Integer operationType;

    public Integer getOperationType() {
        return operationType;
    }

    private AzureOperationEnum(Integer operationType) {
        this.operationType = operationType;
    }
}
