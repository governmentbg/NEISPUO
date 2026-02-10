<template>
  <div>
    <validation-errors-details
      :value="validationErrors"
    />
    <v-alert
      class="mt-5"
      border="left"
      colored-border
      type="info"
      elevation="2"
    >
      Председател на комисията е първият въведен член
    </v-alert>
    <v-form
      :ref="`commissionForm_${_uid}`"
      :disabled="disabled"
    >
      <v-card-title>
        <v-row
          dense
        >
          <v-col
            cols="12"
            md="6"
          >
            <v-text-field
              v-model="value.commissionOrderNumber"
              :label="$t('diplomas.commission.orderNumber')"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
          >
            <date-picker
              v-model="value.commissionOrderData"
              :label="$t('diplomas.commission.orderDate')"
            />
          </v-col>
        </v-row>
      </v-card-title>
      <v-card-subtitle>
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          :disabled="disabled"
          @click="onCommissionMemberAdd"
        >
          <v-icon left>
            mdi-plus
          </v-icon>
          {{ $t("buttons.newMember") }}
        </v-btn>
      </v-card-subtitle>
      <v-card
        v-for="(item, index) in sortedCommissionMembers"
        :key="item.uid"
        class="mb-2"
        outlined
      >
        <v-card-title
          class="pb-0"
        >
          <v-spacer />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-close-thick"
            icon-color="error"
            iclass="mx-2"
            tooltip="buttons.delete"
            bottom
            fab
            :disabled="disabled"
            @click="onCommissionMemberDelete(item.uid)"
          />
        </v-card-title>
        <!-- <v-card-subtitle>
          {{ item }}
        </v-card-subtitle> -->
        <v-card-text
          class="py-0"
        >
          <commission-member-editor
            :value="value.commissionMembers[index]"
          />
        </v-card-text>
      </v-card>
    </v-form>
    <confirm-dlg ref="commissionConfirm" />
  </div>
</template>

<script>
import ValidationErrorsDetails from '@/components/common/ValidationErrorsDetails';
import CommissionMemberEditor from '@/components/diplomas/CommissionMemberEditor';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'DiplomaCommissionEditorComponent',
  components: {
    CommissionMemberEditor,
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
        || this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForDiplomaTemplatesManage);
    },
    sortedCommissionMembers() {
      if (this.value && this.value.commissionMembers) {
        let sortedMembers = this.value.commissionMembers;
        sortedMembers = sortedMembers.sort((a, b) => { return a.position - b.position; });
        return sortedMembers;
      } else {
        return [];
      }
    }
  },
  methods: {
    validate() {
      this.validationErrors = [];
      const form = this.$refs[`commissionForm_${this._uid}`];
      let isValid = false;
      if (form) {
        isValid = form.validate();
        this.validationErrors = this.$helper.getValidationErrorsDetails(form);
      }

      return isValid;
    },
    onCommissionMemberAdd() {
      if (!this.value) return;

      this.value.commissionMembers.push({
        uid: this.$uuid.v4(),
        position: 0
      });
    },
    async onCommissionMemberDelete(uid) {
      if (!this.value || !this.value.commissionMembers) {
        return;
      }
      if(await this.$refs.commissionConfirm.open('', this.$t('common.confirm'))) {
        const index = this.value.commissionMembers.findIndex(x => x.uid === uid);
        this.value.commissionMembers.splice(index, 1);
      }
    }
  }
};
</script>
