<template>
  <v-card
    outlined
    class="mt-2"
  >
    <v-form
      ref="form"
      :disabled="disabled"
      @submit.prevent="onSubmit"
    >
      <v-row
        class="mb-0 mt-1 mr-2"
        justify="space-between"
      >
        <v-col>
          <v-card-title
            :class="model.parentId ? 'warning--text' : 'primary--text'"
          >
            {{
              model.parentId
                ? `${model.type} / ${$t("externalEval.correction")}`
                : model.type
            }}
          </v-card-title>
        </v-col>

        <v-col
          v-if="!disabled"
          class="pt-4"
        >
          <button-tip
            color="primary"
            text="externalEval.addSubject"
            tooltip="externalEval.addSubject"
            iclass=""
            outlined
            bottom
            @click="onSubjectAdd"
          />
        </v-col>

        <v-col>
          <school-year-selector
            v-if="model"
            v-model="model.schoolYear"
            :label="$t('common.schoolYear')"
          />
        </v-col>
        <v-col
          v-if="hasManagePermission"
          class="text-right"
        >
          <!-- Бутонът за редакция и за добавянена поправка се показват когато никои от групата не е в редакция -->
          <!-- Само ДЗИ-то може да има поправка -->
          <button-tip
            v-if="!isEditMode && !isCorrection && model.typeId === 4"
            outlined
            color="secondary"
            icon
            icon-color="secondary"
            iclass=""
            icon-name="mdi-plus"
            tooltip="externalEval.addCorrection"
            bottom
            @click="onCorrectionAddClick"
          />
          <button-tip
            v-if="!isEditMode"
            outlined
            color="primary"
            icon
            icon-color="primary"
            iclass=""
            icon-name="mdi-pencil"
            tooltip="buttons.edit"
            bottom
            :disabled="isLodFinalized(model.schoolYear)"
            :lod-finalized="isLodFinalized(model.schoolYear)"
            @click="disabled = false"
          />
          <!-- Бутонът за изтриване се показва когато текущото външно еценяване е в редакция(disabled == false) -->
          <button-tip
            v-if="!disabled"
            outlined
            color="error"
            icon
            icon-color="error"
            iclass=""
            icon-name="mdi-delete"
            tooltip="buttons.delete"
            bottom
            :disabled="isLodFinalized(model.schoolYear)"
            :lod-finalized="isLodFinalized(model.schoolYear)"
            @click="onExternalEvaluationDelete"
          />
        </v-col>
      </v-row>
      <v-card-text
        v-if="model"
        class="pt-0 pb-0"
      >
        <v-row
          v-for="(item, index) in model.evaluations"
          :key="item.id"
        >
          <v-col cols="1">
            <v-chip outlined>
              {{ (index+1) }}
            </v-chip>
          </v-col>
          <v-col
            md="4"
            sm="10"
            class="pt-0 pb-0"
          >
            <autocomplete
              :ref="`Subject_${_uid}`"
              v-model="item.subjectId"
              api="/api/lookups/GetSubjectOptions"
              :label="$t('basicDocumentSubject.subjectName')"
              :placeholder="$t('buttons.search')"
              hide-no-data
              hide-selected
              :page-size="20"
              :rules="disabled ? [] : [$validator.required()]"
              :class="disabled ? 'custom-small-text' : 'required custom-small-text'"
              defer-options-loading
            >
              <template v-slot:item="data">
                <v-list-item-content
                  v-text="data.item.text"
                />
                <v-list-item-icon>
                  <v-chip
                    color="light"
                    small
                    outlined
                  >
                    {{ data.item.value }}
                  </v-chip>
                </v-list-item-icon>
              </template>
            </autocomplete>
          </v-col>
          <v-col
            md="2"
            sm="2"
            class="pt-0 pb-0"
          >
            <autocomplete
              v-model="item.subjectTypeId"
              api="/api/lookups/GetSubjectTypeOptions"
              :label="$t('diplomas.template.subjectType')"
              :placeholder="$t('buttons.search')"
              hide-no-data
              hide-selected
              :page-size="20"
              :rules="disabled ? [] : [$validator.required()]"
              :class="disabled ? 'custom-small-text' : 'required custom-small-text'"
              defer-options-loading
              persistent-hint
              :hint="$t('common.comboSearchHint', [2])"
            />
          </v-col>
          <v-col
            md="1"
            sm="2"
            class="pt-0 pb-0"
          >
            <v-text-field
              v-model="item.originalPoints"
              type="number"
              :label="$t('externalEval.originalPoints')"
              :rules="disabled ? [] : [
                $validator.required(),
                $validator.min(0),
                $validator.max(100),
              ]"
              :class="disabled ? '' : 'required'"
              @wheel="$event.target.blur()"
            />
          </v-col>
          <v-col
            md="1"
            sm="2"
            class="pt-0 pb-0"
          >
            <v-text-field
              v-model="item.points"
              type="number"
              :label="$t('externalEval.points')"
              :rules="disabled ? [] : [
                $validator.required(),
                $validator.min(0),
                $validator.max(100),
              ]"
              :class="disabled ? '' : 'required'"
              @wheel="$event.target.blur()"
            />
          </v-col>
          <v-col
            md="1"
            sm="2"
            class="pt-0 pb-0"
          >
            <v-text-field
              v-if="disabled"
              :value="item.grade?.toFixed(2)"
              type="number"
              :label="$t('externalEval.grade')"
            />
            <v-text-field
              v-else
              v-model="item.grade"
              type="number"
              :label="$t('externalEval.grade')"
              :rules="[
                $validator.required(),
                $validator.min(3),
                $validator.max(6),
              ]"
              class="required"
              @wheel="$event.target.blur()"
            />
          </v-col>
          <v-col
            v-if="item.flLevel"
            md="1"
            sm="2"
            class="pt-0 pb-0"
          >
            <v-text-field
              v-model="item.flLevel"
              :label="$t('externalEval.flLevel')"
              :class="disabled ? '' : 'required'"
            />
          </v-col>
          <v-col
            md="1"
            sm="2"
            class="pt-0 pb-0"
          >
            <diV class="text-end mt-3">
              <button-tip
                v-if="!disabled"
                icon
                icon-color="error"
                iclass=""
                icon-name="mdi-delete"
                tooltip="buttons.delete"
                bottom
                @click="onSubjectRemove(index)"
              />
            </diV>
          </v-col>
        </v-row>
        <v-row class="mt-2">
          <v-col
            cols="11"
            class="pb-0"
          >
            <v-textarea
              v-model="model.description"
              outlined
              prepend-icon="mdi-comment"
              rows="1"
              :label="$t('common.detailedInformation')"
            />
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-actions v-if="!disabled">
        <v-spacer />
        <v-btn
          ref="submit"
          raised
          color="primary"
          type="submit"
          :disabled="saving"
        >
          <v-icon left>
            fas fa-save
          </v-icon>
          {{ $t("buttons.saveChanges") }}
        </v-btn>

        <v-btn
          raised
          color="error"
          :disabled="saving"
          @click="onCancel"
        >
          <v-icon left>
            fas fa-times
          </v-icon>
          {{ $t("buttons.cancel") }}
        </v-btn>
      </v-card-actions>
    </v-form>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <ConfirmDlg ref="confirm" />
  </v-card>
