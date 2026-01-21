<template>
  <v-card
    outlined
    :hover="hover"
    :color="value && value.gradeCategory == -1 ? 'grey lighten-2' : ''"
  >
    <v-card-title
      v-if="value && value.gradeCategory == -1"
      class="py-0 text-h6"
    >
      {{ $t('diplomas.template.section') }}
    </v-card-title>
    <v-card-text
      class="py-0 pr-0"
    >
      <v-alert
        dismissible
        :color="value && value.gradeCategory == -1 ? 'grey lighten-2' : ''"
        class="pa-0 mb-0"
      >
        <template #close>
          <v-col
            align="center"
            class="px-1"
            :style=" { 'max-width': showAddModuleButton ? '150px' : '40px' }"
          >
            <button-tip
              v-if="!disabled"
              icon
              icon-name="mdi-close-thick"
              icon-color="error"
              iclass="mx-0"
              tooltip="buttons.delete"
              bottom
              small
              @click="$emit('delete', value.uid)"
            />
            <div
              v-if="showAddModuleButton"
              class="text-center mt-2"
            >
              <button-group>
                <v-btn
                  x-small
                  color="primary"
                  @click.stop="onSubjectModuleAddClick()"
                >
                  {{ $t("buttons.addSubjectModule") }}
                </v-btn>
              </button-group>
            </div>
            <div
              v-if="showAddModuleButton"
              class="text-center"
            >
              <button-group>
                <v-btn
                  x-small
                  color="secondary"
                  @click.stop="onSubjectHeaderAddClick(isDiplomaSubject === false)"
                >
                  {{ $t("buttons.addHeaderModule") }}
                </v-btn>
              </button-group>
            </div>
          </v-col>
        </template>
        <v-row
          v-if="value && value.gradeCategory == -1"
          dense
        >
          <v-col
            :style="{ 'max-width': '60px' }"
          >
            <v-text-field
              v-if="disabled"
              :value="value.position"
              :label="$t('diplomas.template.position')"
              class="required custom-small-text"
              disabled
              hide-details
            />
            <number-selector
              v-else
              v-model="value.position"
              :min="minPosition"
              :max="maxPosition"
              :excluded="excludedPositions"
              :label="$t('diplomas.template.position')"
              :rules="[$validator.required(true)]"
              :disabled="disabled"
              class="required custom-small-text"
              hide-navigation
            />
          </v-col>
          <v-col>
            <v-text-field
              v-model="value.subjectName"
              :label="$t('diplomas.template.sectionName')"
              class="required custom-small-text"
              hide-details
            />
          </v-col>
          <v-col
            v-if="value.isProfSubjectHeader"
            cols="12"
            md="2"
          >
            <c-select
              v-model="value.basicClassId"
              :items="basicClassOptions"
              :label="$t('basicDocumentPart.basicClass')"
              :rules="[$validator.required()]"
              class="required"
            />
          </v-col>
        </v-row>
        <v-row
          v-if="value && value.gradeCategory != -1"
          dense
        >
          <v-col
            :style="{ 'max-width': '60px' }"
          >
            <v-text-field
              v-if="disabled"
              :value="value.position"
              :label="$t('diplomas.template.position')"
              class="required custom-small-text"
              disabled
              hide-details
            />
            <number-selector
              v-else
              v-model="value.position"
              :min="minPosition"
              :max="maxPosition"
              :excluded="excludedPositions"
              :label="$t('diplomas.template.position')"
              :disabled="disabled"
              :rules="[$validator.required(true)]"
              class="required custom-small-text"
              hide-navigation
            />
          </v-col>
          <v-col
            v-if="!value.subjectCanChange"
            cols="12"
            md="4"
            lg="3"
          >
            <v-text-field
              :value="value.subjectName"
              :label="$t('diplomas.template.subjectName')"
              class="required custom-small-text"
              disabled
              hide-details
            />
          </v-col>
          <v-col
            v-if="value.subjectCanChange"
            cols="12"
            md="4"
            lg="3"
          >
            <autocomplete
              :ref="`Subject_${_uid}`"
              v-model="value.subjectId"
              api="/api/lookups/GetSubjectOptions"
              :label="$t('basicDocumentSubject.subjectName')"
              :placeholder="$t('buttons.search')"
              hide-no-data
              hide-selected
              :page-size="20"
              :rules="[$validator.required()]"
              class="required custom-small-text"
              defer-options-loading
              @change="v => $emit('subjectChange', v)"
            >
              <template v-slot:item="data">
                <v-list-item-content
                  v-text="data.item.text"
                />
                <v-list-item-icon>
                  <v-chip
                    color="light"
                    small
                    outlined
                  >
                    {{ data.item.value }}
                  </v-chip>
                </v-list-item-icon>
              </template>
              <template #append-outer>
                <v-tooltip bottom>
                  <template v-slot:activator="{ on: subjectCheckboxTooltip }">
                    <v-simple-checkbox
                      v-model="value.showSubjectNamePreview"
                      v-ripple
                      off-icon="mdi-printer"
                      on-icon="mdi-printer-off"
                      v-on="{ ...subjectCheckboxTooltip }"
                    />
                  </template>
                  <span>
                    {{ value.showSubjectNamePreview ? $t('diplomas.template.hideSubjectNamePreview') : $t('diplomas.template.showSubjectNamePreview') }}
                  </span>
                </v-tooltip>
              </template>
            </autocomplete>
          </v-col>
          <v-col
            v-if="value.subjectCanChange && value.showSubjectNamePreview"
          >
            <v-text-field
              v-model="value.subjectName"
              :label="$t('diplomas.template.subjectNamePreview')"
              class="custom-small-text"
              hide-details
              clearable
            />
          </v-col>
          <v-col
            v-if="!value.subjectCanChange"
          >
            <v-text-field
              :value="value.subjectTypeName"
              :label="$t('diplomas.template.subjectType')"
              class="required custom-small-text"
              disabled
              hide-details
            />
          </v-col>
          <v-col
            v-if="value.subjectCanChange"
            cols="6"
            md="2"
          >
            <v-select
              v-if="hasSubjectTypeLimit"
              v-model="value.subjectTypeId"
              :items="subjectTypeOptions"
              :disabled="disabled || !value.subjectCanChange || !subjectTypeOptions || subjectTypeOptions.length <= 1"
              :rules="[$validator.required()]"
              :label="$t('diplomas.template.subjectType')"
              class="required custom-small-text"
            />
            <autocomplete
              v-else
              v-model="value.subjectTypeId"
              api="/api/lookups/GetSubjectTypeOptions"
              :label="$t('diplomas.template.subjectType')"
              :placeholder="$t('buttons.search')"
              hide-no-data
              hide-selected
              :page-size="20"
              :rules="[$validator.required()]"
              class="required custom-small-text"
              defer-options-loading
              persistent-hint
              :hint="$t('common.comboSearchHint', [searchInputMinLength])"
            />
          </v-col>
          <v-col
            v-if="!value.isHorariumHidden"
            :style="{ 'max-width': '80px' }"
          >
            <v-text-field
              v-model="value.horarium"
              :label="$t('diplomas.template.horarium')"
              type="number"
              :disabled="disabled"
              :rules="[$validator.min(1)]"
              class="custom-small-text"
              hide-details
              @wheel="$event.target.blur()"
            />
          </v-col>
          <v-col
            v-if="value.isProfSubjectHeader"
            cols="12"
            md="2"
          >
            <c-select
              v-model="value.basicClassId"
              :items="basicClassOptions"
              :label="$t('basicDocumentPart.basicClass')"
              :rules="[$validator.required()]"
              class="required"
            />
          </v-col>
          <slot name="extension">
            <v-col
              v-if="!hasExternalEvaluationLimit"
              :style="{ 'max-width': '70px' }"
            >
              <v-checkbox
                v-model="value.showFlSubject"
                color="primary"
                class="custom-small-text"
                dense
                :hint="$t('diplomas.showFlSubject')"
                persistent-hint
              />
            </v-col>

            <v-col
              v-if="value.showFlSubject"
              cols="12"
              class="py-0"
            >
              <span class="d-flex justify-end">
                <v-col
                  cols="6"
                  sm="3"
                  class="py-0"
                >
                  <autocomplete
                    :ref="`FlSubject_${_uid}`"
                    v-model="value.flSubjectId"
                    api="/api/lookups/GetFlOptions"
                    :label="$t('diplomas.flsubject')"
                    :placeholder="$t('buttons.search')"
                    :page-size="20"
                    :rules="[$validator.required()]"
                    class="required custom-small-text"
                    :defer-options-loading="false"
                  />
                </v-col>
                <v-col
                  cols="6"
                  md="1"
                  class="py-0"
                >
                  <v-text-field
                    v-model.number="value.flHorarium"
                    type="number"
                    :label="$t('diplomas.template.horarium')"
                    :rules="[$validator.min(1)]"
                    class="custom-small-text"
                    @wheel="$event.target.blur()"
                  />
                </v-col>
              </span>
            </v-col>
          </slot>
        </v-row>
      </v-alert>
      <div
        v-if="sortedModules && sortedModules.length > 0"
        class="ml-2"
      >
        <diploma-template-subject-module-editor
          v-for="(module, moduleIndex) in sortedModules"
          :key="module.uid"
          class="mb-1 border-left-primary"
          :value="sortedModules[moduleIndex]"
          :min-position="1"
          :max-position="999"
          :can-add-modules="false"
          has-subject-type-limit
          :subject-type-options="moduleSubjectTypeOptions"
          hover
          @delete="onSubjectModuleDelete"
        >
          <template
            v-if="isDiplomaSubject"
            #extension
          >
            <v-col>
              <diploma-subject-module-grade-editor
                :value="sortedModules[moduleIndex]"
                has-external-evaluation-limit
              />
            </v-col>
          </template>
        </diploma-template-subject-module-editor>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { BasicDocumentTemplateSubjectModel } from '@/models/diploma/basicDocumentTemplateSubjectModel';
