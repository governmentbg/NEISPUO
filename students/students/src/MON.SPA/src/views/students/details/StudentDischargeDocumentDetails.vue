<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div>
      <v-card
        v-if="studentClass"
        class="my-2"
      >
        <v-card-title>
          {{ studentClass.classGroup.className }}
        </v-card-title>
        <v-card-text>
          <student-class-details
            :value="studentClass"
          />
        </v-card-text>
      </v-card>

      <!-- {{ document }} -->

      <form-layout>
        <template #title>
          <h3>{{ $t('dischargeDocument.reviewTitle') }}</h3>
        </template>
        <template #default>
          <discharge-document-form
            :ref="'dischargeDocumentForm' + _uid"
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
import DischargeDocumentForm from '@/components/tabs/studentMovement/DischargeDocumentForm.vue';
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import { NewStudentDischargeDocumentModel } from '@/models/studentMovement/newStudentDischargeDocumentModel.js';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentDischargeDocumentDetails',
  components: {
    DischargeDocumentForm,
    StudentClassDetails
  },
  props: {
    personId: {
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
      document: null,
      studentClass: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    async load() {
      try {
        const doc = (await this.$api.dischargeDocument.getById(this.docId)).data;
        if(doc) {
          this.document = new NewStudentDischargeDocumentModel(doc, this.$moment);

          if(this.document.studentClassId) {
            this.studentClass = (await this.$api.studentClass.getById(this.document.studentClassId)).data;
          }
        }
        this.loading = false;
      } catch (error) {
        console.log(error);
      }
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
