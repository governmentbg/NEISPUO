<template>
  <div>
    <!-- {{ model }} -->
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row
        dense
      >
        <v-col
          cols="12"
          sm="6"
          lg="4"
          xl="3"
        >
          <c-info
            uid="relocationDocument.currentStudentClassDropdown"
          >
            <v-text-field
              :value="model.currentStudentClassName"
              :label="$t('documents.currentStudentClass')"
              disabled
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          :md="hideStatus ? '9' : '6'"
        >
          <c-info
            uid="relocationDocument.sendingInstitution"
          >
            <v-text-field
              v-model="model.sendingInstitution"
              :label="$t('documents.sendingInstitution')"
              disabled
            />
          </c-info>
        </v-col>
        <v-col
          v-if="!hideStatus"
          cols="12"
          sm="6"
          lg="4"
          xl="3"
        >
          <c-info
            uid="relocationDocument.statusDropdown"
          >
            <v-select
              id="statusDropdown"
              v-model="model.status"
              :items="statusOptions"
              :label="$t('documents.statusLabel')"
              :rules="[$validator.required()]"
              :class="!disabled ? 'required' : ''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="4"
          xl="2"
        >
          <c-info
            uid="relocationDocument.noteNumber"
          >
            <v-text-field
              v-model="model.noteNumber"
              :label="$t('relocationDocument.noteNumber')"
              :rules="[$validator.required(), $validator.maxLength(20)]"
              clearable
              :class="!disabled ? 'required' : ''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="4"
          xl="2"
        >
          <c-info
            uid="relocationDocument.noteDate"
          >
            <date-picker
              id="noteDate"
              ref="noteDate"
              v-model="model.noteDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('relocationDocument.noteDate')"
              clearable
              :rules="[$validator.required()]"
              :class="!disabled ? 'required' : ''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="4"
          xl="2"
        >
          <c-info
            uid="relocationDocument.dischargeDate"
          >
            <date-picker
              id="dischargeDate"
              ref="dischargeDate"
              v-model="model.dischargeDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('relocationDocument.dischargeDate')"
              clearable
              :rules="[$validator.required()]"
              :class="!disabled ? 'required' : ''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-info
            uid="relocationDocument.institutionDropdown"
          >
            <slot name="hostingInstitution">
              <custom-autocomplete
                id="institutionDropdown"
                ref="institutionDropdown"
                v-model="model.institutionId"
                api="/api/lookups/GetInstitutionOptions"
                :label="$t('documents.recipientInstitution')"
                :placeholder="$t('buttons.search')"
                hide-no-data
                hide-selected
                clearable
              />
            </slot>
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="6"
        >
          <c-info
            uid="relocationDocument.relocationReasonType"
          >
            <custom-autocomplete
              id="relocationReasonType"
              ref="relocationReasonType"
              v-model="model.relocationReasonTypeId"
              api="/api/lookups/GetDischargeReasonTypeOptions"
              :label="$t('documents.relocationReasonTypeLabel')"
              clearable
              :defer-options-loading="false"
              :filter="{isForRelocation: true}"
              :rules="[$validator.required()]"
              :class="!disabled ? 'required' : ''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="4"
          xl="3"
        >
          <c-info
            uid="relocationDocument.ruoOrderNumber"
          >
            <v-text-field
              v-model="model.ruoOrderNumber"
              :label="$t('documents.ruoOrderNumber')"
              :rules="isRuoRequired ? [$validator.required()] : []"
              :class="isRuoRequired && !disabled ? 'required' : ''"
              clearable
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          lg="4"
          xl="3"
        >
          <c-info
            uid="relocationDocument.ruoOrderDate"
          >
            <date-picker
              id="ruoOrderDate"
              ref="ruoOrderDate"
              v-model="model.ruoOrderDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('documents.ruoOrderDate')"
              :rules="isRuoRequired ? [$validator.required()] : []"
              :class="isRuoRequired && !disabled ? 'required' : ''"
              clearable
            />
          </c-info>
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
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import { StudentRelocationDocumentModel } from '@/models/studentMovement/studentRelocationDocumentModel.js';
import DocumentStatuses from "@/common/documentStatuses.js";
import FileManager from '@/components/common/FileManager.vue';
import CustomAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { UserRole, ClassKind, Positions } from '@/enums/enums';


export default {
  name: "RelocationDocument",
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
    hostingInstitution: {
      type: Number,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default: false
    },
    hideStatus: {
      type: Boolean,
      default: false
    },
    hideFiles: {
      type: Boolean,
      default: false
    },
  },
  data() {
    return {
      model: null,
      statusOptions: DocumentStatuses,
      roleSchool: UserRole.School,
    };
  },
  computed: {
    ...mapGetters(['currentStudentSummary', 'isInRole', 'userDetails', 'userSelectedRole']),
    isSchoolDirector() {
      return this.isInRole(this.roleSchool);
    },
    isRuoRequired() {
      // Ако dischargeReasonTypeId е "премества се в друго училище след санкция (чл. 199 ал. 1 т.4)" (Таблица student.DischargeReasonType)
      // Номер на заповед на РУО и Дата на заповед на РУО за задължителни.
      return this.model.relocationReasonTypeId === 2;
    },
  },
  watch: {
    document() {
      this.model = this.document ?? new StudentRelocationDocumentModel();
    }
  },
  beforeMount(){
    this.model = new StudentRelocationDocumentModel({
      status: 1,
      sendingInstitution: this.userDetails.institution,
      sendingInstitutionId: this.userDetails.institutionID,
      institutionId: this.hostingInstitution
    });

    if(Array.isArray(this.currentStudentSummary?.allCurrentClasses) && this.currentStudentSummary.allCurrentClasses.length > 0) {
      const currentStudentClass = this.currentStudentSummary && this.currentStudentSummary.allCurrentClasses && this.currentStudentSummary.allCurrentClasses.length > 0
        ? this.currentStudentSummary.allCurrentClasses
            .filter(x => x.institutionId === this.userSelectedRole.InstitutionID && x.isCurrent === true && x.positionId === Positions.Student && x.classKind === ClassKind.Basic)[0]
        : this.currentStudentSummary.currentClass.basicClassId;

      this.model.currentStudentClassId = currentStudentClass?.studentClassId;
      this.model.currentStudentClassName = currentStudentClass?.className;
    }
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
