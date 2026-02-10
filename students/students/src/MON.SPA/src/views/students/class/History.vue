<template>
  <div>
    <v-card
      v-for="(c, index) in classes"
      :key="index"
      class="elevation-2 mb-2"
    >
      <v-card-title>
        <v-row
          dense
        >
          <v-col>
            {{ c.classGroup.className }}
          </v-col>
          <v-spacer />
          <v-col>
            <v-text-field
              :value="`${c.createdBySysUser} - ${c.createDate ? $moment(c.createDate).format(dateTimeFormat) : c.createDate}`"
              :label="$t('common.creation')"
              dense
              disabled
            />
          </v-col>
          <v-col
            v-if="c.modifiedBySysUser"
          >
            <v-text-field
              :value="`${c.modifiedBySysUser} - ${c.modifyDate ? $moment(c.modifyDate).format(dateTimeFormat) : c.modifyDate}`"
              :label="$t('common.edition')"
              dense
              disabled
            />
          </v-col>
        </v-row>
      </v-card-title>
      <v-card-text>
        <student-class-details
          :value="c"
        />
      </v-card-text>
      <v-card-actions
        v-if="hasStudentClassHistoryDeletePermission"
      >
        <v-spacer />
        <button-tip
          icon
          icon-name="mdi-delete"
          icon-color="error"
          iclass=""
          small
          tooltip="buttons.delete"
          bottom
          fab
          raised
          :disabled="deleting"
          @click="onDeleteHistoryRecord(c.id)"
        />
      </v-card-actions>
    </v-card>
    <v-card-actions>
      <v-spacer />
      <v-btn
        raised
        color="primary"
        @click.stop="backClick"
      >
        <v-icon left>
          fas fa-chevron-left
        </v-icon>
        {{ $t('buttons.back') }}
      </v-btn>
    </v-card-actions>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import { StudentClass } from '@/models/studentClass/studentClass.js';
import Constants from '@/common/constants';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentClassHistory',
  components: {
    StudentClassDetails
  },
  props: {
    id: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      classes: [],
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      deleting: false
    };
  },
  computed: {
    ...mapGetters(['hasInstitutionPermission']),
    hasStudentClassHistoryDeletePermission() {
      return this.hasInstitutionPermission(Permissions.PermissionNameForStudentClassHistoryDelete);
    }
  },
  mounted() {
    if(!this.hasInstitutionPermission(Permissions.PermissionNameForStudentClassHistoryRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
    this.load();
  },
  methods: {
    load(){
      this.classes = [];
      this.$api.studentClass.getHistoryById(this.id)
        .then(response => {
          if(response.data && Array.isArray(response.data)) {
            response.data.forEach(el => {
              this.classes.push(new StudentClass(el, this.$moment));
            });
          }

          this.currentClass = new StudentClass(response.data, this.$moment);
        })
        .catch(error => {
          console.log(error.response);
        });
    },
    async onDeleteHistoryRecord(id){
        if(await this.$refs.confirm.open(this.$t('studentClass.delete'), this.$t('common.confirm'))) {
            this.deleting = true;
            this.$api.studentClass.deleteHistoryRecord(id)
            .then(() => {
              this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
              this.load();
            })
            .catch(error => {
              this.$notifier.error('', this.$t('errors.historyRecordDelete', 5000));
              console.log(error);
            })
            .then(() => {
              this.deleting = false;
            });
        }
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};

</script>
