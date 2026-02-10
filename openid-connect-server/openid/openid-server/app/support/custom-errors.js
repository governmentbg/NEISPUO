class RoleRevokedError extends Error {
  constructor(message) {
    super(message);
  }
}

class NeispuoAccessDeniedError extends Error {
  constructor(message) {
    super(message);
  }
}

class NeispuoAccountNotSynchronized extends Error {
  constructor(message) {
    super(message);
  }
}

module.exports = { RoleRevokedError, NeispuoAccessDeniedError, NeispuoAccountNotSynchronized };
