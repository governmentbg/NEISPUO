import {
  BadRequestException,
  HttpException,
  HttpStatus,
  Injectable,
} from '@nestjs/common';
import { ParentRegisterDTO } from 'src/shared/dto/parent-register.dto';
import * as https from 'https';
import axios from 'axios';
const passwordValidator = new (require('password-validator'))();

/* Docs: https://www.npmjs.com/package/password-validator */
passwordValidator
  .is()
  .min(8)
  .is()
  .max(100)
  .has()
  .uppercase()
  .has()
  .lowercase()
  .has()
  .digits(1)
  .has()
  .not()
  .spaces();

function checkIfEmailDomainIsValid(email: string) {
  if (email && email.indexOf('@') !== -1) {
    let [_, domain] = email.split('@');
    if (domain === 'edu.mon.bg' || domain === 'mon.bg') {
      return false;
    }
  }
  return true;
}

@Injectable()
export class ParentRegisterService {
  constructor() {}

  async registerParent(createParentDTO: ParentRegisterDTO) {
    const lettersPattern = /^[a-zA-Zа-яА-Я- ]+$/;

    const validationErrors = passwordValidator.validate(
      createParentDTO.password,
      {
        details: true,
      },
    );

    if (
      !lettersPattern.test(createParentDTO.firstName) ||
      (createParentDTO.middleName &&
        !lettersPattern.test(createParentDTO.middleName)) ||
      !lettersPattern.test(createParentDTO.lastName)
    ) {
      throw new BadRequestException('Name must not contain special symbols!');
    }

    if (validationErrors.length > 0) {
      throw new BadRequestException(validationErrors);
    }

    if (!checkIfEmailDomainIsValid(createParentDTO.email)) {
      throw new BadRequestException('Invalid email domain!');
    }

    const httpsAgent = new https.Agent({ rejectUnauthorized: false });

    const httpOptions = {
      headers: {
        'Content-Type': 'application/json',
        'x-api-key': process.env.INTERNAL_API_KEY,
      },
      httpsAgent: httpsAgent,
    };

    return await axios
      .post(
        `${process.env.USERMNG_SERVER_URL}${process.env.CREATE_PARENT_URI}`,
        createParentDTO,
        httpOptions,
      )
      .then(res => {})
      .catch(err => {
        if (
          err?.response?.data?.message &&
          (err?.response?.data?.statusCode ?? err?.response?.data?.status)
        ) {
          throw new HttpException(
            err?.response?.data?.message,
            err?.response?.data?.statusCode ?? err?.response?.data?.status,
          );
        }
        throw new HttpException(
          `Вътрешна грешка. Свържете се с администратор.`,
          HttpStatus.INTERNAL_SERVER_ERROR,
        );
      });
  }
}
