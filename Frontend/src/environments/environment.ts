// Development environment configuration
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7137/api',
  apiBaseUrl: 'https://localhost:7137/api',
  frontendUrl: 'http://localhost:4200',
  stripePublishableKey: 'pk_test_51T9hpvJ31GzrQr6WvvmwnycLI02XAXiSyyz97ytmguN2ZhbAo57i2xTCTcc7Ip3xt5Oqe3LxwhaDbOyV0zcEo8IS00zVsClpRz',
  enableLogging: true,
  enableDebugMode: true,
  cacheTimeout: 300000,
  apiTimeout: 30000,
  features: {
    enableAnalytics: false,
    enableErrorReporting: true,
    enablePerformanceMonitoring: false
  }
};