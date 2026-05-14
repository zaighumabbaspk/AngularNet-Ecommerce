// Example local environment configuration - Copy this to environment.local.ts and add your real keys
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  apiBaseUrl: 'http://localhost:5000/api',
  frontendUrl: 'http://localhost:4200',
  stripePublishableKey: 'pk_test_YOUR_STRIPE_TEST_PUBLISHABLE_KEY_HERE',
  enableLogging: true,
  enableDebugMode: true,
  cacheTimeout: 60000, // 1 minute for faster development
  apiTimeout: 60000, // 60 seconds for debugging
  features: {
    enableAnalytics: false,
    enableErrorReporting: false,
    enablePerformanceMonitoring: false
  }
};