<template>
  <v-select
    v-model="value.otherGrade"
    :label="$t('diplomas.grade')"
    :items="otherGradeOptions"
    :outlined="outlined"
    :persistent-placeholder="persistentPlaceholder"
    :dense="dense"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'DiplomaSubjectOtherGradeEditorComponent',
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
      otherGradeOptions: []
    };
  },
  mounted() {
    this.loadOptions();
  },
  beforeDestroy() {
    this.value.otherGrade = null;
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getOtherGradeOptions()
        .then((response) => {
          if (response) {
            this.otherGradeOptions = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    }
  }
};
</script>
