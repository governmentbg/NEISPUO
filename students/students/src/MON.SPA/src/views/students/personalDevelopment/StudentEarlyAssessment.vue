<template>
  <div>
    <app-loader
      v-if="loading"
    />
    <form-layout
      v-else
      :disabled="!isEditMode"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        {{ $t("earlyAssessment.title") }}
        <v-spacer />
        <edit-button
          v-if="hasManagePermission && !isEditMode"
          v-model="isEditMode"
        />
      </template>

      <template #default>
        <v-form
          v-if="model"
          :ref="formRefKey"
          :disabled="!isEditMode"
        >
          <v-card
            class="mb-6"
            flat
            outlined
          >
            <v-card-subtitle>
              {{ $t('earlyAssessment.learningDisability.title') }}
            </v-card-subtitle>
            <v-card-text>
              <v-row dense>
                <v-col>
                  <c-select
                    v-if="isEditMode"
                    v-model="model.learningDisability.ageRange"
                    :items="ageOptions"
                    :label="$t('earlyAssessment.learningDisability.ageRange')"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                  />
                  <c-text-field
                    v-else
                    v-model="model.learningDisability.ageRange"
                    :label="$t('earlyAssessment.learningDisability.ageRange')"
                    dense
                    persistent-placeholder
                    outlined
                  />
                </v-col>
                <v-col>
                  <c-select
                    v-if="isEditMode"
                    v-model="model.learningDisability.result"
                    :items="resultOptions"
                    :label="$t('earlyAssessment.learningDisability.result')"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                  />
                  <c-text-field
                    v-else
                    v-model="model.learningDisability.result"
                    :label="$t('earlyAssessment.learningDisability.result')"
                    dense
                    persistent-placeholder
                    outlined
                  />
                </v-col>
                <v-col>
                  <c-text-field
                    v-model.number="model.learningDisability.score"
                    type="number"
                    :label="$t('earlyAssessment.learningDisability.score')"
                    clearable
                    dense
                    persistent-placeholder
                    outlined
                    :rules="[$validator.min(0)]"
                  />
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <v-row
            dense
          >
            <v-col
              cols="12"
            >
              <c-textarea
                v-model="model.additionalInfo"
                :label="$t('earlyAssessment.additionalInfo')"
                outlined
                dense
                persistent-placeholder
                clearable
              />
            </v-col>
            <v-col
              cols="12"
            >
              <c-textarea
                v-model="model.bgAdditionalTrainingInfo"
                :label="$t('earlyAssessment.bgAdditionalTrainingInfo')"
                outlined
                dense
                persistent-placeholder
                clearable
              />
            </v-col>
            <v-col
              cols="12"
            >
              <c-textarea
                v-model="model.conclusionInfo"
                :label="$t('earlyAssessment.conclusionInfo')"
                outlined
                dense
                persistent-placeholder
                clearable
              />
            </v-col>
          </v-row>

          <v-alert
            v-for="(reason, index) in model.disabilityReasons"
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
                  v-if="isEditMode"
                  icon
                  icon-name="mdi-delete"
                  icon-color="error"
                  tooltip="buttons.delete"
                  bottom
                  iclass=""
                  small
                  @click="onReasonDeleteClick(index)"
                />
              </v-col>
            </template>
            <v-row dense>
              <v-col>
                <c-select
                  v-if="isEditMode"
                  v-model="reason.reasonId"
                  :items="reasonOptions"
                  :label="$t('studentOtherInstitutions.reasonLabel')"
                  :rules="[$validator.required()]"
                  class="required"
                  clearable
                  dense
                  persistent-placeholder
                  outlined
                />
                <c-text-field
                  v-else
                  v-model="reason.reasonName"
                  :label="$t('studentOtherInstitutions.reasonLabel')"
                  outlined
                  dense
                  persistent-placeholder
                />
              </v-col>
              <v-col>
                <c-textarea
                  v-model="reason.details"
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
            v-if="isEditMode"
          >
            <button-tip
              icon-name="mdi-plus"
              outlined
              color="primary"
              icon-color="primary"
              text="earlyAssessment.learningDisability.reasonAddBtnText"
              tooltip="earlyAssessment.learningDisability.reasonAddBtnTooltip"
              bottom
              small
              @click="onReasonAddClick"
            />
          </v-card-actions>
          <v-divider />
          <v-card-subtitle>
            {{ $t('common.attachedFiles') }}
          </v-card-subtitle>
          <file-manager
            v-model="model.documents"
            :disabled="!isEditMode"
          />
        </v-form>
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import AppLoader from '@/components/wrappers/loader.vue';
import FileManager from '@/components/common/FileManager.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentEarlyAssessmentView',
  components: {
    AppLoader,
    FileManager
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      loading: false,
      isEditMode: false,
      formRefKey: `EarlyAssessmentForm_${this._uid}`,
      model: null,
      ageOptions: [
        {
          value: '3 г. – 3 г. 3 м. 15 дни',
          text: '3 г. – 3 г. 3 м. 15 дни'
        },
        {
          value: '3 г. 3 м. 16 дни – 3 г. 6 м.',
          text: '3 г. 3 м. 16 дни – 3 г. 6 м.'
        },
      ],
      resultOptions: [
        {
          value: 'високи',
          text: 'високи'
        },
        {
          value: 'средни',
          text: 'средни'
        },
        {
          value: 'ниски',
          text: 'ниски'
        },
        {
          value: 'критични',
          text: 'критични'
        },
      ],
      reasonOptions: [],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentRead
      );
    },
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentManage
      );
    },
  },
  mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.load();
    this.loadOptions();
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.earlyAssessment.getByPerson(this.personId)
        .then((response) => {
          this.model = response?.data ?? { learningDisability: {} };
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.loadError'));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    loadOptions() {
      this.$api.lookups.getEarlyEvaluationReasonOptions()
        .then((response) => {
          this.reasonOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.earlyEvaluationReasonOptionsLoad'));
          console.log(error.response);
        });
    },
    onReasonAddClick() {
      if(!this.model) return;
      if(!this.model.disabilityReasons) this.model.disabilityReasons = [];

      this.model.disabilityReasons.push({
        id: null,
        reasonId: null,
        details: null
      });
    },
    onReasonDeleteClick(index) {
      if(!this.model && !this.this.model.disabilityReasons) return;
      this.model.disabilityReasons.splice(index, 1);
    },
    async onSave() {
      const form = this.$refs[this.formRefKey];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      if (this.model.personId) this.model.personId = this.personId;

      this.saving = true;
      this.$api.earlyAssessment
        .createOrUpdate(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'));
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.load();
      this.isEditMode = false;
    },
  }
};
</script>
