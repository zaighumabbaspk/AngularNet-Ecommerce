export interface ShippingOption {
  id: string;
  name: string;
  description: string;
  price: number;
  estimatedDays: number;
  isDefault: boolean;
}

export interface ShippingOptionsResponse {
  options: ShippingOption[];
  subTotal: number;
  tax: number;
  shippingCost: number;
  total: number;
}

export interface EnhancedCheckoutForm {
  // Customer Information
  customerEmail: string;
  customerName: string;
  phoneNumber: string;
  companyName?: string;

  // Shipping Address
  shippingAddressLine1: string;
  shippingAddressLine2?: string;
  shippingCity: string;
  shippingState: string;
  shippingZipCode: string;
  shippingCountry: string;

  // Billing Address
  billingSameAsShipping: boolean;
  billingAddressLine1?: string;
  billingAddressLine2?: string;
  billingCity?: string;
  billingState?: string;
  billingZipCode?: string;
  billingCountry?: string;

  // Shipping Options
  shippingMethod: string;

  // Additional Information
  specialInstructions?: string;
  isGift: boolean;
  giftMessage?: string;
  newsletterSubscription: boolean;
  smsUpdates: boolean;

  // Terms and Conditions
  acceptTerms: boolean;
  acceptPrivacy: boolean;
}

export interface CreatePaymentIntentRequest {
  // Customer Information
  customerEmail: string;
  customerName: string;
  phoneNumber: string;
  companyName?: string;

  // Shipping Address
  shippingAddressLine1: string;
  shippingAddressLine2?: string;
  shippingCity: string;
  shippingState: string;
  shippingZipCode: string;
  shippingCountry: string;

  // Billing Address
  billingSameAsShipping: boolean;
  billingAddressLine1?: string;
  billingAddressLine2?: string;
  billingCity?: string;
  billingState?: string;
  billingZipCode?: string;
  billingCountry?: string;

  // Shipping Options
  shippingMethod: string;

  // Additional Information
  specialInstructions?: string;
  isGift: boolean;
  giftMessage?: string;
  newsletterSubscription: boolean;
  smsUpdates: boolean;
}

export interface CountryState {
  code: string;
  name: string;
  states?: StateOption[];
}

export interface StateOption {
  code: string;
  name: string;
}

