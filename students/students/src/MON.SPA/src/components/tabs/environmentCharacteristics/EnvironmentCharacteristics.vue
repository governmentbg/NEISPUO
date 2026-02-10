<template>
  <v-form
    ref="form"
    @submit.prevent="validateUser"
  >
    <v-card
      flat
      mt-2
    >
      <v-card-title>
        <v-spacer />
        <edit-button
          v-model="isEditMode"
          @click="editIconClick"
        />
        <v-tooltip bottom>
          <template v-slot:activator="{ on, attrs }">
            <v-btn
              icon
              :disabled="!isEditMode"
              v-bind="attrs"
              v-on="on"
              @click="getRelatives()"
            >
              <v-icon left>
                mdi-magnify
              </v-icon>
            </v-btn>
          </template>
          <span> {{ $t('common.GRAOSearch') }} Regix</span>
        </v-tooltip>
      </v-card-title>

      <v-progress-linear
        v-if="loading"
        indeterminate
      />

      <v-card-text class="ma-0 mt-2 pa-0">
        <v-card>
          <v-card-title class="bg-secondary text-white p-2">
            {{ $t('environmentCharacteristics.environmentCharacteristicsTitle') }}
          </v-card-title>
          <v-card-subtitle>
            {{ $t('environmentCharacteristics.subtitle') }}
          </v-card-subtitle>

          <v-card-text>
            <v-card>
              <v-card-title>
                <v-icon left>
                  fa-user-md
                </v-icon>
                {{ $t('environmentCharacteristics.studentGp') }}
              </v-card-title>

              <v-card-text>
                <v-row dense>
                  <v-col
                    dense
                    cols="12"
                    md="6"
                    sm="12"
                  >
                    <v-text-field
                      v-model="currentGpName"
                      :label=" $t('studentTabs.lastName' )"
                      :disabled="!isEditMode"
                      :value="currentGpName"
                      autocomplete="gpName"
                    />
                  </v-col>
                  <v-col
                    dense
                    cols="12"
                    md="6"
                    sm="12"
                  >
                    <v-text-field
                      v-model="currentGpPhone"
                      :label="$t('studentTabs.phoneNumber')"
                      :disabled="!isEditMode"
                      :value="currentGpPhone"
                      autocomplete="gpPhone"
                      prepend-icon="fa-phone-alt"
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>

            <v-row dense>
              <v-col
                dense
                cols="12"
                md="4"
                sm="6"
              >
                <v-checkbox
                  v-model="currentHasParentConsent"
                  :label=" $t('environmentCharacteristics.parentsConsentedToSubmitData') "
                  color="primary"
                  hide-details
                  :disabled="!isEditMode"
                />
              </v-col>

              <v-col
                dense
                cols="12"
                md="4"
                sm="6"
              >
                <v-checkbox
                  v-model="currentRepresentedByTheMajor"
                  :label=" $t('environmentCharacteristics.representedByTheMajor') "
                  color="primary"
                  hide-details
                  :disabled="!isEditMode"
                />
              </v-col>

              <v-col
                dense
                cols="12"
                md="4"
                sm="6"
              >
                <v-checkbox
                  v-model="currentLivesWithFosterFamily"
                  :label="$t('environmentCharacteristics.studentLivesInHospice') "
                  color="primary"
                  hide-details
                  :disabled="!isEditMode"
                />
              </v-col>

              <v-col
                cols="12"
                md="4"
                sm="6"
              >
                <autocomplete
                  v-model="currentNativeLanguageId"
                  api="/api/lookups/GetLanguages"
                  :label="$t('environmentCharacteristics.environmentLanguage')"
                  :disabled="!isEditMode"
                  clearable
                />
              </v-col>

              <v-col
                v-if="!currentHasParentConsent"
                dense=""
                cols="12"
                md="12"
                sm="12"
                class="red--text font-weight-bold text-h6"
              >
                <span>
                  {{ $t('environmentCharacteristics.noEnvironmentCharacteristicsForTheStudent') }}
                </span>
              </v-col>
            </v-row>

            <div v-if="currentHasParentConsent">
              <v-card
                v-for="(studentRelativeDetail, index) in currentRelativeDetails"
                :key="index"
                class="mt-6"
              >
                <v-card-text>
                  <v-row
                    justify="center"
                  >
                    <v-col
                      cols="12"
                      md="6"
                    >
                      <PersonUniqueId
                        ref="personUniqueId"
                        :personal-i-d.sync="studentRelativeDetail.pin"
                        :personal-i-d-type.sync="studentRelativeDetail.pinTypeId"
                        :initial-personal-i-d="studentRelativeDetail.pin"
                        :initial-personal-type="studentRelativeDetail.pinTypeId.toString()"
                        :disabled="!isEditMode"
                        :unique-check="false"
                        :pin-required="false"
                        @updatePersonDataFromRegix="updatePersonDataFromRegix"
                        @regixQueryStarted="regixQueryStarted"
                        @regixQueryCompleted="regixQueryCompleted"
                      />
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-text-field
                        v-model="studentRelativeDetail.firstName"
                        :label="$t('studentTabs.firstName')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.firstName"
                      />
                    </v-col>

                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-text-field
                        v-model="studentRelativeDetail.middleName"
                        :label="$t('studentTabs.middleName')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.middleName"
                      />
                    </v-col>

                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-text-field
                        v-model="studentRelativeDetail.lastName"
                        :label="$t('studentTabs.lastName')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.lastName"
                      />
                    </v-col>
                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-select
                        v-model="studentRelativeDetail.relativeTypeId"
                        :items="relativeTypeOptions"
                        :label="$t('environmentCharacteristics.studentRelationshipLabel')"
                        :disabled="!isEditMode"
                        clearable
                        :value="studentRelativeDetail.relativeTypeId"
                        :error-messages="getStudentRelativeValidationMessage(index, 'relativeTypeId')"
                        class="required"
                      />
                    </v-col>
                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-text-field
                        v-model="studentRelativeDetail.email"
                        :label="$t('environmentCharacteristics.email')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.email"
                        :error-messages="getStudentRelativeValidationMessage(index,'email')"
                        autocomplete="email"
                        prepend-icon="fa-at"
                        class="required"
                        @input="$v.currentRelativeDetails.$each[index].email.$touch()"
                        @blur="$v.currentRelativeDetails.$each[index].email.$touch()"
                      />
                    </v-col>
                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-text-field
                        v-model="studentRelativeDetail.phoneNumber"
                        :label="$t('studentTabs.phoneNumber')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.phoneNumber"
                        autocomplete="phoneNumber"
                        prepend-icon="fa-phone-alt"
                      />
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col
                      cols="12"
                      md="4"
                      sm="12"
                    >
                      <v-text-field
                        v-model="studentRelativeDetail.address"
                        :label="$t('studentTabs.address')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.address"
                      />
                    </v-col>

                    <v-col
                      cols="12"
                      sm="4"
                    >
                      <v-select
                        v-model="studentRelativeDetail.workStatusId"
                        :items="workStatusOptions"
                        :label="$t('environmentCharacteristics.employment')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.workStatusId"
                      />
                    </v-col>
                    <v-col
                      cols="12"
                      sm="4"
                    >
                      <v-select
                        v-model="studentRelativeDetail.educationTypeId"
                        :items="educationTypeOptions"
                        :label="$t('environmentCharacteristics.education')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.educationStatusId"
                        autocomplete="educationStatusId"
                      />
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col
                      cols="12"
                      sm="12"
                    >
                      <v-textarea
                        v-model="studentRelativeDetail.notes"
                        outlined
                        prepend-icon="mdi-comment"
                        :label="$t('environmentCharacteristics.notes')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.notes"
                      />
                    </v-col>
                  </v-row>
                  <v-row>
                    <v-col
                      cols="12"
                      sm="12"
                    >
                      <v-textarea
                        v-model="studentRelativeDetail.description"
                        outlined
                        prepend-icon="mdi-comment"
                        :label="$t('studentTabs.description')"
                        :disabled="!isEditMode"
                        :value="studentRelativeDetail.description"
                        autocomplete="description"
                      />
                    </v-col>
                  </v-row>

                  <v-row>
                    <v-col
                      cols="12"
                      sm="12"
                      class="text-right"
                    >
                      <button-tip
                        small
                        icon-name="mdi-delete"
                        iclass="mx-2"
                        :disabled="!isEditMode"
                        dark
                        color="red"
                        tooltip="buttons.studentRelativeDeleteTooltip"
                        bottom
                        fab
                        @click="onDeleteStudentRelativeDetailClick(index)"
                      />
                    </v-col>
                  </v-row>
                </v-card-text>
                <v-divider />
              </v-card>
            </div>
          </v-card-text>
        </v-card>
      </v-card-text>

      <v-card-actions
        v-show="isEditMode"
      >
        <button-tip
          v-if="currentHasParentConsent"
          icon-name="mdi-plus"
          iclass="mx-2"
          :disabled="!isEditMode"
          dark
          color="primary"
          tooltip="buttons.studentRelativeAddTooltip"
          bottom
          fab
          @click="onAddNewStudentRelativeDetailClick"
        />

        <v-spacer />

        <div>
          <v-btn
            ref="submit"
            raised
            color="primary"
            type="submit"
          >
            <v-icon left>
              fas fa-save
            </v-icon>
            {{ $t('buttons.saveChanges') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click="onCancel"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('studentTabs.clearLabel') }}
          </v-btn>
        </div>
      </v-card-actions>

      <confirm-dlg ref="confirm" />
    </v-card>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-form>
