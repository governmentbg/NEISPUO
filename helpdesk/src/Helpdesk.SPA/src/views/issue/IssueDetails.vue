<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    />
    <div v-else-if="!loading && issue">
      <form-layout
        v-if="isInEditMode"
        :disabled="disabled"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ `${issue.title} #${issue.id}` }}</h3>
        </template>

        <template #default>
          <issue-form
            v-if="issue !== null"
            :ref="'issueForm' + _uid"
            :issue="issue"
            :disabled="disabled"
          />
        </template>
      </form-layout>
      <v-card v-else>
        <v-card-title>
          {{ `${issue.title} #${issue.id}` }}
          <v-spacer />
          <issue-assigment
            :issue-id="issue.id"
            :assigned-to="issue.assignedToSysUserId"
            :assigned-to-name="issue.assignedToSysUser"
            :disabled="isInEditMode || !issue || issue.statusId === 3"
            class="mr-5"
            @assigned="load"
          />
          <button-tip
            v-if="!isInEditMode && issue.statusId != 3"
            icon-name="mdi-pencil"

            iclass=""
            small
            tooltip="buttons.edit"
            text="buttons.edit"
            bottom
            raised

            @click="onEditModeClick"
          />
        </v-card-title>
        <v-card-subtitle>
          <v-chip
            v-if="issue.priority"
            class="ma-2"
            :color="priorityColor(issue.priorityId)"
            label
          >
            <v-icon
              v-if="issue.priorityId === 1"
              left
            >
              mdi-priority-low
            </v-icon>
            <v-icon
              v-else
              left
            >
              mdi-priority-high
            </v-icon>
            {{ issue.priority }}
          </v-chip>
          <v-chip
            v-if="issue.status"
            class="ma-2"
            :color="statusColor(issue.statusId)"
            label
          >
            <v-icon left>
              mdi-label
            </v-icon>
            {{ issue.status }}
          </v-chip>
          <v-chip
            v-if="issue.category"
            class="ma-2"
            label
          >
            {{ issue.category }}
          </v-chip>
          <v-chip
            v-if="issue.subcategory"
            class="ma-2"
            label
          >
            {{ issue.subcategory }}
          </v-chip>
          <v-chip
            v-if="issue.isEscalated"
            class="ma-2"
            color="warning"
            label
          >
            <v-icon left>
              mdi-arrow-up-bold
            </v-icon>
            {{ $t("issue.isEscalated") }}
          </v-chip>
          <v-chip
            v-if="issue.isLevel3Support"
            class="ma-2"
            color="warning"
            label
          >
            <v-icon left>
              mdi-arrow-up-bold
            </v-icon>
            {{ $t("issue.isLevel3Support") }}
          </v-chip>
          <v-chip
            v-if="issue.requestForInformation"
            class="ma-2"
            color="error"
            label
          >
            <v-icon left>
              mdi-flash-alert
            </v-icon>
            {{ $t("issue.requestForInformation") }}
          </v-chip>
        </v-card-subtitle>
        <v-card-text>
          <v-card v-if="issue.assignedToSysUserId">
            <v-card-text>
              <v-row dense>
                <v-col
                  cols="12"
                >
                  <v-icon left>
                    mdi-account
                  </v-icon>
                  {{ $t("issue.assignee") }}: {{ issue.assignedToSysUser }}
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
          <institution-details
            class="mt-3"
            :institution="issue.institution"
          />
          <issue-details
            class="mt-3"
            :issue="issue"
          />

          <v-timeline
            v-if="issue.events && issue.events.length > 0"
            dense
          >
            <v-timeline-item
              v-for="event in issue.events"
              :key="event.uid"
              :color="event.element.isResolvingComment ? statusColor(3) : undefined"
              small
            >
              <template
                v-if="event.element.isResolvingComment"
                #icon
              >
                <v-tooltip bottom>
                  <template v-slot:activator="{ on: tooltip }">
                    <v-icon
                      color="white"
                      v-on="{ ...tooltip }"
                    >
                      mdi-check
                    </v-icon>
                  </template>
                  <span>{{ statusText(3) }}</span>
                </v-tooltip>
              </template>
              <issue-comment-details
                v-if="event.type == 'comment'"
                :comment="event.element"
              />
              <div v-if="event.type == 'status'">
                {{ event.element.comment }} на {{ event.element.createDate | longDate }} от {{ event.element.creatorUsername }}
              </div>
            </v-timeline-item>
          </v-timeline>


          <issue-comment
            v-if="issue.statusId != 3"
            :issue="issue"
            @save="onCommentSave"
          />
        </v-card-text>
        <v-card-actions v-if="issue.statusId == 3">
          <v-spacer />
          <v-btn
            v-if="hasReopenPermission"
            raised
            color="success"
            @click.stop="onReopen"
          >
            {{ $t('buttons.reopen') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </div>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-tooltip right>
      <template v-slot:activator="{ on: goBackBtnTooltip }">
        <v-fab-transition>
          <v-btn
            class="md-1 mr-3 elevation-21"
            transition="fab-transition"
            dark
            fab
            button
            fixed
            left
            bottom
            color="primary"
            v-on="{ ...goBackBtnTooltip }"
            @click.stop="$router.go(-1)"
          >
            <v-icon dark>
              fa-chevron-left
            </v-icon>
          </v-btn>
        </v-fab-transition>
      </template>
      <span>{{ $t('buttons.back') }}</span>
    </v-tooltip>

    <v-tooltip
      v-if="showScrollToTopBtn"
      left
    >
      <template v-slot:activator="{ on: scrollToTopBtnTooltip }">
        <v-fab-transition>
          <v-btn
            v-scroll="onScroll"
            class="md-1 mr-3 elevation-21"
            transition="fab-transition"
            dark
            fab
            button
            fixed
            right
            bottom
            color="primary"
            v-on="{ ...scrollToTopBtnTooltip }"
            @click="$vuetify.goTo(0, goToOptions)"
          >
            <v-icon dark>
              fa-chevron-up
            </v-icon>
          </v-btn>
        </v-fab-transition>
      </template>
      <span>{{ $t('buttons.scrollToTop') }}</span>
    </v-tooltip>
  </div>
</template>

<script>
import { IssueModel } from "@/models/issueModel";
import IssueForm from "@/components/issue/IssueForm.vue";
import IssueDetails from "@/components/issue/IssueDetails.vue";
import IssueComment from "@/components/issue/IssueComment.vue";
import IssueAssigment from "@/components/issue/IssueAssigment.vue";
import IssueCommentDetails from "@/components/issue/IssueCommentDetails.vue";
import InstitutionDetails from "@/components/institution/InstitutionDetails.vue";
import { mapGetters } from "vuex";
import { UserRole } from '@/enums/enums';

export default {
  name: "EditIssueView",
  components: {
    IssueForm,
    IssueDetails,
    IssueComment,
    IssueAssigment,
    IssueCommentDetails,
    InstitutionDetails
  },
  props: {
    issueId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      saving: false,
      issue: null,
      isInEditMode: false,
      showScrollToTopBtn: false,
      offsetTop: 0,
      goToOptions: {
        duration: 500,
        offset: 0,
        easing: 'easeInOutCubic',
      }

    };
  },
  computed: {
    ...mapGetters(['priorityColor', 'statusColor', 'statusText', 'isInRole']),
    hasEditPermission() {
      return true;
    },
    hasReopenPermission() {
      if(this.issue.statusId === 3 && (this.isInRole(UserRole.Consortium) || this.isInRole(UserRole.Mon))) return true;
      return false;
    },
    disabled() {
      return this.saving || !this.isInEditMode;
    },
  },
  mounted() {
    window.addEventListener("scroll", this.handleScroll);

    this.load();

  },
  beforeDestroy() {
    window.removeEventListener("scroll", this.handleScroll);
  },
  methods: {
    onScroll (event) {
      this.offsetTop = event.target.scrollingElement.scrollTop;
    },
    load() {
      this.loading = true;
      this.$api.issue
        .getById(this.issueId)
        .then((response) => {
          if (response.data) {
            this.issue = new IssueModel(response.data);

            this.logIssueReadActivity(this.issueId);
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    logIssueReadActivity(issueId) {
      this.$api.issue.logIssueReadActivity(issueId);
    },
    async onReopen(){
      this.saving = true;
      this.$api.issue
        .reopen({issueId: this.issueId})
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            error?.response?.data?.message ?? this.$t("errors.saveError"),
            7000
          );
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
          this.load();
        });
    },
    async onSave() {
      const form = this.$refs["issueForm" + this._uid];
      const isValid = form.validate();
      this.issue.survey = JSON.stringify(this.issue.surveyObject);

      if (!isValid) {
        return this.$notifier.error(
          "",
          this.$t("errors.validationErrors"),
          5000
        );
      }

      this.saving = true;
      this.$api.issue
        .update(this.issue)
        .then(() => {
          this.$notifier.success("", this.$t("common.saveSuccess"), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(
            "",
            error?.response?.data?.message ?? this.$t("errors.saveError"),
            7000
          );
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    onCancel() {
      this.load();
      this.isInEditMode = false;
    },
    onEditModeClick() {
      if (!this.hasEditPermission) {
        return this.$notifier.warn("", this.$t("errors.accessDenied"), 5000);
      }

      this.isInEditMode = !this.isInEditMode;
    },
    onCommentSave() {
      this.load();
      this.isInEditMode = false;
    },
    handleScroll() {
      this.showScrollToTopBtn = window.scrollY > 300;
    },
  },
};
</script>
