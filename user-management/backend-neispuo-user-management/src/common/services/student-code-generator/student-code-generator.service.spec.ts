import { Test, TestingModule } from '@nestjs/testing';
import { StudentCodeGeneratorService } from './student-code-generator.service';

describe('StudentCodeGeneratorService', () => {
    let service: StudentCodeGeneratorService;

    beforeEach(async () => {
        const module: TestingModule = await Test.createTestingModule({
            providers: [StudentCodeGeneratorService],
        }).compile();

        service = module.get<StudentCodeGeneratorService>(StudentCodeGeneratorService);
    });

    it('should be defined', () => {
        expect(service).toBeDefined();
    });
});
