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
          <h3>{{ $t('otherDocuments.reviewTitle') }}</h3>
        </template>

        <template #default>
          <other-document-form
            v-if="document !== null"
            :ref="'otherDocumentForm' + _uid"
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
import OtherDocumentForm from '@/components/tabs/otherDocuments/OtherDocumentForm.vue';
import { StudentOtherDocumentModel } from '@/models/studentOtherDocumentModel.js';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentOtherDocumentDetails',
  components: {
    OtherDocumentForm
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
    ...mapGetters(['hasStudentPermission', 'hasPermission'])
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentOtherDocumentRead)
     && !this.hasPermission(Permissions.PermissionNameForStudentOtherDocumentManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.otherDocument.getById(this.docId)
      .then(response => {
        if(response.data) {
          this.document = new StudentOtherDocumentModel(response.data, this.$moment);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('common.loadError', 5000));
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
