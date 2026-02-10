/**
 * Attach a 1-based “row” index to each object in a single dataset,
 * using the given prefix. Avoids confusion by taking prefix and array separately.
 *
 * Example:
 *   const users = [{ id: 1 }, { id: 2 }];
 *   const indexedUsers = addRowIndex("users", users);
 *   // → [
 *   //      { id: 1, "users.row": 1 },
 *   //      { id: 2, "users.row": 2 },
 *   //   ]
 *
 * @param prefix  A string to prefix the “row” property, e.g. "users"
 * @param rows    An array of objects to index
 * @returns       A new array where each object has a `{prefix}.row` property
 */
export function addRowIndex<T extends object>(
  prefix: string,
  rows: T[],
): (T & Record<string, number>)[] {
  return rows.map((item, idx) => ({
    ...item,
    [`${prefix}.row`]: idx + 1,
  }));
}