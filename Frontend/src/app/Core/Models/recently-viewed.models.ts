export interface AddRecentlyViewedRequest {
  productId: string;
  userId: string;
}

export interface RecentlyViewedResponse {
  items: RecentlyViewedItem[];
}

export interface RecentlyViewedItem {
  productId: string;
  productName: string;
  price: number;
  imageUrl: string;
  viewedAt: string;
}