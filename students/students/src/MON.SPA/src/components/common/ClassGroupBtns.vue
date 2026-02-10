<template>
  <button-group>
    <initial-enrollment-btn
      v-if="pid"
      :pid="pid"
    />
    <button-tip
      color="light"
      text="studentClass.entrollBtnText"
      tooltip="studentClass.entrollBtnTooltip"
      bottom
      small
      iclass=""
      :to="isCplrInstitution ? `/student/${pid}/class/cplrEnroll` : `/student/${pid}/class/enroll`"
    />
  </button-group>
</template>

<script>
import InitialEnrollmentBtn from './InitialEnrollmentBtn';
import { Permissions, InstType } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'ClassGroupBtns',
  components: {
    InitialEnrollmentBtn
  },
  props: {
    pid: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      showAddToNewClassBtn: false
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInstType']),
    hasStudentToClassEnrollPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment);
    },
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    }
  },
  mounted() {
    this.canAddToNewClassBtnCheck();
  },
  methods: {
    async canAddToNewClassBtnCheck() {

      let apiVisibilityCheck = false;
      try {
        apiVisibilityCheck = (await this.$api.studentClass.addToNewClassBtnVisibilityCheck(this.pid))?.data;
      } catch (error) {
        console.log(error);
        apiVisibilityCheck = false;
      }

      const traceData = {
        hasStudentToClassEnrollPermission: this.hasStudentToClassEnrollPermission,
        addToNewClassVisibilityFunc: "pid && hasStudentToClassEnrollPermission && apiVisibilityCheck",
        addToNewClassVisibilityCondition: `${!!this.pid} && ${this.hasStudentToClassEnrollPermission} && ${apiVisibilityCheck}`,
        addToNewClassVisibilityConditionEval: !!this.pid && this.hasStudentToClassEnrollPermission && apiVisibilityCheck
      };

      this.$api.trace.trace(traceData);

      this.showAddToNewClassBtn = !!this.pid && this.hasStudentToClassEnrollPermission && apiVisibilityCheck;
    }
  }
};
</script>
