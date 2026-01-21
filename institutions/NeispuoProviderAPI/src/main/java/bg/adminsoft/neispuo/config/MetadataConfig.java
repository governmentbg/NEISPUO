package bg.adminsoft.neispuo.config;

import lombok.Getter;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.ClassPathResource;
import org.springframework.core.io.FileSystemResource;

@Configuration
@Getter
public class MetadataConfig {

    @Value("${com.adminsoft.neispuo.config.directory}")
    private String configDirectory;

    @Value("${com.adminsoft.neispuo.config.validation.file}")
    //private ClassPathResource validationFile;
    private FileSystemResource validationFile;

    @Value("${com.adminsoft.neispuo.config.json.mapping.file}")
    //private ClassPathResource jsonMappingFile;
    private FileSystemResource jsonMappingFile;

    @Value("${com.adminsoft.neispuo.config.schema.mapping.file}")
    //private ClassPathResource schemaMappingFile;
    private FileSystemResource schemaMappingFile;

    @Value("${com.adminsoft.neispuo.config.export.mapping.file}")
    //private ClassPathResource exportMappingFile;
    private FileSystemResource exportMappingFile;

    @Value("${com.adminsoft.neispuo.config.export.id.mapping.file}")
    //private ClassPathResource exportIdMappingFile;
    private FileSystemResource exportIdMappingFile;

    @Value("${com.adminsoft.neispuo.config.params.audit.module.id}")
    private Integer apiModuleId;

    @Value("${com.adminsoft.neispuo.config.params.audit.role.id}")
    private Integer apiUserRoleId;

}
