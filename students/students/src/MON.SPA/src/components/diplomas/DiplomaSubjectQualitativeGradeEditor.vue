<template>
  <v-select
    v-model="value.qualitativeGrade"
    :label="$t('diplomas.grade')"
    :items="qualitativeGradeOptions"
    :outlined="outlined"
    :persistent-placeholder="persistentPlaceholder"
    :dense="dense"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'DiplomaSubjectQualitativeGradeEditorComponent',
  props: {
    value: {
      type: Object,
      required: true,
    },
    persistentPlaceholder: {
      type: Boolean,
      default() {
        return false;
      }
    },
    outlined: {
      type: Boolean,
      default() {
        return false;
      }
    },
    dense: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      qualitativeGradeOptions: []
    };
  },
  mounted() {
    this.loadOptions();
  },
  beforeDestroy() {
    this.value.qualitativeGrade = null;
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getQualitativeGradeOptions()
        .then((response) => {
          if (response) {
            this.qualitativeGradeOptions = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    }
  }
};
</script>
