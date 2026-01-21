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
          class="mb-2"
        >
          <v-text-field
            v-model="model.applicantFullName"
            :label="$t('refugee.headers.applicantFullName')"
            :rules="[$validator.required()]"
            :class="!isDetails ? 'required' : ''"
            clearable
          />
        </v-col>
      </v-row>
      <v-row
        justify="center"
        dense
      >
        <v-col
          cols="12"
          md="8"
        >
          <person-id-details
            v-if="isDetails"
            :personal-id-type="model.personalIdTypeName"
            :personal-id="model.personalId"
          />
          <PersonUniqueId
            v-else
            ref="personUniqueId"
            :personal-i-d.sync="model.personalId"
            :personal-i-d-type.sync="model.personalIdTypeModel"
            :initial-personal-i-d="model.personalId"
            :initial-personal-type="model.personalIdType.toString()"
            :pin-required="true"
            :initial-pin-types="[0,1]"
          />
        </v-col>
      </v-row>
      <v-row
        dense
      >
        <v-col
          cols="12"
          sm="12"
          md="4"
        >
          <v-text-field
            v-if="isDetails"
            :value="model.town"
            :label="$t('refugee.headers.town')"
          />
          <autocomplete
            v-else
            id="town"
            ref="town"
            v-model="model.townId"
            api="/api/lookups/GetAddressesBySearchString"
            :label="$t('refugee.headers.town')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="12"
          sm="12"
          md="8"
        >
          <v-text-field
            v-model="model.address"
            :label="$t('refugee.headers.address')"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            v-if="isDetails"
            :value="model.nationality"
            :label="$t('createStudent.nationality')"
          />
          <autocomplete
            v-else
            id="nationality"
            ref="nationality"
            v-model="model.nationalityId"
            api="/api/lookups/GetCountriesBySearchString"
            :label="$t('createStudent.nationality')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            v-model="model.email"
            prepend-inner-icon="mdi-email"
            :label="$t('refugee.headers.email')"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            v-model="model.phone"
            prepend-inner-icon="mdi-phone"
            :label="$t('refugee.headers.phone')"
            clearable
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <c-info
            uid="admissionDocument.noteDate"
          >
            <date-picker
              id="applicationDate"
              ref="applicationDate"
              v-model="model.applicationDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('refugee.headers.applicationDate')"
              :rules="[$validator.required()]"
              :class="!isDetails ? 'required' : ''"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            v-if="isDetails"
            :value="model.guardianTypeName"
            :label="$t('refugee.headers.guardianType')"
          />
          <v-select
            v-else
            id="guardianType"
            ref="guardianType"
            v-model="model.guardianType"
            :label="$t('refugee.headers.guardianType')"
            :items="guardianTypeOptions"
            clearable
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
      </v-row>

      <v-card
        outlined
      >
        <v-card-subtitle>
          Прикачени файлове
        </v-card-subtitle>
        <v-card-text>
          <file-manager
            v-model="model.documents"
            :disabled="disabled"
          />
        </v-card-text>
      </v-card>

      <v-row
        v-if="!disabled"
        dense
        class="my-2"
      >
        <v-col cols="12">
          <button-tip
            color="primary"
            text="refugee.addNew"
            tooltip="refugee.addNew"
            iclass=""
            outlined
            bottom
            @click="onRefugeeChildAdd()"
          />
        </v-col>
      </v-row>
      <v-card
        v-for="(item, index) in model.children"
        :key="item.uid"
        class="my-1"
      >
        <v-card-title>
          {{ index+1 }}. {{ item.firstName }} {{ item.middleName }} {{ item.lastName }}
          <v-spacer />
          <v-chip
            v-if="item.status != undefined && item.status != null"
            class="ma-2"
            :color="item.status == 0 ? 'info' : (item.status == 1 ? 'success' : 'error')"
            label
            small
          >
            {{ item.statusName }}
          </v-chip>
          <v-spacer />
          <button-group>
            <button-tip
              v-if="item.personId"
              icon
              icon-name="fas fa-info-circle"
              icon-color="primary"
              tooltip="student.details"
              bottom
              iclass=""
              small
              :to="`/student/${item.personId}/details`"
            />
            <button-tip
              v-if="hasManagePermission && (!item.id || item.canBeDeleted)"
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              :disabled="disabled"
              @click="onRefugeeChildRemove(index)"
            />
          </button-group>
        </v-card-title>
        <v-card-text>
          <!-- {{ item }} -->
          <refugee-application-child-form
            :document="item"
            :disabled="disabled || !item.canBeEdited"
            :is-details="isDetails"
          />
        </v-card-text>
      </v-card>
    </v-form>
    <prompt-dlg
      ref="cancellationPrompt"
      persistent
    >
      <template>
        <v-textarea
          v-model="cancellationReason"
          :label="$t('common.cancellationReason')"
          outlined
          filled
          auto-grow
          clearable
          :rules="[$validator.required()]"
          class="required"
        />
      </template>
    </prompt-dlg>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { RefugeeApplicationModel } from '@/models/refugee/refugeeApplicationModel';
