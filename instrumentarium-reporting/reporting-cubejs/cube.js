const AuthUtil = require('./auth');
const { ClickHouseDriver } = require('@cubejs-backend/clickhouse-driver');

module.exports = {
  driverFactory: () => new ClickHouseDriver(),
  checkAuth: async (req, auth) => {
    try {
      const decodedJwt = await AuthUtil.getJwtFromHttp(auth);
      const authObject = AuthUtil.generateAuthObject(decodedJwt);
      req.securityContext = { decodedJwt, authObject };
      const {
        isMonAdmin,
        isMon,
        isRuo,
        isSchool,
        isMunicipality,
        isBudgetingInstitution,
      } = authObject;

      if (
        !isMonAdmin &&
        !isMon &&
        !isRuo &&
        !isSchool &&
        !isMunicipality &&
        !isBudgetingInstitution
      ) {
        throw new Error('Could not authenticate user.');
      }
    } catch (e) {
      throw new Error('Could not authenticate user from OIDC.');
    }
  },

  queryRewrite: (query, { securityContext }) => {
    const schemaName = query.dimensions[0]?.split('.')[0];
    const { isRuo, isSchool, isMunicipality, isBudgetingInstitution } =
      securityContext.authObject;
    const { RegionID, MunicipalityID, InstitutionID, BudgetingInstitutionID } =
      securityContext.decodedJwt.selected_role;

    if (!schemaName) {
      throw new Error(
        'Undefined schema name provided as dimensions in query object.',
      );
    }

    if (isRuo) {
      query.filters.push({
        member: `${schemaName}.RegionID`,
        operator: 'equals',
        values: [`${RegionID}`],
      });
    } else if (isMunicipality) {
      query.filters.push({
        member: `${schemaName}.MunicipalityID`,
        operator: 'equals',
        values: [`${MunicipalityID}`],
      });
    } else if (isSchool) {
      query.filters.push({
        member: `${schemaName}.InstitutionID`,
        operator: 'equals',
        values: [`${InstitutionID}`],
      });
    } else if (isBudgetingInstitution) {
      query.filters.push({
        member: `${schemaName}.BudgetingSchoolTypeID`,
        operator: 'equals',
        values: [`${BudgetingInstitutionID}`],
      });
    }

    return query;
  },
};
