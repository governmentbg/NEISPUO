<template>
  <other-document-list
    :person-id="id"
  />
</template>

<script>
import OtherDocumentList from '@/components/tabs/otherDocuments/OtherDocumentList.vue';

export default {
  name: 'StudentLodDocuments',
  components: {
    OtherDocumentList
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
};
</script>