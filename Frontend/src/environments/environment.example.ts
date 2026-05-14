// Example environment configuration - Copy this to environment.ts and add your real keys
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001/api',
  apiBaseUrl: 'https://localhost:7001/api',
  frontendUrl: 'http://localhost:4200',
  stripePublishableKey: 'pk_test_YOUR_STRIPE_TEST_PUBLISHABLE_KEY_HERE',
  enableLogging: true,
  enableDebugMode: true,
  cacheTimeout: 300000, // 5 minutes
  apiTimeout: 30000, // 30 seconds
  features: {
    enableAnalytics: false,
    enableErrorReporting: true,
    enablePerformanceMonitoring: false
  }
};