import type { ErrorCode } from "@/lib/error-codes.js";

export type SuccessResponse<T> = {
  success: true;
  data: T;
  status: number;
};

export type FailResponse = {
  success: false;
  type: string;
  title: string;
  status: number;
  detail: string;
  errorCode: ErrorCode;
  requestId: string;
  traceId?: string;
};

export type ApiResponse<T> = SuccessResponse<T> | FailResponse;
