import { Test, TestingModule } from '@nestjs/testing';
import { RequestIDGeneratorService } from './request-id-generator.service';

describe('RequestIDGeneratorService', () => {
    let service: RequestIDGeneratorService;

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            providers: [RequestIDGeneratorService],
        }).compile();

        service = module.get<RequestIDGeneratorService>(RequestIDGeneratorService);
    });

    it('should be defined', () => {
        expect(service).toBeDefined();
    });
});
