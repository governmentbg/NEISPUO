import { CampaignService } from './campaign.service';
import { TeachersQuestionaireReminderService } from '../../notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service';
import { Test, TestingModule } from '@nestjs/testing';
import { Campaign } from '../campaign.entity';
import { getRepositoryToken } from '@nestjs/typeorm';
import { CampaignType } from '../enums/campaign.enum';
import { Connection } from 'typeorm';

describe('CampaignService', () => {
    let service: CampaignService;

    // https://github.com/nestjsx/crud/issues/207#issuecomment-593449874
    const metadataObj: any = { connection: { options: { type: null } }, columns: [], relations: [] }

    const mockRepo = () => ({
        create: jest.fn(),
        metadata: metadataObj,
      });

    const conn = {} as Connection;
    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            providers: [
                CampaignService,
                { provide: getRepositoryToken(Campaign), useFactory: mockRepo },
                { provide: Connection, useValue: conn },
                { provide: TeachersQuestionaireReminderService, useValue: {} },
            ]
        }).compile();

        service = module.get<CampaignService>(CampaignService);

    });

    it('should be defined', () => {
        expect(service).toBeDefined();
    });

    it('should successfully insert new campaign object', async () => {
        const dto = {
            name: 'campaignName',
            startDate: new Date('2021-11-01 00:00:00'),
            endDate: new Date('2021-11-16 00:00:00'),
            institutionId: 100004
        } as Campaign;
        
        const hardcodeFuncSpy = jest.spyOn(service, 'hardcodePropertiesForESUICampaignCreation');

        expect(service.hardcodePropertiesForESUICampaignCreation(dto)).toEqual({
            isActive: 0,
            isLocked: 0,
            type: CampaignType.ESUI,
            ...dto
        })

        expect(hardcodeFuncSpy).toBeCalledTimes(1);
    });
    
});