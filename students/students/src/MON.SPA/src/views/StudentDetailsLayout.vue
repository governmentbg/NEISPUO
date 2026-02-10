<template>
  <div>
    <v-navigation-drawer
      v-model="studentMenuDrawer"
      :clipped="studentMenuClipped"
      enable-resize-watcher
      fixed
      app
    >
      <v-list-item
        v-if="currentStudentSummary != undefined"
        class="px-2"
      >
        <v-list-item-content class="wrap-text">
          <v-tooltip bottom>
            <template #activator="{ on }">
              <span v-on="on">
                <v-avatar
                  v-if="studentMiniMenu"
                  color="accent"
                  size="36"
                >
                  <span class="white--text">{{ avatarText }}</span>
                </v-avatar>
                <span v-else>
                  <u>{{ currentStudentSummary.fullName }}</u>
                </span>
              </span>
            </template>
            <span>{{ currentStudentSummary.fullName }}</span>
          </v-tooltip>
        </v-list-item-content>
      </v-list-item>

      <template v-for="(item, i) in items.filter((x) => x.visible)">
        <v-list-group
          v-if="item.children"
          :key="i + 1000"
          v-model="item.expanded"
          no-action
        >
          <template #activator>
            <v-list-item-icon v-if="item.icon">
              <v-icon
                :color="item.iconColor"
                v-text="item.icon"
              />
            </v-list-item-icon>
            <v-list-item-content>
              <v-tooltip right>
                <template #activator="{ on }">
                  <v-list-item-title
                    class="wrap-text"
                    v-on="on"
                  >
                    {{ item.title }}
                  </v-list-item-title>
                </template>
                <span>{{ item.tooltip || item.title }}</span>
              </v-tooltip>
            </v-list-item-content>
          </template>

          <template
            v-for="(child, k) in item.children.filter((x) => x.visible)"
          >
            <v-list-group
              v-if="child.children"
              :key="k + 10000"
              v-model="child.expanded"
              no-action
            >
              <template #activator>
                <v-list-item-icon
                  v-if="child.icon"
                  class="pl-6"
                >
                  <v-icon
                    :color="child.iconColor"
                    v-text="child.icon"
                  />
                </v-list-item-icon>
                <v-list-item-content
                  :class="child.icon ? '' : 'pl-6'"
                >
                  <v-tooltip right>
                    <template #activator="{ on }">
                      <v-list-item-title
                        class="wrap-text"
                        v-on="on"
                      >
                        {{ child.title }}
                      </v-list-item-title>
                    </template>
                    <span>{{ child.tooltip || child.title }}</span>
                  </v-tooltip>
                </v-list-item-content>
              </template>

              <v-list-item
                v-for="(subchild, s) in child.children.filter((x) => x.visible)"
                :key="s + 100000"
                :to="subchild.link"
                link
                class="pl-20"
              >
                <v-tooltip right>
                  <template v-slot:activator="{ on }">
                    <v-list-item-icon
                      v-if="subchild.icon"
                      v-on="on"
                    >
                      <v-icon
                        :color="subchild.iconColor"
                        v-text="subchild.icon"
                      />
                    </v-list-item-icon>
                    <v-list-item-title
                      class="wrap-text"
                      v-on="on"
                    >
                      {{ subchild.title }}
                    </v-list-item-title>
                  </template>
                  <span>{{ subchild.tooltip || subchild.title }}</span>
                </v-tooltip>
              </v-list-item>
            </v-list-group>

            <template v-else>
              <v-list-item
                :key="k + 10000"
                :to="child.link"
                link
                class="pl-10"
              >
                <v-tooltip right>
                  <template v-slot:activator="{ on }">
                    <v-list-item-icon
                      v-if="child.icon"
                      v-on="on"
                    >
                      <v-icon
                        :color="child.iconColor"
                        v-text="child.icon"
                      />
                    </v-list-item-icon>
                    <v-list-item-content
                      class="wrap-text"
                      v-on="on"
                    >
                      {{ child.title }}
                    </v-list-item-content>
                  </template>
                  <span>{{ child.tooltip || child.title }}</span>
                </v-tooltip>
              </v-list-item>
            </template>
          </template>
        </v-list-group>
        <template v-else>
          <v-divider
            v-if="item.divider"
            :key="i"
          />
          <v-list-item
            v-else
            :key="i + 1"
            :to="item.link"
            color="primary"
            link
          >
            <v-tooltip right>
              <template v-slot:activator="{ on }">
                <v-list-item-icon
                  v-if="item.icon"
                  v-on="on"
                >
                  <v-icon
                    :color="item.iconColor"
                    v-text="item.icon"
                  />
                </v-list-item-icon>
                <v-list-item-content
                  class="wrap-text"
                  v-on="on"
                >
                  {{ item.title }}
                </v-list-item-content>
              </template>
              <span>{{ item.tooltip || item.title }}</span>
            </v-tooltip>
          </v-list-item>
        </template>
      </template>
      <v-chip
        v-if="mode !== 'prod'"
        class="ma-2"
        color="deep-purple accent-4"
        outlined
      >
        <v-icon left>
          mdi-wrench
        </v-icon>
        {{ mode }}
      </v-chip>
    </v-navigation-drawer>

    <v-app-bar
      :clipped-left="studentMenuClipped"
      app
      color="stratos"
      dark
    >
      <v-app-bar-nav-icon
        @click.stop="studentMenuDrawer = !studentMenuDrawer"
      />

      <v-img
        class="mx-2"
        src="@/assets/logo.png"
        max-height="30"
        max-width="30"
        contain
      />
      <router-link
        class="homeLink"
        to="/"
      >
        <v-toolbar-title>
          {{ $t("moduleTitle") }} {{ $t("appTitle") }}
        </v-toolbar-title>
      </router-link>

      <v-spacer />

      <v-badge
        v-if="isInStudentsModuleRole"
        :content="getPersonMessagesCount"
        :value="getPersonMessagesCount"
        overlap
      >
        <router-link
          class="notificationsLink"
          to="/notifications/messages"
        >
          <v-icon
            class="fas fa-bell"
            small
          />
        </router-link>
      </v-badge>
      <dashboard-menu />

      <profile-menu />
    </v-app-bar>

    <v-container fluid>
      <v-tooltip top>
        <template v-slot:activator="{ on }">
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

      <student-profile
        class="mb-2"
        :pid="id"
        :show-details-button="false"
        :base-route="'/student'"
      />
      <router-view v-if="permissionsLoaded" />
    </v-container>
  </div>
