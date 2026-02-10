<!-- eslint-disable vue/no-template-key -->
<!-- eslint-disable vue/valid-v-for -->
<!-- eslint-disable vue/no-v-for-template-key-on-child -->
<template>
  <div>
    <v-card
      v-if="value"
      class="mb-2"
    >
      <v-card-title>
        <button-tip
          v-if="isEditMode"
          icon
          icon-name="mdi-close"
          icon-color="error"
          tooltip="buttons.remove"
          bottom
          iclass=""
          @click="$emit('curriculumPartRemove', value)"
        />
        {{ value.curriculumPart }} {{ value.isSelfEduForm === true ? ` - ${$t('lod.assessments.headers.isSelfEduForm')}` : '' }}
        <v-spacer />
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          :label="$t('common.search')"
          single-line
          hide-details
          class="pt-0 mt-0"
        />
      </v-card-title>
      <v-card-text>
        <v-data-table
          v-if="value"
          class="assessment-table"
          :items="subjects"
          :dense="disabled"
          disable-pagination
          hide-default-footer
          item-key="uid"
        >
          <template
            v-once
            v-slot:header
          >
            <thead
              v-if="value.isSelfEduForm === true"
            >
              <tr>
                <th
                  rowspan="2"
                >
                  {{ $t('lod.evaluations.subjectName') }} / {{ $t('lod.evaluations.modules') }}
                </th>
                <th
                  rowspan="2"
                >
                  {{ $t('lod.evaluations.regularSession') }}
                </th>
                <th
                  colspan="3"
                  class="ls-0"
                >
                  {{ $t('lod.evaluations.correctiveSessions') }}
                </th>
                <th
                  rowspan="2"
                >
                  {{ $t('lod.evaluations.annualHours') }}
                </th>
                <th
                  v-if="!disabled"
                  rowspan="2"
                />
              </tr>
              <tr>
                <th>
                  {{ $t('lod.evaluations.sessionFirstGrade') }}
                </th>
                <th>
                  {{ $t('lod.evaluations.sessionSecondGrade') }}
                </th>
                <th>
                  {{ $t('lod.evaluations.sessionThirdGrade') }}
                </th>
              </tr>
            </thead>
            <thead v-else>
              <tr>
                <th
                  rowspan="3"
                >
                  {{ $t('lod.evaluations.subjectName') }} / {{ $t('lod.evaluations.modules') }}
                </th>
                <th
                  rowspan="3"
                >
                  {{ $t('lod.evaluations.firstTermGrade') }}
                </th>
                <th
                  rowspan="3"
                >
                  {{ $t('lod.evaluations.secondTermGrade') }}
                </th>
                <th
                  rowspan="3"
                >
                  {{ $t('lod.evaluations.finalGrade') }}
                </th>
                <th
                  colspan="3"
                  class="ls-0"
                >
                  {{ $t('lod.evaluations.correctiveSessions') }}
                </th>
                <th
                  rowspan="3"
                >
                  {{ $t('lod.evaluations.annualHours') }}
                </th>
                <th
                  v-if="!disabled"
                  rowspan="3"
                />
              </tr>
              <tr>
                <th
                  colspan="2"
                >
                  {{ $t('lod.evaluations.regularSessions') }}
                </th>
                <th
                  rowspan="2"
                >
                  {{ $t('lod.evaluations.sessionThirdGrade') }}
                </th>
              </tr>
              <tr>
                <th>
                  {{ $t('lod.evaluations.sessionFirstGrade') }}
                </th>
                <th>
                  {{ $t('lod.evaluations.sessionSecondGrade') }}
                </th>
              </tr>
            </thead>
          </template>

          <template
            v-slot:body="{ items }"
          >
            <draggable
              :list="items"
              :disabled="true"
              :sort="true"
              tag="tbody"
              ghost-class="ghost"
              @change="updateListSortOrder"
            >
              <template
                v-for="item in items"
              >
                <!-- Това не знаем защо се е появило в долния row-viewer.
                  За да работи draggable трябва да се сетнато key пропъртито, но тога реве, че то трябва да е template при v-for-а
                  За да спре се слага v-if.
                -->
                <!-- Това не знаем защо се е появило в долния row-viewer. Предполагам за да изпълним горното -->
                <!-- v-if="!isEditMode || item.isLoadedFromStudentCurriculum !== true" -->
                <row-viewer
                  v-if="true"
                  :key="item.uid"
                  :value="item"
                  :disabled="disabled || item.isLodSubject != true"
                  :grade-options="gradeOptions"
                  :subject-type-options="subjectTypeOptions"
                  @subjectRemove="onSubjectRemove"
                />
                <row-viewer
                  v-for="m in item.modules"
                  :key="m.uid"
                  :value="m"
                  :disabled="disabled || item.isLodSubject != true"
                  :grade-options="gradeOptions"
                  :subject-type-options="subjectTypeOptions"
                  @subjectRemove="onSubjectRemove"
                />
              </template>
            </draggable>
          </template>

          <template
            v-if="isEditMode"
            v-slot:[`footer`]
          >
            <div class="v-data-footer">
              <v-btn
                small
                color="primary"
                @click.stop="onAddClick"
              >
                {{ $t("lod.assessments.addSubject") }}
              </v-btn>
            </div>
          </template>
        </v-data-table>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import Draggable from 'vuedraggable';
