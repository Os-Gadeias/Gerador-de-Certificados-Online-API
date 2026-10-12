import { HttpErrorResponse } from '@angular/common/http';

export interface ApiError {
  type: string;
  title: string;
  status: number;
  errors: {
    [campo: string]: string[];
  };
  traceId: string;
}
