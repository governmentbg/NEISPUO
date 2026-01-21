<template>
  <notes-list
    :person-id="id"
  />
</template>

<script>
import NotesList from '@/components/tabs/notes/NotesList.vue';

export default {
  name: 'StudentNotesList',
  components: {
      NotesList
  },
   props: {
    id: {
      type: Number,
      required: true
    }
  },
};
</script>