import Constants from '@/common/constants';
import { DiplomaSubjectModel } from '@/models/diploma/diplomaSubjectModel';

export default {
  name: 'DiplomaTemplateSubjectEditorComponent',
  components: {
    Autocomplete,
    DiplomaTemplateSubjectModuleEditor: () => import('@/components/diplomas/DiplomaTemplateSubjectEditor'),
    DiplomaSubjectModuleGradeEditor: () => import('@/components/diplomas/DiplomaSubjectGradeEditor')
  },
  props: {
    value: {
      type: Object,
      required: true,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hasSubjectTypeLimit: {
      type: Boolean,
      default() {
        return false;
      }
    },
    subjectTypeOptions: {
      type: Array,
      default() {
        return [];
      }
    },
    excludedPositions: {
      type: Array,
      default() {
        return null;
      }
    },
    minPosition: {
      type: Number,
      default() {
        return 0;
      }
    },
    maxPosition: {
      type: Number,
      default() {
        return 100;
      }
    },
    canAddModules: {
      type: Boolean,
      default() {
        return false;
      }
    },
    hover: {
      type: Boolean,
      default() {
        return false;
      }
    },
    isDiplomaSubject: {
      // Компонентът се използва при шаблоните На дипломи и при самите дипломи.
      // Когато е в диплома трябва да се покажат компонентите за избор на оценка
      type: Boolean,
      default() {
        return false;
      }
    },
    hasExternalEvaluationLimit: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
       searchInputMinLength: Constants.SEARCH_INPUT_MIN_LENGTH,
       subjectTypeToModulesLimit: Constants.subjectTypeToModulesLimit,
       moduleSubjectTypeOptions: Constants.moduleSubjectTypeOptions,
       basicClassOptions: [
        {
          value: 11,
          text: 'единадесети клас'
        },
        {
          value: 12,
          text: 'дванадесети клас'
        }
       ],
    };
  },
  computed: {
    sortedModules() {
      if (!this.value || !this.value.modules) {
        return [];
      }

      let sortedModules = this.value.modules;
      sortedModules = sortedModules.sort((a, b) => { return a.position - b.position; });
      return sortedModules;
    },
    showAddModuleButton() {
      return this.canAddModules && this.value.subjectTypeId && this.subjectTypeToModulesLimit
        && this.subjectTypeToModulesLimit.includes(this.value.subjectTypeId);
    }
  },
  methods: {
    getSubjectOptions() {
      const selector = this.$refs[`Subject_${this._uid}`];
      if (selector) {
        return selector.optionsList;
      }

      return [];
    },
    onSubjectModuleAddClick() {
      if (!this.value) return;

      if (!this.value.modules) {
        this.value.modules = [];
      }

      const toAdd = new BasicDocumentTemplateSubjectModel({
        subjectCanChange: true,
        basicDocumentPartId: this.value.basicDocumentPartId,
        templateId: this.value.templateId,
        isHorariumHidden: this.value.isHorariumHidden,
        isProfSubjectHeader: true
      });

      this.value.modules.push(toAdd);
    },
    onSubjectHeaderAddClick(isProfSubjectHeader) {
      if (!this.value) return;

      if (!this.value.modules) {
        this.value.modules = [];
      }

      const toAdd = new DiplomaSubjectModel({
        subjectCanChange: true,
        basicDocumentPartId: this.value.basicDocumentPartId,
        subjectName: 'Нова подрубрика',
        gradeCategory: -1,
        subjectId: 0,
        subjectTypeId: -1,
        templateId: this.value.templateId,
        isHorariumHidden: this.value.isHorariumHidden,
        isProfSubjectHeader: isProfSubjectHeader
      });

      this.value.modules.push(toAdd);
    },
    onSubjectModuleDelete(subjectModuleUid) {
      if (!this.value || !this.value.modules) {
        return;
      }
      const subjectModuleIndex = this.value.modules.findIndex(x => x.uid === subjectModuleUid);
      this.value.modules.splice(subjectModuleIndex, 1);
    },
  }
};
</script>

<style>
    .border-left-primary.v-card {
      border-left: 5px solid #2F73DA !important
  }
</style>

<style lang="scss" scoped>
  ::v-deep {
    .v-text-field input {
      font-size: 0.8em;
    }

    .custom-small-text .v-label {
      font-size: 0.9em;
    }

    .custom-small-text .v-label {
      font-size: 0.9em;
    }

    .custom-small-text .fl-subject-checkbox {
      font-size: 0.9em;
    }

    .custom-small-text .v-select__selection {
      font-size: 0.8em;
    }

    .v-input__append-inner {
      padding-left: 0 !important;
    }
  }
</style>
