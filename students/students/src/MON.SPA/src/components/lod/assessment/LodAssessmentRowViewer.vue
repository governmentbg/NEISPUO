<template>
  <tr>
    <td :class="disabled ? '' : 'px-1'">
      <v-row
        v-if="disabled"
        dense
        align="center"
        justify="center"
      >
        <v-col
          v-if="model.isModule"
          cols="1"
          dense
        >
          <v-icon
            color="primary"
            left
            class="ml-5"
            x-small
          >
            far fa-circle fa-solid
          </v-icon>
        </v-col>
        <v-col dense>
          <v-tooltip
            v-if="model.isLoadedFromStudentCurriculum && (mode !== 'prod' || isInRole(userRole.Consortium))"
            bottom
          >
            <template #activator="{ on }">
              <v-icon
                left
                color="info"
                class="mr-0"
                v-on="on"
              >
                mdi-information-slab-box-outline
              </v-icon>
            </template>
            <span>{{ $t('lod.assessments.subjectLoadedFromCurriculum') }}</span>
          </v-tooltip>

          <span>{{ model.subjectName }}</span>
          <v-tooltip bottom>
            <template #activator="{ on }">
              <v-chip
                color="light"
                x-small
                outlined
                class="mx-1"
                v-on="on"
              >
                {{ model.subjectId }}
              </v-chip>
            </template>
            <span>{{ `CurriculumId: ${ Array.isArray(model.curriculumIds) ? model.curriculumIds.join() : model.curriculumId} / CurriculumStudentId: ${model.curriculumStudentId}` }}</span>
          </v-tooltip>
          <span>
            {{ ` / ${model.subjectTypeName}` }}
          </span>
          <v-tooltip
            v-if="model.flSubjectName"
            bottom
          >
            <template #activator="{ on }">
              <v-chip
                v-if="model.flSubjectName"
                color="light"
                x-small
                v-on="on"
              >
                {{ model.flSubjectName }} {{ model.flHorarium ? ` / ${model.flHorarium}` : '' }}
              </v-chip>
            </template>
            <span>{{ $t('diplomas.showFlSubject') }}</span>
          </v-tooltip>
          <span
            v-if="model.categories"
          >
            <v-tooltip
              bottom
            >
              <template #activator="{ on }">
                <v-chip
                  v-for="(category, index) in model.categories"
                  :key="index"
                  color="light"
                  x-small
                  v-on="on"
                >
                  {{ category }}
                </v-chip>
              </template>
              <span>{{ $t('lod.assessments.source') }}</span>
            </v-tooltip>
          </span>
        </v-col>
      </v-row>
      <div v-else>
        <v-row
          dense
          align="center"
          justify="center"
        >
          <v-col
            v-if="model.isLoadedFromStudentCurriculum"
            style="max-width: 30px;"
          >
            <v-tooltip
              bottom
            >
              <template #activator="{ on }">
                <v-icon
                  left
                  color="info"
                  class="mr-0"
                  v-on="on"
                >
                  mdi-information-slab-box-outline
                </v-icon>
              </template>
              <span>{{ $t('lod.assessments.subjectLoadedFromCurriculum') }}</span>
            </v-tooltip>
          </v-col>
          <v-col
            v-if="model.isModule"
            cols="1"
            dense
          >
            <v-icon
              color="primary"
              left
              class="ml-5"
              x-small
            >
              far fa-circle fa-solid
            </v-icon>
          </v-col>
          <v-col
            v-if="isProfSubject"
            :style="{ 'max-width': '30px' }"
          >
            <button-tip
              icon
              icon-name="mdi-plus-thick"
              icon-color="primary"
              tooltip="buttons.addSubjectModule"
              bottom
              iclass=""
              small
              @click="onAddModule"
            />
          </v-col>
          <v-col>
            <c-autocomplete
              :ref="'subject' + _uid"
              v-model="model.subjectId"
              api="/api/lookups/GetSubjectOptions"
              :placeholder="$t('buttons.search')"
              defer-options-loading
              hide-no-data
              hide-selected
              remove-items-on-clear
              :rules="[$validator.required()]"
              class="required"
              persistent-placeholder
              dense
              hide-details
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
                <span style="font-size: 0.8em;">{{ data.item.text }}</span>
                <v-chip
                  color="light"
                  x-small
                  outlined
                >
                  {{ data.item.value }}
                </v-chip>
              </template>
            </c-autocomplete>
          </v-col>
          <v-col
            cols="12"
            md="6"
            lg="3"
            dense
            align="center"
            justify="center"
          >
            <v-autocomplete
              v-model="model.subjectTypeId"
              :items="model.isModule === true ? subjectTypeOptions.filter(x => moduleSubjectTypeOptions.some(m => m.value === x.value)) : subjectTypeOptions"
              :rules="[$validator.required()]"
              class="required"
              persistent-placeholder
              dense
              hide-details
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
                  <v-chip
                    v-if="!data.item.isValid"
                    color="error"
                    small
                    outlined
                    label
                    class="ml-1"
                  >
                    {{ $t('common.unactive') }}
                  </v-chip>
                </v-list-item-icon>
              </template>
            </v-autocomplete>
          </v-col>
          <v-col
            v-if="model.subjectId"
            :style="{ 'max-width': '70px' }"
          >
            <v-checkbox
              v-model="model.showFlSubject"
              color="primary"
              class="custom-small-text"
              dense
              :hint="$t('diplomas.showFlSubject')"
              persistent-hint
            />
          </v-col>
        </v-row>
        <v-row
          v-if="model.subjectId && model.showFlSubject"
          dense
          align="center"
          justify="center"
          class="mb-1 mt-3"
        >
          <v-col>
            <c-autocomplete
              v-model="model.flSubjectId"
              api="/api/lookups/GetFlOptions"
              :label="$t('diplomas.flsubject')"
              :placeholder="$t('buttons.search')"
              :rules="[$validator.required()]"
              class="required"
              :defer-options-loading="false"
              dense
              hide-details
              append-icon=""
              persistent-placeholder
            />
          </v-col>
          <v-col>
            <div>
              <v-text-field
                v-model.number="model.flHorarium"
                type="number"
                :label="$t('diplomas.template.horarium')"
                :min="0"
                dense
                clearable
                :rules="[$validator.min(0)]"
                style="font-size: 0.9em;"
                hide-details
                persistent-placeholder
              />
            </div>
          </v-col>
        </v-row>
      </div>
    </td>
    <td
      v-if="model.isSelfEduForm !== true"
      :class="disabled ? '' : 'px-1'"
      :style="gradesStyleObject(model.firstTermAssessments.length)"
    >
      <div>
        <grade-viewer
          v-for="(grade, gradeIndex) in model.firstTermAssessments"
          :key="gradeIndex"
          :value="grade"
          :disabled="disabled || isProfSubject"
          :grade-options="gradeOptions"
        />
      </div>
    </td>
    <td
      v-if="model.isSelfEduForm !== true"
      :class="disabled ? '' : 'px-1'"
      :style="gradesStyleObject(model.secondTermAssessments.length)"
    >
      <div>
        <grade-viewer
          v-for="(grade, gradeIndex) in model.secondTermAssessments"
          :key="gradeIndex"
          :value="grade"
          :disabled="disabled || isProfSubject"
          :grade-options="gradeOptions"
        />
      </div>
    </td>
    <td
      :class="disabled ? '' : 'px-1'"
      :style="gradesStyleObject(model.annualTermAssessments.length)"
    >
      <div>
        <grade-viewer
          v-for="(grade, gradeIndex) in model.annualTermAssessments"
          :key="gradeIndex"
          :value="grade"
          :disabled="disabled"
          :grade-options="gradeOptions"
          :is-prof-subject="isProfSubject"
        />
      </div>
    </td>
    <td
      :class="disabled ? '' : 'px-1'"
      :style="gradesStyleObject(model.firstRemedialSession.length)"
    >
      <div>
        <grade-viewer
          v-for="(grade, gradeIndex) in model.firstRemedialSession"
          :key="gradeIndex"
          :value="grade"
          :disabled="disabled"
          :grade-options="gradeOptions"
          :is-prof-subject="isProfSubject"
        />
      </div>
    </td>
    <td
      :class="disabled ? '' : 'px-1'"
      :style="gradesStyleObject(model.secondRemedialSession.length)"
    >
      <div>
        <grade-viewer
          v-for="(grade, gradeIndex) in model.secondRemedialSession"
          :key="gradeIndex"
          :value="grade"
          :disabled="disabled"
          :grade-options="gradeOptions"
          :is-prof-subject="isProfSubject"
        />
      </div>
    </td>
    <td
      :class="disabled ? '' : 'px-1'"
      :style="gradesStyleObject(model.additionalRemedialSession.length)"
    >
      <div>
        <grade-viewer
          v-for="(grade, gradeIndex) in model.additionalRemedialSession"
          :key="gradeIndex"
          :value="grade"
          :disabled="disabled"
          :grade-options="gradeOptions"
          :is-prof-subject="isProfSubject"
        />
      </div>
    </td>
    <td
      :class="disabled ? '' : 'px-1'"
      style="width: 80px;"
    >
      <span v-if="disabled">
        {{ model.annualHorarium || model.flHorarium }}
      </span>
      <div v-else>
        <v-text-field
          v-model.number="model.annualHorarium"
          type="number"
          :min="0"
          dense
          clearable
          :rules="[$validator.min(0)]"
          style="font-size: 0.9em;"
          hide-details
        />
      </div>
    </td>
    <td
      v-if="!disabled"
      class="px-0 text-center"
      style="max-width: 30px;"
    >
      <button-tip
        icon
        icon-name="mdi-close"
        icon-color="error"
        tooltip="buttons.remove"
        bottom
        iclass=""
        small
        @click="$emit('subjectRemove', model)"
      />
    </td>
  </tr>
