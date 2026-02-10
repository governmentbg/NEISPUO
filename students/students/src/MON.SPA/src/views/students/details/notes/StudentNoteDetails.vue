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
      <form-layout>
        <template #title>
          <h3>{{ $t('notes.reviewNote') }}</h3>
        </template>

        <template #default>
          <note-form
            v-if="note !== null"
            :ref="'noteForm' + _uid"
            :note="note"
            disabled
          />
        </template>

        <template #actions>
          <v-spacer />
          <v-btn
            raised
            color="primary"
            @click.stop="backClick"
          >
            <v-icon left>
              fas fa-chevron-left
            </v-icon>          
            {{ $t('buttons.back') }}
          </v-btn>
        </template>
      </form-layout>
    </div>
  </div>
</template>

<script>
import NoteForm from "@/components/tabs/notes/NoteForm.vue";

import { StudentNoteModel } from "@/models/studentNoteModel.js";

// import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudenNotetDetails',
  components: {
    NoteForm
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
      note: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission'])
  },
  mounted() {
    // if(!this.hasStudentPermission(Permissions.PermissionNameForStudentNoteRead)) {
    //   return this.$router.push('/errors/AccessDenied');
    // }
    
    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.note.getById(this.noteId)
      .then(response => {
        if(response.data) {
          this.note = new StudentNoteModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('note.noteLoadErrorMessage', 5000));
        console.log(error.response);
      })
      .then(()=> { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>