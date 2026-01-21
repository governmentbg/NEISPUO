<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
    />
    <div v-else>
      <v-container
        v-if="classes"
        fluid
      >
        <v-row
          dense
        >
          <v-col
            v-for="item in classes"
            :key="item.id"
            cols="12"
            sm="12"
            :md="classes.length > 1 ? 6 : 12"
          >
            <student-class
              :model="item"
              show-title
            />
          </v-col>    
        </v-row>
      </v-container>
    </div>
  </div>
</template>

<script>
import StudentClass from '@/components/students/StudentClass.vue';

export default {
  name: "StudentEducation",
  components: {
    StudentClass
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
    showTitle: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      loading: false,
      classes: [],
    };
  },
  beforeMount() {
    this.init();
  },
  methods: {
    init() {
      this.loading = true;

      this.$api.student
        .getStudentEducationByPersonId(this.personId)
        .then((response) => {
          if(response.data) {
            this.classes = response.data;
          }
        })
        .catch((error) => {
          console.log(error);
          this.$notifier.error("", this.$t("errors.studentEducationLoad"));
        })
        .then(() => { this.loading = false; });
    },
  },
};
</script>