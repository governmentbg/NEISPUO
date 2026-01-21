<template>
  <v-row
    v-if="fileExtensions"
    dense
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
            <v-icon :color="getIconClass(ext).color">{{
              getIconClass(ext).name
            }}</v-icon>
          </v-btn>
        </template>
        <span>{{
          $t("buttons.exportInFormat", { fileExtension: ext })
        }}</span>
      </v-tooltip>
    </span>
  </v-row>
</template>

<script>
import XLSX from "xlsx";
import range from "lodash.range";

export default {
  name: "GridExportWrapper",
  components: {},
  props: {
    fileExtensions: {
      type: Array,
      default() {
        return ["xlsx"];
      },
    },
    fileName: {
      type: String,
      default() {
        return "Export";
      },
    },
    items: {
      type: Array,
      default() {
        return [];
      },
    },
    headers: {
      type: Array,
      default() {
        return [];
      },
    },
    skipHeader: {
      type: Boolean,
      default() {
        return false;
      },
    },
    autoFitColumns: {
      type: Boolean,
      default() {
        return false;
      },
    },
    // Този формат ще се приложи за всички колини от type: 'date',
    // за които не е описан dateFormat. Инак се прилага подадение в хедърите формат.
    dateFormat: {
      type: String,
      default: 'DD.MM.YYYY'
    }
  },
  data() {
    return {
      exportableProps: this.headers
        ? this.headers
            .filter(
              (x) =>
                x &&
                x.value !== null &&
                x.value !== undefined &&
                x.value !== "controls"
            )
            .map((x) => x.value)
        : [],
      header: this.headers
        ? this.headers
            .reduce((a, x) => ({ ...a, [x.value]: x.text }), {})
        : {},
    };
  },
  mounted() {},
  methods: {
    getIconClass(extension) {
      switch (extension) {
        case "xlsx":
          return { name: "mdi-file-excel", color: "#008000" };
        case "csv":
          return { name: "mdi-file-delimited", color: "#F1B350" };
        case "txt":
          return { name: "mdi-file-document", color: "#515151" };
        default:
          break;
      }
    },
    exportToFile(extension) {
      // Export json to Worksheet of Excel
      // Only array possible
      const exportableData = this.getExportableData();
      let ws = XLSX.utils.json_to_sheet(exportableData, {
        skipHeader: this.skipHeader,
        dateNF: this.dateFormat
      });

      this.processHeader(ws);
      if (this.autoFitColumns) {
        this.autofit(ws);
      }

      // Make Workbook of Excel
      let wb = XLSX.utils.book_new();

      // Add Worksheet to Workbook
      // Workbook contains one or more worksheets

      let xlsxFileName = this.fileName.substring(0, 31);
      xlsxFileName = xlsxFileName.replaceAll(/[:\\/?*[\]]/g, "_");
      XLSX.utils.book_append_sheet(wb, ws, xlsxFileName);

      // Export Excel file
      XLSX.writeFile(wb, `${this.fileName}.${extension}`);
    },
    getExportableData() {
      // Списък с обекти само с пропъртитата, които се показват в колоните.
      return this.items
        ? this.items.map(this.filterExportableProps)
        : this.items;
    },
    filterExportableProps(item) {
      if (!item) return item;

      let newItem = {};

      for (var index in this.exportableProps) {
        var prop = this.exportableProps[index];

        if (prop.includes(".name")) {
          prop = prop.replace(".name", "");
          this.exportableProps[index] = prop;
        }

        if ( item[prop] &&
          item[prop].value !== undefined &&
          item[prop].name !== undefined) {
            newItem[prop] = item[prop].name;
          } else {
            const header = this.headers.find(header => header.value == prop);
            if (item[prop] != null) {
            if (header && header.type === "boolean"){
              if(header.inverse === true) {
                newItem[prop] = (item[prop] === true
                  ? this.$t('common.no')
                  : item[prop] === false ? this.$t('common.yes') : '');
              } else {
                newItem[prop] = (item[prop] === true
                  ? this.$t('common.yes')
                  : item[prop] === false ? this.$t('common.no') : '');
              }
            }
            else if (header && header.type === "date"){
              if(header.dateFormat) {
                newItem[prop] = item[prop] ? this.$moment(item[prop]).format(header.dateFormat) : '';
              } else {
                newItem[prop] = new Date(item[prop]);
              }
            }
            else
            {
              newItem[prop] = item[prop];
            }
          }
          else {
            newItem[prop] = item[prop];
          }
        }
      }

      return newItem;
    },
    processHeader(ws) {
      if (!ws || this.skipHeader) return;

      const header = this.header;
      var sanitizedHeader = new Object();

      for (var prop in header) {
        var cell = header[prop];

        if (prop.includes(".name")) {
          prop = prop.replace(".name", "");
        }

        sanitizedHeader[prop] = cell;
      }

      var range = XLSX.utils.decode_range(ws["!ref"]);

      for (var colName = range.s.r; colName <= range.e.c; ++colName) {
        var cellAddress = XLSX.utils.encode_col(colName) + "1"; // <-- like A1, B1...
        if (!ws[cellAddress]) continue;

        ws[cellAddress].v = sanitizedHeader[ws[cellAddress].v];
      }
    },
    getEndingDigit(inputString) {
      // Define a regular expression to extract the ending digits
      const regex = /\d+$/;

      // Use the test method to check if the inputString matches the pattern
      if (regex.test(inputString)) {
        // Use the match method to extract the ending digits from the inputString
        const match = inputString.match(regex);
        // Remove any non-digit characters before returning the result
        return match[0].replace(/\D/g, "");
      }

      // Return null if there are no ending digits
      return null;
    },
    autofit(worksheet) {
      let objectMaxLength = [];

      const [startLetter, endLetter] = worksheet["!ref"]
        ?.replace(/\d/, "")
        .split(":");
      const ranges = range(
        startLetter.charCodeAt(0),
        endLetter.charCodeAt(0) + 1
      );

      ranges.forEach((c) => {
        const cellHeader = String.fromCharCode(c);

        const maxCellLengthForWholeColumn = Array.from(
          { length: this.getEndingDigit(worksheet["!ref"]) - 1 },
          (_, i) => i
        ).reduce((acc, i) => {
          const cell = worksheet[`${cellHeader}${i + 1}`];

          // empty cell
          if (!cell) return acc;

          const charLength = (cell.v.length ?? cell.z?.length ?? 0) + 1;

          return acc > charLength ? acc : charLength ;
        }, 0);

        objectMaxLength.push({ wch: maxCellLengthForWholeColumn });
      });
      worksheet["!cols"] = objectMaxLength;
    }
}
};
</script>
