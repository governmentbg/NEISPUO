<template>
  <div>
    <v-form
      @submit.stop.prevent="validate"
    >
      <v-card
        flat
      >
        <v-card-title>
          <v-spacer />
          <edit-button
            v-if="hasEditRights"
            v-model="isEditMode"
            @click="editIconClick"
          />
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
              {{ $t('student.missingInternationalProtectionEditPermission') }}
            </h4>
          </v-alert>
        </v-card-text>
        <v-card-text>
          <v-card>
            <v-card-title class="bg-secondary text-white p-2">
              {{ $t('studentAdditionalData.title') }}
            </v-card-title>
            <v-card-text
              v-for="(internationalProtection, index) in currentInternationalProtections"
              :key="index"
            >
              <v-row dense>
                <v-col
                  cols="12"
                >
                  <v-container
                    class="px-0"
                    fluid
                  >
                    <v-radio-group
                      v-model="internationalProtection.protectionStatus"
                      mandatory
                      row
                      :disabled="!isEditMode"
                    >
                      <v-radio
                        :label="$t('common.internationalProtection')"
                        :value="1"
                      />
                      <v-radio
                        :label="$t('common.temporaryProtection')"
                        :value="2"
                      />
                    </v-radio-group>
                  </v-container>
                </v-col>
              </v-row>

              <v-row dense>
                <v-col
                  cols="12"
                  sm="6"
                  lg="3"
                >
                  <v-text-field
                    v-model="internationalProtection.docNumber"
                    :label="$t('studentAdditionalData.docNumber')"
                    autocomplete="reason"
                    :disabled="!isEditMode"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="6"
                  lg="3"
                >
                  <date-picker
                    id="docDate"
                    ref="docDate"
                    v-model="internationalProtection.docDate"
                    :show-buttons="false"
                    :scrollable="false"
                    :no-title="true"
                    :show-debug-data="false"
                    :disabled="!isEditMode"
                    :label="$t('studentAdditionalData.docDate')"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="6"
                  lg="3"
                >
                  <date-picker
                    id="validFrom"
                    ref="validFrom"
                    v-model="internationalProtection.validFrom"
                    :show-buttons="false"
                    :scrollable="false"
                    :disabled="!isEditMode || saving"
                    :no-title="true"
                    :show-debug-data="false"
                    :label="$t('common.validFrom')"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="5"
                  lg="2"
                >
                  <date-picker
                    id="validTo"
                    ref="validTo"
                    v-model="internationalProtection.validTo"
                    :show-buttons="false"
                    :scrollable="false"
                    :disabled="!isEditMode || saving"
                    :no-title="true"
                    :show-debug-data="false"
                    :label="$t('common.validTo')"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="1"
                  class="text-right"
                >
                  <button-tip
                    small
                    icon-name="mdi-delete"
                    iclass="mx-2"
                    dark
                    :disabled="!isEditMode"
                    color="red"
                    tooltip="buttons.internationalProtectionDeleteTooltip"
                    bottom
                    fab
                    @click="onDeleteInternationalProtectionClick(index)"
                  />
                </v-col>
              </v-row>
              <v-divider
                v-if="index < currentInternationalProtections.length - 1"
              />
            </v-card-text>
          </v-card>
        </v-card-text>
      </v-card>
      <v-card-actions
        v-show="isEditMode"
      >
        <button-tip
          small
          icon-name="mdi-plus"
          iclass="mx-2"
          dark
          color="primary"
          tooltip="buttons.internationalProtectionAddTooltip"
          bottom
          :disabled="!isEditMode"
          fab
          @click="onAddNewInternationalProtectionClick"
        />
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
          @click="onCancel($t('buttons.clear'))"
        >
          <v-icon left>
            fas fa-times
          </v-icon>
          {{ $t('buttons.cancel') }}
        </v-btn>
      </v-card-actions>
    </v-form>

    <confirm-dlg ref="confirm" />

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import { StudentInternationalProtection } from '@/models/studentInternationalProtection.js';
import { InstType } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentInternationalProtection',
  components: {
  },
  props: {
    personId: {
      type: Number,
      required: true,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      isEditMode: false,
      hasInternationalProtectionStatus: false,
      currentInternationalProtections: [],
      saving: false,
      loading: false,
      canEditStudentPersonalDetails: false
    };
  },
  computed: {
    ...mapGetters(['isInstType']),
    hasEditRights() {
      return this.canEditStudentPersonalDetails;
    },
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    }
  },
  mounted() {
    this.load();
    this.checkEditStudentPersonalDetailsPermission();
  },
  methods: {
    load(){
      this.loading = true;

      this.$api.student.getStudentInternationalProtection(this.personId)
      .then(response => {
        if (response.data) {
          this.hasInternationalProtectionStatus = response.data.hasInternationalProtectionStatus;
          if(response.data.internationalProtections) {
            this.currentInternationalProtections = response.data.internationalProtections;
          }
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('errors.studentLoad'));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    checkEditStudentPersonalDetailsPermission() {
      this.$api.student.canEditStudentPersonalDetails(this.personId)
      .then(response => {
        if (response.data) {
          this.canEditStudentPersonalDetails = response.data;
        }
      })
      .catch(error => {
        console.log(error);
      });
    },
    saveInProgress(value) {
      this.$emit('saveInProgress', value);
      this.saving = value;
    },
    onInternationalProtectionCheck() {
      this.currentInternationalProtections = [];
      if(this.hasInternationalProtectionStatus) {
        this.onAddNewInternationalProtectionClick();
      }
    },
    onAddNewInternationalProtectionClick() {
      this.currentInternationalProtections.push(new StudentInternationalProtection());
    },
    onDeleteInternationalProtectionClick(index) {
      this.currentInternationalProtections.splice(index, 1);
      if(this.currentInternationalProtections.length === 0) {
        this.hasInternationalProtectionStatus = false;
      }
    },
    async editIconClick(isEditMode) {
      if (!isEditMode) {
        await this.onCancel('');
      }
    },
    async onCancel(val) {
      if(await this.$refs.confirm.open(val, this.$t('common.confirm'))){
            this.isEditMode = false;
            this.load();
         }
    },
    async validate() {
       var isValid = await this.$refs.confirm.open(this.$t('common.save'), this.$t('common.confirm'));
          if(isValid){
                this.saveInProgress(true);

                    const payload = {
                    personId: this.personId,
                    hasInternationalProtectionStatus: this.hasInternationalProtectionStatus,
                    internationalProtections: this.currentInternationalProtections
                    };

                    this.$api.student
                        .updateInternationalProtection(payload)
                        .then(() => {
                            this.isEditMode = false;
                            this.$notifier.success('', this.$t('common.saveSuccess'));
                            this.load();
                        })
                        .catch((error) => {
                            this.$notifier.error('', this.$t('errors.studentSave'));
                            console.log(error.response);
                        })
                        .finally(() => {
                            this.saveInProgress(false);
                        });
          }
    }
  }
};
</script>
