<template>
  <button-tip
    :icon-name="editIconName"
    :tooltip="editButtonTooltip"
    iclass="mx-2"
    fab
    small
    bottom
    :disabled="disabled"
    v-bind="$attrs"
    @click="editIconClick"
  />
</template>

<script>
export default {
  name: "EditButton",
  props: {
    value: {
      type: Boolean,
      default() {
        return false;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      isEditMode: this.value
    };
  },
  computed: {
    editIconName() {
      return this.isEditMode ? 'mdi-lock' : 'mdi-pencil';
    },
    editButtonTooltip() {
      return this.isEditMode ? "buttons.disableEdit" : "buttons.edit";
    }
  },
  watch: {
    value: function (val) {
      this.isEditMode = val;
    },
  },
  methods: {
    editIconClick() {
      this.isEditMode = !this.isEditMode;
      this.$emit('input', this.isEditMode);
      this.$emit('click', this.isEditMode);
    }
  }
};
</script>
