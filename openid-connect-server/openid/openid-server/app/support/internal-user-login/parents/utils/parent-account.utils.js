const knex = require('knex')({
  client: 'mssql',
  connection: {
    server: process.env.MSSQL_HOST,
    port: +process.env.MSSQL_PORT,
    database: process.env.MSSQL_DB,
    user: process.env.MSSQL_USER,
    password: process.env.MSSQL_PASS,
  },
});
const { internalRolesEnum } = require('../../../shared/roles.enum');
const {
  WorkflowTypeEnum,
  EventStatusEnum,
  IsForArchivationEnum,
} = require('../enums/azure_temp.enum');
const SysUserTypesEnum = require('../../../shared/sys-user-types.enum');

/**
 *
 * @param {{
 * email: string,
 *  firstName: string,
 *  lastName: string,
 *  middleName: string?,
 *  azureID: string
 * }} azureParentAccount
 * @returns {(number | null)}
 */
function createNeispuoParentAccount(azureParentAccount) {
  const { email, firstName, lastName, middleName, azureID } = azureParentAccount;
  return knex.transaction(async (trx) => {
    /**
     * @type {{ RowID: number}}
     */
    const stuckUserWorkflow = await trx('azure_temp.Users')
      .select('RowID')
      .where({
        Email: email,
        WorkflowType: WorkflowTypeEnum.USER_CREATE,
        Status: EventStatusEnum.STUCK,
        IsForArchivation: IsForArchivationEnum.YES,
      })
      .andWhere('InProgressResultCount', '>=', 50)
      .first()
      .transacting(trx);

    /** If there is no stuck user workflow, return null. */
    if (!stuckUserWorkflow) {
      return null;
    }
    /**
     * @type {([number] | {PersonID: number} )}
     */
    let person = await trx('core.Person')
      .select('PersonID')
      .where('PublicEduNumber', email)
      .first()
      .transacting(trx);

    if (!person) {
      person = await trx('core.Person')
        .insert(
          {
            FirstName: firstName,
            MiddleName: middleName,
            LastName: lastName,
            PublicEduNumber: email,
            AzureID: azureID,
            SysUserType: SysUserTypesEnum.PARENT,
          },
          'PersonID',
        )
        .transacting(trx);
    }

    /**
     * @type {[number]}
     */
    const sysUserID = await trx('core.sysUser')
      .insert(
        {
          Username: email,
          PersonID: person.PersonID || person[0],
          IsAzureUser: true,
        },
        'SysUserID',
      )
      .transacting(trx);

    /**
     * @type {[number]}
     */
    const sysUserRole = await trx('core.sysUserSysRole')
      .insert({
        SysUserID: sysUserID[0],
        SysRoleId: internalRolesEnum.PARENT,
      })
      .transacting(trx);

    /**
     * @type {number}
     * Number of updated rows
     */
    const updateAzureTempUserRows = await trx('azure_temp.Users')
      .where({
        RowID: stuckUserWorkflow.RowID,
      })
      .update({
        Status: EventStatusEnum.SYNCHRONIZED,
      })
      .transacting(trx);
    return sysUserID[0];
  });
}

module.exports = { createNeispuoParentAccount };
