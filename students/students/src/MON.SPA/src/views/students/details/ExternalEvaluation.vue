<template>
  <v-card>
    <v-card-title>
      <span class="headline">{{ $t('externalEval.title') }}</span>
    </v-card-title>
    <v-card-text>
      <v-row class="mb-0">
        <v-col
          v-if="hasManagePermission"
          cols="12"
          sm="11"
          class="pb-0"
        >
          <v-select
            id="externalEvaluationType"
            ref="externalEvaluationType"
            v-model="selectedExternalEvaluationType"
            :label="$t('externalEval.type')"
            :items="filteredExternalEvaluationTypes"
            return-object
            item-text="name"
            item-value="id"
            clearable
            class="required"
            :disabled="!hasManagePermission || isEditMode"
          />
        </v-col>
        <v-col
          cols="12"
          sm="1"
          class="pb-0"
        >
          <div class="text-center">
            <button-tip
              v-if="hasManagePermission"
              color="primary"
              iclass=""
              icon-name="mdi-plus"
              tooltip="buttons.newRecord"
              bottom
              fab
              small
              :disabled="isEditMode"
              @click="onAdd"
            />
          </div>
        </v-col>
      </v-row>

      <app-loader v-if="loading" />
      <div v-if="!loading && model && model.externalEvaluations && model.externalEvaluations.length > 0">
        <ext-eval
          v-for="(item, index) in model.externalEvaluations"
          :key="item.uid"
          v-model="model.externalEvaluations[index]"
          :is-edit-mode="isEditMode"
          @removeExternalEvaluation="onRemoveExternalEvaluation(index)"
          @externalEvaluationDeleted="onExternalEvaluationDeleted"
          @externalEvaluationSaved="onExternalEvaluationSaved"
          @cancel="onCancel"
          @isEditMode="onIsEditMode"
          @correctionAddClick="onCorrectionAddClick"
        />
      </div>
      <div v-else>
        <v-alert
          :value="true"
          type="info"
          class="mb-3"
          outlined
        >
          {{ $t("common.noData") }}
        </v-alert>
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import { ExternalEvaluationModel } from "@/models/externalEvaluationModel";
import ExtEval from "@/components/editors/ExternalEvaluationEditor.vue";
import AppLoader from "@/components/wrappers/loader.vue";
import Helper from "@/components/helper";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import Constants from "@/common/constants.js";

export default {
  name: "ExternalEvaluation",
  components: {
    ExtEval,
    AppLoader,
  },
  props: {
    studentId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      externalEvaluationTypes: null,
      selectedExternalEvaluationType: undefined,
      model: null,
      isEditMode: false,
    };
  },
  computed: {
    ...mapGetters(["hasStudentPermission", "gridItemsPerPageOptions", "hasPermission"]),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentExternalEvaluationRead
      );
    },
    hasManagePermission() {
      return (
        this.hasStudentPermission(
          Permissions.PermissionNameForStudentExternalEvaluationManage
        ) || this.hasPermission(Permissions.PermissionNameForStudentExternalEvaluationManage)
      );
    },
    filteredExternalEvaluationTypes() {
      if (!this.externalEvaluationTypes) {
        return [];
      }

      // В ЛОД НВО/ДЗИ един вид ExternalEvaluation да може да се въвежда повече от веднъж #1317
      return this.externalEvaluationTypes;
      // return this.externalEvaluationTypes.filter((type) => {
      //   return (
      //     !this.model ||
      //     !this.model.externalEvaluations ||
      //     !this.model.externalEvaluations.some((e) => e.typeId === type.id)
      //   );
      // });
    },
  },
  mounted() {
    if (
      !this.hasStudentPermission(
        Permissions.PermissionNameForStudentExternalEvaluationRead
      )
    ) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.load();
    this.loadDropdownOptions();
  },
  methods: {
    load() {
      this.isEditMode = false;
      this.loading = true;

      this.$api.externalEvaluation
        .getByPersonId(this.studentId)
        .then((response) => {
          if (response.data) {
            //this.modifySubjectNames(response.data);

            this.model = {
              personId: this.studentId,
              externalEvaluations: response.data,
            };
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    loadDropdownOptions() {
      this.$api.lookups
        .getExternalEvaluationTypeOptions()
        .then((response) => {
          this.externalEvaluationTypes = response.data;
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    onAdd() {
      if (!this.selectedExternalEvaluationType) {
        this.$notifier.error(
          "",
          this.$t("externalEval.errors.missingEvaluationType"),
          5000
        );
        return;
      }

      this.model.externalEvaluations.splice(
        0,
        0,
        new ExternalEvaluationModel({
          typeId: this.selectedExternalEvaluationType.id,
          type: this.selectedExternalEvaluationType.name,
          schoolYear: Helper.getYear(),
          personId: this.studentId,
        })
      );

      this.selectedExternalEvaluationType = undefined;
    },
    onRemoveExternalEvaluation(index) {
      if (this.model.externalEvaluations) {
        this.model.externalEvaluations.splice(index, 1);
      }

      this.isEditMode = false;
    },
    onExternalEvaluationDeleted() {
      this.load();
    },
    onExternalEvaluationSaved() {
      this.load();
    },
    onIsEditMode(val) {
      this.isEditMode = val;
    },
    onCancel() {
      this.load();
    },
    onCorrectionAddClick(parent) {
      const parentIndex = this.model.externalEvaluations.findIndex(
        (x) => x.id === parent.id
      );
      console.log(parentIndex);
      if (parentIndex < 0) {
        this.$notifier.error(
          "",
          this.$t("externalEval.errors.missingParent"),
          5000
        );
        return;
      }

      this.model.externalEvaluations.splice(
        parentIndex,
        0,
        new ExternalEvaluationModel({
          parentId: parent.id,
          typeId: parent.typeId,
          type: parent.type,
          schoolYear: Helper.getYear(),
          personId: this.studentId,
          uid: this.$uuid.v4(),
        })
      );

      this.selectedExternalEvaluationType = undefined;
    },
    modifySubjectNames(data) {
      data.forEach(externalEvaluation => {
        externalEvaluation.evaluations.forEach(evaluation => {
          if(evaluation.subjectTypeId) {
            const profilingSubjectTypeNameIndex = Constants.profilingSubjectTypes.indexOf(evaluation.subjectTypeName);

            if(profilingSubjectTypeNameIndex > -1) {
              evaluation.subject = `ПП - ${evaluation.subject}`;
            }
            else {
              evaluation.subject = `${evaluation.subjectTypeName} - ${evaluation.subject}`;
            }
          }
        });
      });
    }
  },
};
</script>
