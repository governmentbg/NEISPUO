<template>
  <diploma-template-edit
    :id="id"
  />
</template>

<script>
import DiplomaTemplateEdit from '@/components/diplomas/DiplomaTemplateEdit.vue';

export default {
  name: 'DiplomaTemplateEditView',
  components: {
    DiplomaTemplateEdit
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  }
};
</script>
