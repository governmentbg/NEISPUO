/**
 * A descriptor for the “data‐table” partial. 
 * We export both the name (used in {{> name}}) and the raw template string.
 */
export const dataTablePartial = {
    name: 'data-table',
    template: `
  <table style="border-collapse:collapse;width:100%;font-family:Arial,Helvetica">
    <thead>
      <tr>
        {{#each table.columns as |col|}}
          <th style="border:1px solid #ddd;padding:8px;text-align:left">
            {{col.label}}
          </th>
        {{/each}}
      </tr>
    </thead>
    <tbody>
      {{#each table.rows}}
        <tr>
          {{#each ../table.columns as |col|}}
            <td style="border:1px solid #ddd;padding:8px">
              {{lookup .. col.key}}
            </td>
          {{/each}}
        </tr>
      {{/each}}
    </tbody>
  </table>
  `,
  };
  