import { RefugeeApplicationChildModel } from '@/models/refugee/refugeeApplicationChildModel';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import RefugeeApplicationChildForm  from '@/components/refugee/RefugeeApplicationChildForm.vue';
import PersonUniqueId from "@/components/person/PersonUniqueId.vue";
import PersonIdDetails from "@/components/person/PersonIdDetails.vue";
import FileManager from '@/components/common/FileManager.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: "RefugeeApplicationForm",
  components: { Autocomplete, RefugeeApplicationChildForm, PersonUniqueId, FileManager, PersonIdDetails },
  props: {
    document: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
    isDetails: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model: this.document ?? new RefugeeApplicationModel(),
      guardianTypeOptions: [],
      cancellationReason: null,
    };
  },
  computed: {
    ...mapGetters(["hasPermission"]),
    hasManagePermission() {
      return this.hasPermission(
        Permissions.PermissionNameForRefugeeApplicationsManage
      );
    },
    hasUnlockPermission() {
      return this.hasPermission(
        Permissions.PermissionNameForRefugeeApplicationsUnlock
      );
    }
  },
  async mounted() {
    if(!this.isDetails) {
      this.loadDropdownOptions();
    }
  },
  methods: {
    loadDropdownOptions() {
      this.$api.lookups
        .getGuardianTypes()
        .then((response) => {
          const guardianTypes = response.data;
          if (guardianTypes) {
            this.guardianTypeOptions = guardianTypes;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.guardianTypeOptionsLoad'));
          console.log(error);
        });
    },
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
    onRefugeeChildAdd() {
      if (!this.model.children) this.model.children = [];
      this.model.children.splice(0, 0, new RefugeeApplicationChildModel({ canBeEdited: true }, this.$moment));
    },
    onRefugeeChildRemove(index) {
      if (!this.model.children) return; // Няма от какво да махаме
      this.model.children.splice(index, 1);
    },
    async completeApplicationChild(id) {
      if (!this.model.children) {
        return;
      }

      if (await this.$refs.confirm.open(this.$t("refugee.complete"), this.$t("common.confirm"))) {
        this.$emit('onCompleteApplicationChild', id);
      }
    },
    async cancelApplicationChild(id) {
      if (!this.model.children) {
        return;
      }

      if(await this.$refs.cancellationPrompt.open('', this.$t('diplomas.annulment'))) {
        if (!this.cancellationReason) {
          return this.$notifier.error('',`${this.$t("diplomas.annulmentReasonError")}`);
        }

        this.$emit('onCancelApplicationChild', { applicationChildId: id, cancellationReason: this.cancellationReason });
      }
    },
    async unlockApplicationChild(id) {
      if (!this.model.children) {
        return;
      }

      if (await this.$refs.confirm.open(this.$t("diplomas.setAsEditable"),this.$t("common.confirm"))) {
        this.$emit('onUnlockApplicationChild', id);
      }
    }
  },
};
</script>
