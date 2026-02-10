<template>
  <div>
    <div
      v-if="loading"
      class="loader"
    />
    <div v-else>
      <v-card
        v-if="model"
        class="mt-2"
      >
        <v-card-title>{{ model.institution }}</v-card-title>

        <v-card-text>
          <v-card-title>{{ $t("generalTrainingData.institutionStudyPeriod") }}</v-card-title>

          <v-card
            v-if="studentClassDetails"
            class="mb-2"
          >
            <v-card-title
              class="pt-0 pb-0"
            >
              <v-col>
                <span
                  v-if="studentClassDetails.isCurrent"
                >
                  <v-chip
                    :color="studentClassDetails.positionId === 3 ? 'primary' : 'default'"
                  >
                    {{ studentClassDetails.classGroup.className }}
                  </v-chip>
                </span>
                <span
                  v-else
                >
                  {{ studentClassDetails.classGroup.className }}
                </span>
              </v-col>
            </v-card-title>

            <v-card-text>
              <student-class-details
                :value="studentClassDetails"
              />
            </v-card-text>
          </v-card>

          <v-card
            class="mb-2"
          >
            <v-card-subtitle>
              {{ $t('documents.admissionDocumentTitle' ) }}
            </v-card-subtitle>
            <v-card-text>
              <admission-document-form
                v-if="admissionDocumentDetails"
                :person-id="studentId"
                :document="admissionDocumentDetails"
                is-details-view
                hide-files
                hide-status
                disabled
              />
              <v-alert
                v-else
                outlined
                type="info"
              >
                <p class="text-h6 text-center">
                  {{ $t("errors.missingAdmissionDocument") }}
                </p>
              </v-alert>
            </v-card-text>
          </v-card>

          <v-card
            v-if="admissionRelocationDocumentDetails"
            class="mb-2"
          >
            <v-card-subtitle>
              {{ $t('generalTrainingData.admissionRelatedRelocationDocument' ) }}
            </v-card-subtitle>
            <v-card-text>
              <relocation-document-form
                :person-id="studentId"
                :document="admissionRelocationDocumentDetails"
                disabled
                hide-status
                hide-files
              />
            </v-card-text>
          </v-card>

          <v-card
            v-if="model.relocationDocuments && model.relocationDocuments.length > 0"
            class="mb-2"
          >
            <v-card-subtitle>
              Издадени документи за преместване, които са използвани за записване в друга институция
            </v-card-subtitle>
            <v-card-text>
              <v-card
                v-for="item in model.relocationDocuments"
                :key="item.id"
                outlined
              >
                <v-card-text>
                  <v-row
                    dense
                  >
                    <v-col
                      cols="12"
                      sm="6"
                      md="3"
                    >
                      <v-text-field
                        :value="item.noteNumber + ' / ' +
                          (item.noteDate ? $moment(item.noteDate).format(dateFormat) : item.noteDate)"
                        :label="$t('relocationDocument.docTitle')"
                        readonly
                      />
                    </v-col>
                    <v-col
                      cols="12"
                      sm="6"
                      md="3"
                    >
                      <v-text-field
                        :value="item.ruoOrderNumber + ' / ' +
                          (item.ruoOrderDate ? $moment(item.ruoOrderDate).format(dateFormat) : item.ruoOrderDate)"
                        :label="$t('relocationDocument.ruoOrderTitle')"
                        readonly
                      />
                    </v-col>
                    <v-col
                      v-if="item.sendingInstitution"
                      cols="12"
                      sm="6"
                    >
                      <v-text-field
                        :value="item.sendingInstitution"
                        :label="$t('relocationDocument.sendingInstitution')"
                        readonly
                      />
                    </v-col>

                    <v-col
                      v-if="item.hostInstitution"
                      cols="12"
                      sm="6"
                    >
                      <v-text-field
                        :value="item.hostInstitution"
                        :label="$t('relocationDocument.hostInstitution')"
                        readonly
                      />
                    </v-col>
                  </v-row>
                </v-card-text>
              </v-card>
            </v-card-text>
          </v-card>

          <div
            v-if="dischargeDocumentsDetails"
          >
            <v-card
              v-for="dischargeDocument in dischargeDocumentsDetails"
              :key="dischargeDocument.id"
              class="mb-2"
            >
              <v-card-subtitle>
                {{ $t('documents.dischargeDocumentTitle' ) }}
              </v-card-subtitle>
              <v-card-text>
                <discharge-document-form
                  :document="dischargeDocument"
                  disabled
                  hide-status
                  hide-files
                />
              </v-card-text>
            </v-card>
          </div>

          <v-card
            v-if="studentDualFormCompanies && studentDualFormCompanies.length > 0"
            dense
            class="mb-2"
          >
            <v-card-text>
              <v-row dense>
                <v-col
                  v-for="item in studentDualFormCompanies"
                  :key="item.uid"
                  cols="12"
                  :md="Math.max(12 / studentDualFormCompanies.length, 6)"
                  :xl="Math.max(12 / studentDualFormCompanies.length, 4)"
                >
                  <student-class-dual-form-company
                    :value="item"
                    disabled
                  />
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>

          <!-- Учебен план -->
          <v-divider />
          <v-data-table
            :loading="loadingCurriculum"
            :items="curriculumStudentDetails"
            :headers="curriculumHeaders"
            :search="search"
            :items-per-page="-1"
            :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
          >
            <template v-slot:top>
              <v-toolbar
                flat
              >
                <v-card-title>{{ $t("curriculum.title") }}</v-card-title>
                <v-spacer />
                <v-text-field
                  v-model="search"
                  append-icon="mdi-magnify"
                  :label="$t('common.search')"
                  clearable
                  single-line
                  hide-details
                />
              </v-toolbar>
            </template>

            <template v-slot:[`item.subjectName`]="{ item }">
              <span>{{ item.subjectName }}</span>
              <v-chip
                v-if="item.isValid !== true"
                color="error"
                small
                label
                class="ml-3 float-right"
                outlined
              >
                {{ $t('common.unactive') }}
              </v-chip>
            </template>

            <template v-slot:[`item.hoursWeeklyFirstTerm`]="{ item }">
              <span>{{ item.hoursWeeklyFirstTerm === '0' ? '-' : item.hoursWeeklyFirstTerm }}</span>
            </template>
            <template v-slot:[`item.hoursWeeklySecondTerm`]="{ item }">
              <span>{{ item.hoursWeeklySecondTerm === '0' ? '-' : item.hoursWeeklySecondTerm }}</span>
            </template>
          </v-data-table>
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<script>
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import AdmissionDocumentForm from '@/components/tabs/studentMovement/AdmissionDocumentForm';
import RelocationDocumentForm from '@/components/tabs/studentMovement/RelocationDocumentForm';
import DischargeDocumentForm from '@/components/tabs/studentMovement/DischargeDocumentForm.vue';
import StudentClassDualFormCompany from '@/components/students/class/StudentClassDualFormCompany.vue';
import { StudentClassDualFormCompanyModel } from '@/models/studentClass/studentClassDualFormCompanyModel';
import { StudentClass } from "@/models/studentClass/studentClass.js";
import { StudentAdmissionDocumentModel } from "@/models/studentMovement/studentAdmissionDocumentModel.js";
import { StudentRelocationDocumentModel } from "@/models/studentMovement/studentRelocationDocumentModel.js";
import { NewStudentDischargeDocumentModel } from '@/models/studentMovement/newStudentDischargeDocumentModel.js';
import Constants from "@/common/constants.js";
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";

