<template>
  <div>
    <v-row>
      <v-col
        cols="12"
        md="8"
      >
        <v-alert
          v-if="diploma.isCancelled"
          outlined
          type="error"
          prominent
          border="left"
        >
          {{ $t("details.isCancelled") }} на {{ diploma.cancellationDate }}
        </v-alert>
        <v-card color="#26c6da">
          <v-card-text>
            <div>
              <strong>{{ $t("details.personName") }}</strong>: {{ diploma.person.fullName }}
            </div>
            <div>
              <strong>{{ $t("details.personNameLatin") }}</strong>: {{ diploma.person.fullNameLatin }}
            </div>
            <div>
              <strong>{{ $t("details.birthPlace") }}</strong>: {{ diploma.person.birthPlace }}
            </div>
            <div>
              <strong>{{ $t("details.nationality") }}</strong>: {{ diploma.person.nationality }}
            </div>
          </v-card-text>
        </v-card>

        <v-card class="mt-10">
          <v-toolbar
            flat
            outlined
            dense
          >
            <v-card-title>
              {{ diploma.basicDocument.name }}
            </v-card-title>
          </v-toolbar>
          <v-card-text>
            <div>
              <strong>{{ $t("details.series") }}</strong> :
              {{ diploma.series }} {{ diploma.factoryNumber }}
            </div>
            <div>
              <strong>{{ $t("details.regNumber") }}</strong> :
              {{ diploma.registrationNumberTotal }}-{{ diploma.registrationNumberYear }}/{{ diploma.registrationNumberDate }}
            </div>
            <div>
              <strong>{{ $t("details.institutionName") }}</strong> :
              {{ diploma.institutionName }}
            </div>
            <div>
              <strong>{{ $t("details.grade") }}</strong> :
              {{ diploma.gpaText }} {{ diploma.gpa }}
            </div>
            <div>
              <strong>{{ $t("details.yearGraduated") }}</strong> :
              {{ diploma.yearGraduated }}
            </div>
            <div>
              <strong>{{ $t("details.schoolYear") }}</strong> :
              {{ diploma.schoolYearName }}
            </div>
          </v-card-text>
        </v-card>

        <diploma-view
          class="mt-10"
          :diploma-data="JSON.parse(diploma.contentsJson)"
          :schema="JSON.parse(diploma.schemaJson)"
        />

        <v-card class="mt-10">
          <v-toolbar
            flat
            outlined
            dense
          >
            <v-card-title>
              {{ this.$t("details.grades") }}
            </v-card-title>
          </v-toolbar>
          <v-card-text class="pl-0 pr-0">
            <v-card
              v-for="part in diploma.basicDocument.parts"
              :key="part.Id"
              class="ml-0 mr-0 mt-10"
            >
              <v-toolbar
                flat
                outlined
                dense
              >
                <v-card-title class="subtitle-2">
                  {{ part.name }}
                </v-card-title>
              </v-toolbar>
              <v-card-text class="pl-0 pr-0 pb-0">
                <v-data-table
                  :headers="headers(part)"
                  :items="part.subjects"
                  :items-per-page="5"
                  class="elevation-1 pr-0 pl-0"
                  disable-pagination
                  :hide-default-footer="true"
                >
                  <template #[`item.grade`]="{ item }">
                    {{ item.grade ? item.grade : '-' }}
                  </template>
                  <template #[`item.gradeText`]="{ item }">
                    {{ item.grade ? item.gradeText : '-' }}
                  </template>
                </v-data-table>
              </v-card-text>
            </v-card>
          </v-card-text>
        </v-card>
      </v-col>

      <v-col
        cols="12"
        md="4"
      >
        <v-card>
          <v-toolbar
            flat
            outlined
            dense
            color="info"
          >
            <v-card-title>
              {{ this.$t("details.scannedImages") }}
            </v-card-title>
          </v-toolbar>
          <v-card-text>
            <CoolLightBox
              :items="items"
              :index="index"
              @close="index = null"
            />

            <v-row>
              <v-col
                v-for="(image, imageIndex) in items"
                :key="imageIndex"
                class="d-flex child-flex"
                cols="6"
              >
                <v-img
                  :src="image"
                  :lazy-src="image"
                  class="grey lighten-2 image"
                  @click="index = imageIndex"
                >
                  <template #placeholder>
                    <v-row
                      class="fill-height ma-0"
                      align="center"
                      justify="center"
                    >
                      <v-progress-circular
                        indeterminate
                        color="grey lighten-5"
                      />
                    </v-row>
                  </template>
                </v-img>
              </v-col>
            </v-row>
          </v-card-text>

          <v-card-actions class="justify-end" />
        </v-card>

        <v-card class="mt-10">
          <v-toolbar
            flat
            outlined
            dense
            color="info"
          >
            <v-card-title>
              {{ this.$t("info.title") }}
            </v-card-title>
          </v-toolbar>
          <v-card-text>
            <div>{{ $t("details.informationLine1") }}</div>
            <div class="pt-5">
              {{ $t("details.informationLine2") }}
            </div>
          </v-card-text>
        </v-card>

        <v-btn
          class="mt-10"
          block
          color="primary"
          to="/"
        >
          {{ $t("buttons.newSearch") }}
        </v-btn>
      </v-col>
    </v-row>

    <v-tooltip top>
      <template #activator="{ on }">
        <slot>
          <v-btn
            color="primary"
            fab
            dark
            bottom
            left
            fixed
            :to="`/`"
            v-on="on"
          >
            <v-icon>fas fa-home</v-icon>
          </v-btn>
        </slot>
      </template>
      <span>{{ $t("menu.home") }}</span>
    </v-tooltip>
  </div>
</template>

<script>
import CoolLightBox from "vue-cool-lightbox";
import "vue-cool-lightbox/dist/vue-cool-lightbox.min.css";
import { config } from "@/common/config";
import DiplomaView from "@/components/DiplomaView.vue";

export default {
  name: "DiplomaDetails",
  components: {
    CoolLightBox,
    DiplomaView,
  },
  props: {
    diploma: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      items: [],
      index: null,
      APIBaseUrl: config.apiBaseUrl,
      subjectHeaders: [
        {
          text: this.$t("details.subjectHeaders.subject"),
          align: "start",
          sortable: false,
          value: "subject",
          normalPart: true,
          externalEvaluationPart: true
        },
        {
          text: this.$t("details.subjectHeaders.points"),
          value: "points",
          normalPart: false,
          externalEvaluationPart: true
        },
        {
          text: this.$t("details.subjectHeaders.gradeText"),
          value: "gradeText",
          normalPart: true,
          externalEvaluationPart: true
        },
        { text: this.$t("details.subjectHeaders.grade"), value: "grade",
          normalPart: true,
          externalEvaluationPart: true },
        { text: this.$t("details.subjectHeaders.horarium"), value: "horarium",
          normalPart: true,
          externalEvaluationPart: false },
      ],
    };
  },
  mounted() {
    this.items = this.diploma.documents.map((image) => `${image.url}`);
  },
  methods: {
    headers(part){
      if (part.externalEvaluationTypes && part.externalEvaluationTypes.length > 0) {
        return this.subjectHeaders.filter(header => header.externalEvaluationPart);
      }
      else{
        return this.subjectHeaders.filter(header => header.normalPart);
      }
    }
  },
};
</script>

<style scoped>
.image {
  cursor: pointer;
  background-size: cover;
}
</style>
