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
          v-if="hasManagePermission"
          v-model="isEditMode"
          @click="editIconClick"
        />

        <v-tooltip bottom>
          <template v-slot:activator="{ on, attrs }">
            <v-btn
              v-if="hasManagePermission"
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

      <v-card-subtitle>
        <institution-external-so-provider-checker />
      </v-card-subtitle>

      <v-progress-linear
        v-if="loading"
        indeterminate
      />

      <v-card-text class="ma-0 mt-2 pa-0">
        <v-card-title class="bg-secondary text-white p-2">
          {{ $t('environmentCharacteristics.environmentCharacteristicsTitle') }}
        </v-card-title>
        <v-card-subtitle>
          {{ $t('environmentCharacteristics.subtitle') }}
        </v-card-subtitle>

        <v-card-text>
          <EnvironmentCharacteristicsBasicData
            ref="environmentCharacteristicsBasicData"
            :person-id="personId"
            :is-edit-mode="isEditMode"
            :form.sync="form"
            :saving="saving"
          />

          <RelativesList
            ref="relativesList"
            :key="relativeListComponentKey"
            :person-id="personId"
            @reloadData="onReloadData"
          />
        </v-card-text>
      </v-card-text>

      <v-card-actions
        v-show="isEditMode"
      >
        <v-spacer />
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
      </v-card-actions>

      <confirm-dlg ref="confirm" />
    </v-card>
  </v-form>
</template>

<script>

import Regix from "@/services/regix.service.js";
import EnvironmentCharacteristicsBasicData from "@/components/tabs/environmentCharacteristics/EnvironmentCharacteristicsBasicData.vue";
import RelativesList  from "@/components/tabs/environmentCharacteristics/RelativesList.vue";
import InstitutionExternalSoProviderChecker from '@/components/institution/InstitutionExternalSoProviderChecker';
import { Permissions } from "@/enums/enums";
import { EnvironmentCharacteristicsModel } from "@/models/environmentCharacteristics/environmentCharacteristicsModel.js";
import { StudentRelativeModel } from "@/models/environmentCharacteristics/studentRelativeModel.js";
import { mapGetters } from 'vuex';

export default {
    name: "EnvironmentCharacteristicsDetails",
    components: {
    EnvironmentCharacteristicsBasicData,
    RelativesList,
    InstitutionExternalSoProviderChecker
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
      isEditMode: false,
      loading: false,
      saving: false,
      form: new EnvironmentCharacteristicsModel(),
      relativeListComponentKey: 0
    };
  },
  computed: {
    ...mapGetters(['currentStudentSummary', 'hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicManage);
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicRead)
      && !this.hasStudentPermission(Permissions.PermissionNameForStudentEnvironmentCharacteristicManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
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
    load(){
      this.loading = true;

      this.$api.environmentCharacteristics.getStudentEnvironmentCharacteristics(this.personId)
      .then(response => {
        if (response.data) {
            this.form  = new EnvironmentCharacteristicsModel(response.data);
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
    getRelatives(){
      Regix.getRelations(this.currentStudentSummary.pin)
      .then((relatives) => {
        this.saving = true;
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
                studentRelativeDetail.relativeType.value = 1;
                break;
                // Майка
                case 2:
                studentRelativeDetail.relativeType.value = 2;
                break;
                // Осиновител, осиновителка
                case 1:
                case 3:
                studentRelativeDetail.relativeType.value = 5;
                break;
            }

            this.$api.environmentCharacteristics.addRelative(studentRelativeDetail).then(() => {
                    this.$router.push(`/student/${this.personId}/environmentCharacteristics`);
                    })
                    .catch((error) => {
                        this.$notifier.error('',this.$t("errors.environmentCharacteristicsRelativeAdd"));
                        console.log(error.response.data.message);
                    });
            });
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.regixRelativesLoad'));
          console.log(error);
        }).finally(() => {
            this.saving = false;
            this.relativeListComponentKey++;
        });
    },
    async validateUser () {

      let hasErrors = this.$validator.validate(this);

      if(hasErrors) {
            this.$notifier.error('', this.$t('validation.hasErrors'));
            return;
        }

         if(await this.$refs.confirm.open(this.$t('common.save'), this.$t('common.confirm'))) {

            const payload = {
                personId: this.personId,
                hasParentConsent: this.form.hasParentConsent,
                livesWithFosterFamily: this.form.livesWithFosterFamily,
                gpName: this.form.gpName,
                gpPhone: this.form.gpPhone,
                representedByTheMajor: this.form.representedByTheMajor,
                nativeLanguage: this.form.nativeLanguage === null ? null : this.form.nativeLanguage.value !== undefined ? this.form.nativeLanguage : {value: this.form.nativeLanguage}
            };

            this.saving = true;

            this.$api.environmentCharacteristics
            .update(payload)
            .then(() => {
                this.$notifier.success(this.$t('common.save'), this.$t('common.saveSuccess'), 5000);
                this.isEditMode = false;
                this.load();
            })
            .catch((error) => {
                this.$notifier.error('', this.$t('errors.studentSave'));
                console.log(error);
            })
            .then(() => { this.saving = false; });
        }
    },
    onReloadData(){
        this.load();
    }
  }
};
</script>
