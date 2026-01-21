<template>
  <v-tooltip
    :bottom="bottom"
    :top="top"
    :left="left"
    :right="right"
  >
    <template v-slot:activator="{ on: btnTooltip }">
      <slot>
        <v-btn
          icon
          :disabled="disabled || signing"
          v-bind="$attrs"
          v-on="{ ...btnTooltip }"
          @click.stop="onClick"
        >
          <v-icon
            :color="iconColor"
          >
            mdi-file-sign
          </v-icon>
        </v-btn>
        <!-- <span
          v-on="{ ...btnTooltip }"
        >
        </span> -->
      </slot>
    </template>
    <span> {{ $t('common.sign') }} </span>
  </v-tooltip>
</template>

<script>
export default {
  name: 'DigitalSigningButton',
  props: {
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    bottom: {
      type: Boolean,
      required: false,
      default() {
        return false;
      },
    },
    top: {
      type: Boolean,
      required: false,
      default() {
        return false;
      },
    },
    left: {
      type: Boolean,
      required: false,
      default() {
        return false;
      },
    },
    right: {
      type: Boolean,
      required: false,
      default() {
        return false;
      },
    },
    iconColor: {
      type: String,
      default() {
        return 'primary';
      }
    }
  },
  data() {
    return {
      signing: false
    };
  },
  methods: {
    async onClick() {
      const { isChecked } = await this.checkInstallation();
      if(!isChecked) {
        return this.$notifier.modalError('Проверка за инсталация на локален сървър за достъп до електронни сертификати',
          'Липсва инсталация. Инсталацията се извършва от меню "Информационно табло" (горе вдясно), "Локален сървър"');
      }

      this.$emit('click');
    },
    async checkInstallation() {
      let isChecked = true,
        version = 'няма връзка',
        edition = 'няма връзка',
        caps = 'няма връзка',
        settings = 'няма връзка';

      this.signing = true;

      await this.$api.localServer.version()
      .then((data) => {
        version = data.data;
      })
      .catch((error) => {
        version = error;
        isChecked = false;
      });

      await this.$api.localServer.edition()
      .then((data) => {
        edition = data.data;
      })
      .catch((error) => {
        edition = error;
        isChecked = false;
      });

      await this.$api.localServer.capabilities()
      .then((data) => {
        caps = data.data;
      })
      .catch((error) => {
        caps = error;
        isChecked = false;
      });

      await this.$api.localServer.settings()
      .then((data) => {
        settings = data.data;
      })
      .catch((error) => {
        settings = error;
        isChecked = false;
      });

      this.signing = false;

      return { isChecked, version, edition, caps, settings };
    },
  }
};
</script>
