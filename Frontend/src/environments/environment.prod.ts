// Production environment configuration
export const environment = {
  production: true,
  apiUrl: 'https://ecommerce-zaighum.viewdns.net/api',
  apiBaseUrl: 'https://ecommerce-zaighum.viewdns.net/api',
  frontendUrl: 'https://ecommercezaighum.z29.web.core.windows.net',
  stripePublishableKey: 'pk_test_51T9hpvJ31GzrQr6WvvmwnycLI02XAXiSyyz97ytmguN2ZhbAo57i2xTCTcc7Ip3xt5Oqe3LxwhaDbOyV0zcEo8IS00zVsClpRz',
  enableLogging: false,
  enableDebugMode: false,
  cacheTimeout: 600000,
  apiTimeout: 15000,
  features: {
    enableAnalytics: true,
    enableErrorReporting: true,
    enablePerformanceMonitoring: true
  }
};