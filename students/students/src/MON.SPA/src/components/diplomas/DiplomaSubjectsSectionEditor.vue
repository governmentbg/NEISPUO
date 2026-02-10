<template>
  <div>
    <validation-errors-details
      :value="validationErrors"
    />
    <v-card-subtitle>
      <button-group :id="'add-to-top-' + _uid">
        <v-btn
          v-if="!disabled && hasManagePermission"
          small
          color="primary"
          :disabled="disabled"
          @click.stop="onSubjectAddClick(0)"
        >
          {{ $t("buttons.addSubject") }}
        </v-btn>
        <v-btn
          v-if="!disabled && hasManagePermission"
          small
          color="secondary"
          :disabled="disabled"
          @click.stop="onHeaderAddClick(0)"
        >
          {{ $t("buttons.addHeader") }}
        </v-btn>
      </button-group>
    </v-card-subtitle>
    <v-form
      :ref="`sectionForm_${_uid}`"
      :disabled="disabled"
    >
      <!-- <diploma-subject-editor
        v-for="(subject, subjectIndex) in sortedSubjects"
        :key="subject.uid"
        :value="value.subjects[subjectIndex]"
        :disabled="disabled"
        :has-subject-type-limit="value.hasSubjectTypeLimit"
        :has-external-evaluation-limit="value.hasExternalEvaluationLimit"
        :subject-type-options="value.subjectTypeOptions"
        :is-horarium-hidden="value.isHorariumHidden"
        :excluded-positions="value.excludedPositions"
        :min-position="0"
        :max-position="value.totalLines"
        :show-ects-grade="value.showEctsGrade"
        hover
        :can-add-modules="false"
        @delete="onSubjectDelete"
      /> -->
      <diploma-subject-editor
        v-for="(subject, subjectIndex) in sortedSubjects"
        :key="subject.uid"
        :value="value.subjects[subjectIndex]"
        :disabled="disabled"
        :has-subject-type-limit="value.hasSubjectTypeLimit"
        :has-external-evaluation-limit="value.hasExternalEvaluationLimit"
        :subject-type-options="value.subjectTypeOptions"
        :is-horarium-hidden="value.isHorariumHidden"
        :excluded-positions="value.excludedPositions"
        :min-position="0"
        :max-position="value.totalLines"
        :show-ects-grade="value.showEctsGrade"
        hover
        :can-add-modules="!disabled && hasManagePermission"
        @delete="onSubjectDelete"
      />
    </v-form>
    <v-card-subtitle
      v-show="(value && value.subjects && value.subjects.length > 6)"
    >
      <button-group :id="'add-to-bottom-' + _uid">
        <v-btn
          v-if="!disabled && hasManagePermission"
          small
          color="primary"
          :disabled="disabled"
          @click.stop="onSubjectAddClick(getPosition())"
        >
          {{ $t("buttons.addSubject") }}
        </v-btn>
        <v-btn
          v-if="!disabled && hasManagePermission"
          small
          color="secondary"
          :disabled="disabled"
          @click.stop="onHeaderAddClick(getPosition())"
        >
          {{ $t("buttons.addHeader") }}
        </v-btn>
      </button-group>
    </v-card-subtitle>
  </div>
</template>

<script>
import DiplomaSubjectEditor from '@/components/diplomas/DiplomaSubjectEditor';
import ValidationErrorsDetails from '@/components/common/ValidationErrorsDetails';
import { DiplomaSubjectModel } from '@/models/diploma/diplomaSubjectModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'DiplomaSubjectsSectionEditorComponent',
  components: {
    DiplomaSubjectEditor,
    ValidationErrorsDetails
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      validationErrors: []
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
        ||  this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
        ||  this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage)
        ||  this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage)        ;
    },
    sortedSubjects() {
      if (!this.value || !this.value.subjects) {
        return [];
      }

      let sortedSubjects = this.value.subjects;
      sortedSubjects = sortedSubjects.sort((a, b) => { return a.position - b.position; });
      return sortedSubjects;
    }
  },
  methods: {
    validate() {
      this.validationErrors = [];
      const form = this.$refs[`sectionForm_${this._uid}`];
      let isValid = false;
      if (form) {
        isValid = form.validate();
        this.validationErrors = this.$helper.getValidationErrorsDetails(form);
      }

      return isValid;
    },
    onSubjectAddClick(position) {
      if (!this.value || !this.value.subjects) return;

      const toAdd = new DiplomaSubjectModel({
        subjectCanChange: true,
        basicDocumentPartId: this.value.id,
        templateId: this.value.templateId,
        position: position
      });

      if (this.value.hasSubjectTypeLimit && this.value.subjectTypeOptions && this.value.subjectTypeOptions.length === 1) {
        // Имаме ограничение в типовете оценка и обцията е само една. Тогава ще я изберем по подразбиране.
        toAdd.subjectTypeId = this.value.subjectTypeOptions[0].value;
        toAdd.subjectTypeName = this.value.subjectTypeOptions[0].text;
      }

      this.value.subjects.push(toAdd);
      this.scrollToPosition(position);
    },
    onHeaderAddClick(position) {
      if (!this.value || !this.value.subjects) return;

      const toAdd = new DiplomaSubjectModel({
        subjectName: 'Нова подрубрика',
        gradeCategory: -1,
        subjectTypeId: -1,
        subjectCanChange: true,
        basicDocumentPartId: this.value.id,
        templateId: this.value.templateId,
        position: position
      });

      this.value.subjects.push(toAdd);
      this.scrollToPosition(position);
    },
    scrollToPosition(position) {
      const options = {
          duration: 300,
          offset: 0,
          easing: 'easeInOutCubic'
      };

      const selector = (position > 0 ? '#add-to-bottom' : '#add-to-top') + `-${this._uid}`;
      this.$vuetify.goTo(selector, options);
    },
    onSubjectDelete(subjectUid) {
      if (!this.value || !this.value.subjects) {
        return;
      }
      const subjectIndex = this.value.subjects.findIndex(x => x.uid === subjectUid);
      this.value.subjects.splice(subjectIndex, 1);
    },
    getPosition() {
      const positions = this.sortedSubjects.map(x => x.position);
      if(!positions || positions.length === 0) return 0;

      return Math.max(...positions) + 1;
    }
  }
};
</script>
