package bg.adminsoft.neispuo.config;

import bg.adminsoft.neispuo.repository.DBTicketRepository;
import bg.adminsoft.neispuo.service.AzureService;
import bg.adminsoft.neispuo.service.ExportService;
import bg.adminsoft.neispuo.service.ImportService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.scheduling.concurrent.ThreadPoolTaskExecutor;

@Configuration
@Slf4j
@RequiredArgsConstructor
public class DataProcessingThreads {

    @Value("${com.adminsoft.neispuo.config.treads.import}")
    private Integer maxImportTreads;

    @Value("${com.adminsoft.neispuo.config.treads.export}")
    private Integer maxExportTreads;

    @Value("${com.adminsoft.neispuo.config.treads.azure}")
    private Integer maxAzureTreads;

    private final ImportService importService;
    private final ExportService exportService;
    private final AzureService azureService;
    private final DBTicketRepository dbTicketRepository;

    @Bean
    public void processingThreads() {

        dbTicketRepository.stopProcessingTickets();

        ThreadPoolTaskExecutor importExecutor = new ThreadPoolTaskExecutor();
        importExecutor.setCorePoolSize(maxImportTreads);
        importExecutor.afterPropertiesSet();
        for (int i = 0; i < maxImportTreads; i++) {
            final int tread = i;
            importExecutor.execute(() -> {
                log.info(String.format("Start import tread %d", tread));
                importService.importInstitutionData();
            });
        }

        ThreadPoolTaskExecutor exportExecutor = new ThreadPoolTaskExecutor();
        exportExecutor.setCorePoolSize(maxExportTreads);
        exportExecutor.afterPropertiesSet();
        for (int i = 0; i < maxExportTreads; i++) {
            final int tread = i;
            exportExecutor.execute(() -> {
                log.info(String.format("Start export tread %d", tread));
                exportService.exportInstitutionData();
            });
        }

        ThreadPoolTaskExecutor azureExecutor = new ThreadPoolTaskExecutor();
        azureExecutor.setCorePoolSize(maxAzureTreads);
        azureExecutor.afterPropertiesSet();
        for (int i = 0; i < maxAzureTreads; i++) {
            final int tread = i;
            azureExecutor.execute(() -> {
                log.info(String.format("Start azure tread %d", tread));
                azureService.azureSync();
            });
        }

    }
}
