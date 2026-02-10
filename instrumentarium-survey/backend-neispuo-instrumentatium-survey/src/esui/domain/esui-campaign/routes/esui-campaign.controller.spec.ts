import { CampaignService } from '../../../../domain/campaigns/routes/campaign.service';
import { TeachersQuestionaireReminderService } from '../../../../domain/notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service';
import { Test, TestingModule } from '@nestjs/testing';
import { DSSMailService } from '../../../../shared/services/dss-mail/dss-mail.service';
import { EsuiCampaignController } from './esui-campaign.controller';
import { Campaign } from '../../../../domain/campaigns/campaign.entity';
import { getRepositoryToken } from '@nestjs/typeorm';
import { EsuiGuard } from '../../esui.guard';

describe('EsuiCampaignController', () => {
    let controller: EsuiCampaignController;
    let service: CampaignService;

    const mockCampaignService = {
        CampaignRepository: {},
        Connection: {},
        TeachersQuestionaireReminderService: {},
        hardcodePropertiesForESUICampaignCreation: () => { return 1 + 1 }
    };

    const repositoryMockFactory = jest.fn(() => ({
        hardcodePropertiesForESUICampaignCreation: jest.fn(entity => entity),
    }));

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            controllers: [EsuiCampaignController],
            providers: [
                { provide: EsuiGuard, useValue: jest.fn().mockImplementation(() => true) },
                { provide: CampaignService, useValue: mockCampaignService },
                { provide: TeachersQuestionaireReminderService, useValue: {} },
                { provide: DSSMailService, useValue: {} },
                { provide: getRepositoryToken(Campaign), useFactory: repositoryMockFactory },
            ]
        }).compile();

        controller = module.get<EsuiCampaignController>(EsuiCampaignController);
        service = module.get<CampaignService>(CampaignService);

    });

    it('should be defined', () => {
        expect(controller).toBeDefined();
    })
});
