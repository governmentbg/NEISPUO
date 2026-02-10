<template>
  <v-container
    fluid
    class="pa-0 mt-0"
  >
    <v-row dense>
      <v-col
        v-if="fileExtensions"
        cols="12"
        sm="3"
      >
        <span
          v-for="ext in fileExtensions"
          :key="ext"
        >
          <v-tooltip bottom>
            <template v-slot:activator="{ on, attrs }">
              <v-btn
                v-bind="attrs"
                icon
                v-on="on"
                @click="exportToFile(ext)"
              >
                <v-icon :color="getIconClass(ext).color">{{ getIconClass(ext).name }}</v-icon>
              </v-btn>
            </template>
            <span>{{ $t('buttons.exportInFormat', { fileExtension: ext }) }}</span>
          </v-tooltip>
        </span>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import XLSX from 'xlsx';

export default {
    name: 'GridExportWrapper',
    components: {},
    props: {
        fileExtensions: {
            type: Array,
            default() {
                return ['xlsx'];
            }
        },
        fileName: {
            type: String,
            default() {
                return 'Export';
            }
        },
        items: {
            type: Array,
            default() {
                return [];
            }
        },
        headers: {
            type: Array,
            default() {
                return [];
            }
        },
        skipHeader: {
            type: Boolean,
            default() {
                return false;
            }
        }
    },
    data() {
        return {
            exportableProps: this.headers ? this.headers.filter(x => x && x.value !== null && x.value !== undefined && x.value !== 'controls').map(x => x.value) : [],
            header: this.headers ? this.headers.reduce((a,x) => ({...a, [x.value]: x.text}), {}) : {},
        };
    },
    mounted() {},
    methods: {
        getIconClass(extension) {
            switch (extension) {
                case 'xlsx':
                    return {name: 'mdi-file-excel', color: '#008000'};
                case 'csv':
                    return {name: 'mdi-file-delimited', color: '#F1B350'};
                case 'txt':
                    return {name: 'mdi-file-document', color: '#515151'};
                default:
                    break;
            }
        },
        exportToFile(extension) {
            // Export json to Worksheet of Excel
            // Only array possible
            const exportableData = this.getExportableData();
            let ws = XLSX.utils.json_to_sheet(exportableData, { skipHeader: this.skipHeader });

            this.processHeader(ws);

            // Make Workbook of Excel
            let wb = XLSX.utils.book_new();

            // Add Worksheet to Workbook
            // Workbook contains one or more worksheets
            XLSX.utils.book_append_sheet(wb, ws, this.fileName);

            // Export Excel file
            XLSX.writeFile(wb, `${this.fileName}.${extension}`);
        },
        getExportableData() {
            return this.items ? this.items.map(this.filterExportableProps) : this.items;
        },
        filterExportableProps(item) {
            if(!item) return item;

            const exportableProps = this.exportableProps;
            let newItem = {};

            for(var index in exportableProps){
                var prop = exportableProps[index];

                if(prop.includes(".name")){
                    prop = prop.replace(".name", "");
                    exportableProps[index] = prop;
                }
            }

            for (var key in item) {
                if(!exportableProps.includes(key)) {
                    continue;
                }

                if(item[key] && item[key].value !== undefined && item[key].name !== undefined){
                    newItem[key] =  item[key].name;
                }else{
                    newItem[key] = item[key];
                }
            }

            return newItem;
        },
        processHeader(ws) {
            if(!ws || this.skipHeader) return;

            const header = this.header;

            var sanitizedHeader = new Object();

            for(var prop in header){

                var cell = header[prop];

                if(prop.includes(".name")){
                    prop = prop.replace(".name", "");
                }

                sanitizedHeader[prop] = cell;
            }

            var range = XLSX.utils.decode_range(ws['!ref']);

            for(var colName = range.s.r; colName <= range.e.c; ++colName) {
                var cellAddress = XLSX.utils.encode_col(colName) + "1"; // <-- like A1, B1...
                if(!ws[cellAddress]) continue;

                ws[cellAddress].v = sanitizedHeader[ws[cellAddress].v];
            }
        }
    }
};
</script>
