<template>
  <v-select
    v-model="value.specialNeedsGrade"
    :label="$t('diplomas.grade')"
    :items="specialNeedsGradeOptions"
    :outlined="outlined"
    :persistent-placeholder="persistentPlaceholder"
    :dense="dense"
    v-bind="$attrs"
    v-on="$listeners"
  />
</template>

<script>
export default {
  name: 'DiplomaSubjectSpecialNeedsGradeEditorComponent',
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
    },
  },
  data() {
    return {
      specialNeedsGradeOptions: []
    };
  },
  mounted() {
    this.loadOptions();
  },
  beforeDestroy() {
    this.value.specialNeedsGrade = null;
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getSpecialNeedGradeOptions()
        .then((response) => {
          if (response) {
            this.specialNeedsGradeOptions = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    }
  }
};
</script>
