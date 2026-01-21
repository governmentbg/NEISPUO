package bg.adminsoft.neispuo.model;

public enum ApiOperationEnum {
    SOB_API("SOB API"),
    IMPORT("SOB API"),
    EXPORT("SOB API"),
    DOCUMENTATION("SOB API"),
    AZURE("AzureCall"),
    AZURE_RECALL("AzureRepeatCall");

    private final String userName;

    public String getUserName() {
        return userName;
    }

    private ApiOperationEnum(String userName) {
        this.userName = userName;
    }
}
