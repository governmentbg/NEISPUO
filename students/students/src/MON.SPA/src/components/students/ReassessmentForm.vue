<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row dense>
        <v-col
          cols="12"
          sm="6"
          lg="3"
        >
          <c-text-field
            v-if="disabled"
            :value="model.schoolYearName"
            :label="$t('reassessment.schoolYear')"
            outlined
            persistent-placeholder
            class="required"
            dense
          />
          <school-year-selector
            v-else
            v-model="model.schoolYear"
            :label="$t('reassessment.schoolYear')"
            class="required"
            :clearable="false"
            :rules="[$validator.required()]"
            show-current-school-year-button
            outlined
            persistent-placeholder
            dense
            :show-navigation-buttons="false"
          />
        </v-col>
        <v-col>
          <c-text-field
            v-if="disabled"
            :value="model.reasonName"
            :label="$t('equalization.reason')"
            outlined
            persistent-placeholder
            class="required"
            dense
          />
          <custom-autocomplete
            v-else
            v-model="model.reasonId"
            api="/api/lookups/GetReasonsForReassessment"
            :label="$t('reassessment.reason')"
            :placeholder="$t('buttons.search')"
            :defer-options-loading="false"
            clearable
            hide-no-data
            hide-selected
            :rules="[$validator.required()]"
            class="required"
            outlined
            persistent-placeholder
            dense
          />
        </v-col>
        <v-col
          v-if="model.reasonId === Constants.ReassessmentEnableClassReasonId"
          cols="12"
          sm="6"
          lg="3"
        >
          <c-info uid="reassessment.basicClass">
            <v-text-field
              v-if="disabled"
              :value="model.basicClassName"
              :label="$t('reassessment.basicClass')"
              outlined
              persistent-placeholder
              dense
            />
            <custom-autocomplete
              v-else
              v-model="model.inClass"
              api="/api/lookups/GetBasicClassOptions"
              :filter="{ minId, maxId }"
              :label="$t('reassessment.basicClass')"
              :placeholder="$t('buttons.search')"
              :defer-options-loading="false"
              clearable
              hide-no-data
              hide-selected
              :rules="[$validator.required()]"
              class="required"
              outlined
              persistent-placeholder
              dense
            />
          </c-info>
        </v-col>
      </v-row>
      <v-row dense>
        <v-col>
          <file-manager
            v-if="!disabled || (model.documents && model.documents.length > 0)"
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
      <v-expansion-panels
        v-model="panelModel"
        multiple
        popout
        class="mt-4"
      >
        <v-expansion-panel>
          <v-expansion-panel-header>
            {{ $t('reassessment.evaluations') }}
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-alert
              v-for="(item, index) in model.reassessmentDetails"
              :key="item.uid"
              class="pa-0 mb-0"
              dense
            >
              <template #close>
                <v-col
                  align="center"
                  :style="{ 'max-width': '40px' }"
                  class="pa-0 mb-10"
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
                    @click="onEqualizationRemove(index)"
                  />
                </v-col>
              </template>
              <v-row
                dense
                class="small-text"
              >
                <v-col
                  cols="12"
                  sm="6"
                  md="3"
                >
                  <c-info
                    uid="reassessment.subject"
                  >
                    <c-text-field
                      v-if="disabled"
                      :value="item.subjectName"
                      :label="$t('reassessment.subject')"
                      outlined
                      persistent-placeholder
                      dense
                      class="required"
                    >
                      <template #append>
                        <v-chip
                          v-if="item.subjectId"
                          color="light"
                          small
                          outlined
                        >
                          {{ `${ item.subjectId }` }}
                        </v-chip>
                      </template>
                    </c-text-field>
                    <custom-autocomplete
                      v-else
                      id="subject"
                      :ref="'subject' + item.index"
                      v-model="item.subjectId"
                      api="/api/lookups/GetSubjectOptions"
                      :label="$t('reassessment.subject')"
                      :placeholder="$t('buttons.search')"
                      defer-options-loading
                      hide-no-data
                      hide-selected
                      remove-items-on-clear
                      :disabled="disabled"
                      :rules="[$validator.required()]"
                      clearable
                      class="required"
                      outlined
                      persistent-placeholder
                      dense
                      append-icon=""
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
                      <template v-slot:selection="data">
                        <span>{{ data.item.text }}</span>
                        <v-chip
                          color="light"
                          small
                          outlined
                        >
                          {{ data.item.value }}
                        </v-chip>
                      </template>
                    </custom-autocomplete>
                  </c-info>
                </v-col>
                <v-col>
                  <c-text-field
                    v-if="disabled"
                    :value="item.subjectTypeName"
                    :label="$t('reassessment.subjectType')"
                    class="required"
                    outlined
                    persistent-placeholder
                    dense
                  />
                  <v-autocomplete
                    v-else
                    v-model="item.subjectTypeId"
                    :items="subjectTypeOptions"
                    :label="$t('reassessment.subjectType')"
                    :rules="[$validator.required()]"
                    class="required"
                    outlined
                    persistent-placeholder
                    dense
                    append-icon=""
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
                          {{ data.item.partName }}
                        </v-chip>
                      </v-list-item-icon>
                    </template>
                  </v-autocomplete>
                </v-col>
                <v-col
                  cols="12"
                  sm="6"
                  md="3"
                >
                  <diploma-subject-grade-editor
                    :value="item"
                    outlined
                    persistent-placeholder
                    dense
                    append-icon=""
                  />
                </v-col>
              </v-row>
            </v-alert>
            <v-row
              v-if="!disabled"
              dense
            >
              <v-col cols="12">
                <button-tip
                  color="primary"
                  text="equalization.addNew"
                  tooltip="equalization.addNew"
                  iclass=""
                  outlined
                  bottom
                  small
                  @click="onEqualizationAdd()"
                />
              </v-col>
            </v-row>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
    </v-form>
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import groupBy from 'lodash.groupby';
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import CustomAutocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import DiplomaSubjectGradeEditor from '@/components/diplomas/DiplomaSubjectGradeEditor';
import FileManager from '@/components/common/FileManager.vue';
import Constants from "@/common/constants.js";
import { ReassessmentModel } from "@/models/reassessmentModel";

