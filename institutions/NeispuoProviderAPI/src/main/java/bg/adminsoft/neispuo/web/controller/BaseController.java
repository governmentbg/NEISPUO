package bg.adminsoft.neispuo.web.controller;

import bg.adminsoft.neispuo.exception.UnauthorizedException;
import bg.adminsoft.neispuo.model.ApiOperationEnum;
import bg.adminsoft.neispuo.model.ProviderInfo;
import bg.adminsoft.neispuo.model.TicketData;
import bg.adminsoft.neispuo.repository.DBAuditLogRepository;
import bg.adminsoft.neispuo.repository.DBTicketRepository;
import com.fasterxml.jackson.databind.ObjectMapper;
import lombok.extern.slf4j.Slf4j;

import javax.xml.bind.DatatypeConverter;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.security.cert.CertificateEncodingException;
import java.security.cert.CertificateFactory;
import java.security.cert.X509Certificate;
import java.util.Base64;

@Slf4j
public abstract class BaseController {

    protected final DBTicketRepository dbTicketRepository;
    protected final DBAuditLogRepository dbAuditLogRepository;

    protected BaseController(DBTicketRepository dbTicketRepository, DBAuditLogRepository dbAuditLogRepository) {
        this.dbTicketRepository = dbTicketRepository;
        this.dbAuditLogRepository = dbAuditLogRepository;
    }

    protected ProviderInfo checkCertificate(String clientCertificate, TicketData ticket,
                                            ApiOperationEnum operation) {
        ProviderInfo provider = new ProviderInfo(1, 3); // null; todo todo !!!
        try {
            InputStream targetStream = new ByteArrayInputStream(Base64.getDecoder().decode(clientCertificate));
            X509Certificate x509Certificate = (X509Certificate) CertificateFactory
                    .getInstance("X509")
                    .generateCertificate(targetStream);
            String thumbprint = getThumbprint(x509Certificate);
            provider = dbTicketRepository.checkAccess(thumbprint, ticket);
            if (provider == null) {
                log.error("Invalid client certificate (" + clientCertificate + ")");
                dbAuditLogRepository.logDBError(ticket, operation,
                        "Invalid client certificate (" + clientCertificate + ")");
                throw new UnauthorizedException("Invalid client certificate");
            };
        } catch (Exception e) {
            log.error(e.getMessage());
            dbAuditLogRepository.logDBError(ticket, operation, e.getMessage());
            throw new UnauthorizedException("Invalid client certificate");
        }
        return provider;
    }

    private String getThumbprint(X509Certificate cert)
            throws NoSuchAlgorithmException, CertificateEncodingException {
        MessageDigest md = MessageDigest.getInstance("SHA-1");
        md.update(cert.getEncoded());
        return DatatypeConverter.printHexBinary(md.digest()).toLowerCase();
    }

    protected boolean isJSONValid(String jsonString) {
        try {
            final ObjectMapper mapper = new ObjectMapper();
            mapper.readTree(jsonString);
            return true;
        } catch (IOException e) {
            return false;
        }
    }
}
