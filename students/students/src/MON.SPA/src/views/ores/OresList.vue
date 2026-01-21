<template>
  <div>
    <ores-list
      :person-id="personId"
      :class-id="classId"
      :institution-id="institutionId || userInstitutionId"
      @oresDelete="onOresDelete"
    />

    <ores-calendar
      v-if="userInstitutionId"
      :ref="'oresCalendar_' + _uid"
      class="mt-3"
    />
  </div>
</template>

<script>
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import OresList from '@/components/ores/OresList';
import OresCalendar from '@/components/ores/OresCalendar';

export default {
  name: 'OresListView',
  components: { OresList, OresCalendar },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    classId: {
      type: Number,
      default() {
        return null;
      }
    },
    institutionId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnOresModule', 'userInstitutionId']),
    hasReadPermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresRead);
    },
  },
  mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }
  },
  methods: {
    onOresDelete() {
      const calendar = this.$refs['oresCalendar_' + this._uid];
      calendar?.refresh();
    }
  }
};
</script>
