<template>
  <v-row>
    <v-col
      cols="12"
      sm="4"
      xs="12"
    >
      <v-select
        id="personalIDType"
        v-model="personalIDType"
        :label="$t('createStudent.pinType')"
        :items="pinTypeOptions"
        :readonly="readonly"
        :disabled="disabled"
        :single-line="true"
        persistent-placeholder
        :error-messages="getRequiredFieldValidationMessage('personalIDType')"
        :return-object="true"
        @input="$emit('update:personalIDType', $event)"
      />
    </v-col>
    <v-col
      cols="12"
      sm="8"
      xs="12"
    >
      <v-text-field
        v-model="personalID"
        :disabled="disabled || personalIDType.value === -1"
        :label="$t('createStudent.pin')"
        :error-messages="pinErrors"
        clearable
        persistent-placeholder
        @blur="$v.personalID.$touch()"
        @input="$v.personalID.$touch(); $emit('update:personalID', $event)"
      >
        <template #prepend>
          <v-btn
            v-if="existingPerson"
            icon
            color="info"
            small
            @click.stop="onExistingPersonClick"
          >
            <v-icon>
              mdi-information-outline
            </v-icon>
          </v-btn>
        </template>
        <template
          v-if="!disabled && showGRAOSearch"
          v-slot:append-outer
        >
          <v-btn
            small
            tile
            class="ma-0"
            :disabled="!personalID"
            @click="getDataForPerson"
          >
            <v-icon>mdi-magnify</v-icon>
            {{ $t('common.GRAOSearch') }}
          </v-btn>
        </template>
      </v-text-field>
    </v-col>
    <v-dialog
      v-model="dialog"
      width="500"
    >
      <v-card
        v-if="existingPerson"
      >
        <v-card-title />
        <v-card-text>
          <v-simple-table>
            <template #default>
              <tbody>
                <tr>
                  <td>{{ $t('student.headers.firstName') }}</td>
                  <td>{{ existingPerson.fullName }}</td>
                </tr>
                <tr>
                  <td>{{ $t('student.headers.school') }}</td>
                  <td>{{ existingPerson.school }}</td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-row>
</template>

<script>
import { validationMixin } from 'vuelidate';
import { requiredIf } from 'vuelidate/lib/validators';
import Constants from "@/common/constants.js";
import { mapGetters } from 'vuex';
import { UserRole } from '@/enums/enums';
import Regix from "@/services/regix.service.js";

