package bg.adminsoft.neispuo.model;

public enum ValidationTypeEnum {
    INFO(0), WARNING(1), ERROR(2);

    private final Integer value;

    private ValidationTypeEnum(Integer value) {
        this.value = value;
    }

    public Integer getValue() {
        return value;
    }

    public static ValidationTypeEnum getByValue(int value) {
        for(ValidationTypeEnum validationTypeEnum : ValidationTypeEnum.values()){
            if( validationTypeEnum.value == value) return validationTypeEnum;
        }
        return null;
    }
}
