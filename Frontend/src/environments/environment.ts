// Local development environment configuration
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7138/api',
  apiBaseUrl: 'https://localhost:7138/api',
  frontendUrl: 'http://localhost:4200',
  stripePublishableKey: 'pk_test_51T9hpvJ31GzrQr6WvvmwnycLI02XAXiSyyz97ytmguN2ZhbAo57i2xTCTcc7Ip3xt5Oqe3LxwhaDbOyV0zcEo8IS00zVsClpRz',
  enableLogging: true,
  enableDebugMode: true,
  cacheTimeout: 60000,
  apiTimeout: 60000,
  features: {
    enableAnalytics: false,
    enableErrorReporting: false,
    enablePerformanceMonitoring: false
  }
};