</template>

<script>
import GradeViewer from './LodAssessmentViewer.vue';
import CAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import constants from '@/common/constants.js';
import { LodAssessmentGradeModel } from '@/models/lodModels/lodAssessmentGradeModel';
import { UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'LodAssessmentRowViewer',
  components: {
    GradeViewer,
    CAutocomplete
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
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
      model: this.value,
      subjectTypeToModulesLimit: constants.subjectTypeToModulesLimit,
      moduleSubjectTypeOptions: constants.moduleSubjectTypeOptions,
      userRole: UserRole
    };
  },
  computed: {
    ...mapGetters(['isInRole', 'mode']),
    isProfSubject() {
      return this.model && !this.model.isModule && this.model.subjectId && this.subjectTypeToModulesLimit.includes(this.model.subjectTypeId);
    },
    modulesAnnualHorariums() {
      if(!this.model || !this.model.modules) return null;

      const mapResult = this.model.modules.map(m => m.annualHorarium);
      return  mapResult;
    },
    modulesAnnualTermAssessments() {
      if(!this.model || !this.model.modules) return null;

      const mapResult = this.model.modules.map(m => m.annualTermAssessments);
      return mapResult;
    },
  },
  watch: {
    'model.firstTermAssessments': {
      deep:true,
      handler: function() {
        if(!this.disabled) {
          this.calcAnnualAssessment();
        }
      }
    },
    'model.secondTermAssessments': {
      deep:true,
      handler: function() {
        if(!this.disabled) {
          this.calcAnnualAssessment();
        }
      }
    }
  },
  mounted() {
    if(!this.disabled) {
      this.$watch('modulesAnnualHorariums', function () {
        if(!this.model.modules) {
          this.model.annualHorarium = null;
          this.model.firstTermAssessments = [new LodAssessmentGradeModel()];
          this.model.secondTermAssessments = [new LodAssessmentGradeModel()];
          this.model.annualTermAssessments = [new LodAssessmentGradeModel()];
          this.model.firstRemedialSession = [new LodAssessmentGradeModel()];
          this.model.secondRemedialSession = [new LodAssessmentGradeModel()];
          this.model.additionalRemedialSession = [new LodAssessmentGradeModel()];
          return;
        }

        const annualHorariumSum = this.model.modules.reduce((annualHorariumSum, value) => {
          return annualHorariumSum + value?.annualHorarium ?? 0;
        }, 0);

        this.model.annualHorarium = annualHorariumSum <= 0 ? null : annualHorariumSum;

        // const annualTermAssessmentSum = this.model.modules.reduce((annualTermAssessmentSum, value) => {
        //   return annualHorariumSum + value?.annualHorarium ?? 0;
        // }, 0);

      }, {deep:true});
      this.$watch('modulesAnnualTermAssessments', function () {
        if(!this.model.modules) {
          this.model.annualHorarium = null;
          this.model.firstTermAssessments = [new LodAssessmentGradeModel()];
          this.model.secondTermAssessments = [new LodAssessmentGradeModel()];
          this.model.annualTermAssessments = [new LodAssessmentGradeModel()];
          this.model.firstRemedialSession = [new LodAssessmentGradeModel()];
          this.model.secondRemedialSession = [new LodAssessmentGradeModel()];
          this.model.additionalRemedialSession = [new LodAssessmentGradeModel()];
          return;
        }

        let annualTermAssessments = [];

        this.model.modules.forEach(m => {
          const grades = m.annualTermAssessments.filter(x => !isNaN(x.gradeId) && x.gradeId >= 2 && x.gradeId <= 6).map(x => {
            return x.gradeId;
          });

          annualTermAssessments = [...annualTermAssessments, ...grades];
        });

        const annualTermAssessmentSum = annualTermAssessments.reduce((annualTermAssessmentSum, value) => {
          return annualTermAssessmentSum + value;
        }, 0);

        const avgGrade = Math.round(((annualTermAssessmentSum / annualTermAssessments.length) + Number.EPSILON) * 100) / 100;
        if(isNaN(avgGrade)) {
          return;
        }
        if (Array.isArray(this.model.annualTermAssessments) && this.model.annualTermAssessments.length > 0) {
          this.model.annualTermAssessments[0].decimalGrade = avgGrade;
          this.model.annualTermAssessments[0].gradeId = Math.round(avgGrade);
        } else {
          this.model.annualTermAssessments.push(new LodAssessmentGradeModel({
            decimalGrade: avgGrade,
            gradeId: Math.round(avgGrade)
          }));
        }

      }, {deep:true});
    }
  },
  methods: {
    onAddModule() {
      this.model.modules.push({
        uid: this.$uuid.v4(),
        subjectId: null,
        subjectTypeId: null,
        annualHorarium: null,
        isModule: true,
        assessments: [],
        firstTermAssessments: [new LodAssessmentGradeModel()],
        secondTermAssessments: [new LodAssessmentGradeModel()],
        annualTermAssessments: [new LodAssessmentGradeModel()],
        firstRemedialSession: [new LodAssessmentGradeModel()],
        secondRemedialSession: [new LodAssessmentGradeModel()],
        additionalRemedialSession: [new LodAssessmentGradeModel()],
        isLodSubject: true,
        sortOrder: 1,
        isSelfEduForm: this.value.isSelfEduForm
      });
    },
    gradesStyleObject(gradesNum) {
      return gradesNum < 2
        ? { maxWidth: '150px'}
        : {};
    },
    calcAnnualAssessment() {
      if(this.model.isProfSubject === true
        || !this.model.firstTermAssessments.some(x => !isNaN(x.gradeId) && x.gradeId >= 2 && x.gradeId <= 6)
        || !this.model.secondTermAssessments.some(x => !isNaN(x.gradeId) && x.gradeId >= 2 && x.gradeId <= 6)
      ) {
        if (Array.isArray(this.model.annualTermAssessments) && this.model.annualTermAssessments.length > 0
          && this.model.annualTermAssessments[0].gradeId >= 2 && this.model.annualTermAssessments[0].gradeId <= 6) {
          this.model.annualTermAssessments[0].gradeId = null;
        }
        return;
      }

      const annualAssessments =  [...this.model.firstTermAssessments, ...this.model.secondTermAssessments]
        .filter(x => !isNaN(x.gradeId) && x.gradeId >= 2 && x.gradeId <= 6)
        .map(x => { return x.gradeId; });

      const annualAssessmentsSum = annualAssessments
        .reduce((annualGradesSum, value) => {
          return annualGradesSum + value;
        }, 0);

      const avgGrade = Math.round(((annualAssessmentsSum / annualAssessments.length) + Number.EPSILON) * 100) / 100;
      if(isNaN(avgGrade)) {
        return;
      }

      if (Array.isArray(this.model.annualTermAssessments) && this.model.annualTermAssessments.length > 0) {
        this.model.annualTermAssessments[0].decimalGrade = null;
        this.model.annualTermAssessments[0].gradeId = Math.round(avgGrade);
      } else {
        this.model.annualTermAssessments.push(new new LodAssessmentGradeModel({
          decimalGrade: null,
          gradeId: Math.round(avgGrade)
        }));
      }
    }
  }
};
</script>

<style lang="scss" scoped>
  .assessment-table tbody td {
    border: 1px solid #cddaea;
    font-size: small !important;
  }

  ::v-deep {
    .v-autocomplete input {
      font-size: 0.8em;
    }
  }
</style>
