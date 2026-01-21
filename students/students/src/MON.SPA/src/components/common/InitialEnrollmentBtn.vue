<template>
  <button-tip
    v-if="isVisible"
    color="primary"
    text="studentClass.initialEntrollBtnText"
    tooltip="studentClass.initialEntrollBtnTooltip"
    bottom
    small
    iclass="mr-1"
    @click="enrollInClass"
  />
</template>

<script>
import { mapGetters } from 'vuex';
import { InstType } from '@/enums/enums';

export default {
  name: 'InitialEnrollmentBtnComponent',
  props: {
    pid: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      isVisible: true,
      initialEnrollmentPosition: null
    };
  },
  computed: {
    ...mapGetters(['isInstType']),
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    }
  },
  created() {
    this.$studentHub.joinStudentGroup(this.pid);
    this.$studentHub.$on('student-class-edu-state-change', this.studentClassEduStateChange);
  },
  destroyed() {
    this.$studentHub.$off('student-class-edu-state-change');
    this.$studentHub.leaveStudentGroup(this.pid);
  },
  mounted() {
    this.initialEnrollmentSecretBtnVisibilityCheck();
  },
  methods: {
    studentClassEduStateChange(personId) {
      if (personId !== this.pid) return;

      this.initialEnrollmentSecretBtnVisibilityCheck();
    },
    initialEnrollmentSecretBtnVisibilityCheck() {
      this.isVisible = false;
      this.initialEnrollmentPosition = null;

      if (this.isCplrInstitution) {
        console.log('Cplr institution. Intitail enrollment btn is not allowed.');
        return;
      }

      this.$api.student.initialEnrollmentSecretBtnVisibilityCheck(this.pid)
      .then(response => {
        if (response.data) {
          this.isVisible = response.data.isVisible;
          this.initialEnrollmentPosition = response.data.entrollmentPosition;
          console.log('initialEnrollmentSecretBtnVisibilityCheck', { isVisible: this.isVisible, initialEnrollmentPosition:  this.initialEnrollmentPosition});
        }
      })
      .catch(error => {
        console.log(error);
      });
    },
    enrollInClass() {
      if (this.isCplrInstitution) {
        console.log('Cplr institution. Intitail enrollment btn is not allowed.');
        return;
      }

      if (!this.initialEnrollmentPosition) {
        return this.$notifier.error('', 'Не може да бъде еднозначно определена позицията.');
      }

      this.$router.push({ name: 'StudentClassInitialEnrollment', params: { id: this.pid }, query: { initialEnrollmentPosition: this.initialEnrollmentPosition} });
    },
  }
};
</script>
