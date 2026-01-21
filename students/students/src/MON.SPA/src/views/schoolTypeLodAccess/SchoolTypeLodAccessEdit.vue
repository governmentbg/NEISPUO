<template>
  <school-type-lod-access-form
    :detailed-school-type-id="id"
  />
</template>

<script>
import SchoolTypeLodAccessForm from '@/components/schoolTypeLodAccess/SchoolTypeLodAccessEdit';

export default {
  name: 'SchoolTypeLodAccessEdit',
  components: {
    SchoolTypeLodAccessForm
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  }
};
</script>