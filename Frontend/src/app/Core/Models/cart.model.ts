export interface AddToCartRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  cartItemId: string;
  quantity: number;
}

export interface GetCartItem {
  id: string;
  productId: string;
  productName: string;
  productImage: string;
  imageUrl: string;
  productPrice: number;
  price: number;
  categoryName: string;
  quantity: number;
  subtotal: number;
  availableStock: number;
}

export interface GetCart {
  id: string;
  userId: string;
  cartItems: GetCartItem[];
  total: number;
  totalItems: number;
}