export default {
  name: 'StudentGeneralTrainingDataDetails',
  components: {
    StudentClassDetails,
    AdmissionDocumentForm,
    RelocationDocumentForm,
    DischargeDocumentForm,
    StudentClassDualFormCompany
  },
  props: {
    studentId: {
      type: Number,
      required: true
    },
    institutionId: {
      type: Number,
      required: true
    },
    classId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      search: '',
      model: null,
      loading: false,
      loadingCurriculum: false,
      curriculumStudentDetails: undefined,
      studentClassDetails: null,
      admissionDocumentDetails: null,
      admissionRelocationDocumentDetails: null,
      dischargeDocumentsDetails: [],
      curriculumHeaders: [
        {text: this.$t('curriculum.headers.subjectName'), value: "subjectName", sortable: true},
        {text: this.$t('curriculum.headers.subjectTypeName'), value: "subjectTypeName", sortable: true},
        {text: this.$t('curriculum.headers.hoursWeeklyFirstTerm'), value: "hoursWeeklyFirstTerm", sortable: true},
        {text: this.$t('curriculum.headers.hoursWeeklySecondTerm'), value: "hoursWeeklySecondTerm", sortable: true}
      ],
      studentPoistionId: 3,
      dateFormat: Constants.DATEPICKER_FORMAT,
      studentDualFormCompanies: null
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions', 'hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentGeneralTrainingDataRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
    this.loadStudentClassDetails(this.classId);
    this.loadStudentClassDualFormCompanies(this.classId);
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.studentLod
        .getStudentGeneralTrainingDataDetails(this.studentId, this.classId)
        .then((response) => {
          if (response.data) {
              this.model = response.data;

              if(this.model.admissionDocumentId) {
                // StudentClass има обвързан документ за записване.
                this.loadAdmissionDocument(this.model.admissionDocumentId);
              }

              if(this.model.admissionRelocationDocumentId) {
                // Документът за записване има обвързан документ за преместване.
                this.loadRelocationDocument(this.model.admissionRelocationDocumentId);
              }

              if(this.model.dischargeDocumentsIds) {
                // Документи за отписване, чиито CurrentStudentClassId е текущия клас.
                for (const dischargeDocId of this.model.dischargeDocumentsIds) {
                   this.loadDischargeDocument(dischargeDocId);
                }
              }

              this.loadCurriculumStudentDetails(this.model.classId);
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.studentGeneralTrainingDataDetailsLoad'));
          console.log(error);
        })
        .then(() => {
          this.loading = false;
        });
    },
    loadStudentClassDetails(id) {
      this.$api.studentClass
        .getById(id)
        .then((response) => {
          if(response.data) {
            this.studentClassDetails = new StudentClass(response.data, this.$moment);
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    loadCurriculumStudentDetails(studentClassId) {
      this.loadingCurriculum = true;

      this.$api.studentLod
        .getCurriculumDetailsByStudentClass(studentClassId)
        .then((response) => {
          this.curriculumStudentDetails = response.data || [];
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.studentGeneralTrainingDataDetailsLoad'));
          console.log(error);
        })
        .then(() => {
          this.loadingCurriculum = false;
        });
    },
    loadAdmissionDocument(id) {
      this.$api.admissionDocument
        .getById(id)
        .then((response) => {
          if(response.data) {
            this.admissionDocumentDetails = new StudentAdmissionDocumentModel(response.data, this.$moment);
          }
        })
        .catch(error => {
          console.log(error.response);
        });
    },
    loadRelocationDocument(id) {
      this.$api.relocationDocument
        .getById(id)
        .then((response) => {
          if(response.data) {
            this.admissionRelocationDocumentDetails = new StudentRelocationDocumentModel(response.data, this.$moment);
          }
        })
        .catch(error => {
          console.log(error.response);
        });
    },
    loadDischargeDocument(id) {
      this.$api.dischargeDocument
        .getById(id)
        .then((response) => {
          if(response.data) {
            this.dischargeDocumentsDetails.push(new NewStudentDischargeDocumentModel(response.data, this.$moment));
          }
        })
        .catch(error => {
          console.log(error.response);
        });
    },
    loadStudentClassDualFormCompanies(studentClassId) {
      this.$api.studentClass
        .getDualFormCompanies(studentClassId)
        .then((response) => {
          if(response.data) {
            this.studentDualFormCompanies = response.data.map(x => new StudentClassDualFormCompanyModel(x, this.$moment));
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    }
  }
};
</script>
