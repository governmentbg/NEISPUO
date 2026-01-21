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
          <h3>{{ $t('recognition.reviewTitle') }}</h3>
        </template>

        <template #default>
          <recognition-form
            v-if="document !== null"
            :ref="'recognitionForm' + _uid"
            :document="document"
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
import RecognitionForm from '@/components/students/RecognitionForm.vue';
import { RecognitionModel } from '@/models/recognitionModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentRecognitionDetails',
  components: {
    RecognitionForm
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
    docId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: true,
      document: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentRecognitionRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.recognition.getById(this.docId)
      .then(response => {
        if(response.data) {
          this.document = new RecognitionModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
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