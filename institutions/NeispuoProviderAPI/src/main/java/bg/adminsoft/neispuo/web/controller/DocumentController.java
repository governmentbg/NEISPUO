package bg.adminsoft.neispuo.web.controller;

import bg.adminsoft.neispuo.service.DocumentationService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping({ "/documentation" })
public class DocumentController {

    private final DocumentationService documentationService;

    public DocumentController(DocumentationService documentationService) {
        this.documentationService = documentationService;
    }

    @Operation(summary = "Export format documentation")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Export format documentation",
                    content = { @Content(mediaType = "application/json",
                            schema = @Schema(implementation = String.class)) })
    })
    @GetMapping(path = "export", produces = MediaType.APPLICATION_JSON_VALUE)
    public String exportDocument() {
        return this.documentationService.exportDocument();
    }


    @Operation(summary = "Import format documentation")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Import format documentation",
                    content = { @Content(mediaType = "application/json",
                            schema = @Schema(implementation = String.class)) })
    })
    @GetMapping(path = "import", produces = MediaType.APPLICATION_JSON_VALUE)
    public String importDocument() {
        return this.documentationService.importDocument();
    }
}
