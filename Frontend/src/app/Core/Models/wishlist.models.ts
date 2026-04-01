export interface AddToWishlistRequest {
  productId: string;
  userId: string;
}

export interface WishlistResponse {
  items: WishlistItem[];
  totalCount: number;
}

export interface WishlistItem {
  id: string;
  productId: string;
  productName: string;
  productDescription: string;
  price: number;
  imageUrl: string;
  categoryName: string;
  isInStock: boolean;
  addedAt: string;
}