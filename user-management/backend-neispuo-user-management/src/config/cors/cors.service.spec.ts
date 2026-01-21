import { Test, TestingModule } from '@nestjs/testing';
import { CorsService } from './cors.service';

describe('CorsService', () => {
    let service: CorsService;

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            providers: [CorsService],
        }).compile();

        service = module.get<CorsService>(CorsService);
    });

    it('should be defined', () => {
        expect(service).toBeDefined();
    });
});
