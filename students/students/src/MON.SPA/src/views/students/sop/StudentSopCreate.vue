<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('sop.createTitle') }}</h3>
      </template>

      <template #default>
        <sop-form
          v-if="model !== null"
          :ref="'sopForm' + _uid"
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
import SopForm from '@/components/students/SopForm.vue';
import { StudentSopModel } from '@/models/studentSopModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentSopCreate',
  components: {
    SopForm
  },
  props: {
    personId: {
      type: Number,
      required: true
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSopManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.init();
  },
  methods: {
    init() {
      this.model = new StudentSopModel({ personId: this.personId, sopDetails: null });

      this.$api.institution
        .getCurrentYear(this.userInstitutionId)
        .then((response) => {
          if(response.data) {
            this.model.schoolYear = response.data;
          }
        }
      );
    },
    onSave() {
      const form = this.$refs['sopForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }


      this.saving = true;
      this.$api.studentSOP
        .create(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error?.response?.data?.message ?? this.$t('common.error'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
