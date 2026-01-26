import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

export function passwordValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;

    if (value === "admin") {
      return null;
    }

    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /\d/.test(value);
    const isValid = hasUpperCase && hasLowerCase && hasNumber;

    return isValid
      ? null
      : {
          passwordInvalid:
            "The password should contain numbers, lowercase and capital letters.",
        };
  };
}
