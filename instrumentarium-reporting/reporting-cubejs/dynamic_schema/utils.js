export const convertStringPropToFunction = (propNames, dimensionDefinition) => {
    const newResult = { ...dimensionDefinition };
    propNames.forEach((propName) => {
        const propValue = newResult[propName];

        if (!propValue) {
            return;
        }

        newResult[propName] = () => propValue;
    });
    return newResult;
};

export const transformDimensions = (dimensions) => Object.keys(dimensions).reduce((result, dimensionName) => {
    const dimensionDefinition = dimensions[dimensionName];
    return {
        ...result,
        [dimensionName]: convertStringPropToFunction(
            ['sql'],
            dimensionDefinition
        ),
    };
}, {});

export const transformMeasures = (measures) => Object.keys(measures).reduce((result, aggregationName) => {
    const dimensionDefinition = measures[aggregationName];
    return {
        ...result,
        [aggregationName]: convertStringPropToFunction(
            ['sql', 'drillMembers'],
            dimensionDefinition
        ),
    };
}, {});


export const transformPreAggregations = (preAggregations) => Object.keys(preAggregations).reduce((result, dimensionName) => {
    const dimensionDefinition = preAggregations[dimensionName];
    return {
        ...result,
        [dimensionName]: convertStringPropToFunction(
            ['dimensions', 'measures'],
            dimensionDefinition
        ),
    };
}, {});
