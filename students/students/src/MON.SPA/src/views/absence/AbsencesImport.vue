<template>
  <div>
    <active-absence-campaigns
      class="mb-2"
      @noAbsencesSubmited="onSubmitFile"
    />
    <div
      v-if="userInstitutionId"
    >
      <absences-import
        :file="file"
        :institution-id="userInstitutionId"
        @submitFile="onSubmitFile"
      />
    </div>
    <v-alert
      v-else
      outlined
      type="warning"
      prominent
      dense
    >
      <h4>{{ $t('absence.invalidInstitutionCode') }}</h4>
    </v-alert>

    <absence-imports-list
      v-if="userInstitutionId"
      ref="absenceList"
      class="mt-10"
      :institution-id="userInstitutionId"
    />
  </div>
</template>

<script>
import AbsencesImport from '@/components/absence/AbsencesImport.vue';
import AbsenceImportsList from '@/components/absence/AbsenceImportsList.vue';
import ActiveAbsenceCampaigns from '@/components/absence/ActiveAbsenceCampaigns.vue';
import { mapGetters } from 'vuex';

export default {
  name: "AbsencesImportView",
  components: { AbsencesImport, AbsenceImportsList, ActiveAbsenceCampaigns },
  data() {
    return {
      file: '',
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId'])
  },
  methods:{
    onSubmitFile(){
      this.$refs.absenceList.gridReload();
    }
  }
};
</script>
