import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EntityType, SyncHistoryEntityEnum } from './shared/types/sync-history.type';

const ENTITY_PARAM_MAP: Record<EntityType, string> = {
    institutions: 'institutionID',
    teachers: 'personID',
    students: 'personID',
    parents: 'personID',
};

@Component({
    selector: 'app-sync-history',
    templateUrl: './sync-history.component.html',
    styleUrls: ['./sync-history.component.scss'],
})
export class SyncHistoryComponent implements OnInit {
    entityType: EntityType | '' = '';

    entityId: string = '';

    SyncHistoryEntityEnum = SyncHistoryEntityEnum;

    constructor(private route: ActivatedRoute) {}

    ngOnInit() {
        this.setEntityTypeAndId();
    }

    private setEntityTypeAndId() {
        const url = this.route.snapshot.url;
        if (url.length > 1) {
            const type = url[0].path as EntityType;
            this.entityType = type;
            const paramName = ENTITY_PARAM_MAP[type];

            if (!paramName) {
                this.entityId = '';
                return;
            }

            const paramValue = this.route.snapshot.paramMap.get(paramName);
            this.entityId = paramValue || '';
        }
    }
}
