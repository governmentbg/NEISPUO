<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('personalDevelopment.createTitle') }}</h3>
      </template>

      <template #default>
        <personal-development-support-form
          v-if="model !== null"
          :ref="'personalDevelopmentSupportForm' + _uid"
          v-model="model"
          :disabled="saving"
        />
      </template>
    </form-layout>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import PersonalDevelopmentSupportForm from '@/components/students/PersonalDevelopmentSupportForm.vue';
import { PersonalDevelopmentSupportModel } from '@/models/personalDevelopmentSupportModel.js';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentPersonalDevelopmentCreate',
  components: {
    PersonalDevelopmentSupportForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: false,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      saving: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'userInstitutionId']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.init();
  },
  methods: {
    init() {
      this.model = new PersonalDevelopmentSupportModel({ personId: this.personId });

      if(this.schoolYear) {
        this.model.schoolYear = this.schoolYear;
      } else {
        this.$api.institution
          .getCurrentYear(this.userInstitutionId)
          .then((response) => {
            if(response.data) {
              this.model.schoolYear = response.data;
            }
          }
        );
      }
    },
    onSave() {
      const form = this.$refs['personalDevelopmentSupportForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }


      this.saving = true;
      this.$api.studentPDS
        .create(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          const { message } = this.$helper.parseError(error.response);
          this.$notifier.error(this.$t('common.save'), message);
        })
        .finally(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
