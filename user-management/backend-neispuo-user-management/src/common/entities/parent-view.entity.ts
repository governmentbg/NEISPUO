import { Column, ViewEntity } from 'typeorm';

@ViewEntity({ schema: 'azure_temp', name: 'ParentView' })
export class ParentViewEntity {
    @Column()
    username: string;

    @Column()
    threeNames: string;

    @Column()
    personID: string;

    @Column()
    azureID: string;
}
