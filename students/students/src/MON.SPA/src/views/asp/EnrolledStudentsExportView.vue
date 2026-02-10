<template>
  <div>
    <enrolled-students-export
      v-if="hasAspEnrolledStudentsExportPermission"
      @submitFile="onSubmitFile"
    />
    <enrolled-students-export-list
      ref="enrolledStudentsExportList"
      class="mt-10"
    />
  </div>
</template>

<script>
import EnrolledStudentsExport from "@/components/asp/EnrolledStudentsExport.vue";
import EnrolledStudentsExportList from "@/components/asp/EnrolledStudentsExportList.vue";
import { mapGetters } from "vuex";
import { Permissions } from '@/enums/enums';

export default {
  name: "EnrolledStudentsExportView",
  components: {
    EnrolledStudentsExport,
    EnrolledStudentsExportList
  },
  computed:{
    ...mapGetters(['hasPermission']),
    hasAspEnrolledStudentsExportPermission() {
      return this.hasPermission(Permissions.PermissionNameForASPEnrolledStudentsExport);
    }
  },
  methods:{
    async onSubmitFile(){
      this.$refs.enrolledStudentsExportList.loadData();
    }
  }  
};
</script>