export function info(message: string, ...args: any[]): void {
  console.log(message, ...args);
}

export function error(message: string, ...args: any[]): void {
  console.error(message, ...args);
}

export function warn(message: string, ...args: any[]): void {
  console.warn(message, ...args);
}

export function debug(message: string, ...args: any[]): void {
  console.debug(message, ...args);
}
