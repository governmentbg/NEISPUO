/**
 * This is a simple utility to get the drillMembers property from a schema's dimensions.
 * 1. Replace the dimensions object with the one from the schema .js files
 * 2. Run with node dimensions node `node dimensions-to-drill-members-utils.js`
 * 3. Open the resulting dimensions-list.json and use editor replace to remove quotes (")
 */
const fs = require('fs');

const dimensions = {}
fs.writeFileSync('./dimensions-list.json', JSON.stringify(Object.keys(dimensions)))