import RowViewer from './LodAssessmentRowViewer.vue';
import { LodAssessmentGradeModel } from '@/models/lodModels/lodAssessmentGradeModel';

export default {
  name: 'LodAssessmentTableComponent',
  components: {
    Draggable,
    RowViewer,
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    isEditMode: {
      type: Boolean,
      default() {
        return false;
      }
    },
    gradeOptions: {
      type: Array,
      default() {
        return null;
      }
    },
    subjectTypeOptions: {
      type: Array,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      search: '',
    };
  },
  computed: {
    disabled() {
      return !this.isEditMode;
    },
    subjects() {
      if(!this.value || !this.value.subjectAssessments) return [];

      if(!this.search) return this.value.subjectAssessments;

      return this.value.subjectAssessments.filter(x => (x.subjectName || '').toLowerCase().includes((this.search || '').toLowerCase()));
    }
  },
  methods: {
    updateListSortOrder () {
      if(!this.value || !this.value.subjectAssessments) return;
      const newList = [...this.value.subjectAssessments].map((item, index) => {
        const newSort = index + 1;
       // also add in a new property called has changed if you want to style them / send an api call
        item.hasChanged = item.sortOrder !== newSort;
        if (item.hasChanged) {
          item.sortOrder = newSort;
        }
        return item;
      });

      this.value.subjectAssessments = newList;
      this.$emit('input', this.value);
    },
    onAddClick() {
      if(!this.value?.subjectAssessments) return;

      this.value.subjectAssessments.push({
        uid: this.$uuid.v4(),
        subjectId: null,
        subjectTypeId: null,
        annualHorarium: null,
        assessments: [],
        firstTermAssessments: [new LodAssessmentGradeModel()],
        secondTermAssessments: [new LodAssessmentGradeModel()],
        annualTermAssessments: [new LodAssessmentGradeModel()],
        firstRemedialSession: [new LodAssessmentGradeModel()],
        secondRemedialSession: [new LodAssessmentGradeModel()],
        additionalRemedialSession: [new LodAssessmentGradeModel()],
        modules: [],
        isLodSubject: true,
        sortOrder: 1,
        isSelfEduForm: this.value.isSelfEduForm
      });
    },
    onSubjectRemove(item) {
      if(!this.value.subjectAssessments) return; // Няма от какво да махаме

      if(!item.isModule) {
        const index = this.value.subjectAssessments.findIndex(x => x.uid === item.uid);
        if(index < 0) {
          return;
        }
        this.value.subjectAssessments.splice(index, 1);
      } else {
        const subject = this.value.subjectAssessments.find(x => x.modules && x.modules.some(m => m.uid === item.uid));
        if(!subject) return;

        const index = subject.modules.findIndex(x => x.uid === item.uid);
        if(index < 0) {
          return;
        }

        subject.modules.splice(index, 1);
      }
    }
  }
};
</script>

<style lang="scss" scoped>
  ::v-deep {
    .assessment-table thead th {
      background-color: #f4f7fa;
      border: 1px solid #cddaea;
      padding: 2px !important;
      vertical-align: center;
      height: fit-content;
      height:fit-content !important;
    }
    .assessment-table .v-data-footer {
      border: 1px solid #cddaea !important;
      padding: 5px;
    }
  }
</style>
