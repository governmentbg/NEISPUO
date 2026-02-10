/**
 * Validates and deduplicates an array of strings.
 *
 * - Ensures the input is an array.
 * - Trims each element, rejects empty or whitespace-only strings.
 * - Throws if any element is not a string or is empty after trimming.
 * - Returns a new array preserving the first occurrence of each unique value.
 *
 * @param values - The array to validate and dedupe
 * @throws {TypeError} if `values` is not an array
 * @throws {Error} if any element is not a non-empty string
 * @returns A deduplicated array of trimmed, non-empty strings
 */
export function dedupeAndValidateStrings(values: unknown[]): string[] {
  if (!Array.isArray(values)) {
    throw new TypeError(
      `Expected an array of strings, but received ${typeof values}`,
    );
  }

  const seen = new Set<string>();
  for (const item of values) {
    if (typeof item !== 'string') {
      throw new Error(`Invalid element "${item}": not a string`);
    }

    const trimmed = item.trim();
    if (trimmed === '') {
      throw new Error(
        `Invalid element "${item}": empty or whitespace-only string`,
      );
    }

    if (!seen.has(trimmed)) {
      seen.add(trimmed);
    }
  }

  return Array.from(seen);
}
