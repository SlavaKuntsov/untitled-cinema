import { ApplicationConfig, provideZoneChangeDetection } from "@angular/core";
import { provideRouter } from "@angular/router";
import { providePrimeNG } from "primeng/config";

import { provideAnimations } from "@angular/platform-browser/animations";
import { MessageService } from "primeng/api";

import { provideHttpClient, withInterceptors } from "@angular/common/http";
import MyPreset from "../assets/styles/utils/mypreset";
import { routes } from "./app.routes";
import { authTokenInterceptor } from "./core/interceptor/auth/auth.interceptor";

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authTokenInterceptor])),
    providePrimeNG({
      theme: {
        preset: MyPreset,
      },
    }),
    MessageService,
    provideAnimations(),
  ],
};
