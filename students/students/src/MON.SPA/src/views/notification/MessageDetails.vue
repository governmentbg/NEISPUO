<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div
      v-else
    >
      <v-card
        v-if="model"
      >
        <v-card-title>
          {{ model.subject }}
          <v-spacer />
          {{ model.date }}
        </v-card-title>
        <v-card-subtitle>
          {{ model.senderName }} / {{ model.sendDate ? $moment(model.sendDate).format(dateAndTimeFormat): '' }}
        </v-card-subtitle>
        <v-card-text>
          <!-- eslint-disable-next-line vue/no-v-html -->
          <div v-html="model.content" />
        </v-card-text>
        <v-card-actions>
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
          <v-spacer />
          <v-btn
            name="archive"
            color="primary"
            tooltip="buttons.archive"
            @click="archiveMessage()"
          >
            <v-icon left>
              fas fa-archive
            </v-icon>

            Архивирай
          </v-btn>
          <v-btn
            name="mdi-delete"
            color="error"
            tooltip="buttons.delete"
            @click="deleteMessage()"
          >
            <v-icon left>
              fas fa-trash
            </v-icon>
            Изтрий
          </v-btn>
        </v-card-actions>
      </v-card>
    </div>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Constants from '@/common/constants';

export default {
  name: "MessageDetails",
  props: {
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      model: null,
      loading: false,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    backClick() {
      this.$router.go(-1);
    },
    load() {
      this.loading = true;
      this.$api.message.getById(this.id)
        .then((response) => {
          if (response.data) {
            this.markAsRead();
            this.model = response.data;
          }
        })
        .catch((error) => {
          console.log(error);
        })
        .then(() => {
          this.loading = false;
        });
    },
    archiveMessage() {
      this.$api.message.archive(this.id)
        .then(() => {
          this.$notifier.success('',this.$t('common.saveSuccess'));
          this.backClick();
        })
        .catch(() => {
          this.$notifier.error('',this.$t('common.saveError'));
        });
    },
    async deleteMessage() {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.$api.message.delete(this.id)
          .then(() => {
            this.$notifier.success('',this.$t('common.deleteSuccess'));
            this.backClick();
          })
          .catch(() => {
            this.$notifier.error('',this.$t('common.deleteError'));
          });
      }
    },
    markAsRead() {
      try {
        this.$api.message.markRead(this.id);
      } catch {
        // ignore;
      }
    }
  },
};
</script>
