<template>
  <div>
    <enrolled-students-submitted-data-import
      @submitFile="onSubmitFile"
    />
    <enrolled-students-submitted-data-list
      :ref="'grid' + _uid"
      class="mt-2"
    />
  </div>
</template>

<script>
import EnrolledStudentsSubmittedDataImport from '@/components/asp/EnrolledStudentsSubmittedDataImport.vue';
import EnrolledStudentsSubmittedDataList from '@/components/asp/EnrolledStudentsSubmittedDataList.vue';

export default {
  name: 'EnrolledStudentSubmittedDataHistoryView',
  components: {
    EnrolledStudentsSubmittedDataImport,
    EnrolledStudentsSubmittedDataList
  },
  methods: {
    onSubmitFile() {
       const grid = this.$refs['grid' + this._uid];
       if (grid) {
         grid.get();
       }
    }
  }
};
</script>
