package bg.adminsoft.neispuo.service;

import bg.adminsoft.neispuo.config.MetadataConfig;
import bg.adminsoft.neispuo.exception.InternalServerErrorException;
import bg.adminsoft.neispuo.repository.DocumentationRepository;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.core.io.FileSystemResource;
import org.springframework.stereotype.Service;

import java.nio.charset.StandardCharsets;

@Service
@Slf4j
@RequiredArgsConstructor
public class DocumentationServiceImpl extends BaseServiceImpl implements DocumentationService {

    private static String exportDocument = null;
    private static String importDocument = null;

    private final MetadataConfig metadataConfig;
    private final DocumentationRepository documentationRepository;

    @Override
    public String exportDocument() {
        if (exportDocument != null) {
            return exportDocument;
        }
        String exportJson = getExportJson();
        return exportDocument = this.documentationRepository.exportDocument(exportJson);
    }

    @Override
    public String importDocument() {
        if (importDocument != null) {
            return importDocument;
        }
        return ""; // t o d o
    }

    private String getExportJson() {
        FileSystemResource exportMapping = metadataConfig.getExportMappingFile();
        try {
            if (exportMapping != null) {
                String json = new String(exportMapping.getInputStream().readAllBytes(), StandardCharsets.UTF_8);
                if (!isJSONValid(json)) {
                    throw new InternalServerErrorException("Invalid json format for EXPORT JSON");
                }
                return json;
            } else {
                throw new InternalServerErrorException("Wrong or missing EXPORT JSON");
            }
        } catch (Exception e) {
            log.error(e.getMessage());
            throw new InternalServerErrorException("Wrong or missing EXPORT JSON");
        }
    }
}