export default {
  name: "PersonUniqueId",
  mixins: [validationMixin],
  props: {
    showGRAOSearch: {
      type:Boolean,
        default(){
          return true;
        }
    },
    uniqueCheck:{
        type:Boolean
    },
    pinRequired:{
        type:Boolean,
        default(){
            return true;
        }
    },
    initialPinTypes:{
      type: Array,
      required: false,
      default(){
        return [];
      }
    },
    initialPersonalID: {
        type: String,
        default() {
            return undefined;
        }
    },
    initialPersonalType:{
        type: [String, Number],
        default(){
            '0';
        }
    },
    disabled:{
        type:Boolean,
        default() {
          return false;
        }
    },
    readonly:{
        type:Boolean,
        default() {
          return false;
        }
    },
    clearable:{
        type:Boolean,
        default() {
          return true;
        }
    },
    skipInitialSet: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
        pinTypeOptions: [],
        personalIDType: {
             type: Object,
             required: true,
        },
        personalID: {
          type: String,
          default() {
              return undefined;
          }
        },
        existingPerson: null,
        dialog: false,
        hasToValidateEgn: true,
        hasToValidateLnch: true,
    };
  },
  computed: {
    ...mapGetters(['egnAnonymization', 'isInRole']),
    pinErrors () {
      const errors = [];
      const field = this.$v.personalID;
      if (!field.$dirty) {
         return errors;
      }

      !field.required && errors.push(this.$t('validation.requiredField'));
      !field.isPinValid && errors.push(this.$t('validation.invalidIndentifier'));
      !field.isPinUnique && errors.push(this.$t('validation.existingIndentifier'));

      return errors;
    }
  },
  mounted() {
    this.loadAppConfiguration();

    this.$api.lookups
        .getPinTypes()
        .then((response) => {
          var pinTypes = response.data;
          if (pinTypes) {

            if(this.pinRequired === true){
               pinTypes = pinTypes.filter(function(e) { return e.value !== -1; });
            }

            if(this.initialPinTypes && this.initialPinTypes.length > 0){
              pinTypes = pinTypes.filter(e => this.initialPinTypes.includes(e.value) );
            }

            this.pinTypeOptions = pinTypes;

            if(!this.skipInitialSet) {
                if(this.initialPersonalType != undefined){
                    var selectedPinType = pinTypes.filter(x => x.value == this.initialPersonalType)[0];

                    if(selectedPinType == undefined){
                        selectedPinType = pinTypes.filter(x => x.text === this.initialPersonalType)[0];
                    }

                    this.personalIDType = selectedPinType;
                    this.$emit('update:personalIDType', selectedPinType);
                }else{
                  this.personalIDType = pinTypes[0];
                  this.$emit('update:personalIDType', pinTypes[0]);
                }
            }
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.pinOptionsLoad'));
          console.log(error);
        })
        .finally(() => {
          this.$v.$touch();
        });


    this.personalID = this.egnAnonymization ? (this.initialPersonalID ? Constants.AnonymizedEgn : this.initialPersonalID) : this.initialPersonalID;

    // Долното не може да се използва, тъй като и други роли трябва да въвеждат идентификатори
    // if(!this.isSchoolDirector() && this.personalID !== Constants.AnonymizedEgn){
    //     this.personalID = Constants.AnonymizedEgn;
    // }
  },
  validations: {
    personalIDType: {
        required: requiredIf(function () {
          return this.pinRequired;
    })
    },
    personalID: {
      required: requiredIf(function () {
        return this.pinRequired;
      }),
      isPinValid() {
        return this.validatePin(this.personalIDType, this.personalID);
      },
      async isPinUnique() {
        this.existingPerson = null;

        if(this.$v.personalID.isPinValid && this.personalID && this.uniqueCheck === true && this.personalID != this.initialPersonalID && this.personalID.length > 5)
        {
          this.existingPerson = (await this.$api.student.checkPinUniqueness(this.personalID))?.data;
          const isUnique = !this.existingPerson;

          if(isUnique === true && this.personalIDType.value === Constants.PIN_TYPE_EGN){
            this.fireExtractInfoFromEGNEvent(this.personalID);
          }

          return isUnique;
        }
        else
        {
          return true;
        }
      }
    },
  },
  methods:{
    loadAppConfiguration() {
      this.$api.appConfiguration.getValueByKey('ValidateEgn')
      .then(response => {
        this.hasToValidateEgn = response.data ? (response.data.toLowerCase() === "true") : false;
      })
      .catch(error => {
        console.log(error.response);
      });

      this.$api.appConfiguration.getValueByKey('ValidateLnch')
      .then(response => {
        this.hasToValidateLnch = response.data ? (response.data.toLowerCase() === "true") : false;
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    isSchoolDirector() {
      return this.isInRole(UserRole.School);
    },
    validatePin(pinType,pin) {
        if(pinType.value == Constants.PIN_TYPE_EGN){

          const isValid = this.validateEGN(pin);

          if(isValid === true && this.uniqueCheck === false){
             this.fireExtractInfoFromEGNEvent(pin);
          }

          return isValid;
        }else if(pinType.value == Constants.PIN_TYPE_LNCH){
           return this.validateLnch(pin);
        }else if(pinType.value == Constants.PIN_TYPE_IDN){
          if(pin){
            return true;
          }

          return false;
        }else{
           return true;
        }
    },
    fireExtractInfoFromEGNEvent(pin) {
      const birthDateAndGenderData = {
              birthDate: this.getBirthDateFromEGN(pin),
              genderId: this.getGenderIdFromEGN(pin)
          };

      this.$emit('extractBirthDateAndGenderFromEGN', birthDateAndGenderData);
    },
    validateEGN(personalIDNumber){

         if(this.isNullOrWhitespace(personalIDNumber)){
            return false;
         }

         // В конфигурацията на приложението (таблица AppSettings) в сетнато да не се валидира ЕГН
         if(!this.hasToValidateEgn) return true;

        if (personalIDNumber.length != 10)
        {
            return false;
        }

        for (var i = 0; i < personalIDNumber.length; i++) {
            if (typeof parseInt(personalIDNumber[i]) !== 'number') {
                return false;
            }
        }

        var month = parseInt(personalIDNumber.substr(2, 2));
        var year = 0;

        if (month < 13)
        {
            year = parseInt("19" + personalIDNumber.substr(0, 2));
        }
        else if (month < 33)
        {
            month -= 20;
            year = parseInt("18" + personalIDNumber.substr(0, 2));
        }
        else
        {
            month -= 40;
            year = parseInt("20" + personalIDNumber.substr(0, 2));
        }

        var day = parseInt(personalIDNumber.substr(4, 2));

        if(!this.isValidDate(new Date(year, month - 1, day))){
            return false;
        }

        var weights =  [2, 4, 8, 5, 10, 9, 7, 3, 6 ];
        var totalControlSum = 0;

        for (var x = 0; x < 9; x++)
        {
            totalControlSum += weights[x] * (personalIDNumber[x] - '0');
        }

        var controlDigit = 0;
        var reminder = totalControlSum % 11;

        if (reminder < 10){
            controlDigit = reminder;
        }

        var lastDigitFromIDNumber = parseInt(personalIDNumber[9]);

        if (lastDigitFromIDNumber != controlDigit){
            return false;
        }

        return true;
    },
    validateLnch(lnch){
         if(this.isNullOrWhitespace(lnch)){
            return false;
         }

         // В конфигурацията на приложението (таблица AppSettings) в сетнато да не се валидира ЛНЧ
         if(!this.hasToValidateLnch) return true;

         if (lnch.length != 10){
            return false;
         }

        const array = Array.from(lnch).map(str => Number(str));
        const numbeWeights = [21,19,17,13,11,9,7,3,1];

        let checksum = 0;
        for (var i = 0; i < 9; i++)
        {
            checksum += array[i] * numbeWeights[i];
        }
        return ((checksum % 10) === array[9]);
    },
    isValidDate(d) {
        return d instanceof Date && !isNaN(d);
    },
    getRequiredFieldValidationMessage(fieldName) {
      const field = this.$v[fieldName];

      if (!field.$dirty) {
        return '';
      }

      if(!field.required) {
        return this.$t('validation.requiredField');
      }
    },
    isNullOrWhitespace( input ) {

        if (typeof input === 'undefined' || input == null){
            return true;
        }

        return input.replace(/\s/g, '').length < 1;
    },
    getBirthDateFromEGN(pin) {
      let month = parseInt(pin.substring(2, 4));
      let year;

      if (month < 13) {
        year = parseInt('19' + pin.substring(0, 2));
      }
      else if (month < 33) {
        month -= 20;
        year = parseInt('18' + pin.substring(0, 2));
      }
      else {
        month -= 40;
        year = parseInt('20' + pin.substring(0, 2));
      }

      const day = parseInt(pin.substring(4, 6));
      const birthDate = `${day.toString().padStart(2, '0')}/${month.toString().padStart(2, '0')}/${year}`;

      return birthDate;
    },
    getGenderIdFromEGN(pin){
      const genderId = (parseInt(pin.substring(0, pin.length - 1)) % 2 === 0) ? Constants.MALE : Constants.FEMALE;

      return genderId;
    },
    async getDataForPerson(){
      this.$v.$touch();
      switch (this.personalIDType?.value){
        case 0:
          Regix.getValidPerson(this.personalID)
            .then(async (person) => {
                this.$emit('regixQueryStarted');

                let temporaryAddress = await Regix.getTemporaryAddress(this.personalID, new Date());
                let permanentAddress = await Regix.getPermanentAddress(this.personalID, new Date());
                let personData = await Regix.getPersonSearch(this.personalID);

                // По някаква причина за определни ЕГН-та се връщата навалидни ЕКАТТЕ кодове за населините места.
                // Пример 97029 => ГР.СОФИЯ,КВ.ДРАГАЛЕВЦИ
                // Ще правим проверка дали този код съществува в номенклатурата location.Town.
                const graoTowns = [];
                if (temporaryAddress.data != '' && temporaryAddress.data.settlementCode && !graoTowns.includes(temporaryAddress.data.settlementCode)) {
                  graoTowns.push(temporaryAddress.data.settlementCode);
                }

                if (permanentAddress.data != '' && permanentAddress.data.settlementCode && !graoTowns.includes(permanentAddress.data.settlementCode)) {
                  graoTowns.push(permanentAddress.data.settlementCode);
                }

                let towns = [];
                if (graoTowns.length > 0) {
                  towns = (await this.$api.lookups.getCittes('', graoTowns.join('|'))).data;
                }

                let addressString = '';
                let permanentAddressString = '';
                let usualResidenceObj = null;
                let permanentResidenceObj =  null;

                if(temporaryAddress.data != '') {
                    addressString = `${temporaryAddress.data.settlementName} ${temporaryAddress.data.locationName} Бл. ${temporaryAddress.data.buildingNumber} Вх.${temporaryAddress.data.entrance} ет.${temporaryAddress.data.floor} ап.${temporaryAddress.data.apartment}`;
                    if (!towns.some(x => x.value === Number(temporaryAddress.data.settlementCode))) {
                        this.$notifier.toast(this.$t('createStudent.permanentResidenceAddress'), `Не е намерено населено място с ЕКАТТЕ код: "${temporaryAddress.data.settlementCode}" и име: "${temporaryAddress.data.settlementName}". Ако е необходимо, моля да го изберете ръчно.`, 'warning', { duration: 15000 });
                    } else {
                      usualResidenceObj = {value : temporaryAddress.data.settlementCode, text : temporaryAddress.data.settlementName};
                    }
                }

                if(permanentAddress.data != '') {
                  permanentAddressString = `${permanentAddress.data.settlementName} ${permanentAddress.data.locationName} Бл. ${permanentAddress.data.buildingNumber} Вх.${permanentAddress.data.entrance} ет.${permanentAddress.data.floor} ап.${permanentAddress.data.apartment}`;
                  if (!towns.some(x => x.value === Number(permanentAddress.data.settlementCode))) {
                    this.$notifier.toast(this.$t('createStudent.usualResidenceAddress'), `Не е намерено населено място с ЕКАТТЕ код:"${permanentAddress.data.settlementCode}" и име:"${permanentAddress.data.settlementName}". Ако е необходимо, моля да го изберете ръчно.`, 'warning', { duration: 15000 });
                  } else {
                    permanentResidenceObj = {value : permanentAddress.data.settlementCode, text : permanentAddress.data.settlementName};
                  }
                }

                const personDetails = {
                    permanentAddress: permanentAddressString,
                    address: addressString,
                    usualResidence: usualResidenceObj,
                    permanentResidence: permanentResidenceObj,
                    nationality: {value : personData.data.nationality.nationalityId, text : personData.data.nationality.nationalityName, code:personData.data.nationality.nationalityCode },
                    firstName: person.data.firstName,
                    middleName: person.data.surName,
                    lastName: person.data.familyName,
                    pinType: this.personalIDType,
                    uniqueId: this.personalID,
                    placeBirthId: personData.data.placeBirthId
                };

                this.$emit('updatePersonDataFromRegix', personDetails);
              })
              .catch((error) => {
                this.$notifier.error('', this.$t('errors.regixDataLoad'));
                console.log(error);
              })
            .finally(() => {
              this.$emit('regixQueryCompleted');
            });
        break;
        case 1:
           Regix.getForeignIdentity(this.personalID)
            .then(async (person) => {
               this.$emit('regixQueryStarted');
               console.log(JSON.stringify(person));
               let nationality = person.data.nationalityList[0];

                let temporaryAddress = person.data.temporaryAddress;
                let permanentAddress = person.data.permanentAddress;
                const graoTowns = [];
                if (temporaryAddress != '' && temporaryAddress.settlementCode && !graoTowns.includes(temporaryAddress.settlementCode)) {
                  graoTowns.push(temporaryAddress.settlementCode);
                }

                if (permanentAddress != '' && permanentAddress.settlementCode && !graoTowns.includes(permanentAddress.settlementCode)) {
                  graoTowns.push(permanentAddress.settlementCode);
                }

                let towns = [];
                if (graoTowns.length > 0) {
                  towns = (await this.$api.lookups.getCittes('', graoTowns.join('|'))).data;
                }

                let addressString = '';
                let permanentAddressString = '';
                let usualResidenceObj = null;
                let permanentResidenceObj =  null;

                if(temporaryAddress.data != '') {
                    addressString = `${temporaryAddress.settlementName} ${temporaryAddress.locationName} Бл. ${temporaryAddress.buildingNumber} Вх.${temporaryAddress.entrance} ет.${temporaryAddress.floor} ап.${temporaryAddress.apartment}`;
                    if (!towns.some(x => x.value === Number(temporaryAddress.settlementCode))) {
                        this.$notifier.toast(this.$t('createStudent.permanentResidenceAddress'), `Не е намерено населено място с ЕКАТТЕ код: "${temporaryAddress.settlementCode}" и име: "${temporaryAddress.settlementName}". Ако е необходимо, моля да го изберете ръчно.`, 'warning', { duration: 15000 });
                    } else {
                      usualResidenceObj = {value : temporaryAddress.settlementCode, text : temporaryAddress.settlementName};
                    }
                }

                if(permanentAddress != '') {
                  permanentAddressString = `${permanentAddress.settlementName} ${permanentAddress.locationName} Бл. ${permanentAddress.buildingNumber} Вх.${permanentAddress.entrance} ет.${permanentAddress.floor} ап.${permanentAddress.apartment}`;
                  if (!towns.some(x => x.value === Number(permanentAddress.settlementCode))) {
                    this.$notifier.toast(this.$t('createStudent.usualResidenceAddress'), `Не е намерено населено място с ЕКАТТЕ код:"${permanentAddress.settlementCode}" и име:"${permanentAddress.settlementName}". Ако е необходимо, моля да го изберете ръчно.`, 'warning', { duration: 15000 });
                  } else {
                    permanentResidenceObj = {value : permanentAddress.settlementCode, text : permanentAddress.settlementName};
                  }
                }

                const personDetails = {
                    permanentAddress: permanentAddressString,
                    address: addressString,
                    usualResidence: usualResidenceObj,
                    permanentResidence: permanentResidenceObj,
                    nationality: {value : nationality.nationalityId, text : nationality.nationalityName, code:nationality.nationalityCode },
                    firstName: person.data.personNames.firstName,
                    middleName: person.data.personNames.surname,
                    lastName: person.data.personNames.familyName,
                    pinType: this.personalIDType,
                    uniqueId: this.personalID,
                };

                this.$emit('updatePersonDataFromRegix', personDetails);
            })
            .catch((error) => {
                this.$notifier.error('', this.$t('errors.regixDataLoad'));
                console.log(error);
              })
            .finally(() => {
              this.$emit('regixQueryCompleted');
            });
          break;
        default:
          break;
      }
    },
    onExistingPersonClick() {
      if(!this.existingPerson) return;
      this.dialog = true;
    }
  }
};
</script>