// Pakistan-focused countries and states
export const COUNTRIES: CountryState[] = [
  {
    code: 'PK',
    name: 'Pakistan',
    states: [
      { code: 'PB', name: 'Punjab' },
      { code: 'SD', name: 'Sindh' },
      { code: 'KP', name: 'Khyber Pakhtunkhwa' },
      { code: 'BL', name: 'Balochistan' },
      { code: 'GB', name: 'Gilgit-Baltistan' },
      { code: 'AJK', name: 'Azad Jammu and Kashmir' },
      { code: 'ICT', name: 'Islamabad Capital Territory' }
    ]
  },
  {
    code: 'US',
    name: 'United States',
    states: [
      { code: 'AL', name: 'Alabama' },
      { code: 'AK', name: 'Alaska' },
      { code: 'AZ', name: 'Arizona' },
      { code: 'AR', name: 'Arkansas' },
      { code: 'CA', name: 'California' },
      { code: 'CO', name: 'Colorado' },
      { code: 'CT', name: 'Connecticut' },
      { code: 'DE', name: 'Delaware' },
      { code: 'FL', name: 'Florida' },
      { code: 'GA', name: 'Georgia' },
      { code: 'HI', name: 'Hawaii' },
      { code: 'ID', name: 'Idaho' },
      { code: 'IL', name: 'Illinois' },
      { code: 'IN', name: 'Indiana' },
      { code: 'IA', name: 'Iowa' },
      { code: 'KS', name: 'Kansas' },
      { code: 'KY', name: 'Kentucky' },
      { code: 'LA', name: 'Louisiana' },
      { code: 'ME', name: 'Maine' },
      { code: 'MD', name: 'Maryland' },
      { code: 'MA', name: 'Massachusetts' },
      { code: 'MI', name: 'Michigan' },
      { code: 'MN', name: 'Minnesota' },
      { code: 'MS', name: 'Mississippi' },
      { code: 'MO', name: 'Missouri' },
      { code: 'MT', name: 'Montana' },
      { code: 'NE', name: 'Nebraska' },
      { code: 'NV', name: 'Nevada' },
      { code: 'NH', name: 'New Hampshire' },
      { code: 'NJ', name: 'New Jersey' },
      { code: 'NM', name: 'New Mexico' },
      { code: 'NY', name: 'New York' },
      { code: 'NC', name: 'North Carolina' },
      { code: 'ND', name: 'North Dakota' },
      { code: 'OH', name: 'Ohio' },
      { code: 'OK', name: 'Oklahoma' },
      { code: 'OR', name: 'Oregon' },
      { code: 'PA', name: 'Pennsylvania' },
      { code: 'RI', name: 'Rhode Island' },
      { code: 'SC', name: 'South Carolina' },
      { code: 'SD', name: 'South Dakota' },
      { code: 'TN', name: 'Tennessee' },
      { code: 'TX', name: 'Texas' },
      { code: 'UT', name: 'Utah' },
      { code: 'VT', name: 'Vermont' },
      { code: 'VA', name: 'Virginia' },
      { code: 'WA', name: 'Washington' },
      { code: 'WV', name: 'West Virginia' },
      { code: 'WI', name: 'Wisconsin' },
      { code: 'WY', name: 'Wyoming' }
    ]
  },
  {
    code: 'CA',
    name: 'Canada',
    states: [
      { code: 'AB', name: 'Alberta' },
      { code: 'BC', name: 'British Columbia' },
      { code: 'MB', name: 'Manitoba' },
      { code: 'NB', name: 'New Brunswick' },
      { code: 'NL', name: 'Newfoundland and Labrador' },
      { code: 'NS', name: 'Nova Scotia' },
      { code: 'ON', name: 'Ontario' },
      { code: 'PE', name: 'Prince Edward Island' },
      { code: 'QC', name: 'Quebec' },
      { code: 'SK', name: 'Saskatchewan' },
      { code: 'NT', name: 'Northwest Territories' },
      { code: 'NU', name: 'Nunavut' },
      { code: 'YT', name: 'Yukon' }
    ]
  },
  {
    code: 'GB',
    name: 'United Kingdom'
  },
  {
    code: 'AU',
    name: 'Australia'
  },
  {
    code: 'DE',
    name: 'Germany'
  },
  {
    code: 'FR',
    name: 'France'
  },
  {
    code: 'IT',
    name: 'Italy'
  },
  {
    code: 'ES',
    name: 'Spain'
  },
  {
    code: 'NL',
    name: 'Netherlands'
  },
  {
    code: 'IN',
    name: 'India'
  },
  {
    code: 'AE',
    name: 'United Arab Emirates'
  },
  {
    code: 'SA',
    name: 'Saudi Arabia'
  }
];

// Pakistan-focused shipping options with PKR pricing
export const DEFAULT_SHIPPING_OPTIONS: ShippingOption[] = [
  {
    id: 'standard',
    name: 'Standard Delivery',
    description: '3-5 business days',
    price: 250, // PKR
    estimatedDays: 5,
    isDefault: true
  },
  {
    id: 'express',
    name: 'Express Delivery',
    description: '1-2 business days',
    price: 500, // PKR
    estimatedDays: 2,
    isDefault: false
  },
  {
    id: 'same-day',
    name: 'Same Day Delivery',
    description: 'Same day (Karachi, Lahore, Islamabad only)',
    price: 800, // PKR
    estimatedDays: 0,
    isDefault: false
  },
  {
    id: 'free',
    name: 'Free Delivery',
    description: '5-7 business days (orders over PKR 2000)',
    price: 0, // PKR
    estimatedDays: 7,
    isDefault: false
  }
];