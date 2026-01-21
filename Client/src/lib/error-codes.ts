export const ErrorCodes = {
  AUTH_INVALID_CREDENTIALS: "AUTH_001",
  AUTH_INVALID_LOGIN_METHOD: "AUTH_002",
  AUTH_EXPIRED: "AUTH_003",

  VAL_FAILED: "VAL_001",

  RES_NOT_FOUND: "RES_001",
  RES_CONFLICT: "RES_002",
  RES_CONFLICT_EMAIL: "RES_003",
  RES_CONFLICT_USERNAME: "RES_004",

  SYS_INTERNAL_ERROR: "SYS_001",
} as const;

export type ErrorCode = (typeof ErrorCodes)[keyof typeof ErrorCodes];
