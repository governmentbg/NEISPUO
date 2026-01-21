<template>
  <v-dialog
    v-model="show"
    max-width="800px"
    persistent
  >
    <template v-slot:activator="{ on: dialog }">
      <v-tooltip bottom>
        <template v-slot:activator="{ on: tooltip }">
          <div
            v-on="{ ...tooltip }"
          >
            <v-btn
              icon
              color="primary"
              small
              :disabled="disabled"
              v-on="{ ...dialog }"
            >
              <v-icon
                color="primary"
              >
                mdi-pencil
              </v-icon>
            </v-btn>
          </div>
        </template>
        <span>{{ tooltipText || $t('buttons.edit') }}</span>
      </v-tooltip>
    </template>
    <form-layout
      :disabled="saving"
      skip-cancel-prompt
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        {{ $t("absence.editAbsence") }}
      </template>

      <template
        v-if="studentAbsenceModel.name"
        #subtitle
      >
        <span class="headline">{{ studentAbsenceModel.name }}</span>
      </template>

      <template #default>
        <absence-form
          :ref="'absenceForm' + _uid"
          v-model="studentAbsenceModel"
        />
      </template>
    </form-layout>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-dialog>
</template>

<script>
import AbsenceForm from '@/components/absence/AbsenceForm';

export default {
  name: "EditAbsence",
  components: {
    AbsenceForm
  },
  props: {
    studentAbsenceModel: {
      type: Object,
      required: true
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    tooltipText: {
      type: String,
      default() {
        return this.$t('buttons.edit');
      }
    }
  },
  data() {
    return {
      show: false,
      saving: false,
    };
  },
  methods: {
    save() {
      this.editStudentAbsence();
      this.close();
    },
    onSave() {
      const form = this.$refs['absenceForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      if(this.studentAbsenceModel.id) {
        this.update();
      } else {
        this.create();
      }
    },
    update() {
      this.saving = true;

      this.$api.absence.update(this.studentAbsenceModel)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.onCancel();
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('common.saveError'), 5000);
        console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
    create() {
      this.saving = true;

      this.$api.absence.create(this.studentAbsenceModel)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.onCancel();
      })
      .catch((error) => {
        this.$notifier.error('', this.$t('common.saveError'), 5000);
        console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
    onCancel() {
      this.show = false;
      this.$emit("reset");
    }
  }
};
</script>
