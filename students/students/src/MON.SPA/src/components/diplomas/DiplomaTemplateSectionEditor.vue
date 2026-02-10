<template>
  <v-card>
    <v-card-subtitle>
      <button-group :id="'add-to-top-' + _uid">
        <v-btn
          v-if="!disabled && hasManagePermission"
          small
          color="primary"
          @click.stop="onSubjectAddClick(0)"
        >
          {{ $t("buttons.addSubject") }}
        </v-btn>
        <v-btn
          v-if="!disabled && hasManagePermission"
          small
          color="secondary"
          @click.stop="onHeaderAddClick(0, false)"
        >
          {{ $t("buttons.addHeader") }}
        </v-btn>
      </button-group>
    </v-card-subtitle>
    <v-card-text>
      <v-form
        :ref="`sectionForm_${_uid}`"
        :disabled="disabled"
      >
        <diploma-template-subject-editor
          v-for="(subject, subjectIndex) in sortedSubjects"
          :key="subject.uid"
          :value="value.subjects[subjectIndex]"
          :has-subject-type-limit="value.hasSubjectTypeLimit"
          :subject-type-options="value.subjectTypeOptions"
          :excluded-positions="value.excludedPositions"
          :has-external-evaluation-limit="value.hasExternalEvaluationLimit"
          :min-position="0"
          :max-position="value.totalLines"
          :can-add-modules="!disabled && hasManagePermission"
          class="mb-1"
          hover
          @delete="onSubjectDelete"
        />
      </v-form>
      <v-card-subtitle
        v-if="(value && value.subjects && value.subjects.length > 6)"
      >
        <button-group :id="'add-to-bottom-' + _uid">
          <v-btn
            v-if="!disabled && hasManagePermission"
            small
            color="primary"
            @click.stop="onSubjectAddClick(getPosition())"
          >
            {{ $t("buttons.addSubject") }}
          </v-btn>
          <v-btn
            v-if="!disabled && hasManagePermission"
            small
            color="secondary"
            @click.stop="onHeaderAddClick(getPosition(), false)"
          >
            {{ $t("buttons.addHeader") }}
          </v-btn>
        </button-group>
      </v-card-subtitle>
    </v-card-text>
  </v-card>
</template>

<script>
import DiplomaTemplateSubjectEditor from '@/components/diplomas/DiplomaTemplateSubjectEditor';
import { BasicDocumentTemplateSubjectModel } from '@/models/diploma/basicDocumentTemplateSubjectModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';
import ButtonGroup from '../wrappers/ButtonGroup.vue';

export default {
  name: 'DiplomaTemplateSectionEditorComponent',
  components: {
    DiplomaTemplateSubjectEditor,
    ButtonGroup
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
  computed: {
    ...mapGetters(['hasPermission']),
    hasManagePermission() {
      return this.hasPermission(
        Permissions.PermissionNameForDiplomaTemplatesManage
      );
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
      const form = this.$refs[`sectionForm_${this._uid}`];
      return form ? form.validate() : false;
    },
    onSubjectAddClick(position) {
      if (!this.value || !this.value.subjects) return;

      const toAdd = new BasicDocumentTemplateSubjectModel({
        subjectCanChange: true,
        basicDocumentPartId: this.value.id,
        templateId: this.value.templateId,
        isHorariumHidden: this.value.isHorariumHidden,
        position: position,
      });

      if (this.value.hasSubjectTypeLimit && this.value.subjectTypeOptions && this.value.subjectTypeOptions.length === 1) {
        // Имаме ограничение в типовете оценка и опцията е само една. Тогава ще я изберем по подразбиране.
        toAdd.subjectTypeId = this.value.subjectTypeOptions[0].value;
        toAdd.subjectTypeName = this.value.subjectTypeOptions[0].text;
      }

      this.value.subjects.push(toAdd);
      this.scrollToPosition(position);
    },
    onHeaderAddClick(position, isProfSubjectHeader) {
      if (!this.value || !this.value.subjects) return;

      const toAdd = new BasicDocumentTemplateSubjectModel({
        subjectCanChange: true,
        basicDocumentPartId: this.value.id,
        templateId: this.value.templateId,
        isHorariumHidden: this.value.isHorariumHidden,
        gradeCategory: -1,
        subjectName: 'Нова подрубрика',
        position: position,
        isProfSubjectHeader: isProfSubjectHeader
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
