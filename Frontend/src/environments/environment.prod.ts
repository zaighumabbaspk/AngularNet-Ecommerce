export const environment = {
  production: true,
  apiUrl: 'https://ecommerce-prod-api.azurewebsites.net/api',
  apiBaseUrl: 'https://ecommerce-prod-api.azurewebsites.net/api',
  frontendUrl: 'https://ecommerceprodstore.z23.web.core.windows.net',
  stripePublishableKey: 'pk_test_51T9hpvJ31GzrQr6WvvmwnycLI02XAXiSyyz97ytmguN2ZhbAo57i2xTCTcc7Ip3xt5Oqe3LxwhaDbOyV0zcEo8IS00zVsClpRz',
  enableLogging: false,
  enableDebugMode: false,
  cacheTimeout: 300000, // 5 minutes
  apiTimeout: 30000,
  features: {
    enableAnalytics: true,
    enableErrorReporting: true,
    enablePerformanceMonitoring: true
  }
}