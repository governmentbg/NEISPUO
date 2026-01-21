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
    <form-layout
      v-else
    >
      <template #title>
        <h3>{{ $t("leadTeacher.detailsTitle") }} </h3>
      </template>

      <template #default>
        <lead-teacher-form
          v-if="document"
          :ref="'leadTeacherForm' + _uid"
          :document="document"
          :disabled="true"
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
</template>

<script>
import LeadTeacherForm from "@/components/leadTeacher/LeadTeacherForm.vue";
import { LeadTeacherModel } from "@/models/leadTeacher/leadTeacherModel";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'LeadTeacherEditView',
  components: {
    LeadTeacherForm,
  },
  props: {
    classId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      document: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForLeadTeacherManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.leadTeacher.getByClassId(this.classId)
      .then(response => {
        if(response.data) {
          this.document = new LeadTeacherModel(response.data);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('documents.documentLoadErrorMessage', 5000));
        console.log(error);
      })
      .then(() => { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    },
  }
};
</script>
