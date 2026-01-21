<template>
  <div>
    <v-form
      v-if="model"
      :ref="'form' + _uid"
      :disabled="isDetailsView || disabled"
    >
      <v-row
        dense
      >
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <c-info
            uid="admissionDocument.noteNumber"
          >
            <v-text-field
              v-model="model.noteNumber"
              :label="$t('documents.noteNumberLabel')"
              :rules="[$validator.required(), $validator.maxLength(20)]"
              clearable
              :class="isDetailsView || disabled ? '' : 'required'"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <c-info
            uid="admissionDocument.noteDate"
          >
            <v-text-field
              v-if="isDetailsView"
              :value="model.noteDate"
              :label="$t('documents.noteDate')"
              prepend-icon="mdi-calendar"
            />
            <date-picker
              v-else
              id="noteDate"
              ref="noteDate"
              v-model="model.noteDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('documents.datePickerLabel')"
              :rules="[$validator.required()]"
              :class="isDetailsView || disabled ? '' : 'required'"
            />
          </c-info>
        </v-col>

        <v-col
          cols="12"
          sm="6"
          md="6"
        >
          <c-info
            uid="admissionDocument.relocationDocumentOptions"
          >
            <v-autocomplete
              id="relocationDocument"
              v-model="model.relocationDocument"
              :label="$t('documents.relocationDocumentLabel')"
              :items="relocationDocumentOptions"
              return-object
              clearable
              hide-no-data
              hide-selected
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <c-info
            uid="admissionDocument.admissionDate"
          >
            <v-text-field
              v-if="isDetailsView"
              :value="model.admissionDate"
              :label="$t('documents.admissionDateLabel')"
              prepend-icon="mdi-calendar"
            />
            <date-picker
              v-else
              id="admissionDate"
              ref="admissionDate"
              v-model="model.admissionDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('documents.admissionDateLabel')"
              :rules="[$validator.required()]"
              :class="isDetailsView || disabled ? '' : 'required'"
            />
          </c-info>
        </v-col>
        <!-- <v-col
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <c-info
            uid="admissionDocument.dischargeDate"
          >
            <v-text-field
              v-if="isDetailsView"
              :value="model.dischargeDate ? $moment(model.dischargeDate).format(dateFormat) : ''"
              :label="$t('documents.dischargeDateLabel')"
              prepend-icon="mdi-calendar"
            />
            <date-picker
              v-else
              id="dischargeDate"
              ref="dischargeDate"
              v-model="model.dischargeDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('documents.dischargeDateLabel')"
            />
          </c-info>
        </v-col> -->
        <v-col
          cols="12"
          md="6"
        >
          <c-info
            uid="admissionDocument.institutionOptions"
          >
            <v-text-field
              v-if="isDetailsView || editConfirmedDocument"
              :value="model.institutionName"
              :label="$t('documents.institutionDropdownLabel')"
            />
            <custom-autocomplete
              v-if="!isDetailsView && mounted && !editConfirmedDocument"
              id="institutionDropdown"
              ref="institutionDropdown"
              v-model="model.institutionId"
              api="/api/lookups/GetInstitutionOptions"
              :label="$t('documents.institutionDropdownLabel')"
              :placeholder="$t('buttons.search')"
              hide-no-data
              hide-selected
              clearable
              :rules="[$validator.required()]"
              :disabled="institutionDisabled"
              :class="isDetailsView || disabled ? '' : 'required'"
              @change="institutionChange"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <c-info
            uid="admissionDocument.positionOptions"
          >
            <v-text-field
              v-if="isDetailsView || editConfirmedDocument"
              :value="model.positionName"
              :label="$t('documents.position')"
            />
            <v-autocomplete
              v-else
              id="positionDropdown"
              v-model="model.position"
              :items="studentPositionOptions"
              :label="$t('documents.position')"
              :placeholder="$t('buttons.search')"
              :rules="[$validator.required()]"
              :class="isDetailsView || disabled ? '' : 'required'"
              hide-no-data
              hide-selected
              clearable
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
        >
          <c-info
            uid="admissionDocument.admissionReasonOptions"
          >
            <v-text-field
              v-if="isDetailsView"
              :value="model.admissionReasonTypeName"
              :label="$t('documents.admissionReasonTypeLabel')"
            />
            <v-autocomplete
              v-else
              ref="admissionReasonDropdown"
              v-model="model.admissionReasonTypeId"
              :items="admissionReasonOptions"
              :label="$t('documents.admissionReasonTypeLabel')"
              :rules="[$validator.required()]"
              clearable
              :class="isDetailsView || disabled ? '' : 'required'"
              hide-no-data
              hide-selected
            />
          </c-info>
        </v-col>
        <v-col
          v-if="!hideStatus"
          cols="12"
          sm="6"
          md="4"
          lg="2"
        >
          <c-info
            uid="admissionDocument.statusOptions"
          >
            <v-text-field
              v-if="isDetailsView || editConfirmedDocument"
              :value="model.statusName"
              :label="$t('documents.statusLabel')"
            />
            <v-autocomplete
              v-else
              id="statusDropdown"
              v-model="model.status"
              :items="statusOptions"
              :label="$t('documents.statusLabel')"
              :rules="[$validator.required()]"
              :class="isDetailsView || disabled ? '' : 'required'"
              :disabled="model.isReferencedInStudentClass"
              hide-no-data
              hide-selected
            />
          </c-info>
        </v-col>
      </v-row>
      <v-alert
        border="left"
        colored-border
        type="info"
        elevation="2"
      >
        Предоставени допълнителни документи. Попълва се при записване на дете/ученик с международна/временна закрила!
      </v-alert>
      <v-row dense>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <v-container
            class="px-0"
            fluid
          >
            <v-radio-group
              v-model="model.hasHealthStatusDocument"
              mandatory
            >
              <template v-slot:label>
                <div><v-icon>fas fa-medkit</v-icon> {{ $t('admissionDocument.healthStatusDocument') }}</div>
              </template>
              <v-radio
                :label="$t('common.no')"
                :value="false"
              />
              <v-radio
                :label="$t('common.yes')"
                :value="true"
              />
            </v-radio-group>
          </v-container>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="4"
          lg="3"
        >
          <v-container
            class="px-0"
            fluid
          >
            <v-radio-group
              v-model="model.hasImmunizationStatusDocument"
              mandatory
            >
              <template v-slot:label>
                <div><v-icon>fas fa-syringe</v-icon> {{ $t('admissionDocument.immunizationStatusDocument') }}</div>
              </template>
              <v-radio
                :label="$t('common.no')"
                :value="false"
              />
              <v-radio
                :label="$t('common.yes')"
                :value="true"
              />
            </v-radio-group>
          </v-container>
        </v-col>
      </v-row>
      <v-row
        v-if="!hideFiles"
      >
        <v-col>
          <file-manager
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-col>
      </v-row>
    </v-form>
    <confirm-dlg ref="confirm" />
    <confirm-dlg ref="noCancelBtnDialog" />
  </div>
