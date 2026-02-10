<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row
        dense
      >
        <v-col
          cols="12"
          md="6"
          lg="3"
        >
          <school-year-selector
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
      </v-row>

      <v-tabs
        v-model="tab"
        color="deep-purple accent-4"
        centered
      >
        <v-tab
          key="commonPds"
        >
          {{ $t('lod.commonPersonalDevelopmentSupport') }}
        </v-tab>

        <v-tab
          key="additionalPds"
        >
          {{ $t('lod.additionalPersonalDevelopmentSupport') }}
        </v-tab>

        <v-tabs-items
          v-model="tab"
        >
          <v-tab-item
            key="commonPds"
            eager
            class="pt-3"
          >
            <v-alert
              v-for="(commonSupportType, index) in model.commonSupportTypeReasons"
              :key="index"
              dense
              class="mb-0"
            >
              <template #close>
                <v-col
                  align="center"
                  :style="{ 'max-width': '40px' }"
                  class="pa-0"
                >
                  <button-tip
                    v-if="!disabled"
                    icon
                    icon-name="mdi-delete"
                    icon-color="error"
                    tooltip="buttons.delete"
                    bottom
                    iclass=""
                    small
                    @click="onDeleteCommonSupportTypeReasonClick(index)"
                  />
                </v-col>
              </template>
              <v-row dense>
                <v-col>
                  <c-select
                    v-model="commonSupportType.reasonId"
                    :items="commonSupportTypeOptions"
                    :label="$t('studentOtherInstitutions.reasonLabel')"
                    :rules="[$validator.required()]"
                    class="required"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                  />
                </v-col>
                <v-col>
                  <c-textarea
                    v-model="commonSupportType.information"
                    :label="$t('common.detailedInformation')"
                    outlined
                    dense
                    persistent-placeholder
                    clearable
                  />
                </v-col>
              </v-row>
            </v-alert>
            <v-card-actions
              v-if="!disabled"
            >
              <button-tip
                v-if="!disabled"
                icon-name="mdi-plus"
                icon-color="primary"
                color="primary"
                text="buttons.lodCommonSupportTypeAddTooltip"
                tooltip="buttons.lodCommonSupportTypeAddTooltip"
                bottom
                outlined
                small
                @click="onAddCommonSupportTypeReasonClick"
              />
            </v-card-actions>
            <v-divider />
            <v-card-subtitle>
              {{ $t('common.attachedFiles') }}
            </v-card-subtitle>
            <file-manager
              v-model="model.commonSupportDocuments"
              :disabled="disabled"
            />
          </v-tab-item>
          <v-tab-item
            key="additionalPds"
            eager
            class="pt-3"
          >
            <v-row dense>
              <v-col>
                <c-select
                  v-model="model.supportPeriodTypeId"
                  :items="supportPeriodTypeOptions"
                  :label="$t('common.period')"
                  clearable
                  dense
                  persistent-placeholder
                  outlined
                />
              </v-col>
              <v-col>
                <c-select
                  v-model="model.studentTypeId"
                  :items="studentTypeOptions"
                  :label="$t('lod.studentType') "
                  clearable
                  dense
                  persistent-placeholder
                  outlined
                />
              </v-col>
            </v-row>
            <v-alert
              v-for="(additionalSupportType, index) in model.additionalSupportTypeReasons"
              :key="index"
              dense
              class="mb-0"
            >
              <template #close>
                <v-col
                  align="center"
                  :style="{ 'max-width': '40px' }"
                  class="pa-0"
                >
                  <button-tip
                    v-if="!disabled"
                    icon
                    icon-name="mdi-delete"
                    icon-color="error"
                    tooltip="buttons.delete"
                    bottom
                    iclass=""
                    small
                    @click="onDeleteAdditionalSupportTypeClick(index)"
                  />
                </v-col>
              </template>
              <v-row
                dense
              >
                <v-col>
                  <c-select
                    v-model="additionalSupportType.reasonId"
                    :items="additionalSupportTypeOptions"
                    :label="$t('lod.supportType')"
                    :rules="[$validator.required()]"
                    class="required"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                  />
                </v-col>

                <v-col>
                  <c-textarea
                    v-model="additionalSupportType.information"
                    :label="$t('common.detailedInformation')"
                    prepend-icon="mdi-comment"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                  />
                </v-col>
              </v-row>
            </v-alert>
            <v-card-actions
              v-if="!disabled"
            >
              <button-tip
                icon-name="mdi-plus"
                icon-color="primary"
                color="primary"
                text="buttons.lodAdditionalSupportTypeAddTooltip"
                tooltip="buttons.lodAdditionalSupportTypeAddTooltip"
                bottom
                outlined
                small
                @click="onAddAdditionalSupportTypeClick"
              />
            </v-card-actions>
            <sop-to-resource-support-check-status
              v-if="hasResourceSupportSelected && personId && schoolYear"
              :person-id="personId"
              :school-year="schoolYear"
              :disabled="disabled"
            />
            <v-divider />
            <v-card-subtitle>
              {{ $t('common.attachedFiles') }}
            </v-card-subtitle>
            <file-manager
              v-model="model.additionalSupportDocuments"
              :disabled="disabled"
            />
          </v-tab-item>
        </v-tabs-items>
      </v-tabs>
    </v-form>
  </div>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import FileManager from '@/components/common/FileManager.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";
