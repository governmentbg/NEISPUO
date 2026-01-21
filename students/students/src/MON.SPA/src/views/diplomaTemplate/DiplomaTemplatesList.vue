<template>
  <templates-list />
</template>

<script>
import TemplatesList from "@/components/diplomas/TemplatesList.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
    name: "DiplomaTemplatesList",
    components: {
        TemplatesList,
    },
    computed: {
        ...mapGetters(["hasPermission"]),
    },
    mounted() {
        if (
            !this.hasPermission(
                Permissions.PermissionNameForDiplomaTemplatesRead
            ) &&
            !this.hasPermission(
                Permissions.PermissionNameForDiplomaTemplatesManage
            )
        ) {
            return this.$router.push("/errors/AccessDenied");
        }
    },
};
</script>
