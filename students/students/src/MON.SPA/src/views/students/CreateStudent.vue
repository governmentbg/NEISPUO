<template>
  <div>
    <v-form
      class="text-left"
      :disabled="disabled || sending"
      @submit.stop.prevent="validateUser"
    >
      <v-card>
        <v-card-title>
          {{ $t('createStudent.title') }}
        </v-card-title>
        <v-card-subtitle>
          <v-alert
            border="bottom"
            colored-border
            type="info"
            elevation="2"
          >
            Нов ученик се създава при първоначално записване на дете, ако ученикът не присъства в НЕИСПУО.
            <br>Ако ученикът вече присъства в НЕИСПУО и искате да го запишете, използвайте функцията <router-link to="/student">
              Търсене
            </router-link>
          </v-alert>
        </v-card-subtitle>
        <v-card-text class="pa-0 ma-0">
          <v-card>
            <v-card-title>
              <v-row justify="space-between">
                <div>
                  <v-icon left>
                    fa-id-card
                  </v-icon>
                  {{ $t('createStudent.personalData') }}
                </div>
                <div v-show="disabled">
                  <button-tip
                    icon
                    icon-color="primary"
                    icon-name="fas fa-unlock-alt"
                    iclass="mx-2"
                    tooltip="createStudent.editBtnTooltip"
                    left
                    @click="unlockForEdit"
                  />
                </div>
              </v-row>
            </v-card-title>
            <v-card-text>
              <v-row
                justify="center"
              >
                <v-col
                  cols="12"
                  md="6"
                >
                  <c-info
                    uid="person.pin"
                  >
                    <PersonUniqueId
                      ref="personUniqueId"
                      :key="personUniqueIdComponentKey"
                      :personal-i-d-type.sync="form.pinType"
                      :personal-i-d.sync="form.pin"
                      :unique-check="true"
                      :pin-required="true"
                      @updatePersonDataFromRegix="updatePersonDataFromRegix"
                      @extractBirthDateAndGenderFromEGN="onExtractBirthDateAndGenderFromEGN"
                      @regixQueryStarted="regixQueryStarted"
                      @regixQueryCompleted="regixQueryCompleted"
                    />
                  </c-info>
                </v-col>
              </v-row>
              <v-row>
                <v-col
                  cols="12"
                  md="4"
                  sm="12"
                >
                  <c-info
                    uid="person.firstName"
                  >
                    <v-text-field
                      id="firstName"
                      ref="firstName"
                      v-model="form.firstName"
                      :label="$t('createStudent.firstName')"
                      autocomplete="firstName"
                      name="firstName"
                      clearable
                      :rules="[$validator.required(), $validator.maxLength(255)]"
                      class="required"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="4"
                  sm="12"
                >
                  <c-info
                    uid="person.middleName"
                  >
                    <v-text-field
                      id="middleName"
                      ref="middleName"
                      v-model="form.middleName"
                      :label="$t('createStudent.middleName')"
                      name="middleName"
                      autocomplete="middleName"
                      clearable
                      :rules="[$validator.maxLength(255)]"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="4"
                  sm="12"
                >
                  <c-info
                    uid="person.lastName"
                  >
                    <v-text-field
                      id="lastName"
                      ref="lastName"
                      v-model="form.lastName"
                      :label="$t('createStudent.lastName')"
                      name="lastName"
                      autocomplete="lastName"
                      clearable
                      :rules="[$validator.required(), $validator.maxLength(255)]"
                      class="required"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="2"
                  sm="6"
                >
                  <c-info
                    uid="person.gender"
                  >
                    <v-select
                      id="gender"
                      ref="gender"
                      v-model="form.gender"
                      :label="$t('createStudent.gender')"
                      :items="genderOptions"
                      clearable
                      :rules="[$validator.required()]"
                      class="required"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="2"
                  sm="6"
                >
                  <c-info
                    uid="person.birthDate"
                  >
                    <date-picker
                      id="birthDate"
                      ref="birthDate"
                      v-model="form.birthDate"
                      :show-buttons="false"
                      :scrollable="false"
                      :no-title="true"
                      :show-debug-data="false"
                      :label="$t('createStudent.birthDate')"
                      :rules="[$validator.required()]"
                      clearable
                      class="required"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="4"
                  sm="6"
                >
                  <c-info
                    uid="person.nationality"
                  >
                    <autocomplete
                      id="nationality"
                      ref="nationality"
                      v-model="form.nationalityId"
                      api="/api/lookups/GetCountriesBySearchString"
                      :label="$t('createStudent.nationality')"
                      :placeholder="$t('buttons.search')"
                      clearable
                      hide-no-data
                      hide-selected
                      :rules="[$validator.required()]"
                      class="required"
                    />
                  </c-info>
                </v-col>

                <v-col
                  cols="12"
                  md="4"
                  sm="6"
                >
                  <c-info
                    uid="person.birthPlaceCountry"
                  >
                    <autocomplete
                      id="birthPlaceCountry"
                      ref="birthPlaceCountry"
                      v-model="form.birthPlaceCountryId"
                      api="/api/lookups/GetCountriesBySearchString"
                      :label="$t('createStudent.birthPlaceCountry')"
                      :placeholder="$t('buttons.search')"
                      clearable
                      hide-no-data
                      hide-selected
                      :rules="[$validator.required()]"
                      class="required"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="4"
                  sm="6"
                >
                  <c-info
                    uid="person.birthPlace"
                  >
                    <v-text-field
                      v-if="showBirthPlaceTownTextBox"
                      id="birthPlace"
                      ref="birthPlace"
                      v-model="form.birthPlace"
                      :label="$t('createStudent.birthPlace')"
                      name="birthPlace"
                      autocomplete="birthPlace"
                      clearable
                    />
                    <autocomplete
                      v-else
                      id="birthPlace"
                      ref="birthPlace"
                      v-model="form.birthPlaceId"
                      api="/api/lookups/GetAddressesBySearchString"
                      :label="$t('createStudent.birthPlace')"
                      :placeholder="$t('buttons.search')"
                      clearable
                      hide-no-data
                      hide-selected
                      remove-items-on-clear
                      :rules="[$validator.required()]"
                      class="required"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="4"
                  sm="6"
                >
                  <c-info
                    uid="person.phoneNumber"
                  >
                    <phone-field
                      id="phoneNumber"
                      ref="phoneNumber"
                      v-model="form.phoneNumber"
                      clearable
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="4"
                  sm="6"
                >
                  <c-info
                    uid="person.email"
                  >
                    <email-field
                      id="email"
                      ref="email"
                      v-model="form.email"
                      clearable
                    />
                  </c-info>
                </v-col>
              </v-row>
              <v-col
                cols="12"
              >
                <c-info
                  uid="person.addressCoincidesCheck"
                >
                  <v-checkbox
                    v-model="form.addressCoincidesCheck"
                    class="addressCoincidesCheck"
                    color="primary"
                    :label="$t('createStudent.addressCoincidesCheck')"
                    @change="onAddressCoincidesCheckChange"
                  />
                </c-info>
              </v-col>
              <v-row>
                <v-col
                  cols="12"
                  md="6"
                >
                  <c-info
                    uid="person.permanentResidence"
                  >
                    <autocomplete
                      id="permanentResidence"
                      ref="permanentResidence"
                      v-model="form.permanentResidenceId"
                      api="/api/lookups/GetAddressesBySearchString"
                      :label="$t('createStudent.permanentResidenceAddress')"
                      :placeholder="$t('buttons.search')"
                      clearable
                      hide-no-data
                      hide-selected
                      @change="onPermanentResidenceChange($event)"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <c-info
                    uid="person.permanentAddress"
                  >
                    <v-text-field
                      id="permanentAddress"
                      ref="permanentAddress"
                      v-model="form.permanentAddress"
                      :label="$t('createStudent.permanentAddress')"
                      name="permanentAddress"
                      autocomplete="permanentAddress"
                      clearable
                      :rules="[$validator.required(), $validator.maxLength(2048)]"
                      class="required"
                      @input="onPermanentAddressChange()"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <c-info
                    uid="person.usualResidence"
                  >
                    <autocomplete
                      id="usualResidence"
                      ref="usualResidence"
                      v-model="form.usualResidenceId"
                      api="/api/lookups/GetAddressesBySearchString"
                      :label="$t('createStudent.usualResidenceAddress')"
                      :placeholder="$t('buttons.search')"
                      clearable
                      hide-no-data
                      hide-selected
                      @change="onUsualResidenceChange()"
                    />
                  </c-info>
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <c-info
                    uid="person.currentAddress"
                  >
                    <v-text-field
                      id="currentAddress"
                      ref="currentAddress"
                      v-model="form.currentAddress"
                      :label="$t('createStudent.currentAddress')"
                      name="currentAddress"
                      autocomplete="currentAddress"
                      clearable
                      :rules="[$validator.required(), $validator.maxLength(2048)]"
                      class="required"
                      @input="onCurrentAddressChange()"
                    />
                  </c-info>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
          <v-divider />
        </v-card-text>

        <v-progress-linear
          v-if="sending"
          indeterminate
        />

        <v-card-actions class="justify-end">
          <v-btn
            ref="submit"
            raised
            color="primary"
            type="submit"
            :disabled="sending"
          >
            {{ $t('buttons.save') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click="onReset"
          >
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { StudentModel } from "@/models/studentCreateModel.js";
import PersonUniqueId from "@/components/person/PersonUniqueId.vue";
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import CONSTANTS from '@/common/constants';

export default {
  name: "CreateStudent",
  components: {
    PersonUniqueId,
    Autocomplete
  },
  data() {
    return {
      sending: false,
      form: new StudentModel(),
      genderOptions: [],
      commuterOptions: [],
      personUniqueIdComponentKey: 0,
      disabled: false,
    };
  },
  computed: {
    showBirthPlaceTownTextBox() {
      // Показва текстово поле за свъбодно въвеждане на месторождение при условие,
      // че е избрана държава на месторождение, различна от България.
      return this.form && this.form.birthPlaceCountryId && this.form.birthPlaceCountryId !== CONSTANTS.BULGARIA_COUNTRY_ID;
    },
  },
  mounted() {
    this.loadDropdownOptions();
  },
  methods: {
    onUsualResidenceChange() {
      if(this.form.addressCoincidesCheck) {
        this.form.permanentResidenceId = this.form.usualResidenceId;
      }
    },
    onPermanentResidenceChange() {
      if(this.form.addressCoincidesCheck) {
        this.form.usualResidenceId = this.form.permanentResidenceId;
      }
    },
    onPermanentAddressChange() {
      if(this.form.addressCoincidesCheck) {
        this.form.currentAddress = this.form.permanentAddress;
      }
    },
    onCurrentAddressChange() {
      if(this.form.addressCoincidesCheck) {
        this.form.permanentAddress = this.form.currentAddress;
      }
    },
    onAddressCoincidesCheckChange() {
      if(this.form.permanentResidenceId) {
        this.form.usualResidenceId = this.form.permanentResidenceId;
      }
      else if(this.form.usualResidenceId) {
        this.form.permanentResidenceId = this.form.usualResidenceId;
      }

      if(this.form.permanentAddress) {
        this.form.currentAddress = this.form.permanentAddress;
      }
      else if(this.form.currentAddress) {
        this.form.permanentAddress = this.form.currentAddress;
      }
    },
    onExtractBirthDateAndGenderFromEGN(data){
        this.form.gender = this.genderOptions.filter(el => el.value === data.genderId)[0].value;
        this.form.birthDate = data.birthDate;
    },
    async validateUser () {
      const hasErrors = this.$validator.validate(this);
      if(hasErrors || this.$refs.personUniqueId.$v.$invalid) {
        this.$notifier.error('', this.$t('validation.hasErrors'));
        return;
      }

      if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))){
        this.sending = true;

        const payload = {
          pin: this.form.pin,
          pinTypeId: this.$refs.personUniqueId.personalIDType.value,
          firstName: this.form.firstName,
          middleName: this.form.middleName,
          lastName: this.form.lastName,
          birthDate: !this.$refs.birthDate.isValidIsoFormattedDate() ? this.$refs.birthDate.isoDate : this.form.birthDate,
          genderId: this.form.gender,
          nationalityId: this.form.nationalityId,
          birthPlaceCountryId: this.form.birthPlaceCountryId,
          birthPlaceId: this.form.birthPlaceId,
          permanentAddress: this.form.permanentAddress,
          currentAddress: this.form.currentAddress,
          phoneNumber: this.form.phoneNumber,
          email: this.form.email,
          permanentResidenceId: this.form.permanentResidenceId,
          usualResidenceId: this.form.usualResidenceId,
          internationalProtectionStatus: this.form.internationalProtectionStatus,
          hasIndividualStudyPlan: this.form.hasIndividualStudyPlan,
          hasSupportiveEnvironment: this.form.hasSupportiveEnvironment,
          supportiveEnvironment: this.form.supportiveEnvironment,
          birthPlace: this.form.birthPlace
        };

        this.createStudent(payload);
      }
    },
    createStudent(model) {
      this.$api.student
        .create(model)
        .then((response) => {
          this.sending = false;
          console.log(response);
          const id = response.data;

          this.$router.push({ name: 'StudentDetails', params: { id: id } });
          this.resetForm();
        })
        .catch((error) => {
          this.sending = false;
          this.$notifier.error('', this.$t('errors.studentCreate'));
          console.log(error.response.data.message);
        });
    },
    loadDropdownOptions() {
      this.$api.lookups
        .getGenders()
        .then((response) => {
          const genders = response.data;
          if (genders) {
            this.genderOptions = genders;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.genderOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups
        .getCommuterOptions()
        .then((response) => {
          this.commuterOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.commuterOptionsLoad'));
          console.log(error);
        });
    },
   async onReset() {
      if(await this.$refs.confirm.open(this.$t('buttons.clear'), this.$t('common.confirm'))){
        this.resetForm();
      }
    },
    resetForm() {
      this.$nextTick(() => {
        this.form = new StudentModel();
        this.personUniqueIdComponentKey++;
        this.unlockForEdit();
      });
    },
    updatePersonDataFromRegix(data) {
      if(!data) return;

      this.form.permanentAddress = data.permanentAddress;
      this.form.currentAddress = data.address;
      if(data.nationality) {
        this.form.nationalityId = data.nationality.value;
        this.form.birthPlaceCountryId = data.nationality.value;
      }
      this.form.firstName = data.firstName;
      this.form.middleName = data.middleName;
      this.form.lastName = data.lastName;

      this.form.birthPlaceId = data.placeBirthId;

      if (data.usualResidence && data.usualResidence.value) {
        this.form.usualResidenceId =  Number(data.usualResidence.value);
      }

      if (data.permanentResidence && data.permanentResidence.value) {
        this.form.permanentResidenceId = Number(data.permanentResidence.value);
      }

      this.disabled = true; // Заключваме за редакция при зареждане на данни от ГРАО
    },
    regixQueryStarted() {
      this.sending = true;
    },
    regixQueryCompleted() {
      this.sending = false;
    },
    unlockForEdit() {
      // Todo: трабва ли да има потвърждение?
      this.disabled = false;
    }
  }
};
</script>
<style lang="scss" scoped>
  .v-progress-linear {
    position: absolute;
    top: 0;
    right: 0;
    left: 0;
  }
  .addressCoincidesCheck {
    width: fit-content;
  }
</style>
