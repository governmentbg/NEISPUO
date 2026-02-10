<template>
  <institution-basic-details
    v-if="institutionId"
    :id="institutionId"
  />
</template>

<script>
import InstitutionBasicDetails from '@/components/institution/InstitutionBasicDetails.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentCurrentInstitutionDetails',
  components: {
    InstitutionBasicDetails
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      institutionId: undefined
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentCurrentInstitutionDetailsRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.$api.institution
        .getCurrentForStudent(this.id)
        .then((response) => {
          this.institutionId = response.data;
        })
        .catch((error) => {
          console.log(error);
        });
    }
  }
};
</script>