</template>

<script>
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import { ExternalEvaluationItemModel } from "@/models/externalEvaluationItemModel";
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: "ExternalEvaluationEditor",
  components: {
    SchoolYearSelector,
    Autocomplete
  },
  props: {
    value: {
      type: Object,
      default() {
        return {};
      },
      required: true,
    },
    isEditMode: {
      // Показава дали цялата група е в edit mode
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model: this.value,
      saving: false,
      disabled: true,
    };
  },
  computed: {
    ...mapGetters(["isLodFinalized", "hasPermission"]),
    isNew() {
      return !this.model.id;
    },
    isCorrection() {
      return this.model && this.model.parentId;
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentExternalEvaluationManage);
    },
  },
  watch: {
    disabled(val) {
      this.$emit("isEditMode", !val);
    },
  },
  mounted() {
    if (this.isNew) {
      this.disabled = false;
    }
  },
  methods: {
    async onExternalEvaluationDelete() {
      if (await this.$refs.confirm.open("", this.$t("common.confirm"))) {
        if (this.isNew) {
          this.$emit("removeExternalEvaluation");
          return;
        }

        // Не е нов. Ще го изтрием от базата
        this.doDelete();
      }
    },
    doDelete() {
      this.saving = true;

      this.$api.externalEvaluation
        .delete(this.model.id)
        .then(() => {
          this.$emit("externalEvaluationDeleted");
        })
        .catch((err) => {
          console.log(err.response);
          this.$notifier.error("", this.$t("errors.externalEvaluationDelete"));
        })
        .then(() => {
          this.saving = false;
        });
    },
    onSubjectAdd() {
      this.model.evaluations.splice(0, 0, new ExternalEvaluationItemModel());
    },
    onSubjectRemove(index) {
      this.model.evaluations.splice(index, 1);
    },
    async onCancel() {
      if (
        await this.$refs.confirm.open(
          this.$t("buttons.cancel"),
          this.$t("common.confirm")
        )
      ) {
        if (this.isNew) {
          // Нов е. Ще го махнем от масива.
          this.$emit("removeExternalEvaluation");
        } else {
          this.$emit("cancel");
        }
      }
    },
    async onSubmit() {
      let hasErrors = this.$validator.validate(this);
      if (hasErrors) {
        this.$notifier.error("", this.$t("validation.hasErrors"), 5000);
        return;
      }

      if (
        await this.$refs.confirm.open(
          this.$t("buttons.save"),
          this.$t("common.confirm")
        )
      ) {
        const payload = {
          id: this.model.id,
          parentId: this.model.parentId,
          typeId: this.model.typeId,
          type: this.model.type,
          personId: this.model.personId,
          schoolYear: this.model.schoolYear,
          description: this.model.description,
          evaluations: this.model.evaluations,
        };

        this.saving = true;

        if (this.isNew) {
          this.$api.externalEvaluation
            .create(payload)
            .then(() => {
              this.disabled = true;
              this.$emit("externalEvaluationSaved");
            })
            .catch((err) => {
              console.log(err.response);
              this.$notifier.error(
                "",
                this.$t("errors.externalEvaluationSave"),
                5000
              );
            })
            .then(() => {
              this.saving = false;
            });
        } else {
          this.$api.externalEvaluation
            .update(payload)
            .then(() => {
              this.disabled = true;
              this.$emit("externalEvaluationSaved");
            })
            .catch((err) => {
              console.log(err.response);
              this.$notifier.error(
                "",
                this.$t("errors.externalEvaluationSave"),
                5000
              );
            })
            .then(() => {
              this.saving = false;
            });
        }
      }
    },
    async onCorrectionAddClick() {
      if (
        await this.$refs.confirm.open(
          this.$t("externalEval.addCorrection"),
          this.$t("common.confirm")
        )
      ) {
        this.$emit("correctionAddClick", this.model);
      }
    },
  },
};
</script>

<style>
input[type='number'] {
    -moz-appearance:textfield;
}
input::-webkit-outer-spin-button,
input::-webkit-inner-spin-button {
    -webkit-appearance: none;
}
</style>