</template>

<script>
import { validationMixin } from 'vuelidate';
import { StudentRelativeModel } from "@/models/environmentCharacteristics/studentRelativeModel.js";
import { required, email  } from 'vuelidate/lib/validators';

import { NotificationSeverity } from "@/enums/enums";
import Helper from "@/components/helper.js";

import Regix from "@/services/regix.service.js";
import { mapGetters } from 'vuex';

import PersonUniqueId from "@/components/person/PersonUniqueId.vue";
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
name: "EnvironmentCharacteristics",
components: {
    PersonUniqueId,
    Autocomplete
  },
  mixins: [validationMixin],
  validations: {
      currentRelativeDetails : {
        $each: {
          pinTypeId: {
            required
          },
          relativeTypeId: {
            required
          },
          email:{
              email
          }
        }
      }
  },
props: {
    personId: {
      type: Number,
      required: true
    },
    pin: {
      type: String,
      required: false,
      default() {
        return undefined;
      }
    }
  },
   data() {
    return {
      relativeTypeOptions: [],
      workStatusOptions: [],
      educationTypeOptions: [],
      pinTypeOptions: [],
      currentRelativeDetails: [],
      currentHasParentConsent: false,
      currentLivesWithFosterFamily: false,
      currentRepresentedByTheMajor: false,
      currentNativeLanguageId: null,
      currentGpName: '',
      currentGpPhone: '',
      isEditMode: false,
      loading: false,
      saving: false,
      helper: Helper,
    };
  },
  computed: {
    ...mapGetters(['currentStudentSummary'])
  },
  mounted() {
    this.fillSelectOptions();
    this.load();
  },
  methods: {
    load(){
      this.loading = true;

      this.$api.environmentCharacteristics.getStudentEnvironmentCharacteristics(this.personId)
      .then(response => {
        if (response.data) {
          this.currentRelativeDetails = response.data.studentRelativeDetails;
          this.currentHasParentConsent =  response.data.hasParentConsent;
          this.currentLivesWithFosterFamily = response.data.livesWithFosterFamily;
          this.currentGpName = response.data.gpName;
          this.currentGpPhone = response.data.gpPhone;
          this.currentRepresentedByTheMajor = response.data.representedByTheMajor;
          this.currentNativeLanguageId = response.data.nativeLanguageId;

          this.$v.$touch();
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studentEnvironmentCharacteristicsLoad'));
        console.log(error);
      })
      .then(() => {
        this.loading = false;
      });
    },
    showModal(notificationSeverity, title, text) {
      this.$notifier.modal(
          title,
          text,
          notificationSeverity
      );
    },
    getRelatives(){
      Regix.getRelations(this.currentStudentSummary.pin)
      .then((relatives) => {
        console.log(JSON.stringify(relatives.data));
        relatives.data.personRelations.forEach(relative => {
          const studentRelativeDetail = new StudentRelativeModel();
          studentRelativeDetail.firstName = relative.firstName;
          studentRelativeDetail.middleName = relative.surName;
          studentRelativeDetail.lastName = relative.familyName;
          studentRelativeDetail.pin = relative.egn;
          switch (relative.relationCode){
            // Баща
            case 0:
              studentRelativeDetail.relativeTypeId = 1;
            break;
            // Майка
            case 2:
              studentRelativeDetail.relativeTypeId = 2;
            break;
            // Осиновител, осиновителка
            case 1:
            case 3:
              studentRelativeDetail.relativeTypeId = 5;
            break;
          }
          this.currentRelativeDetails.push(studentRelativeDetail);
        });

        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.regixRelativesLoad'));
          console.log(error);
        });
    },
    getStudentRelativeValidationMessage(index, fieldName) {
      const field = this.$v.currentRelativeDetails.$each[index][fieldName];

      if(fieldName == 'email' ){
          if(field.$invalid){
             return this.$t('validation.emailNotValid');
          }
         return '';
      }

      if (!field.$dirty) {
        return '';
      }

      if(!field.required) {
        return this.$t('validation.requiredField');
      }

      if(fieldName === 'pin'
        && !field.isPinValid
      ) {
        return this.$t('validation.invalidIndentifier');
      }
    },
    onAddNewStudentRelativeDetailClick() {
      const studentRelativeDetail = new StudentRelativeModel();
      this.currentRelativeDetails.push(studentRelativeDetail);
    },
    onDeleteStudentRelativeDetailClick(index) {
      this.currentRelativeDetails.splice(index, 1);
    },
    fillSelectOptions() {
      this.$api.lookups.getStudentRelativeTypeOptions()
        .then((response) => {
          this.relativeTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.relativeTypeOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getStudentRelativeWorkStatusOptions()
        .then((response) => {
          this.workStatusOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.workStatusOptionsLoad'));
          console.log(error);
        });

      this.$api.lookups.getPinTypes()
        .then((response) => {
          const pinTypes = response.data;
          if (pinTypes) {
            this.pinTypeOptions = pinTypes;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.pinTypeOptionsLoad'));
          console.log(error);
        });

         this.$api.lookups.getEducationTypeOptions()
        .then((response) => {
          const educationTypeOptions = response.data;
          if (educationTypeOptions) {
            this.educationTypeOptions = educationTypeOptions;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.educationTypeOptionsLoad'));
          console.log(error);
        });
    },
    async validateUser () {
      this.$v.$touch();

       var allsubComponentsValid = false;

       if( this.$refs.personUniqueId != undefined && this.$refs.personUniqueId.length != 0){
        this.$refs.personUniqueId.forEach(function(entry) {
                    entry.$v.$touch();
                allsubComponentsValid = !entry.$v.$invalid;
            });
       }else{
           allsubComponentsValid = true;
       }

      if (!this.$v.$invalid && allsubComponentsValid) {
         if(await this.$refs.confirm.open(this.$t('common.save'), this.$t('common.confirm'))) {

            this.currentRelativeDetails.forEach(function(entry) {
                if(entry.pinTypeId.value != undefined){
                    entry.pinTypeId = entry.pinTypeId.value;
                }
            });

            const payload = {
                personId: this.personId,
                studentRelativeDetails: this.currentHasParentConsent ? this.currentRelativeDetails : [],
                hasParentConsent: this.currentHasParentConsent,
                livesWithFosterFamily: this.currentLivesWithFosterFamily,
                gpName: this.currentGpName,
                gpPhone: this.currentGpPhone,
                representedByTheMajor: this.currentRepresentedByTheMajor,
                nativeLanguageId: this.currentNativeLanguageId
            };

            this.saving = true;

            this.$api.environmentCharacteristics
            .update(payload)
            .then(() => {
                this.showModal(NotificationSeverity.Success, "", this.$t('common.saveSuccess'));
                this.isEditMode = false;
                this.load();
            })
            .catch((error) => {
                this.$notifier.error('', this.$t('errors.studentSave'));
                console.log(error);
            })
            .then(() => { this.saving = false; });
        }
      }
      else{
       this.$notifier.error('', 'Възникна грешка при проверка на задължителните полета!');
      }
    },
    async editIconClick(val) {
      if(val === false) {
        if(await this.$refs.confirm.open('', this.$t('common.confirm'))) {
          this.load();
        }
      }
    },
    async onCancel() {
        if(await this.$refs.confirm.open(this.$t('buttons.clear'), this.$t('common.confirm'))) {
          this.isEditMode = false;
          this.load();
        }
    },
    updatePersonDataFromRegix(data){
      if(!data) return;

      var relative = this.currentRelativeDetails.filter(relative => relative.pin ===  data.uniqueId && relative.pinTypeId == data.pinType.value)[0];

      if(relative == undefined){
        relative = this.currentRelativeDetails.filter(relative => relative.pin ===  data.uniqueId && relative.pinTypeId == data.pinType.value);
      }

      relative.address = data.address;
      relative.firstName = data.firstName;
      relative.middleName = data.middleName;
      relative.lastName = data.lastName;
    },
    regixQueryStarted(){
        this.saving = true;
    },
    regixQueryCompleted(){
        this.saving = false;
    }
  }

};
</script>
