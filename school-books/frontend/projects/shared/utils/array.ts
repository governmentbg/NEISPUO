interface Comparator<T> {
  (a: T, b: T): number;
}

const defaultCmp: Comparator<any> = (a, b) => {
  if (a < b) return -1;
  if (a > b) return 1;
  return 0;
};

export function stableSort<T>(array: T[], cmp: Comparator<T> = defaultCmp): T[] {
  const stabilized = array.map((el, index) => <[T, number]>[el, index]);

  const stableCmp: Comparator<[T, number]> = (a, b) => {
    const order = cmp(a[0], b[0]);
    if (order !== 0) return order;
    return a[1] - b[1];
  };

  stabilized.sort(stableCmp);

  return stabilized.map((e) => e[0]);
}

interface Selector<T, V> {
  (el: T): V;
}

export function groupBy<T, K>(array: T[], keySelector: Selector<T, K>): [K, T[]][] {
  return [
    ...array
      .reduce((m, el) => {
        const key = keySelector(el);
        const group = m.get(key) || [];

        group.push(el);

        m.set(key, group);

        return m;
      }, new Map<K, T[]>())
      .entries()
  ];
}

export function sum<T>(array: T[], valueSelector: Selector<T, number | null | undefined>): number | null {
  let sum: number | null = null;

  for (const el of array.map(valueSelector)) {
    if (el != null) {
      sum = sum ?? 0;
      sum += el;
    }
  }

  return sum;
}

export function average<T>(array: T[], valueSelector: Selector<T, number | null | undefined>): number | null {
  const arraySum = sum(array, valueSelector);
  const nonNullLength = array.filter((el) => el != null).length;

  return arraySum != null ? arraySum / nonNullLength : null;
}

export function min<T>(array: T[], valueSelector: Selector<T, number | null | undefined>): number | null {
  let min: number | null = null;

  for (const el of array.map(valueSelector)) {
    if (el != null) {
      min = Math.min(el, min ?? Number.MAX_SAFE_INTEGER);
    }
  }

  return min;
}

export function max<T>(array: T[], valueSelector: Selector<T, number | null | undefined>): number | null {
  let max: number | null = null;

  for (const el of array.map(valueSelector)) {
    if (el != null) {
      max = Math.max(el, max ?? Number.MIN_SAFE_INTEGER);
    }
  }

  return max;
}

export function range(start: number, end: number): number[] {
  const result = [];

  for (let i = start; i <= end; i++) {
    result.push(i);
  }

  return result;
}
