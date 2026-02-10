<template>
  <form-layout
    disabled
  >
    <template #title>
      <h3>{{ $t('diplomas.reviewDiplomaTitle') }}</h3>
    </template>

    <template #default>
      <app-loader
        v-if="loading"
      />
      <student-diploma-form
        :value="model"
        disabled
        :person-id="model.personId"
      />
      <div class="text-right">
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
      </div>
    </template>
  </form-layout>
</template>

<script>
import AppLoader from '@/components/wrappers/loader.vue';
import StudentDiplomaForm from '@/components/diplomas/StudentDiplomaForm.vue';
import { DynamicSectionModel } from '@/models/dynamic/dynamicSectionModel';
import { DiplomaAdditionalDocumentModel } from '@/models/diploma/diplomaAdditionalDocumentModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';

export default {
  name: 'StudentDiplomaDetailsView',
  components: {
    AppLoader,
    StudentDiplomaForm
  },
  props: {
    diplomaId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      loading: false,
      model: {
        personId: null,
        templateId: null,
        basicDocumentId: null,
        basicDocumentName: null,
        institutionId: null,
        schema: null,
        diplomaData: {
          commissionOrderNumber: null,
          commissionOrderData: null,
          generalDataModel: {},
          parts: null,
          additionalDocuments: [],
          commissionMembers: []
        }
      },
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'hasPermission' ]),
    hasStudentDiplomaReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaRead);
    },
    hasStudentDiplomaByCreateRequestReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentDiplomaByCreateRequestRead);
    },
    hasAdminDiplomaReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForAdminDiplomaRead);
    },
    hasMonHrDiplomaReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead);
    },
    hasRuoHrDiplomaReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaRead);
    },
    hasInstitutionDiplomaReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaRead);
    }
  },
  async mounted() {
    if (!(this.hasStudentDiplomaReadPermission || this.hasStudentDiplomaByCreateRequestReadPermission
      || this.hasAdminDiplomaReadPermission || this.hasInstitutionDiplomaReadPermission
      || this.hasMonHrDiplomaReadPermission || this.hasRuoHrDiplomaReadPermission)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.loading = true;
    await this.load(this.diplomaId);
    this.loading = false;
  },
  methods: {
    async load(diplomaId) {
      try {
        const data = (await this.$api.diploma.getUpdateModel(diplomaId)).data;
        if (data) {
          this.model.diplomaId = data.diplomaId;
          this.model.personId = data.personId;
          this.model.diplomaData.commissionOrderNumber = data.commissionOrderNumber;
          this.model.diplomaData.commissionOrderData = data.commissionOrderData
            ? this.$moment(data.commissionOrderData).format(Constants.DATEPICKER_FORMAT)
            : data.commissionOrderData;

          if (data.basicDocumentTemplate) {
            this.model.basicDocumentId = data.basicDocumentTemplate.basicDocumentId;
            this.model.basicDocumentName = data.basicDocumentTemplate.basicDocumentName;
            this.model.institutionId = data.basicDocumentTemplate.institutionId;

            if (data.basicDocumentTemplate.parts) {
              this.model.diplomaData.parts = data.basicDocumentTemplate.parts || null;
            }

            if (data.basicDocumentTemplate.schema) {
              const json = JSON.parse(data.basicDocumentTemplate.schema);
              this.model.schema = json.map(x => new DynamicSectionModel(x));
            }

            if (data.basicDocumentTemplate.commissionMembers) {
              this.model.diplomaData.commissionMembers = data.basicDocumentTemplate.commissionMembers;
            }
          }

          if (data.contents) {
            this.model.diplomaData.generalDataModel = JSON.parse(data.contents);
          }

          if (data.additionalDocuments && Array.isArray(data.additionalDocuments)) {
            this.model.diplomaData.additionalDocuments = data.additionalDocuments.map(x => new DiplomaAdditionalDocumentModel(x));
          }
        }

      } catch (error) {
        this.$notifier.error('', this.$t('errors.diplomaSchemaLoad'));
        console.log(error);
      }
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
