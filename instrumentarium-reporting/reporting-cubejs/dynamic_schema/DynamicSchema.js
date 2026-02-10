/* eslint-disable no-undef */
/**
 * Currently disabled since drillMembers is not being correctly processed by the parsing functions in utils
 * 
 */
const fetch = require('node-fetch')
const { transformDimensions, transformMeasures, transformPreAggregations } = require('./utils');
const { environment } = require('../node-environment')

asyncModule(async () => {
    const cubeResponse = await (
        await fetch(environment.REPORTING_BACKEND_SCHEMA_URL, {
            headers: { 'X-API-KEY': environment.REPORTING_BACKEND_X_API_KEY }
        })
    ).json();
    const dynamicCubes = cubeResponse.map(c => c.Definition);
    if (!dynamicCubes && dynamicCubes.length === 0) return;

    // eslint-disable-next-line no-restricted-syntax
    for (const dc of dynamicCubes) {
        const { description, title, sql, dataSource } = { ...dc }
        const dimensions = transformDimensions(dc.dimensions);
        const measures = transformMeasures(dc.measures);
        const preAggregations = transformPreAggregations(dc.preAggregations);

        cube(dc.meta.name, {
            sql,
            title,
            description,
            preAggregations,
            dimensions,
            measures,
            dataSource
        });
    };
})