import Constants from "@/common/constants.js";
import SopToResourceSupportCheckStatus from '@/components/tabs/studentSOP/SopToResourceSupportCheckStatus.vue';

import { ReasonModel } from '@/models/reasonModel.js';

export default {
  name: 'PersonalDevelopmentSupportForm',
  components: {
    SchoolYearSelector,
    FileManager,
    SopToResourceSupportCheckStatus
  },
  props: {
    value: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    }
  },
  data() {
    return {
      model: this.value,
      tab: null,
      commonSupportTypeOptions: [],
      studentTypeOptions: [],
      supportPeriodTypeOptions: [],
      additionalSupportTypeOptions: [],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermisssion() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage);
    },
    hasResourceSupportSelected() {
      if(!this.model || !this.model.additionalSupportTypeReasons) return false;

      return this.model.additionalSupportTypeReasons.some(x => x.reasonId === Constants.personalDevelopmentResourceSupportReasonTypeId);
    }
  },
  watch: {
    value() {
      this.model = this.value;
    }
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getCommonSupportTypeOptions()
        .then((response) => {
          this.commonSupportTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.commonSupportTypeOptionsLoad'));
          console.log(error.response);
        });

      this.$api.lookups.getStudentTypeOptions()
        .then((response) => {
          this.studentTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.studentTypeOptionsLoad'));
          console.log(error.response);
        });

      this.$api.lookups.getSupportPeriodOptions()
        .then((response) => {
          this.supportPeriodTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.supportPeriodTypeOptionsLoad'));
          console.log(error.response);
        });

      this.$api.lookups.getAdditionalSupportTypeOptions()
        .then((response) => {
          this.additionalSupportTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.additionalSupportTypeOptionsLoad'));
          console.log(error.response);
        });
    },
    validate() {
      // Използва се в StudentPersonalDevelopmentCreate.vue и StudentPersonalDevelopmentEdit.vue
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
    onAddCommonSupportTypeReasonClick() {
      if(!this.model) return;
      if(!this.model.commonSupportTypeReasons) this.model.commonSupportTypeReasons = [];

      this.model.commonSupportTypeReasons.push(new ReasonModel());
    },
    onAddAdditionalSupportTypeClick() {
      if(!this.model) return;
      if(!this.model.additionalSupportTypeReason) this.model.additionalSupportTypeReason = [];

      this.model.additionalSupportTypeReasons.push(new ReasonModel());
    },
    onDeleteCommonSupportTypeReasonClick(index) {
      if(!this.model && !this.this.model.commonSupportTypeReasons) return;
      this.model.commonSupportTypeReasons.splice(index, 1);
    },
    onDeleteAdditionalSupportTypeClick(index ) {
      if(!this.model && !this.this.model.additionalSupportTypeReasons) return;
      this.model.additionalSupportTypeReasons.splice(index, 1);
    },
  }
};
</script>
