<template>
  <div>
    <v-tabs
      v-model="tab"
      fixed-tabs
      centered
      background-color="primary"
      dark
    >
      <v-tab
        v-if="showBasicData"
        key="basicData"
      >
        {{ this.$t('studentTabs.basicData') }}
      </v-tab>
      <v-tab
        v-if="showEducation"
        key="education"
      >
        {{ this.$t('studentTabs.education') }}
      </v-tab>
      <v-tab
        v-if="showAdditionalData"
        key="additionalData"
      >
        {{ this.$t('studentTabs.additionalData') }}
      </v-tab>

      <v-tabs-items
        v-model="tab"
      >
        <v-tab-item
          v-if="showBasicData"
          key="basicData"
        >
          <person
            :id="id"
            edit
            pin-required
            public-edu-required
            first-name-required
            last-name-required
            birth-date-required
            gender-required
            nationality-required
            birth-place-country-required
            birth-place-required
            permanent-residence-required
            permanent-address-required
            current-address-required
            current-residence-required
          />
        </v-tab-item>
        <v-tab-item
          v-if="showEducation"
          key="education"
        >
          <student-education
            :person-id="id"
          />
        </v-tab-item>
        <v-tab-item
          v-if="showAdditionalData"
          key="additionalData"
        >
          <student-international-protection
            :person-id="id"
          />
        </v-tab-item>

        <v-progress-linear
          v-if="loading"
          indeterminate
        />
      </v-tabs-items>
    </v-tabs>
  </div>
</template>

<script>
import Person from '@/components/person/Person.vue';
import StudentEducation from '@/components/students/StudentEducation.vue';
import StudentInternationalProtection from "@/components/tabs/studentDetails/StudentInternationalProtection.vue";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentBasicDetails',
  components: {
    Person,
    StudentEducation,
    StudentInternationalProtection
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      tab: null,
      loading: false
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    showBasicData() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDataRead)
        || this.hasStudentPermission(Permissions.PermissionNameForStudentPartialPersonalDataRead)
        || this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDataManage);
    },
    showEducation() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentEducationRead);
    },
    showAdditionalData() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalProtectionRead)
        || this.hasStudentPermission(Permissions.PermissionNameForStudentInternationalProtectionManage);
    },
  },
  mounted() {
    if(!this.showBasicData && !this.showEducation && this.showAdditionalData) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
