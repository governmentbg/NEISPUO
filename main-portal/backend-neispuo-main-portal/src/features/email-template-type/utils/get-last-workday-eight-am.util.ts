/**
 * Returns a Date object for 8 AM on the last workday (Mon–Fri) relative to "today".
 * - If today is Tuesday–Friday, that’s simply yesterday at 08:00.
 * - If today is Monday, returns last Friday at 08:00.
 * - If today is Saturday or Sunday, returns last Friday at 08:00.
 */
export const getLastWorkdayEightAm = (): Date => {
    const today = new Date(); 
    const base = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    const weekday = base.getDay(); // 0 = Sunday, …, 6 = Saturday
  
    let daysToSubtract: number;
    if (weekday === 0) {
      daysToSubtract = 2;
    } else if (weekday === 1) {
      daysToSubtract = 3;
    } else {
      daysToSubtract = 1;
    }
  
    base.setDate(base.getDate() - daysToSubtract);
  
    base.setHours(8, 0, 0, 0);
  
    return base;
  };
  