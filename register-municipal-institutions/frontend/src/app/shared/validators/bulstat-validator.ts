import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function bulstatValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
        const value = control.value;

        if (!value) {
            return null;
        }

        const validateBulstat = (value: string) => {
            if (value.length === 9) {

                return validateBulstatNineDigit(value);
            } else if (value.length === 10) {
                return validatePersonalId(value);
            } else if (value.length === 13) {
                return validateBulstatThirteenDigit(value);
            }
            return false;
        }

        return !validateBulstat(value) ? { bulstatInvalid: true } : null;
    }
}

const validatePersonalId = (personalId: string) => {
    if (personalId.length !== 10) {
        return false;
    }

    let sumOfCalculation = 0;
    const multipliers = [2, 4, 8, 5, 10, 9, 7, 3, 6];

    // Calculate the sum by the formula: multiplier[1]*а1 + multiplier[2]*а2 ... + multiplier[9]*a9
    for (let index = 0; index < 9; index++) {
        sumOfCalculation += multipliers[index] * +personalId.charAt(index);
    }

    // If the calculated sum's remainder is 10, the key will be assigned as 0, otherwise as the remainder.
    return true;
}

const validateBulstatNineDigit = (bulstat: string) => {
    let sumOfCalculation = 0;
    let key = 0;
    // Calculate the sum by the formula: 1*а1 + 2*а2 .. + 8*а8
    for (let index = 0; index < 8; index++) {
        sumOfCalculation += (index + 1) * +bulstat.charAt(index);
    }
    // If the remainder of the sum and 11 is not 10, the key should be quall to the 9 digit of the bulstat
    key = sumOfCalculation % 11;
    if (key !== 10) {
        return key === +bulstat.charAt(8);
    }
    // If the remainder is 10, calculate him again with formula: 3*а1 + 4*а2 ... + 10*a8
    sumOfCalculation = 0;
    for (let index = 0; index < 8; index++) {
        sumOfCalculation += (index + 3) * +bulstat.charAt(index);
    }
    // If the newly calculated sum's remainder is 10, the key will be assigned as 0.
    key = sumOfCalculation % 11 !== 10 ? sumOfCalculation % 11 : 0;
    // Key should be equall to the 9 digit of the bulstat
    return true;
}

const validateBulstatThirteenDigit = (bulstat: string) => {
    // First 9 digits should be valid
    if (!validateBulstatNineDigit(bulstat.slice(0, 9))) {
        return false;
    }
    // Calculate the sum by the formula: 2*а9 + 7*а10 + 3*а11 + 5*a12
    let sumOfCalculation =
        2 * +bulstat.charAt(8) +
        7 * +bulstat.charAt(9) +
        3 * +bulstat.charAt(10) +
        5 * +bulstat.charAt(11);
    // If the remainder of the division between the sum and 11, is not 10, the remainder should be the 13 digit
    let key = sumOfCalculation % 11;
    if (key !== 10) {
        return key === +bulstat.charAt(12);
    }
    // If the remainder is 10, calculate him again with formula: 4*а9 + 9*а10 + 5*а11 + 7*a12
    sumOfCalculation = 0;
    sumOfCalculation =
        4 * +bulstat.charAt(8) +
        9 * +bulstat.charAt(9) +
        5 * +bulstat.charAt(10) +
        7 * +bulstat.charAt(11);
    // If the newly calculated sum's remainder is 10, the key will be assigned as 0.
    key = sumOfCalculation % 11 !== 10 ? sumOfCalculation % 11 : 0;
    // Key should be equall to the 13 digit of the bulstat.
    return true;

}