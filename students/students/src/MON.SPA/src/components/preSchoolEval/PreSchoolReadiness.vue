<template>
  <v-card
    v-if="model"
  >
    <v-card-title>
      {{ this.$t('preSchool.readinessFirstGrade') }}
      <v-spacer />
      <edit-button
        v-if="hasManagePermission"
        v-model="isEditMode"
        icon-color="primary"
        icon
      />
      <button-tip
        v-if="hasManagePermission"
        icon
        icon-name="mdi-delete"
        icon-color="error"
        iclass="mx-2"
        tooltip="buttons.delete"
        bottom
        @click="onDelete"
      />
    </v-card-title>
    <v-card-subtitle>{{ this.$t('preSchoolEvaluation.readiness.title') }}</v-card-subtitle>
    <v-card-text>
      <v-row dense>
        <v-col>
          <v-textarea
            v-model="model.contents"
            counter
            auto-grow
            outlined
            :disabled="!isEditMode"
            persistent-placeholder
            :label="$t('preSchool.endOfYearEvaluation')"
          />
        </v-col>
      </v-row>
    </v-card-text>
    <v-card-actions
      v-if="hasManagePermission && isEditMode"
    >
      <v-spacer />
      <v-btn
        ref="submit"
        raised
        color="primary"
        type="submit"
        @click="onSave"
      >
        <v-icon left>
          fas fa-save
        </v-icon>
        {{ $t('buttons.saveChanges') }}
      </v-btn>

      <v-btn
        raised
        color="error"
        @click="onReset"
      >
        <v-icon left>
          fas fa-times
        </v-icon>
        {{ $t('buttons.cancel') }}
      </v-btn>
    </v-card-actions>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name:"PreSchoolReadinessComponent",
  props:{
    personId: {
      type: Number,
      required: true,
    },
  },
  data(){
    return {
      isEditMode: false,
      model: null,
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentPreSchoolEvaluationManage);
    }
  },
  mounted() {
    this.load();
  },
  methods:{
    load() {
      this.$api.preSchool.getReadinessForFirstGrade(this.personId)
        .then((response) => {
          if (response.data) {
            this.model = response.data;
          }
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("common.loadError"));
          console.error(error);
        });
    },
    async onSave(){
      if(await this.$refs.confirm.open(this.$t('buttons.save'), this.$t('common.confirm'))) {
        try {
          await this.$api.preSchool.updateReadinessForFirstGrade(this.model);
          this.$notifier.success('', this.$t('common.saveSuccess'), 2000);
          this.isEditMode = false;
        } catch (error) {
          this.$notifier.error('', this.$t('common.saveError'), 2000);
          console.log(error);
        }
      }
    },
    async onDelete() {
      if ( await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t("common.confirm"))) {
        try {
          await this.$api.preSchool.deleteReadiness(this.model.id);
          this.$notifier.success("", this.$t("common.deleteSuccess"), 2000);
          this.model = null;
        } catch (error) {
          this.$notifier.error("", this.$t("common.deleteError"), 2000);
          console.log(error);
        }
      }
    },
    onReset(){
      this.isEditMode = false;
    }
  }
};

</script>
