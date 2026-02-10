import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { EditInstitutionResponseDTO } from '@shared/business-object-model/responses/edit-institution-response.dto';
import { AzureOrganizationsService } from 'src/app/shared/services/azure-organizations.service';
import { InstitutionsService } from 'src/app/shared/services/institutions.service';

@Component({
    selector: 'app-update-institution-page',
    templateUrl: './update-institution-page.component.html',
    styleUrls: ['./update-institution-page.component.scss'],
})
export class UpdateInstitutionPageComponent implements OnInit {
    institution!: EditInstitutionResponseDTO;

    constructor(
        private institutionsService: InstitutionsService,
        private azureOrganizationsService: AzureOrganizationsService,
        private route: ActivatedRoute,
    ) {}

    async ngOnInit(): Promise<void> {
        let institutionID: number;
        this.route.paramMap.subscribe((params) => {
            institutionID = +params!.get('institutionID')!;
            this.getInstitution(institutionID);
        });
    }

    async getInstitution(institutionID: number) {
        this.institutionsService
            .getInstitution(institutionID)
            .pipe(take(1))
            .subscribe(
                (response) => {
                    this.institution = response;
                },
                (error) => {
                    console.log('error fetching paginated data', error);
                },
            );
    }

    updateInstitution() {
        this.azureOrganizationsService.updateAzureOrganization(this.institution).subscribe(
            (response) => {},
            (error) => {
                console.log('error fetching paginated data', error);
            },
        );
    }
}
