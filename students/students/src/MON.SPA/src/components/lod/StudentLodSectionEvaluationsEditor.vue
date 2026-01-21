<template>
  <div>
    <v-simple-table>
      <template #default>
        <thead>
          <tr>
            <th rowspan="2">
              {{ $t('lod.evaluations.subjectName') }}/<br>{{ $t('lod.evaluations.modules') }}
            </th>
            <th rowspan="2">
              {{ $t('lod.evaluations.firstTermGrade') }}
            </th>
            <th rowspan="2">
              {{ $t('lod.evaluations.secondTermGrade') }}
            </th>
            <th rowspan="2">
              {{ $t('lod.evaluations.finalGrade') }}
            </th>
            <th
              v-if="[1,2,3].includes(curriculumPart)"
              colspan="3"
            >
              {{ $t('lod.evaluations.correctiveSessions') }}
            </th>
            <th rowspan="2">
              {{ $t('lod.evaluations.annualHours') }}
            </th>
          </tr>
          <tr>
            <th
              v-if="[1,2,3].includes(curriculumPart)"
            >
              {{ $t('lod.evaluations.sessionFirstGrade') }}
            </th>
            <th
              v-if="[1,2,3].includes(curriculumPart)"
            >
              {{ $t('lod.evaluations.sessionSecondGrade') }}
            </th>
            <th
              v-if="[1,2,3].includes(curriculumPart)"
            >
              {{ $t('lod.evaluations.sessionThirdGrade') }}
            </th>
          </tr>
        </thead>
        <draggable
          :list="value"
          :sort="true"
          tag="tbody"
          ghost-class="ghost"
          @change="updateListSortOrder"
        >
          {{ value }}
        </draggable>
      </template>
    </v-simple-table>
  </div>
</template>

<script>
import Draggable from 'vuedraggable';

export default {
  name: 'StudentLodSectionEvaluationsEditorComponent',
  components: { Draggable },
  props: {
    value: {
      type: Array,
      default() {
        return null;
      }
    },
    curriculumPart: {
      type: Number,
      reqired: true,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      model: null,
    };
  },
  computed: {
    headers() {
      switch (this.curriculumPart) {
        case 1:
        case 2:
          return [
            { value: 'index' },
            { value: 'subject' },
            { value: 'sectionAFirstTermGrade'},
            { value: 'sectionASecondTermGrade' },
            { value: 'sectionAFinalGrade' },
            { value: 'sectionASession1Grade' },
            { value: 'sectionASession2Grade' },
            { value: 'sectionASession3Grade' },
            { value: 'sectionBFirstTermGrade' },
            { value: 'sectionBSecondTermGrade' },
            { value: 'sectionBFinalGrade' },
            { value: 'sectionBSession1Grade' },
            { value: 'sectionBSession2Grade' },
            { value: 'sectionBSession3Grade' },
            { value: 'sectionAHours' },
            { value: 'sectionBHours' },
            { value: 'actions' }
          ];
        case 3:
          return [
            { value: 'index' },
            { value: 'subject' },
            { value: 'sectionVFirstTermGrade' },
            { value: 'sectionVSecondTermGrade' },
            { value: 'sectionVFinalGrade' },
            { value: 'sectionVSession1Grade' },
            { value: 'sectionVSession2Grade' },
            { value: 'sectionVSession3Grade' },
            { value: 'sectionVHours' },
            { value: 'actions' }
          ];
        case 4:
          return [];
        default:
          return [];
      }
    }
  },
  watch: {
    value(newValue) {
      this.model = newValue;
    }
  },
  methods: {
    updateListSortOrder () {
      // const newList = [...this.model.equalizationDetails].map((item, index) => {
      //   const newSort = index + 1;
      //  // also add in a new property called has changed if you want to style them / send an api call
      //   item.hasChanged = item.sortOrder !== newSort;
      //   if (item.hasChanged) {
      //     item.sortOrder = newSort;
      //   }
      //   return item;
      // });

      // this.model.equalizationDetails = newList;
      // this.$emit('input', this.model.equalizationDetails);
    },
  }
};
</script>

