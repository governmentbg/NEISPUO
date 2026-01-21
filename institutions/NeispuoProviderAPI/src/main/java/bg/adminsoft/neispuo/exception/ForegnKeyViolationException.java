package bg.adminsoft.neispuo.exception;

public class ForegnKeyViolationException extends RuntimeException {

    private static final long serialVersionUID = 1L;

    public ForegnKeyViolationException(String message) {
        super(message);
    }

}
