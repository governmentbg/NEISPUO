<template>
  <div>
    <student-lod-section-evaluations-editor
      :value="value.filter(x => x.curriculumPartID === 1)"
      :curriculum-part="1"
    />
    <student-lod-section-evaluations-editor
      :value="value.filter(x => x.curriculumPartID === 2)"
      :curriculum-part="2"
    />
    <student-lod-section-evaluations-editor
      :value="value.filter(x => x.curriculumPartID === 3)"
      :curriculum-part="3"
    />
    <student-lod-section-evaluations-editor
      :value="value.filter(x => x.curriculumPartID === 4)"
      :curriculum-part="4"
    />
    <vue-json-pretty
      v-if="value"
      path="res"
      :data="value"
      show-length
    />
  </div>
</template>

<script>

import StudentLodSectionEvaluationsEditor from '@/components/lod/StudentLodSectionEvaluationsEditor.vue';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';

export default {
  name: 'StudentLodEvaluationsEditorComponent',
  components: {
    StudentLodSectionEvaluationsEditor,
    VueJsonPretty
  },
  props: {
    value: {
      type: Array,
      reqired: true,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
    };
  }
};
</script>
