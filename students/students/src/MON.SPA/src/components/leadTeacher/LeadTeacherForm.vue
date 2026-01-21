<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row dense>
        <v-col
          cols="12"
          sm="12"
          md="6"
        >
          <v-text-field
            readonly
            :value="model.classGroupName"
            :label="$t('leadTeacher.headers.classGroupName')"
          />
        </v-col>

        <v-col
          cols="12"
          sm="12"
          md="6"
        >
          <v-text-field
            v-if="isDetails"
            :value="model.leadTeacherName"
            :label="$t('leadTeacher.headers.leadTeacherName')"
          />
          <autocomplete
            v-else
            id="staffPosition"
            ref="staffPosition"
            v-model="model.staffPositionId"
            api="/api/lookups/GetStaff"
            :label="$t('leadTeacher.headers.leadTeacherName')"
            :placeholder="$t('buttons.search')"
            clearable
            hide-no-data
            hide-selected
            :rules="[$validator.required()]"
            class="required"
          />
        </v-col>
      </v-row>
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { LeadTeacherModel } from "@/models/leadTeacher/leadTeacherModel";
import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";

export default {
  name: "RefugeeApplicationForm",
  components: { Autocomplete },
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
      model: this.document ?? new LeadTeacherModel(),
    };
  },
  computed: {
    ...mapGetters(["hasPermission"]),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForLeadTeacherManage);
    },
  },
  async mounted() {},
  methods: {
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
  },
};
</script>
