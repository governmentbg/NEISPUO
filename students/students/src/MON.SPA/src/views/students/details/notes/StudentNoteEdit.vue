<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <!-- {{ note }} -->

      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('otherDocuments.editTitle') }}</h3>
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
    </div>
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

import { UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";

export default {
  name: 'StudentOtherDocumentEdit',
  components: {
    NoteForm,
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
    noteId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: true,
      saving: false,
      note: null,
      roleSchool: UserRole.School,
      currentSchoolYear: null
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
    this.load();
  },
  methods: {
    async init() {
      this.currentSchoolYear = (await this.$api.institution.getCurrentYear(this.userSelectedRole.InstitutionID)).data;
    },
    load() {
      this.loading = true;
      this.$api.note.getById(this.noteId)
      .then(response => {
        if(response.data) {
          this.note = new StudentNoteModel(response.data, this.$moment);
          console.log(this.note.noteId);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('notes.noteLoadErrorMessage', 5000));
        console.log(error.response);
      })
      .then(()=> { this.loading = false; });
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
        .update(this.note)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), this.$t('errors.noteSaveError'), 5000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    }
  }
};
</script>