export default {
  name: "ReassessmentForm",
  components: { SchoolYearSelector, CustomAutocomplete, DiplomaSubjectGradeEditor, FileManager },
  props: {
    document: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model: this.document ?? new ReassessmentModel(),
      panelModel: [0],
      subjectTypeOptions: [],
      Constants,
      minId: 5,
      maxId: 12
    };
  },
  computed: {
    ...mapGetters(["termOptions"]),
  },
  created(){
    if (!this.model.reassessmentDetails?.length) {
      this.onEqualizationAdd();
    }
  },
  mounted() {
    this.loadSubjectTypeOptions();
  },
  methods: {
    onEqualizationAdd() {
      if (!this.model.reassessmentDetails) this.model.reassessmentDetails = [];

      this.model.reassessmentDetails.push({
        id: 0,
        subjectId: null,
        basicClassId: null,
        gradeCategory: 1,
        grade: null,
        specialNeedsGrade: null,
        otherGrade: null,
        uid: this.$uuid.v4()
      });

      this.updateListSortOrder();
    },
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
    updateListSortOrder () {
      if (!this.model || !this.model.reassessmentDetails) return;

      const newList = [...this.model.reassessmentDetails].map((item, index) => {
        const newSort = index + 1;
       // also add in a new property called has changed if you want to style them / send an api call
        item.hasChanged = item.sortOrder !== newSort;
        if (item.hasChanged) {
          item.sortOrder = newSort;
        }
        return item;
      });

      this.model.reassessmentDetails = newList;
    },
    loadSubjectTypeOptions() {
      this.$api.lookups.getSubjectTypeOptions()
        .then((response) => {
          if (response.data) {
            const groups = groupBy(response.data, 'partName');

            let options = [];
            for (const key in groups) {
              options.push({ header: key });
              options = [...options, ...groups[key].map(x => {
                return { value: x.value, text: x.text, partId: x.partId, partName: x.partName };
              })];
              options.push({ divider: true });
            }

            this.subjectTypeOptions = options;
          }
        })
        .catch((error) => {
          console.log(error);
        });
    },
    onEqualizationRemove(index) {
      if (!this.model.reassessmentDetails) return; // Няма от какво да махаме
      this.model.reassessmentDetails.splice(index, 1);
      this.updateListSortOrder();
    },
  },
};
</script>

<style lang="scss" scoped>

  ::v-deep {
    .small-text {
      & .v-select__slot {
        font-size: 0.8em;
      }
      & .v-text-field__slot {
        font-size: 0.8em;
      }
    }
  }
</style>
