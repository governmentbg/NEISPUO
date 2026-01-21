<template>
  <v-expansion-panels
    v-model="panel"
    multiple
    flat
    class="ma-0"
  >
    <v-expansion-panel>
      <v-expansion-panel-header>
        <v-row dense>
          <v-col
            cols="4"
            class="py-1"
          >
            {{ $t('common.institution') }}
          </v-col>
          <v-col
            cols="8"
            class="py-1"
          >
            {{ value.classGroup.institutionId + ' ' + value.classGroup.institutionName }}
          </v-col>
          <v-col
            cols="4"
            class="py-1"
          >
            {{ $t('common.position') }}
          </v-col>
          <v-col
            cols="8"
            class="py-1"
          >
            {{ value.position }}
          </v-col>
          <v-col
            cols="4"
            class="py-1"
          >
            {{ $t('institution.details.headers.eduForm') }}
          </v-col>
          <v-col
            cols="8"
            class="py-1"
          >
            {{ value.studentEduFormName }}
          </v-col>
          <v-col
            cols="4"
            class="py-1"
          >
            {{ $t('institution.details.headers.classType') }}
          </v-col>
          <v-col
            cols="8"
            class="py-1"
          >
            {{ value.classGroup.classTypeName }}
          </v-col>
          <v-col
            v-if="value.positionId!== positions.ProfessionalEducation"
            cols="4"
            class="py-1"
          >
            {{ $t('student.profession') }}
          </v-col>
          <v-col
            v-if="value.positionId!== positions.ProfessionalEducation"
            cols="8"
            class="py-1"
          >
            {{ value.studentProfession }}
          </v-col>
          <v-col
            v-if="value.positionId!== positions.ProfessionalEducation"
            cols="4"
            class="py-1"
          >
            {{ $t('student.speciality') }}
          </v-col>
          <v-col
            v-if="value.positionId!== positions.ProfessionalEducation"
            cols="8"
            class="py-1"
          >
            {{ value.studentSpeciality }}
          </v-col>
        </v-row>
      </v-expansion-panel-header>
      <v-expansion-panel-content>
        <v-simple-table
          dense
        >
          <template v-slot:default>
            <tbody>
              <tr>
                <td>{{ $t('documents.basicClassName') }}</td>
                <td>{{ value.basicClassName }}</td>
              </tr>
              <tr>
                <td>{{ $t('student.class') }}</td>
                <td>{{ value.classGroup.className }}</td>
              </tr>
              <tr>
                <td>{{ $t('documents.currentStudentClass') }}</td>
                <td>
                  <v-chip
                    :color="value.isCurrent ? 'success' : 'light'"
                    small
                  >
                    <yes-no :value="value.isCurrent" />
                  </v-chip>
                </td>
              </tr>
              <tr>
                <td>{{ $t('enroll.enrollmentDate') }}</td>
                <td>{{ value.enrollmentDate && $helper.isValidIsoFormattedDate(value.enrollmentDate) ? $helper.formatFromIsoDate(value.enrollmentDate) : value.enrollmentDate }}</td>
              </tr>
              <tr v-if="value.entryDate">
                <td>{{ $t('documents.entryDateLabel') }}</td>
                <td>{{ value.entryDate && $helper.isValidIsoFormattedDate(value.entryDate) ? $helper.formatFromIsoDate(value.entryDate) : value.entryDate }}</td>
              </tr>
              <tr v-if="value.dischargeDate">
                <td>{{ $t('documents.dischargeDateLabel') }}</td>
                <td v-if="value.isCurrent" />
                <td v-else>
                  {{ value.dischargeDate && $helper.isValidIsoFormattedDate(value.dischargeDate) ? $helper.formatFromIsoDate(value.dischargeDate) : value.dischargeDate }}
                </td>
              </tr>
              <tr>
                <td>{{ $t('common.schoolYear') }}</td>
                <td>{{ value.schoolYearName }}</td>
              </tr>
              <tr>
                <td>{{ $t('enroll.nraNotSubmitted') }}</td>
                <td>
                  <yes-no
                    :value="value.isNotForSubmissionToNra"
                  />
                </td>
              </tr>
              <tr v-if="value.classGroup.classKindId === 1">
                <td>{{ $t('studentClass.isNotPresentForm') }}</td>
                <td>
                  <yes-no
                    :value="value.isNotPresentForm"
                  />
                </td>
              </tr>
              <tr v-if="value.classGroup.classKindId === 1">
                <td>{{ $t('enroll.individualCurriculum') }}</td>
                <td>
                  <yes-no
                    :value="value.hasIndividualStudyPlan"
                  />
                </td>
              </tr>
              <tr v-if="value.classGroup.classKindId === 1">
                <td>{{ $t('enroll.hourlyOrganization') }}</td>
                <td>
                  <yes-no
                    :value="value.isHourlyOrganization"
                  />
                </td>
              </tr>
            </tbody>
          </template>
        </v-simple-table>
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script>
import { Positions } from '@/enums/enums';

export default {
  name: 'StudentClassDetails',
  props: {
    value: {
      type: Object,
      required: true
    },
    expanded: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      panel: this.expanded ? [0] : [],
      positions: Positions
    };
  }
};
</script>
