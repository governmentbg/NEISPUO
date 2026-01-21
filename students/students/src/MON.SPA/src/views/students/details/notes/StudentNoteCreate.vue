<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('notes.createTitle') }}</h3>
      </template>

      <template #default>
        <note-form
          v-if="note !== null"
          :ref="'noteForm' + _uid"
          :note="note"
          :disabled="saving"
          :min-school-year="isSchoolDirector ? currentSchoolYear : null"
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
import NoteForm from "@/components/tabs/notes/NoteForm.vue";
import { StudentNoteModel } from "@/models/studentNoteModel.js";
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";

export default {
  name: 'StudentNoteCreate',
  components: {
    NoteForm
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      saving: false,
      note: null,
    };
  },
  computed: {
    ...mapGetters(['userSelectedRole', 'isInRole', 'hasStudentPermission']),
    isSchoolDirector() {
      return this.isInRole(this.roleSchool);
    }
  },
  async mounted() {
  if(!this.hasStudentPermission(Permissions.PermissionNameForStudentNoteManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    await this.init();
  },
  methods: {
      async init() {
      this.currentSchoolYear = (await this.$api.institution.getCurrentYear(this.userSelectedRole.InstitutionID)).data;

      this.note = new StudentNoteModel({
        personId: this.pid,
        schoolYear: this.currentSchoolYear,
        issueDate: new Date(),
      }, this.$moment);
    },
    onSave() {
      const form = this.$refs['noteForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.note.issueDate = this.$helper.parseDateToIso(this.note.issueDate, '');

      this.saving = true;
      this.$api.note
        .create(this.note)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.noteSaveError'), 5000);
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
