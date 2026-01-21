<template>
  <grid
    :ref="'absenceGrid' + _uid"
    url="/api/absence/getStudentAbsences"
    file-export-name="Списък с импортирани файлове"
    :headers="headers"
    :title="$t('absence.absence')"
    :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    :filter="{ studentId: studentId }"
  >
    <template #actions="item">
      <button-group>
        <edit-absence
          v-if="hasManagePermission"
          :student-absence-model="item.item"
          :disabled="item.item.isLodFinalized"
          :tooltip-text="item.item.isLodFinalized ? $t('student.signedLodEditTooltip') : ''"
          @reset="onReset"
        />
        <absence-history
          v-if="hasReadPermission"
          :absence-id="item.item.id"
        />
      </button-group>
    </template>
  </grid>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import AbsenceHistory from '@/components/absence/AbsenceHistory';
import EditAbsence from '@/components/absence/EditAbsence';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentAbsence',
  components: { AbsenceHistory, EditAbsence, Grid },
 props: {
    studentId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      headers: [
        {
          text: this.$t('absence.headers.schoolYear'),
          value: 'schoolYearName',
          sortable: false,
        },
        {
          text: this.$t('absence.headers.month'),
          value: 'monthName',
          sortable: false,
        },
        {
          text: this.$t('absence.headers.excused'),
          value: 'excused',
          sortable: false,
        },
        {
          text: this.$t('absence.headers.unexcused'),
          value: 'unexcused',
          sortable: false,
        },
        {text: '', value: 'controls', sortable: false, align: 'end'},
      ],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAbsenceManage);
    },
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentAbsenceRead);
    }
  },
  methods: {
    onReset() {
      const grid = this.$refs['absenceGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    }
  }
};
</script>
