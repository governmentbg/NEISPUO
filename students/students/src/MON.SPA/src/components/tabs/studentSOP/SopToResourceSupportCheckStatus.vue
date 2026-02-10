<template>
  <v-alert
    v-if="message"
    type="warning"
    outlined
    dense
    prominent
    border="left"
  >
    <v-card-title
      class="my-0 py-0 justify-center"
    >
      {{ message }}
    </v-card-title>
    <v-card-actions
      class="my-0 py-0"
    >
      <v-spacer />
      <v-btn
        v-if="hasManagePermission"
        :disabled="disabled"
        text
        color="secondary"
        @click.stop="navigateToCreate"
      >
        {{ $t('resourceSupport.createTitle') }}
      </v-btn>

      <button-tip
        icon
        icon-color="secondary"
        iclass=""
        icon-name="mdi-sync"
        tooltip="Проверка"
        small
        bottom
        :disabled="disabled"
        @click="doCheck"
      />
    </v-card-actions>
  </v-alert>
</template>

<script>
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'SopToResourceSupportCheckStatus',
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    schoolYear: {
      type: Number,
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
      message: ''
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportManage);
    },
  },
  watch: {
    personId() {
      this.doCheck();
    },
    schoolYear() {
      this.doCheck();
    }
  },
  mounted() {
    this.doCheck();
  },
  methods: {
    doCheck() {
      this.message = '';

      this.$api.resourceSupport.chechForExistingByPerson(this.personId, this.schoolYear)
      .then(response => {
        if (response.data) {
            this.message = '';
          } else {
            this.message = `Липсва ${this.$t('resourceSupport.documentForDirectingFromTo')} за ${this.$t('resourceSupport.history.schoolYear')}: ${this.schoolYear}`;
        }
      })
      .catch(error => {
          console.log(error.response);
      });
    },
    navigateToCreate() {
      let routeData = this.$router.resolve({name: 'ResourceSupportCreate', params: {personId: this.personId}, query: {schoolYear: this.schoolYear}});
      window.open(routeData.href, '_blank');
    }
  }
};
</script>