</template>

<script>
import { StudentAdmissionDocumentModel } from "@/models/studentMovement/studentAdmissionDocumentModel.js";
import { DropDownModel } from '@/models/dropdownModel.js';

import Constants from '@/common/constants.js';
import DocumentStatuses from "@/common/documentStatuses.js";

import FileManager from '@/components/common/FileManager.vue';
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { UserRole } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "AdmissionDocument",
  components: {
    FileManager,
    CustomAutocomplete
  },
  props: {
    document: {
      type: Object,
      default: null
    },
    personId: {
      type: Number,
      default: undefined
    },
    disabled: {
      type: Boolean,
      default: false
    },
    isDetailsView: {
      type: Boolean,
      default: false
    },
    hideFiles: {
      type: Boolean,
      default: false
    },
    hideStatus: {
      type: Boolean,
      default: false
    },
    // При редакция да потвърден документ не е позволено да се редактират
    // полетата Институция, Позиция и Статус
    editConfirmedDocument: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.document || new StudentAdmissionDocumentModel({status: 1}),
      statusOptions: DocumentStatuses,
      relocationDocumentOptions: [],
      admissionReasonOptions: [],
      studentPositionOptions: [],
      documentAvailableOptions: [{"value": null, "name": "не е избрано"}, {"value": false, "name": "няма"}, {"value": true, "name": "има"}],
      mounted: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
    };
  },
  computed: {
    ...mapGetters(['userSelectedRole', 'isInRole']),
    institutionDisabled() {
      return !this.isInRole(UserRole.Ruo);
    }
  },
  watch: {
    document() {
      this.model = this.document ?? new StudentAdmissionDocumentModel({status: 1});
      if(this.document) {
        this.setSelectedRelocationDocument();
      }
    }
  },
  async mounted() {
    this.loadReasonTypeOptions();
    this.loadRelocationDocumentOptions();

    if(!this.isDetailsView && !this.model.id) {
      await this.setDefaultInstitution();
    }

    this.loadPositionOptions();

    this.mounted = true;
  },
  methods: {
    loadReasonTypeOptions() {
      this.$api.lookups.getAdmissionReasonTypeOptions()
      .then((response) => {
        if(response.data) {
          this.admissionReasonOptions = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    },
    loadRelocationDocumentOptions() {
      this.$api.relocationDocument
        .getRelocationDocumentOptions(this.personId)
        .then((response) => {
          if(response.data) {
            this.relocationDocumentOptions = response.data.map(el =>
              new DropDownModel({ value: el.id, text: `№${el.noteNumber} от ${ this.$moment(el.noteDate).format(Constants.DATEPICKER_FORMAT) }`, name: el.noteNumber })
            );
            this.relocationDocumentOptions.unshift(new DropDownModel({ value: null, text: this.$t('documents.emptyRelocationDocumentOption'),
              name: this.$t('documents.emptyRelocationDocumentOption') }));

            if(this.document) {
              this.setSelectedRelocationDocument();
            }
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('documents.relocationDocumentsLoadErrorMessage'));
          console.log(error.response);
        });
    },
    loadPositionOptions() {
      this.$api.lookups.getStudentPositionOptionsByCondition({ selectedValue : this.model.position, institutionId: this.model.institutionId, personId: this.personId })
      .then((response) => {
        if(response.data) {
          this.studentPositionOptions = response.data;

          // Ако не е дошла позиция с модела(редакция) и имаме само една опция я избираме.
          if(!this.model.position && Array.isArray(this.studentPositionOptions) && this.studentPositionOptions.length === 1) {
            this.model.position = this.studentPositionOptions[0].value;
          }
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    },
    institutionChange() {
      this.loadPositionOptions();
    },
    validate() {
      // Използва се в StudentAdmissionDocumentCreate.vue и StudentAdmissionDocumentEdit.vue
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    },
    async setDefaultInstitution() {
      const selectedRole = this.$store.getters.user.profile.selected_role;
        if (selectedRole && selectedRole.SysRoleID === UserRole.School && selectedRole.InstitutionID) {
          this.model.institutionId = (await this.$api.institution.getDropdownModelById(selectedRole.InstitutionID)).data.value;
          }
    },
    checkForExistingAdmissionDocument(institutionId) {
      return new Promise((resolve) => {
        this.$api.admissionDocument
          .checkForExistingAdmissionDocument(this.personId, institutionId)
          .then((response) => {
            resolve(response);
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('documents.checkForAnotherAdmissionDocumentErrorMessage'));
            console.log(error);
          });
      });
    },
    checkForAdmissionDocumentInTheSameInstitution(institutionId) {
      return new Promise((resolve) => {
        this.$api.admissionDocument
          .checkForAdmissionDocumentInTheSameInstitution(this.personId, institutionId)
          .then((response) => {
            resolve(response);
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('documents.checkForAdmissionDocumentInTheSameInstitutionErrorMessage'));
            console.log(error);
          });
      });
    },
    setSelectedRelocationDocument() {
      this.model.relocationDocument = this.relocationDocumentOptions.filter(el => el.value === this.model.relocationDocumentId)[0];
    }
  }
};
</script>
