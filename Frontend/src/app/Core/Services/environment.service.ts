import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EnvironmentService {
  
  get isProduction(): boolean {
    return environment.production;
  }

  get apiUrl(): string {
    return environment.apiUrl;
  }

  get frontendUrl(): string {
    return environment.frontendUrl;
  }

  get stripePublishableKey(): string {
    return environment.stripePublishableKey;
  }

  get enableLogging(): boolean {
    return environment.enableLogging;
  }

  get enableDebugMode(): boolean {
    return environment.enableDebugMode;
  }

  get cacheTimeout(): number {
    return environment.cacheTimeout;
  }

  get apiTimeout(): number {
    return environment.apiTimeout;
  }

  get features() {
    return environment.features;
  }

  // Helper methods
  log(message: any, ...optionalParams: any[]): void {
    if (this.enableLogging) {
      console.log(message, ...optionalParams);
    }
  }

  warn(message: any, ...optionalParams: any[]): void {
    if (this.enableLogging) {
      console.warn(message, ...optionalParams);
    }
  }

  error(message: any, ...optionalParams: any[]): void {
    if (this.enableLogging) {
      console.error(message, ...optionalParams);
    }
  }

  debug(message: any, ...optionalParams: any[]): void {
    if (this.enableDebugMode) {
      console.debug(message, ...optionalParams);
    }
  }

  getEnvironmentInfo(): any {
    return {
      production: this.isProduction,
      apiUrl: this.apiUrl,
      frontendUrl: this.frontendUrl,
      features: this.features,
      enableLogging: this.enableLogging,
      enableDebugMode: this.enableDebugMode
    };
  }
}