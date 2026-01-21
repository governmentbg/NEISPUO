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
            :label="$t('equalization.schoolYear')"
            outlined
            persistent-placeholder
            class="required"
            dense
          />
          <school-year-selector
            v-else
            v-model="model.schoolYear"
            :label="$t('equalization.schoolYear')"
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
            api="/api/lookups/GetReasonsForEqualization"
            :label="$t('equalization.reason')"
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
          v-if="(model.reasonId === Constants.EqualizationEnableClassReasonId)"
          cols="12"
          sm="6"
          lg="2"
        >
          <c-text-field
            v-if="disabled"
            :value="model.basicClassName"
            :label="$t('equalization.inClass')"
            outlined
            persistent-placeholder
            dense
          />
          <custom-autocomplete
            v-else
            v-model="model.inClass"
            api="/api/equalization/GetEqualizationReasonTypeClasses"
            :label="$t('equalization.inClass')"
            :placeholder="$t('buttons.search')"
            :defer-options-loading="false"
            clearable
            hide-no-data
            hide-selected
            :disabled="disabled"
            outlined
            persistent-placeholder
            dense
          />
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
      <hr>
      <v-expansion-panels
        v-model="panelModel"
        multiple
        popout
        class="mt-4"
      >
        <v-expansion-panel>
          <v-expansion-panel-header>
            {{ $t('equalization.evaluations') }}
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <draggable
              :list="model.equalizationDetails"
              :disabled="disabled"
              :sort="true"
              ghost-class="ghost"
              @change="updateListSortOrder"
            >
              <transition-group>
                <v-alert
                  v-for="(item, index) in model.equalizationDetails"
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
                      md="2"
                    >
                      <c-info
                        uid="equalization.subject"
                      >
                        <c-text-field
                          v-if="disabled"
                          :value="item.subjectName"
                          :label="$t('equalization.subject')"
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
                          :ref="'subject' + item.uid"
                          v-model="item.subjectId"
                          api="/api/lookups/GetSubjectOptions"
                          :label="$t('equalization.subject')"
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
                        :label="$t('equalization.subjectType')"
                        class="required"
                        outlined
                        persistent-placeholder
                        dense
                      />
                      <v-autocomplete
                        v-else
                        v-model="item.subjectTypeId"
                        :items="subjectTypeOptions"
                        :label="$t('equalization.subjectType')"
                        :rules="[$validator.required()]"
                        class="required"
                        clearable
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
                    <v-col>
                      <c-info
                        uid="equalization.basicClass"
                      >
                        <v-text-field
                          v-if="disabled"
                          :value="item.basicClassName"
                          :label="$t('equalization.basicClass')"
                          outlined
                          persistent-placeholder
                          dense
                        />
                        <v-autocomplete
                          v-else
                          id="subjectBasicClass"
                          ref="subjectBasicClass"
                          v-model="item.basicClassId"
                          :items="basicClassOptions"
                          :label="$t('equalization.basicClass')"
                          :placeholder="$t('buttons.search')"
                          clearable
                          hide-no-data
                          hide-selected
                          outlined
                          persistent-placeholder
                          dense
                          append-icon=""
                        />
                      </c-info>
                    </v-col>
                    <v-col>
                      <c-info
                        uid="equalization.term"
                      >
                        <v-text-field
                          v-if="disabled"
                          :value="getTermText(item.term)"
                          :label="$t('equalization.term')"
                          outlined
                          persistent-placeholder
                          dense
                        />
                        <v-select
                          v-else
                          id="term"
                          ref="term"
                          v-model="item.term"
                          :items="termOptions"
                          :label="$t('equalization.term')"
                          clearable
                          outlined
                          persistent-placeholder
                          dense
                          append-icon=""
                        />
                      </c-info>
                    </v-col>
                    <v-col
                      cols="12"
                      sm="6"
                      md="1"
                    >
                      <v-text-field
                        v-model.number="item.horarium"
                        type="number"
                        :label="$t('lod.evaluations.annualHours')"
                        :min="0"
                        :rules="[$validator.min(0)]"
                        outlined
                        persistent-placeholder
                        dense
                        clearable
                      />
                    </v-col>
                    <v-col>
                      <c-info
                        uid="equalization.session"
                      >
                        <v-text-field
                          v-if="disabled"
                          :value="item.sessionName"
                          :label="$t('equalization.session')"
                          outlined
                          persistent-placeholder
                          dense
                        />
                        <v-autocomplete
                          v-else
                          v-model="item.sessionId"
                          :items="sessionOptions"
                          :label="$t('equalization.session')"
                          :rules="[$validator.required()]"
                          class="required"
                          clearable
                          hide-no-data
                          outlined
                          persistent-placeholder
                          dense
                          append-icon=""
                        />
                      </c-info>
                    </v-col>
                  </v-row>
                </v-alert>
              </transition-group>
            </draggable>

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
import { EqualizationModel } from '@/models/equalizationModel';
import FileManager from '@/components/common/FileManager.vue';
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Draggable from 'vuedraggable';
import DiplomaSubjectGradeEditor from '@/components/diplomas/DiplomaSubjectGradeEditor';
import Constants from '@/common/constants.js';
import groupBy from 'lodash.groupby';
import { mapGetters } from 'vuex';

export default {
  name: 'EqualizationForm',
  components: { FileManager, CustomAutocomplete, Draggable, DiplomaSubjectGradeEditor, SchoolYearSelector },
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
    isFromEdit: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model: this.document ?? new EqualizationModel(),
      panelModel: [0],
      grades: [],
      basicClassOptions: [],
      subjectTypeOptions: [],
      sessionOptions: [],
      Constants: Constants,
    };
  },
  computed: {
    ...mapGetters(['termOptions']),
  },
  created(){
    if (!this.model.equalizationDetails?.length) {
      this.onEqualizationAdd();
    }
  },
  mounted() {
    this.loadGradeOptions();
    this.loadBasicClassOptions();
    this.loadSubjectTypeOptions();
    this.loadSessionOptions();
  },
  methods: {
    onEqualizationAdd() {
      if (!this.model.equalizationDetails) this.model.equalizationDetails = [];

      this.model.equalizationDetails.push({
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
    onEqualizationRemove(index) {
      if (!this.model.equalizationDetails) return; // Няма от какво да махаме
      this.model.equalizationDetails.splice(index, 1);
      this.updateListSortOrder();
    },
    loadGradeOptions() {
      this.$api.lookups
        .getGradesByBasicClassId({ filterSpecialGrade: true })
        .then((response) => {
          if (response.data) {
            this.grades = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassOptions({ minId: 5, maxId: 12 })
        .then((response) => {
          if (response.data) {
            this.basicClassOptions = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
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
    loadSessionOptions(){
      this.$api.lookups.getStudentSessionOptions()
        .then((response) => {
          if (response.data) {
            this.sessionOptions = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
    updateListSortOrder () {
      if (!this.model || !this.model.equalizationDetails) return;

      const newList = [...this.model.equalizationDetails].map((item, index) => {
        const newSort = index + 1;
       // also add in a new property called has changed if you want to style them / send an api call
        item.hasChanged = item.sortOrder !== newSort;
        if (item.hasChanged) {
          item.sortOrder = newSort;
        }
        return item;
      });

      this.model.equalizationDetails = newList;
    },
    getTermText(term) {
      if(!this.termOptions) return '';

      return this.termOptions.find(x => x.value === term)?.text ?? '';
    }
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

