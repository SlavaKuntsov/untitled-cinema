import { Injectable } from "@angular/core";
import { IError } from "../../../../../entities/error/model/error";

@Injectable({
  providedIn: "root",
})
export class ErrorService {
  getErrorMessage(error: IError): string {
    console.log(error.error.title);
    console.log("--------------");
    if (error.error && typeof error.error === "string") {
			console.log(1)
      try {
        const errorObj = JSON.parse(error.error);
        return (
          errorObj.detail || errorObj.message || "An unknown error occurred"
        );
      } catch (e) {
        return error.error;
      }
    } else if (error.error && error.error.detail) {
			console.log(2)
      return error.error.detail;
    } else if (error.error && error.error.title) {
			console.log(3)
      return error.error.title;
    } else if (error.message) {
			console.log(4)
      return error.message;
    }
    return "An unknown error occurred";
  }
}
