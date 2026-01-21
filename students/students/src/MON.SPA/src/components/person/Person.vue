<template>
  <div>
    <v-card>
      <v-card-title>
        <div v-if="showTitle">
          <v-icon left>
            fa-id-card
          </v-icon>
          {{ this.$t('studentTabs.personalData') }}
        </div>
        <v-spacer />
        <edit-button
          v-if="edit && hasEditRights"
          v-model="isEditMode"
          @click="editIconClick"
        />
        <v-tooltip
          v-if="hasStudentPersonalDataReadPermission"
          bottom
        >
          <template v-slot:activator="{ on, attrs }">
            <v-btn
              icon
              v-bind="attrs"
              v-on="on"
              @click="showStudentHistory()"
            >
              <v-icon>mdi-history</v-icon>
            </v-btn>
          </template>
          <span>{{ $t('common.showHistory') + $t('studentTabs.personalData') }} </span>
        </v-tooltip>
      </v-card-title>
      <v-progress-linear
        v-if="loading"
        indeterminate
      />
      <v-card-text
        v-if="!canEditStudentPersonalDetails"
      >
        <v-alert
          outlined
          type="info"
          prominent
          dense
        >
          <h4 v-if="isCplrInstitution">
            {{ $t('common.missingEditPermission') }}
          </h4>
          <h4 v-else>
            {{ $t('student.missingPersonalDetailsEditPermission') }}
          </h4>
        </v-alert>
      </v-card-text>

      <v-card-text
        v-if="!loading"
      >
        <v-form
          :disabled="!isEditMode"
        >
          <v-row
            justify="center"
            dense
          >
            <v-col
              cols="12"
              md="6"
            >
              <PersonUniqueId
                ref="personUniqueId"
                :personal-i-d.sync="model.pin"
                :personal-i-d-type.sync="model.pinType"
                :initial-personal-i-d="initialPin"
                :initial-personal-type="initialPinType"
                :unique-check="true"
                :pin-required="pinRequired"
                :disabled="!isEditMode"
                @updatePersonDataFromRegix="updatePersonDataFromRegix"
                @extractBirthDateAndGenderFromEGN="onExtractBirthDateAndGenderFromEGN"
                @regixQueryStarted="regixQueryStarted"
                @regixQueryCompleted="regixQueryCompleted"
              />
            </v-col>
          </v-row>
          <v-row
            dense
            justify="center"
          >
            <v-col
              cols="12"
              md="4"
              sm="12"
            >
              <v-text-field
                :value="model.publicEduNumber"
                :label="$t('studentTabs.publicEduNumber')"
                disabled
                persistent-placeholder
              />
            </v-col>
          </v-row>
          <v-row
            dense
          >
            <v-col
              cols="12"
              md="4"
              sm="12"
            >
              <v-text-field
                id="firstName"
                ref="firstName"
                v-model="model.firstName"
                :label="$t('studentTabs.firstName')"
                clearable
                persistent-placeholder
                :rules="firstNameRequired
                  ? [$validator.required()]
                  : []"
                :class="firstNameRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              sm="12"
            >
              <v-text-field
                id="middleName"
                ref="middleName"
                v-model="model.middleName"
                :label="$t('studentTabs.middleName')"
                clearable
                persistent-placeholder
                :rules="middleNameRequired
                  ? [$validator.required()]
                  : []"
                :class="middleNameRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              sm="12"
            >
              <v-text-field
                id="lastName"
                ref="lastName"
                v-model="model.lastName"
                :label="$t('studentTabs.lastName')"
                clearable
                persistent-placeholder
                :rules="lastNameRequired
                  ? [$validator.required()]
                  : []"
                :class="lastNameRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="2"
              sm="6"
            >
              <v-select
                id="gender"
                ref="gender"
                v-model="model.genderId"
                :label="$t('createStudent.gender')"
                :items="genderOptions"
                clearable
                persistent-placeholder
                :rules="genderRequired
                  ? [$validator.required()]
                  : []"
                :class="genderRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="2"
              sm="6"
            >
              <date-picker
                id="birthDate"
                ref="birthDate"
                v-model="model.birthDate"
                :show-buttons="false"
                :scrollable="false"
                :no-title="true"
                :show-debug-data="false"
                :label="$t('createStudent.birthDate')"
                :rules="birthDateRequired
                  ? [$validator.required()]
                  : []"
                clearable
                :disabled="!isEditMode"
                persistent-placeholder
                :class="birthDateRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              sm="6"
            >
              <autocomplete
                id="nationality"
                ref="nationality"
                v-model="model.nationalityId"
                api="/api/lookups/GetCountriesBySearchString"
                :label="$t('createStudent.nationality')"
                :placeholder="isEditMode ? $t('buttons.search') : ''"
                clearable
                hide-no-data
                hide-selected
                persistent-placeholder
                :rules="nationalityRequired
                  ? [$validator.required()]
                  : []"
                :class="nationalityRequired && isEditMode ? 'required' :''"
              />
            </v-col>

            <v-col
              cols="12"
              md="4"
              sm="6"
            >
              <autocomplete
                id="birthPlaceCountry"
                ref="birthPlaceCountry"
                v-model="model.birthPlaceCountryId"
                api="/api/lookups/GetCountriesBySearchString"
                :label="$t('createStudent.birthPlaceCountry')"
                :placeholder="isEditMode ? $t('buttons.search') : ''"
                clearable
                hide-no-data
                hide-selected
                persistent-placeholder
                :rules="birthPlaceCountryRequired
                  ? [$validator.required()]
                  : []"
                :class="birthPlaceCountryRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              sm="6"
            >
              <v-text-field
                v-if="showBirthPlaceTownTextBox"
                id="birthPlace"
                ref="birthPlace"
                v-model="model.birthPlace"
                :label="$t('createStudent.birthPlace')"
                name="birthPlace"
                autocomplete="birthPlace"
                persistent-placeholder
                clearable
              />
              <autocomplete
                v-else
                id="birthPlace"
                ref="birthPlace"
                v-model="model.birthPlaceId"
                api="/api/lookups/GetAddressesBySearchString"
                :label="$t('createStudent.birthPlace')"
                :placeholder="isEditMode ? $t('buttons.search') : ''"
                clearable
                hide-no-data
                hide-selected
                persistent-placeholder
                :rules="birthPlaceRequired
                  ? [$validator.required()]
                  : []"
                :class="birthPlaceRequired && isEditMode ? 'required' :''"
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              sm="6"
            >
              <phone-field
                v-model="model.phoneNumber"
                :required="phoneNumberRequired"
                clearable
                persistent-placeholder
              />
            </v-col>
            <v-col
              cols="12"
              md="4"
              sm="6"
            >
              <email-field
                v-model="model.email"
                clearable
                persistent-placeholder
              />
            </v-col>
            <v-col
              cols="12"
            >
              <v-checkbox
                v-model="model.addressCoincidesCheck"
                color="primary"
                :label="$t('createStudent.addressCoincidesCheck')"
                @change="onAddressCoincidesCheckChange"
              />
            </v-col>
            <v-col
              cols="12"
              sm="12"
              md="6"
            >
              <autocomplete
                id="permanentResidence"
                ref="permanentResidence"
                v-model="model.permanentResidenceId"
                api="/api/lookups/GetAddressesBySearchString"
                :label="$t('studentTabs.permanentResidenceAddress')"
                :placeholder="isEditMode ? $t('buttons.search') : ''"
                clearable
                hide-no-data
                hide-selected
                persistent-placeholder
                :rules="permanentResidenceRequired
                  ? [$validator.required()]
                  : []"
                :class="permanentResidenceRequired && isEditMode ? 'required' :''"
                @change="onPermanentResidenceChange()"
              />
            </v-col>
            <v-col
              cols="12"
              md="6"
              sm="12"
            >
              <v-text-field
                id="permanentAddress"
                ref="permanentAddress"
                v-model="model.permanentAddress"
                :rules="permanentAddressRequired
                  ? [$validator.required()]
                  : []"
                :label="$t('createStudent.permanentAddress')"
                autocomplete="permanentAddress"
                clearable
                persistent-placeholder
                :class="permanentAddressRequired && isEditMode ? 'required' :''"
                @input="onPermanentAddressChange()"
              />
            </v-col>
            <v-col
              cols="12"
              sm="12"
              md="6"
            >
              <autocomplete
                id="usualResidence"
                ref="usualResidence"
                v-model="model.usualResidenceId"
                api="/api/lookups/GetAddressesBySearchString"
                :label="$t('studentTabs.usualResidenceAddress')"
                :placeholder="isEditMode ? $t('buttons.search') : ''"
                clearable
                hide-no-data
                hide-selected
                persistent-placeholder
                :rules="currentResidenceRequired
                  ? [$validator.required()]
                  : []"
                :class="currentResidenceRequired && isEditMode ? 'required' :''"
                @change="onUsualResidenceChange($event, 'usualResidence')"
              />
            </v-col>
            <v-col
              cols="12"
              md="6"
              sm="12"
            >
              <v-text-field
                id="currentAddress"
                ref="currentAddress"
                v-model="model.currentAddress"
                :rules="currentAddressRequired
                  ? [$validator.required()]
                  : []"
                :label="$t('createStudent.currentAddress')"
                autocomplete="currentAddress"
                clearable
                persistent-placeholder
                :class="currentAddressRequired && isEditMode ? 'required' :''"
                @input="onCurrentAddressChange()"
              />
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>

      <v-card-actions
        v-if="isEditMode && hasEditRights"
      >
        <slot name="actions">
          <v-spacer />
          <v-btn
            raised
            color="primary"
            @click="onSave"
          >
            <v-icon left>
              fas fa-save
            </v-icon>
            {{ $t('buttons.saveChanges') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click="onCancel($t('buttons.clear'))"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </slot>
      </v-card-actions>
    </v-card>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <ConfirmDlg ref="confirm" />
    <student-personal-data-history
      ref="studentPersonalDataHistory"
      :person-id="id"
    />
  </div>
</template>

<script>
import StudentPersonalDataHistory from "@/components/history/StudentPersonalDataHistory.vue";
import { StudentModel } from '@/models/studentCreateModel.js';
import PersonUniqueId from "@/components/person/PersonUniqueId.vue";
import Constants from '@/common/constants';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { InstType, Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "Person",
  components: {
      PersonUniqueId,
      StudentPersonalDataHistory,
      Autocomplete
  },
  props: {
    id: {
      type: Number,
      required: true
    },
    edit: {
      type: Boolean,
      default() {
        return false;
      }
    },
    showTitle: {
      type: Boolean,
      default() {
        return false;
      }
    },
    pinRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    publicEduRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    firstNameRequired: {
      type: Boolean,
      default() {
        return true; // В базата е not null
      }
    },
    middleNameRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    lastNameRequired: {
      type: Boolean,
      default() {
        return true; // В базата е not null
      }
    },
    birthDateRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    genderRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    nationalityRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    birthPlaceCountryRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    birthPlaceRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    permanentResidenceRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    currentResidenceRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    permanentAddressRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    currentAddressRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
    phoneNumberRequired: {
      type: Boolean,
      default() {
        return false;
      }
    },
  },
  data() {
    return {
      loading: false,
      saving: false,
      isEditMode: false,
      model: new StudentModel(),
      dateFormat: Constants.DATEPICKER_FORMAT,
      genderOptions: [],
      commuterOptions: [],
      initialPin: undefined,
      initialPinType: undefined,
      canEditStudentPersonalDetails: false
    };
  },
  computed: {
    ...mapGetters(['isInstType', 'hasStudentPermission']),
    hasEditRights() {
      return this.canEditStudentPersonalDetails;
    },
    showBirthPlaceTownTextBox() {
      // Показва текстово поле за свъбодно въвеждане на месторождение при условие,
      // че е избрана държава на месторождение, различна от България.
      return this.model && this.model.birthPlaceCountryId && this.model.birthPlaceCountryId !== Constants.BULGARIA_COUNTRY_ID;
    },
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    },
    hasStudentPersonalDataReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDataRead);
    }
  },
  mounted() {
    this.load();
    this.checkEditStudentPersonalDetailsPermission();
    this.loadDropdownOptions();
  },
  methods:{
    load(){
      this.loading = true;

      this.$api.student.getPersonDataById(this.id)
      .then(response => {
        if (response.data) {
          this.model = new StudentModel(response.data);
          this.model.birthDate = this.model.birthDate ? this.$moment(this.model.birthDate).format(Constants.DATEPICKER_FORMAT) : this.model.birthDate;

          // Тези първоначални стойности се използват от PersonUniqueId компонента.
          // Задължителнотелно е да му се подават като отделни data пропъртита.
          // Ако PersonUniqueId се подаде директно this.model.pin и this.model.pinType,
          // то initialPersonalID и personalID винаги се синхронизират, дори и при опит за deep copy на стринга.
          this.initialPin = this.model.pin;
          this.initialPinType = this.model.pinType.text;
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studentLoad'));
        console.log(error.response);
      })
      .then(() => { this.loading = false; });
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
    checkEditStudentPersonalDetailsPermission() {
      this.$api.student.canEditStudentPersonalDetails(this.id)
      .then(response => {
        if (response.data) {
          this.canEditStudentPersonalDetails = response.data;
        }
      })
      .catch(error => {
        console.log(error);
      });
    },
    async editIconClick(isEditMode) {
      if (isEditMode === false) {
        await this.onCancel('');
      }
    },
    async onSave() {
      this.$refs.personUniqueId.$v.$touch();
      const hasErrors = this.$validator.validate(this);
      if(hasErrors || this.$refs.personUniqueId.$v.$invalid) {
        this.$notifier.error('', this.$t('validation.hasErrors'));
        return;
      }

      if(await this.$refs.confirm.open('', this.$t('common.confirm'))) {
        this.saveRecord();
      }
    },
    async onCancel(val) {
      if(await this.$refs.confirm.open(val, this.$t('common.confirm'))) {
        this.isEditMode = false;
        this.load();
      }
    },
    saveRecord() {
      const payload = {
        id: this.model.id,
        pin: this.model.pin,
        pinTypeId: this.$refs.personUniqueId.personalIDType.value,
        firstName: this.model.firstName,
        middleName: this.model.middleName,
        lastName: this.model.lastName,
        birthDate: !this.$refs.birthDate.isValidIsoFormattedDate() ? this.$refs.birthDate.isoDate : this.model.birthDate,
        genderId: this.model.genderId,
        nationalityId: this.model.nationalityId,
        birthPlaceCountryId: this.model.birthPlaceCountryId,
        birthPlaceId: this.model.birthPlaceId,
        permanentAddress: this.model.permanentAddress,
        currentAddress: this.model.currentAddress,
        phoneNumber: this.model.phoneNumber,
        email: this.model.email,
        permanentResidenceId: this.model.permanentResidenceId,
        usualResidenceId: this.model.usualResidenceId,
        birthPlace: this.model.birthPlace
      };

      this.saving = true;

      this.$api.student
        .updateBasicDetails(payload)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.isEditMode = false;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.studentSave'));
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    updatePersonDataFromRegix(data){
      if(!data) {
        return;
      }

      this.model.permanentAddress = data.permanentAddress;
      this.model.currentAddress = data.address;
      if(data.usualResidence) {
        this.model.usualResidenceId = Number(data.usualResidence.value);
      }
      if(data.permanentResidence) {
        this.model.permanentResidenceId =  Number(data.permanentResidence.value);
      }
      if(data.nationality) {
        this.model.nationalityId = data.nationality.value;
        this.model.birthPlaceCountryId = data.nationality.value;
      }
      this.model.firstName = data.firstName;
      this.model.middleName = data.middleName;
      this.model.lastName = data.lastName;
    },
    onExtractBirthDateAndGenderFromEGN(data){
      this.model.genderId = this.genderOptions.filter(el => el.value === data.genderId)[0].value;
      this.model.birthDate = data.birthDate;
    },
    regixQueryStarted(){
        this.saving = true;
    },
    regixQueryCompleted(){
        this.saving = false;
    },
    showStudentHistory() {
      this.$refs.studentPersonalDataHistory.getStudentPersonalDataHistory();
    },
    onUsualResidenceChange() {
      if(this.model.addressCoincidesCheck) {
        this.model.permanentResidenceId = this.model.usualResidenceId;
      }
    },
    onPermanentResidenceChange() {
      if(this.model.addressCoincidesCheck) {
        this.model.usualResidenceId = this.model.permanentResidenceId;
      }
    },
    onPermanentAddressChange() {
      if(!this.model.addressCoincidesCheck) return;
      this.model.currentAddress = this.model.permanentAddress;
    },
    onCurrentAddressChange() {
      if(!this.model.addressCoincidesCheck) return;
      this.model.permanentAddress = this.model.currentAddress;
    },
    onAddressCoincidesCheckChange() {
      if(this.model.permanentResidenceId) {
        this.model.usualResidenceId = this.model.permanentResidenceId;
      }
      else if(this.model.usualResidenceId) {
        this.model.permanentResidenceId = this.model.usualResidenceId;
      }

      if(this.model.permanentAddress) {
        this.model.currentAddress = this.model.permanentAddress;
      }
      else if(this.model.currentAddress) {
        this.model.permanentAddress = this.model.currentAddress;
      }
    },
  },
};
</script>