</template>

<script>
import StudentProfile from '@/components/students/Profile.vue';
import ProfileMenu from '@/ProfileMenu.vue';
import DashboardMenu from '@/views/account/DashboardMenu.vue';
import { mapGetters, mapActions } from 'vuex';
import Helper from '@/components/helper';
import { Permissions } from '@/enums/enums';

export default {
  name: 'StudentDetailsLayout',
  components: {
    StudentProfile,
    ProfileMenu,
    DashboardMenu
  },
  props: {
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      studentMenuDrawer: true,
      studentMiniMenu: false,
      studentMenuClipped: true,
      pin: undefined,
      permissionsLoaded: false,
    };
  },
  computed: {
    ...mapGetters([
      'currentStudentSummary',
      'hasStudentPermission',
      'hasPermission',
      'isInStudentsModuleRole',
      'mode',
      'getPersonMessagesCount',
      'turnOnOresModule',
      'isDevelopment',
      'personalDevelopmentSuppert_v2'
    ]),
    showPersonalDevelopmentSuppert_v2() {
      return this.isDevelopment || this.personalDevelopmentSuppert_v2;
    },
    avatarText() {
      if (!this.currentStudentSummary) return "";

      return Helper.getAvatarText(this.currentStudentSummary.fullName);
    },
    items() {
      return [
        {
          title: this.$t('student.menu.details'),
          icon: 'fas fa-info-circle',
          iconColor: 'primary',
          link: `/student/${this.id}/details`,
          visible:
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentPersonalDataRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentPartialPersonalDataRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentPersonalDataManage
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentEducationRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentInternationalProtectionRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentInternationalProtectionManage
            ),
        },
        {
          title: this.$t('student.menu.movement'),
          expanded: false,
          icon: 'fa-exchange-alt',
          'icon-alt': 'mdi-chevron-down',
          iconColor: 'primary',
          visible:
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentAdmissionDocumentRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentDischargeDocumentRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentRelocationDocumentRead
            ),
          children: [
            {
              title: this.$t('student.menu.admissionDocuments'),
              icon: 'fas fa-sign-in-alt',
              iconColor: '',
              link: `/student/${this.id}/admissionDocuments`,
              visible: this.hasStudentPermission(
                Permissions.PermissionNameForStudentAdmissionDocumentRead
              ),
            },
            {
              title: this.$t('student.menu.dischargeDocuments'),
              icon: 'fas fa-sign-out-alt',
              iconColor: '',
              link: `/student/${this.id}/dischargeDocuments`,
              visible: this.hasStudentPermission(
                Permissions.PermissionNameForStudentDischargeDocumentRead
              ),
            },
            {
              title: this.$t('student.menu.relocationDocuments'),
              icon: 'fas fa-exchange-alt',
              iconColor: '',
              link: `/student/${this.id}/relocationDocuments`,
              visible: this.hasStudentPermission(
                Permissions.PermissionNameForStudentRelocationDocumentRead
              ),
            },
          ],
        },
        {
          title: this.$t('student.menu.otherInstitutions'),
          icon: 'fas fa-university',
          iconColor: 'primary',
          link: `/student/${this.id}/otherInstitutions`,
          visible:
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentOtherInstitutionRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentOtherInstitutionManage
            ),
        },
        {
          title: this.$t('student.menu.otherDocuments'),
          icon: 'fas fa-folder-open',
          iconColor: 'primary',
          link: `/student/${this.id}/otherDocuments`,
          visible:
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentOtherDocumentRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentOtherDocumentManage
            ) ||
            this.hasPermission(
              Permissions.PermissionNameForStudentOtherDocumentManage
            ), // Роля училище или детска градина да може да управялева Други документи, независимо дали ученикът е записан в институцията
        },
        {
          title: this.$t('student.menu.notes'),
          icon: 'fas fa-sticky-note',
          iconColor: 'primary',
          link: `/student/${this.id}/notes`,
          visible:
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentNoteRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentNoteManage
            ),
        },
        {
          title: this.$t('student.menu.lod'),
          expanded: false,
          icon: 'fa-user',
          'icon-alt': 'fa-rocket',
          iconColor: 'primary',
          visible: true,
          children: [
            {
              title: this.$t('student.menu.environmentCharacteristics'),
              icon: 'mdi-image-text',
              iconColor: '',
              link: `/student/${this.id}/environmentCharacteristics`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentEnvironmentCharacteristicRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentEnvironmentCharacteristicManage
                ),
            },
            {
              title: this.$t('student.menu.institutionDetails'),
              icon: 'fas fa-university',
              iconColor: '',
              link: `/student/${this.id}/institutionDetails`,
              visible: this.hasStudentPermission(
                Permissions.PermissionNameForStudentCurrentInstitutionDetailsRead
              ),
            },
            {
              title: this.$t('student.menu.generalTrainingData'),
              icon: 'fas fa-table',
              iconColor: '',
              link: `/student/${this.id}/generalTrainingData`,
              visible: this.hasStudentPermission(
                Permissions.PermissionNameForStudentGeneralTrainingDataRead
              ),
            },
            {
              title: this.$t('student.menu.preSchoolEvaluation'),
              icon: 'fas fa-child',
              iconColor: '',
              link: `/student/${this.id}/preSchoolEvaluations`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentPreSchoolEvaluationRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentPreSchoolEvaluationManage
                ),
            },
            {
              title: this.$t('student.menu.evaluations'),
              icon: 'mdi-clipboard-list-outline',
              iconColor: '',
              link: `/student/${this.id}/lod/assessments`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentEvaluationRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentEvaluationManage
                ),
            },
            {
              title: this.$t('student.menu.externalEval'),
              icon:'fas fa-medal',
              iconColor: '',
              link: `/student/${this.id}/externalEval`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentExternalEvaluationRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentExternalEvaluationManage
                ),
            },
            {
              title: this.$t('student.menu.equalizations'),
              icon: 'fas fa-clone',
              iconColor: '',
              link: `/student/${this.id}/equalizations`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentEqualizationRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentEqualizationManage
                ),
            },
            {
              title: this.$t('student.menu.recognitions'),
              icon: 'fas fa-clone',
              iconColor: '',
              link: `/student/${this.id}/recognitions`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentRecognitionRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentRecognitionManage
                ),
            },
            {
              title: this.$t('student.menu.reassessment'),
              icon: 'fas fa-clone',
              iconColor: '',
              link: `/student/${this.id}/reassessments`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentReassessmentRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentReassessmentManage
                ),
            },
            {
              title: this.$t('validationDocument.menuTitle'),
              icon: 'fas fa-certificate',
              iconColor: '',
              link: `/student/${this.id}/validations`,
              visible:
                (this.hasStudentPermission(
                  Permissions.PermissionNameForStudentRecognitionRead
                  // Permissions.PermissionNameForAdminDiplomaRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentRecognitionManage
                  // Permissions.PermissionNameForStudentDiplomaRead
                )
                // ||
                // this.hasStudentPermission(
                //   Permissions.PermissionNameForStudentDiplomaManage
                // ) ||
                // this.hasStudentPermission(
                //   Permissions.PermissionNameForStudentDiplomaByCreateRequestRead
                // ) ||
                // this.hasStudentPermission(
                //   Permissions.PermissionNameForStudentDiplomaByCreateRequestManage
                // )
                ),
            },
            {
              title: this.$t('student.menu.otherDocuments'),
              icon: 'fas fa-folder-open',
              iconColor: '',
              link: `/student/${this.id}/lod/documents`,
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentOtherDocumentRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentOtherDocumentManage
                ) ||
                this.hasPermission(
                  Permissions.PermissionNameForStudentOtherDocumentManage
                ), // Роля училище или детска градина да може да управялева Други документи, независимо дали ученикът е записан в институцията
            },
            {
              title: this.$t('student.menu.personalDevelopment'),
              expanded: false,
              icon: 'fa-user-friends',
              'icon-alt': 'mdi-chevron-down',
              iconColor: '',
              tooltip: this.$t('lod.personalDevelopmentSupport'),
              visible: this.showPersonalDevelopmentSuppert_v2 &&
                (this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentRead)
                || this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage)
                || this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportRead)
                || this.hasStudentPermission(Permissions.PermissionNameForStudentResourceSupportManage)),
              children: [
                {
                  title: this.$t('student.menu.earlyAssessment'),
                  icon: 'fa-child',
                  iconColor: '',
                  link: `/student/${this.id}/earlyAssessment`,
                  tooltip: this.$t('lod.earlyEvaluationLabel'),
                  visible:
                    this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentRead)
                    || this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage),
                },
                {
                  title: this.$t('student.menu.commonPersonalDevelopment'),
                  icon: 'fa-hand-holding',
                  iconColor: '',
                  link: `/student/${this.id}/commonPersonalDevelopment/list`,
                  tooltip: this.$t('lod.commonPersonalDevelopmentSupport'),
                  visible:
                    this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentRead)
                    || this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage),
                },
                {
                  title: this.$t('student.menu.additionalPersonalDevelopment'),
                  icon: 'fa-hands',
                  iconColor: '',
                  link: `/student/${this.id}/additionalPersonalDevelopment/list`,
                  tooltip: this.$t('lod.additionalPersonalDevelopmentSupport'),
                  visible:
                    this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentRead)
                    || this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage),
                },
                {
                  title: this.$t('student.menu.resourceSupport'),
                  icon: 'mdi-human-male-female-child',
                  iconColor: '',
                  link: `/student/${this.id}/resourceSupports`,
                  tooltip: this.$t('student.menu.resourceSupport'),
                  visible:
                    this.hasStudentPermission(
                      Permissions.PermissionNameForStudentResourceSupportRead
                    ) ||
                    this.hasStudentPermission(
                      Permissions.PermissionNameForStudentResourceSupportManage
                    ),
                },
              ]
            },
            {
              title: `${this.$t('student.menu.personalDevelopment')} - ${this.$t('student.menu.earlyAssessment')}`,
              icon: 'mdi-developer-board',
              iconColor: '',
              link: `/student/${this.id}/earlyAssessment`,
              tooltip: this.$t('lod.earlyEvaluationLabel'),
              visible: !this.personalDevelopmentSuppert_v2
                && (
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentPersonalDevelopmentRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentPersonalDevelopmentManage
                )),
            },
            {
              title: `${this.$t('student.menu.personalDevelopment')} - ${this.$t('student.menu.commonPersonalDevelopment')}/${this.$t('student.menu.additionalPersonalDevelopment')}`,
              icon: 'mdi-developer-board',
              iconColor: '',
              link: `/student/${this.id}/personalDevelopment`,
              tooltip: this.$t('lod.personalDevelopmentSupport'),
              visible: !this.personalDevelopmentSuppert_v2
                && (this.hasStudentPermission(
                  Permissions.PermissionNameForStudentPersonalDevelopmentRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentPersonalDevelopmentManage
                )),
            },
            {
              title: this.$t('student.menu.sop'),
              icon: 'fas fa-hands-helping',
              iconColor: '',
              link: `/student/${this.id}/sop`,
              tooltip: this.$t('sop.subtitle'),
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentSopRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentSopManage
                ),
            },
            {
              title: this.$t('student.menu.scholarships'),
              icon: 'fas fa-coins',
              iconColor: '',
              link: `/student/${this.id}/scholarships`,
              tooltip: this.$t('student.menu.scholarships'),
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentScholarshipRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentScholarshipManage
                ),
            },
            {
              title: this.$t('student.menu.studentAwards'),
              icon: 'fas fa-award',
              iconColor: '',
              link: `/student/${this.id}/lod/awards`,
              tooltip: this.$t('student.menu.studentAwards'),
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentAwardRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentAwardManage
                ),
            },
            {
              title: this.$t('student.menu.studentSanctions'),
              icon: 'fas fa-gavel',
              iconColor: '',
              link: `/student/${this.id}/lod/sanctions`,
              tooltip: this.$t('student.menu.studentSanctions'),
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentSanctionRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentSanctionManage
                ),
            },
            {
              title: this.$t('student.menu.selfGovernment'),
              icon: 'fas fa-school',
              iconColor: '',
              link: `/student/${this.id}/lod/selfGovernments`,
              tooltip: this.$t('student.menu.selfGovernment'),
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentSelfGovernmentRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentSelfGovernmentManage
                ),
            },
            {
              title: this.$t('student.menu.internationalMobility'),
              icon: 'fas fa-globe',
              iconColor: '',
              link: `/student/${this.id}/lod/internationalMobilities`,
              tooltip: this.$t('student.menu.internationalMobility'),
              visible:
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentInternationalMobilityRead
                ) ||
                this.hasStudentPermission(
                  Permissions.PermissionNameForStudentInternationalMobilityManage
                ),
            },
            {
              title: this.$t('student.menu.lodFinalizations'),
              icon: 'fa-file-signature',
              iconColor: '',
              link: `/student/${this.id}/lodFinalizations`,
              tooltip: this.$t('student.menu.lodFinalizations'),
              visible:
                this.hasStudentPermission(Permissions.PermissionNameForStudentLodFinalizationRead)
            },
          ],
        },
        {
          title: this.$t('menu.diplomas.title'),
          icon: 'fas fa-certificate',
          iconColor: 'primary',
          link: `/student/${this.id}/diplomas`,
          visible:
            (this.hasStudentPermission(
              Permissions.PermissionNameForAdminDiplomaRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentDiplomaRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentDiplomaManage
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentDiplomaByCreateRequestRead
            ) ||
            this.hasStudentPermission(
              Permissions.PermissionNameForStudentDiplomaByCreateRequestManage
            )),
        },
        // Да се скрие меню "Отсъствия" в ЛОД #1201
        // https://github.com/Neispuo/students/issues/1201
        // {
        //   title: this.$t('student.menu.absences'),
        //   icon: 'fas fa-calendar-times',
        //   iconColor: 'primary',
        //   link: `/student/${this.id}/absences`,
        //   visible:
        //     this.hasStudentPermission(
        //       Permissions.PermissionNameForStudentAbsenceRead
        //     ) ||
        //     this.hasStudentPermission(
        //       Permissions.PermissionNameForStudentAbsenceManage
        //     ),
        // },
        {
          title: this.$t('student.menu.ores'),
          icon: 'fas fa-viruses',
          iconColor: 'primary',
          link: `/student/${this.id}/ores`,
          visible:
            this.turnOnOresModule
            && ( this.hasStudentPermission( Permissions.PermissionNameForOresRead)
              || this.hasStudentPermission(Permissions.PermissionNameForOresManage)
            )
           ,
        },
      ];
    },
  },
  async created() {
    this.$store.commit('hideMainMenu');
    this.$store.dispatch('setCurrentStudentSummary', this.id);
    await this.loadPermissionsForStudent(this.id);
    await this.loadPermissionsForInstitutionForLoggedUser();
    await this.loadStudentFinalizedLods(this.id);
    this.permissionsLoaded = true;

    this.$studentEventBus.$on('studentMovementUpdate', (personId) =>
      this.onStudentMovementUpdate(personId)
    );

    this.$studentHub.joinStudentGroup(this.id);
    this.$studentHub.$on('student-finalized-lods-reloaded', this.loadStudentFinalizedReloaded);
  },
  mounted() {
    if(!this.isInStudentsModuleRole) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  beforeDestroy() {
    this.$store.commit('showMainMenu');
    this.$store.dispatch('clearCurrentStudentSummary');
    this.clearPermissionsForStudent();
    this.clearPermissionsForInstitution();
    this.clearStudentFinalizedLods();
  },
  destroyed() {
    this.$studentEventBus.$off('studentMovementUpdate');
    this.$studentHub.$off('student-finalized-lods-reloaded');

    this.$studentHub.leaveStudentGroup(this.id);
  },
  methods: {
    ...mapActions([
      'clearPermissionsForStudent',
      'loadPermissionsForStudent',
      'loadPermissionsForInstitutionForLoggedUser',
      'clearPermissionsForInstitution',
      'setCurrentStudentSummary',
      'loadStudentFinalizedLods',
      'clearStudentFinalizedLods'
    ]),
    onStudentMovementUpdate(personId) {
      if (personId === this.id) {
        this.loadPermissionsForStudent(personId);
        this.setCurrentStudentSummary(personId);
      }
    },
    loadStudentFinalizedReloaded(finalizedLods) {
      this.$store.commit('setStudentFinalizedLods', finalizedLods);
      this.$notifier.success(this.$t('notifications.title'), this.$t('notifications.studentFinalizedLodsReloaded'));
    }
  },
};
</script>

<style scoped>
.homeLink {
  text-decoration: none;
  color: white;
}
.wrap-text {
  white-space: normal !important;
  font-size: 0.929rem !important;
}
</style>
