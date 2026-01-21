package bg.adminsoft.neispuo.exception;

import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ControllerAdvice;
import org.springframework.web.bind.annotation.ExceptionHandler;

@ControllerAdvice
@Slf4j
public class NeispuoExceptionHandler {

    @ExceptionHandler(value = {InvalidStatusException.class})
    public ResponseEntity<ExceptionBody> handleInvalidStatusException(InvalidStatusException exception) {
        log.error(exception.getMessage());
        return new ResponseEntity<>(new ExceptionBody(HttpStatus.BAD_REQUEST, exception.getMessage()),
                HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(value = {InvalidImportPeriodException.class})
    public ResponseEntity<ExceptionBody> handleInvalidImportPeriodException(InvalidImportPeriodException exception) {
        log.error(exception.getMessage());
        return new ResponseEntity<>(new ExceptionBody(HttpStatus.FORBIDDEN, exception.getMessage()),
                HttpStatus.FORBIDDEN);
    }

    @ExceptionHandler(value = {ForegnKeyViolationException.class})
    public ResponseEntity<ExceptionBody> handleForegnKeyViolationException(ForegnKeyViolationException exception) {
        log.error(exception.getMessage());
        return new ResponseEntity<>(new ExceptionBody(HttpStatus.BAD_REQUEST, exception.getMessage()),
                HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(value = {BadRequestException.class})
    public ResponseEntity<ExceptionBody> handleBadRequestException(BadRequestException exception) {
        log.error(exception.getMessage());
        return new ResponseEntity<>(new ExceptionBody(HttpStatus.BAD_REQUEST, exception.getMessage()),
                HttpStatus.BAD_REQUEST);
    }

    @ExceptionHandler(value = {UnauthorizedException.class})
    public ResponseEntity<ExceptionBody> handleUnauthorizedException(UnauthorizedException exception) {
        log.error(exception.getMessage());
        return new ResponseEntity<>(new ExceptionBody(HttpStatus.UNAUTHORIZED, exception.getMessage()),
                HttpStatus.UNAUTHORIZED);
    }

    @ExceptionHandler(value = {InternalServerErrorException.class})
    public ResponseEntity<ExceptionBody> handleInternalServerErrorException(InternalServerErrorException exception) {
        log.error(exception.getMessage());
        return new ResponseEntity<>(new ExceptionBody(HttpStatus.INTERNAL_SERVER_ERROR, exception.getMessage()),
                HttpStatus.BAD_REQUEST);
    }


    static class ExceptionBody {
        private final int status;
        private final String error;
        private final String message;

        public ExceptionBody(HttpStatus status, String message) {
            super();
            this.status = status.value();
            this.error = status.getReasonPhrase();
            this.message = message;
        }

        public int getStatus() {
            return status;
        }

        public String getError() {
            return error;
        }

        public String getMessage() {
            return message;
        }
    }
}
